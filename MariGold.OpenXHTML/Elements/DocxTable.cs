﻿namespace MariGold.OpenXHTML
{
	using System;
	using MariGold.HtmlParser;
	using DocumentFormat.OpenXml;
	using DocumentFormat.OpenXml.Wordprocessing;
	using System.Linq;
	
	internal sealed class DocxTable : DocxElement
	{
		private void SetThStyleToRun(IHtmlNode run)
		{
			DocxNode docxNode = new DocxNode(run);
			
			string value = docxNode.ExtractStyleValue(DocxFont.fontWeight);
			
			if (string.IsNullOrEmpty(value))
			{
				docxNode.SetStyleValue(DocxFont.fontWeight, DocxFont.bold);
			}
		}
		
		private void ProcessTd(int colIndex, IHtmlNode td, TableRow row, DocxTableProperties docxProperties)
		{
			TableCell cell = new TableCell();
				
			DocxTableCellStyle style = new DocxTableCellStyle();
			style.Process(colIndex, cell, docxProperties, td);
			
			if (td.HasChildren)
			{
				Paragraph para = null;
				
				foreach (IHtmlNode child in td.Children)
				{
					//If the cell is th header, apply font-weight:bold to the text
					if (docxProperties.IsCellHeader)
					{
						SetThStyleToRun(child);
					}
					
					if (child.IsText && !IsEmptyText(child.InnerHtml))
					{
						if (para == null)
						{
							para = cell.AppendChild(new Paragraph());
							ParagraphCreated(td, para);
						}
						
						Run run = para.AppendChild(new Run(new Text() {
							Text = ClearHtml(child.InnerHtml),
							Space = SpaceProcessingModeValues.Preserve
						}));
						
						RunCreated(child, run);
					}
					else
					{
						ProcessChild(child, cell, ref para);
					}
				}
			}
			
			//Cell must contain atleast one paragraph. Adding an empty paragraph if there is not html content
			if (!cell.Descendants<Paragraph>().Any())
			{
				cell.AppendChild(new Paragraph());
			}
			
			row.Append(cell);
		}
		
		private void ProcessVerticalSpan(ref int colIndex, TableRow row, DocxTableProperties docxProperties)
		{
			int rowSpan;
			
			docxProperties.RowSpanInfo.TryGetValue(colIndex, out rowSpan);
			
			while (rowSpan > 0)
			{
				TableCell cell = new TableCell();
				
				DocxTableCellStyle style = new DocxTableCellStyle();
				style.Process(colIndex, cell, docxProperties, docxProperties.RowSpanNode[colIndex]);
			
				if (cell.TableCellProperties == null)
				{
					cell.TableCellProperties = new TableCellProperties();
				}
				
				cell.TableCellProperties.Append(new VerticalMerge());
				
				cell.AppendChild(new Paragraph()).Append(new Run(new Text()));
			
				row.Append(cell);
				
				docxProperties.RowSpanInfo[colIndex] = --rowSpan;
				++colIndex;
				docxProperties.RowSpanInfo.TryGetValue(colIndex, out rowSpan);
			}
		}
		
		private void ProcessTr(IHtmlNode tr, Table table, DocxTableProperties docxProperties)
		{
			if (tr.HasChildren)
			{
				TableRow row = new TableRow();
				
				DocxTableRowStyle style = new DocxTableRowStyle();
				style.Process(row, docxProperties);
			
				int colIndex = 0;
				
				foreach (IHtmlNode td in tr.Children)
				{
					ProcessVerticalSpan(ref colIndex, row, docxProperties);
					
					docxProperties.IsCellHeader = string.Compare(td.Tag, DocxTableProperties.thName, StringComparison.InvariantCultureIgnoreCase) == 0;
					
					if (string.Compare(td.Tag, DocxTableProperties.tdName, StringComparison.InvariantCultureIgnoreCase) == 0 || docxProperties.IsCellHeader)
					{
						ProcessTd(colIndex++, td, row, docxProperties);
					}
				}
				
				if (colIndex < docxProperties.RowSpanInfo.Count)
				{
					ProcessVerticalSpan(ref colIndex, row, docxProperties);
				}
				
				table.Append(row);
			}
		}
		
		internal DocxTable(IOpenXmlContext context)
			: base(context)
		{
		}
		
		internal override bool CanConvert(IHtmlNode node)
		{
			return string.Compare(node.Tag, DocxTableProperties.tableName, StringComparison.InvariantCultureIgnoreCase) == 0;
		}
		
		internal override void Process(IHtmlNode node, OpenXmlElement parent, ref Paragraph paragraph)
		{
			if (node == null || parent == null || !CanConvert(node))
			{
				return;
			}
			
			paragraph = null;
			
			if (node.HasChildren)
			{
				Table table = new Table();
				DocxTableProperties docxProperties = new DocxTableProperties();
				
				docxProperties.FetchTableProperties(node);
				docxProperties.ApplyTableProperties(table, node);
				
				foreach (IHtmlNode tr in node.Children)
				{
					if (string.Compare(tr.Tag, DocxTableProperties.trName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						ProcessTr(tr, table, docxProperties);
					}
				}
				
				parent.Append(table);
			}
		}
	}
}
