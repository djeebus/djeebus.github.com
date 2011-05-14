using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Resume.Models
{
	public class ResumeToDocx
	{
		const string normalTextStyleId = "normal";
		const string linkTextStyleId = "link";

		public Resume Resume { get; private set; }

		public ResumeToDocx(Resume resume)
		{
			this.Resume = resume;
		}

		public MemoryStream ToDocxStream()
		{
			MemoryStream ms = new MemoryStream();

			using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
			{
				var main = doc.AddMainDocumentPart();
				main.Document = new Document();
				main.Document.Body = new Body();

				var body = main.Document.Body;

				BuildStyles(main);

				BuildHeader(doc);

				body.AppendChild<SectionProperties>(new SectionProperties(new PageMargin { Left = 360, Right = 360, Top = 360, Bottom = 360 }));

				doc.Close();
			}

			ms.Position = 0;

			return ms;
		}

		private static void BuildStyles(MainDocumentPart main)
		{
			var stylesPart = main.AddNewPart<StyleDefinitionsPart>();
			stylesPart.Styles = new Styles();

			stylesPart.Styles.Append(new Style {
					StyleId = normalTextStyleId,
					StyleRunProperties = new StyleRunProperties(
						new RunFonts { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" },
						new FontSize { Val = "21" })
				},
				new Style {
					StyleId = linkTextStyleId,
					LinkedStyle = new LinkedStyle { Val = normalTextStyleId },
					StyleRunProperties = new StyleRunProperties(
						new Underline { Val = new EnumValue<UnderlineValues>(UnderlineValues.Single) },
						new Color { Val = "0000FF" }
					)
				});
		}

		private void BuildHeader(WordprocessingDocument document)
		{
			var body = document.MainDocumentPart.Document.Body;

			var table = body.AppendChild(new Table());
			table.AppendChild(new TableProperties(
				new TableBorders(
					new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
					new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
					new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
					new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
					new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
					new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.None) })));

			var row = table.AppendChild(new TableRow());

			var cell1 = BuildContactCell();

			var cell2 = new TableCell(
				CreateCellProperties(TableWidthUnitValues.Pct, "33"),
				new Paragraph(
					new ParagraphProperties(AlignParagraph(JustificationValues.Center)) { ParagraphStyleId = new ParagraphStyleId { Val = normalTextStyleId } },
					new Run(new Text(this.Resume.developer.name)),
					new Break(),
					new Run(new Text(string.Format("{0}, {1}", this.Resume.developer.city, this.Resume.developer.state)))));

			var cell3 = BuildLinksCell(document.MainDocumentPart);

			row.Append(cell1, cell2, cell3);
		}

		private TableCell BuildLinksCell(MainDocumentPart main)
		{
			var parts = (from l in this.Resume.links
						 let hr = main.AddHyperlinkRelationship(new Uri(l.Href), true)
						 select new Hyperlink(new Run(new RunProperties(new RunStyle { Val = linkTextStyleId }), new Text(l.Title))) { Id = hr.Id, TargetFrame = "_blank" }).ToArray();

			var paragraph = new Paragraph(new ParagraphProperties(AlignParagraph(JustificationValues.Right)) { ParagraphStyleId = new ParagraphStyleId { Val = normalTextStyleId } });

			for (int index = 0; index < parts.Length; index++)
			{
				if (index != 0)
					paragraph.Append(new Break());

				paragraph.Append(parts[index]);
			}

			return new TableCell(
				CreateCellProperties(TableWidthUnitValues.Pct, "33"),
				paragraph);
		}

		private OpenXmlElement AlignParagraph(JustificationValues align)
		{
			return new Justification { Val = new EnumValue<JustificationValues>(align) };
		}

		private TableCell BuildContactCell()
		{
			var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;

			var cells = (from c in this.Resume.contact
						 select new Run(new Text(string.Format("{0}: {1}", textInfo.ToTitleCase(c.Method), c.Value)))).ToArray();

			var paragraph = new Paragraph(new ParagraphProperties(AlignParagraph(JustificationValues.Left)) { ParagraphStyleId = new ParagraphStyleId { Val = normalTextStyleId } });

			for (int index = 0; index < cells.Length; index++)
			{
				if (index != 0)
					paragraph.Append(new Break());

				paragraph.Append(cells[index]);
			}

			return new TableCell(
				CreateCellProperties(TableWidthUnitValues.Pct, "33"),
				paragraph);
		}

		private TableCellProperties CreateCellProperties(TableWidthUnitValues unit, string width)
		{
			return new TableCellProperties(new TableCellWidth { Type = unit, Width = width });
		}
	}
}