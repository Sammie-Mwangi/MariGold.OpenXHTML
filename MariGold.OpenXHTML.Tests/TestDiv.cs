﻿namespace MariGold.OpenXHTML.Tests
{
	using System;
	using NUnit.Framework;
	using MariGold.OpenXHTML;
	using System.IO;
	using DocumentFormat.OpenXml.Wordprocessing;
	using Word = DocumentFormat.OpenXml.Wordprocessing;
	using DocumentFormat.OpenXml.Validation;
	using System.Linq;
	
	[TestFixture]
	public class TestDiv
	{
		[Test]
		public void SingleDivPercentageFontSize()
		{
			using (MemoryStream mem = new MemoryStream())
			{
				WordDocument doc = new WordDocument(mem);
			
				doc.Process(new HtmlParser("<div style='font-size:100%'>test</div>"));
				
				Assert.IsNotNull(doc.Document.Body);
				Assert.AreEqual(1, doc.Document.Body.ChildElements.Count);
				
				Paragraph paragraph = doc.Document.Body.ChildElements[0] as Paragraph;
				
				Run run = paragraph.ChildElements[0] as Run;
				Assert.IsNotNull(run);
				Assert.AreEqual(2, run.ChildElements.Count);
				Assert.IsNotNull(run.RunProperties);
				FontSize fontSize = run.RunProperties.ChildElements[0] as FontSize;
				Assert.AreEqual("24", fontSize.Val.Value);
				
				Word.Text text = run.ChildElements[1] as Word.Text;
				Assert.IsNotNull(text);
				Assert.AreEqual(0, text.ChildElements.Count);
				Assert.AreEqual("test", text.InnerText);
				
				OpenXmlValidator validator = new OpenXmlValidator();
				var errors = validator.Validate(doc.WordprocessingDocument);
				Assert.AreEqual(0, errors.Count());
			}
		}
		
		[Test]
		public void SingleDivOneEmFontSize()
		{
			using (MemoryStream mem = new MemoryStream())
			{
				WordDocument doc = new WordDocument(mem);
			
				doc.Process(new HtmlParser("<div style='font-size:1em'>test</div>"));
				
				Assert.IsNotNull(doc.Document.Body);
				Assert.AreEqual(1, doc.Document.Body.ChildElements.Count);
				
				Paragraph paragraph = doc.Document.Body.ChildElements[0] as Paragraph;
				
				Run run = paragraph.ChildElements[0] as Run;
				Assert.IsNotNull(run);
				Assert.AreEqual(2, run.ChildElements.Count);
				Assert.IsNotNull(run.RunProperties);
				FontSize fontSize = run.RunProperties.ChildElements[0] as FontSize;
				Assert.AreEqual("24", fontSize.Val.Value);
				
				Word.Text text = run.ChildElements[1] as Word.Text;
				Assert.IsNotNull(text);
				Assert.AreEqual(0, text.ChildElements.Count);
				Assert.AreEqual("test", text.InnerText);
				
				OpenXmlValidator validator = new OpenXmlValidator();
				var errors = validator.Validate(doc.WordprocessingDocument);
				Assert.AreEqual(0, errors.Count());
			}
		}
		
		[Test]
		public void SingleDivXXLargeFontSize()
		{
			using (MemoryStream mem = new MemoryStream())
			{
				WordDocument doc = new WordDocument(mem);
			
				doc.Process(new HtmlParser("<div style='font-size:xx-large'>test</div>"));
				
				Assert.IsNotNull(doc.Document.Body);
				Assert.AreEqual(1, doc.Document.Body.ChildElements.Count);
				
				Paragraph paragraph = doc.Document.Body.ChildElements[0] as Paragraph;
				
				Run run = paragraph.ChildElements[0] as Run;
				Assert.IsNotNull(run);
				Assert.AreEqual(2, run.ChildElements.Count);
				Assert.IsNotNull(run.RunProperties);
				FontSize fontSize = run.RunProperties.ChildElements[0] as FontSize;
				Assert.AreEqual("48", fontSize.Val.Value);
				
				Word.Text text = run.ChildElements[1] as Word.Text;
				Assert.IsNotNull(text);
				Assert.AreEqual(0, text.ChildElements.Count);
				Assert.AreEqual("test", text.InnerText);
				
				OpenXmlValidator validator = new OpenXmlValidator();
				var errors = validator.Validate(doc.WordprocessingDocument);
				Assert.AreEqual(0, errors.Count());
			}
		}
		
		[Test]
		public void MarginDivAndWidthoutMarginDiv()
		{
			using (MemoryStream mem = new MemoryStream())
			{
				WordDocument doc = new WordDocument(mem);
			
				doc.Process(new HtmlParser("<div style='margin:5px'>1</div><div>2</div>"));
				
				Assert.IsNotNull(doc.Document.Body);
				Assert.AreEqual(2, doc.Document.Body.ChildElements.Count);
				
				Paragraph paragraph = doc.Document.Body.ChildElements[0] as Paragraph;
				Assert.IsNotNull(paragraph);
				Assert.AreEqual(2, paragraph.ChildElements.Count);
				
				ParagraphProperties paragraphProperties = paragraph.ChildElements[0] as ParagraphProperties;
				Assert.IsNotNull(paragraphProperties);
				Assert.AreEqual(2, paragraphProperties.ChildElements.Count);
				SpacingBetweenLines spacing = paragraphProperties.ChildElements[0] as SpacingBetweenLines;
				Assert.IsNotNull(spacing);
				Assert.AreEqual("100", spacing.Before.Value);
				Assert.AreEqual("100", spacing.After.Value);
				Indentation ind = paragraphProperties.ChildElements[1] as Indentation;
				Assert.IsNotNull(ind);
				Assert.AreEqual("100", ind.Left.Value);
				Assert.AreEqual("100", ind.Right.Value);
				
				Run run = paragraph.ChildElements[1] as Run;
				Assert.IsNotNull(run);
				Assert.AreEqual(1, run.ChildElements.Count);
				
				Word.Text text = run.ChildElements[0] as Word.Text;
				Assert.IsNotNull(text);
				Assert.AreEqual(0, text.ChildElements.Count);
				Assert.AreEqual("1", text.InnerText);
				
				paragraph = doc.Document.Body.ChildElements[1] as Paragraph;
				Assert.IsNotNull(paragraph);
				Assert.AreEqual(1, paragraph.ChildElements.Count);
				
				run = paragraph.ChildElements[0] as Run;
				Assert.IsNotNull(run);
				Assert.AreEqual(1, run.ChildElements.Count);
				
				text = run.ChildElements[0] as Word.Text;
				Assert.IsNotNull(text);
				Assert.AreEqual(0, text.ChildElements.Count);
				Assert.AreEqual("2", text.InnerText);
				
				OpenXmlValidator validator = new OpenXmlValidator();
				var errors = validator.Validate(doc.WordprocessingDocument);
				Assert.AreEqual(0, errors.Count());
			}
		}
	}
}