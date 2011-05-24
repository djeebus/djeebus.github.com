using System;
using System.Linq;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

namespace Resume.Converters.Docx
{
	[ResumeConverter("docx", "Microsoft Word 2010", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
	public class DocxResumeConverter : IResumeConverter
	{
		public void WriteToStream(Models.Resume resume, System.IO.Stream outputStream)
		{
			using (var doc = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document, true))
			{
				var main = doc.AddMainDocumentPart();
				main.Document = new Document();
				main.Document.Body = new Body();

				var body = main.Document.Body;

				BuildStyles(main);

				BuildHeader(resume, doc);

				BuildPreamble(resume, doc);

				BuildSkillHighlights(resume, doc);

				BuildExperience(resume, doc);

				body.AppendChild<SectionProperties>(new SectionProperties(new PageMargin { Left = 360, Right = 360, Top = 360, Bottom = 360 }));

				doc.Close();
			}
		}

		int positionId = 20;
		private void BuildExperience(Models.Resume resume, WordprocessingDocument doc)
		{
			var body = doc.MainDocumentPart.Document.Body;

			foreach (var experienceCatalog in resume.ExperienceTypes)
			{
				body.AppendChild(
					new Paragraph(
						new ParagraphProperties(
							new ParagraphStyleId { Val = h1TextStyleId },
							new Justification { Val = JustificationValues.Center },
							new OutlineLevel { Val = 1 }),
						new Run(new Text(experienceCatalog.Name))));

				foreach (var experience in experienceCatalog.Experiences)
				{
					var table = body.AppendChild(
						new Table(
							new TableRow(
								new TableCell(
									CreateCellProperties(TableWidthUnitValues.Pct, "60"),
									new Paragraph(
										new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId { Val = h2TextStyleId } },
										new Run(new Text(experience.Company)))),
								new TableCell(
									CreateCellProperties(TableWidthUnitValues.Pct, "40"),
									new Paragraph(
										new ParagraphProperties(
											new ParagraphStyleId { Val = h2TextStyleId },
											new Justification { Val = JustificationValues.Right }),
										new Run(new Text(experience.Dates)))))));

					if (!string.IsNullOrEmpty(experience.Locale))
					{
						body.AppendChild(
							new Paragraph(
								new ParagraphProperties(
									new FontSize { Val = "11" }),
								new Run(new Text(experience.Locale))));
					}

					if (!string.IsNullOrEmpty(experience.Description))
					{
						body.AppendChild(
							new Paragraph(
								new ParagraphProperties(
									new Indentation { Left = "720" }),
								new Run(new Text(experience.Description))));
					}

					foreach (var position in experience.Positions)
					{
						body.AppendChild(
							new Paragraph(
								new ParagraphProperties(new SpacingBetweenLines { After = "0" })
								{
									ParagraphStyleId = new ParagraphStyleId { Val = h3TextStyleId }
								},
								new Run(new Text(position.Title))));

						if (position.Accomplishments != null)
						{
							foreach (var accomplishment in position.Accomplishments)
							{
								body.AppendChild(
									new Paragraph(
										new ParagraphProperties(
											new Indentation { Left = "1200" }),
										new Run(new Text(accomplishment))));
							}
						}

						positionId++;
					}
				}
			}
		}

		private void BuildSkillHighlights(Models.Resume resume, WordprocessingDocument doc)
		{
			var body = doc.MainDocumentPart.Document.Body;

			body.AppendChild(
				new Paragraph(
					new ParagraphProperties(
						new ParagraphStyleId { Val = h1TextStyleId },
						new Justification { Val = JustificationValues.Center }),
					new Run(new Text("Skill Highlights"))));

			var table = body.AppendChild(new Table());
			TableRow row = null;
			var index = 0;
			foreach (var catalog in resume.Catalog)
			{
				if (catalog.Hidden)
					continue;

				if (index % 4 == 0) // 4 columns
					row = table.AppendChild(new TableRow());

				var cell = row.AppendChild(new TableCell());
				var p = cell.AppendChild(new Paragraph());

				var h3 = p.AppendChild(new Run(new RunProperties(new Bold()), new Text(catalog.Name)));

				foreach (var skill in catalog.Skills)
				{
					if (skill.Hidden)
						continue;

					p.AppendChild(new Break());
					if (!string.IsNullOrEmpty(skill.Href))
						p.AppendChild(CreateHyperlink(skill.Name, skill.Href, doc.MainDocumentPart));
					else
						p.AppendChild(new Run(new Text(skill.Name)));
				}

				index++;
			}
		}

		private void BuildPreamble(Models.Resume resume, WordprocessingDocument doc)
		{
			var body = doc.MainDocumentPart.Document.Body;

			body.AppendChild(
				new Paragraph(
					new ParagraphProperties(
						new ParagraphStyleId { Val = h1TextStyleId },
						new OutlineLevel { Val = 0 }, 
						new Justification { Val = JustificationValues.Center }),
					new Run(
						new Text(resume.Preamble.Title))));

			body.AppendChild(new Paragraph(new Run(new Text(resume.Preamble.Content))));

			body.AppendChild(
				new Paragraph(
					new ParagraphProperties(
						new ParagraphStyleId { Val = h1TextStyleId },
						new OutlineLevel { Val = 0 },
						new Justification { Val = JustificationValues.Center }),
					new Run(new Text("Areas of Expertise"))));

			int index = 0;
			var expertiseTable = body.AppendChild(new Table());
			expertiseTable.AppendChild(HideBorders());

			TableRow row = expertiseTable.AppendChild(new TableRow());
			TableCell cell1 = row.AppendChild(new TableCell(CreateCellProperties(TableWidthUnitValues.Pct, "50"))), 
					  cell2 = row.AppendChild(new TableCell(CreateCellProperties(TableWidthUnitValues.Pct, "50")));

			Paragraph p1 = cell1.AppendChild(new Paragraph()),
					  p2 = cell2.AppendChild(new Paragraph());

			foreach (var e in resume.Expertise)
			{
				Paragraph p;
				if (index % 2 == 0) // 2 columns
					p = p1;
				else
					p = p2;

				if (index >= 2)
					p.AppendChild(new Break());

				p.AppendChild(new Run(new Text(e)));

				index++;
			}
		}

		const string h1TextStyleId = "h1";
		const string h2TextStyleId = "h2";
		const string h3TextStyleId = "h3";

		private static void BuildStyles(MainDocumentPart main)
		{
			var stylesPart = main.AddNewPart<StyleDefinitionsPart>();
			stylesPart.Styles = new Styles();
			stylesPart.Styles.DocDefaults = new DocDefaults();

			stylesPart.Styles.DocDefaults.Append(
				new ParagraphPropertiesDefault(
					new SpacingBetweenLines { After = "0" })
			);
			
			stylesPart.Styles.DocDefaults.Append(
				new RunPropertiesDefault(
					new RunFonts { Ascii = "Verdana", HighAnsi = "Verdana", ComplexScript = "Verdana" },
					new FontSize { Val = "22" })
			);

			stylesPart.Styles.Append(new Style
			{
				StyleId = h1TextStyleId,
				StyleRunProperties = new StyleRunProperties(
						new FontSize { Val = "48" },
						new Bold { Val = true })
			});

			stylesPart.Styles.Append(new Style
			{
				StyleId = h2TextStyleId,
				StyleRunProperties = new StyleRunProperties(
						new FontSize { Val = "33" },
						new Bold { Val = true })
			});

			stylesPart.Styles.Append(new Style
			{
				StyleId = h3TextStyleId,
				StyleRunProperties = new StyleRunProperties(
						new Bold { Val = true })
			});
		}

		private void BuildHeader(Resume.Models.Resume resume, WordprocessingDocument document)
		{
			var body = document.MainDocumentPart.Document.Body;

			var table = body.AppendChild(new Table());
			table.AppendChild(HideBorders());

			var row = table.AppendChild(new TableRow());

			var cell1 = BuildContactCell(resume, document.MainDocumentPart);

			var cell2 = new TableCell(
				CreateCellProperties(TableWidthUnitValues.Pct, "33"),
				new Paragraph(
					new ParagraphProperties(AlignParagraph(JustificationValues.Center)),
					new Run(new Text(resume.Developer.Name)),
					new Break(),
					new Run(new Text(string.Format("{0}, {1}", resume.Developer.City, resume.Developer.State)))));

			var cell3 = BuildLinksCell(resume, document.MainDocumentPart);

			row.Append(cell1, cell2, cell3);
		}

		private static TableProperties HideBorders()
		{
			return new TableProperties(
				new TableBorders(
					new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
					new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
					new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
					new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
					new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
					new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.None) }));
		}

		private TableCell BuildLinksCell(Resume.Models.Resume resume, MainDocumentPart main)
		{
			var parts = (from l in resume.Links
						 select CreateHyperlink(l.Title, l.Href, main)).ToArray();

			var paragraph = new Paragraph(new ParagraphProperties(AlignParagraph(JustificationValues.Right)));

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

		private static Hyperlink CreateHyperlink(string text, string uri, MainDocumentPart main)
		{
			var hr = main.AddHyperlinkRelationship(new Uri(uri), true);

			return new Hyperlink(
				new Run(
					new RunProperties(
						new RunPropertiesBaseStyle(
							new Underline { Val = new EnumValue<UnderlineValues>(UnderlineValues.Single) },
							new Color { Val = "0000FF" })),
					new Text(text)))
			{
				Id = hr.Id,
				TargetFrame = "_blank"
			};
		}

		private OpenXmlElement AlignParagraph(JustificationValues align)
		{
			return new Justification { Val = new EnumValue<JustificationValues>(align) };
		}

		private TableCell BuildContactCell(Resume.Models.Resume resume, MainDocumentPart main)
		{
			var cells = (from c in resume.Contact
						 select CreateContactItem(c, main)).ToArray();

			var paragraph = new Paragraph(new ParagraphProperties(AlignParagraph(JustificationValues.Left)));

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

		private IEnumerable<OpenXmlElement> CreateContactItem(Models.Contact c, MainDocumentPart main)
		{
			yield return new Run(new Text(string.Format("{0}: ", c.Method)) { Space = PreserveSpace() });

			switch (c.Method)
			{
				case "email":
					yield return CreateHyperlink(c.Value, string.Format("mailto:{0}", c.Value), main);
					yield break;

				case "aim":
					yield return CreateHyperlink(c.Value, string.Format("aim:goim?screenname={0}", c.Value), main);
					yield break;

				case "gtalk": // gtalk:chat?jid=digitaljeebus@gmail.com
					yield return CreateHyperlink(c.Value, string.Format("gtalk:chat?jid={0}", c.Value), main);
					yield break;

				case "skype":
					yield return new Run(new Text(string.Format("{0} (", c.Value)));
					yield return CreateHyperlink("call", string.Format("skype:{0}?call", c.Value), main);
					yield return new Run(new Text(" or ") { Space = PreserveSpace() });
					yield return CreateHyperlink("chat", string.Format("skype:{0}?chat", c.Value), main);
					yield return new Run(new Text(")"));
					yield break;

				default:
					yield return new Run(new Text(c.Value));
					yield break;
			}
		}

		private static EnumValue<SpaceProcessingModeValues> PreserveSpace()
		{
			return new EnumValue<SpaceProcessingModeValues>(SpaceProcessingModeValues.Preserve);
		}

		private TableCellProperties CreateCellProperties(TableWidthUnitValues unit, string width)
		{
			return new TableCellProperties(new TableCellWidth { Type = unit, Width = width });
		}

	}
}