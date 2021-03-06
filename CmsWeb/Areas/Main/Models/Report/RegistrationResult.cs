/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using CmsData.Registration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Collections;
using CmsData;
using UtilityExtensions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Diagnostics;
using CmsData.Codes;
using CmsWeb.Models;

namespace CmsWeb.Areas.Main.Models.Report
{
	public class RegistrationResult : ActionResult
	{
		private const float FLOAT_t1SpacingAfter = 20f;
		private Font monofont = FontFactory.GetFont(FontFactory.COURIER, 8);
		private Font boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
		private Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
		private Font smallfont = FontFactory.GetFont(FontFactory.HELVETICA, 8, new GrayColor(50));
		private Font xsmallfont = FontFactory.GetFont(FontFactory.HELVETICA, 7, new GrayColor(50));
		private PageEvent pageEvents = new PageEvent();
		private Document doc;
		private DateTime dt;
		private PdfContentByte dc;

		private object qid;
		private int? oid;
		public RegistrationResult(object id, int? oid)
		{
			qid = id;
			this.oid = oid;
		}

		private static void SetDefaults(PdfPTable t)
		{
			t.DefaultCell.SetLeading(2.0f, 1f);
			t.DefaultCell.Border = PdfPCell.NO_BORDER;
			t.LockedWidth = false;
		}
		public override void ExecuteResult(ControllerContext context)
		{
			var Response = context.HttpContext.Response;
			Response.ContentType = "application/pdf";
			Response.AddHeader("content-disposition", "filename=foo.pdf");

			dt = Util.Now;

			doc = new Document(PageSize.LETTER, 72, 72, 72, 72);
			var w = PdfWriter.GetInstance(doc, Response.OutputStream);
			w.PageEvent = pageEvents;
			doc.Open();
			dc = w.DirectContent;

			if (qid != null) // print using a query
			{
				var re = new Regex("((?<label>.*?:) (?<value>.*$))|(?<value0>.*$)", RegexOptions.Multiline);
				pageEvents.StartPageSet("Registration Report: {0:d}".Fmt(dt));
				var q2 = DbUtil.Db.PeopleQuery(qid);
				var q = from p in q2
						orderby p.Name2
						select new
						{
							p,
							h = p.Family.HeadOfHousehold,
							s = p.Family.HeadOfHouseholdSpouse,
							m = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == oid),
							o = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == oid).Organization
						};
				if (!q.Any())
					doc.Add(new Phrase("no data"));
				else
					foreach (var i in q)
					{
						Settings setting = null;
						if (i.o != null)
							setting = new Settings(i.o.RegSetting, DbUtil.Db, i.o.OrganizationId);
						var t1 = new PdfPTable(1);
						SetDefaults(t1);
						t1.AddCell(i.p.Name);
						t1.AddCell(i.p.PrimaryAddress);
						if (i.p.PrimaryAddress2.HasValue())
							t1.AddCell(i.p.PrimaryAddress2);
						t1.AddCell(i.p.CityStateZip);
						t1.AddCell(i.p.EmailAddress);
						if (i.p.HomePhone.HasValue())
							t1.AddCell(i.p.HomePhone.FmtFone("H"));
						if (i.p.CellPhone.HasValue())
							t1.AddCell(i.p.CellPhone.FmtFone("C"));
						t1.SpacingAfter = FLOAT_t1SpacingAfter;
						doc.Add(t1);

						var t2 = new PdfPTable(new float[] { 35, 65 });
						SetDefaults(t2);
						if (i.h != null
							&& i.h.PeopleId != i.p.PeopleId
							&& i.h.PositionInFamilyId == PositionInFamily.PrimaryAdult)
						{
							t2.AddCell(i.h.Name);
							if (i.h.CellPhone.HasValue())
								t2.AddCell(i.h.CellPhone.FmtFone("C"));
							else if (i.h.HomePhone.HasValue())
								t2.AddCell(i.h.HomePhone.FmtFone("H"));
							else
								t2.AddCell(" ");
						}
						if (i.s != null)
						{
							t2.AddCell(i.s.Name);
							if (i.s.CellPhone.HasValue())
								t2.AddCell(i.s.CellPhone.FmtFone("C"));
							else if (i.h.HomePhone.HasValue())
								t2.AddCell(i.s.HomePhone.FmtFone("H"));
							else
								t2.AddCell(" ");
						}
						t2.AddCell(" ");
						t2.AddCell(" ");

						var rr = GetRecRegOrTemp(i.p);

						t2.AddCell("Date of Birth");
						t2.AddCell(i.p.DOB);
						if (i.o == null || SettingVisible(setting, "AskSize"))
						{
							t2.AddCell("Shirt Size:");
							t2.AddCell(rr.ShirtSize);
						}
						t2.SpacingAfter = FLOAT_t1SpacingAfter;
						doc.Add(t2);

						if (rr.MedicalDescription.HasValue())
							doc.Add(new Phrase("Allergies or Medical Problems: " + rr.MedicalDescription));

						if (i.o == null || SettingVisible(setting, "AskTylenolEtc"))
						{
							var t4 = new PdfPTable(new float[] { 20, 80 });
							SetDefaults(t4);
							t4.AddCell("Tylenol:");
							t4.AddCell(rr.Tylenol == true ? "Yes" : "No");
							t4.AddCell("Advil:");
							t4.AddCell(rr.Advil == true ? "Yes" : "No");
							t4.AddCell("Robitussin:");
							t4.AddCell(rr.Robitussin == true ? "Yes" : "No");
							t4.AddCell("Maalox:");
							t4.AddCell(rr.Maalox == true ? "Yes" : "No");
							t4.SpacingAfter = FLOAT_t1SpacingAfter;
							doc.Add(t4);
						}
						var t5 = new PdfPTable(new float[] { 45, 55 });
						SetDefaults(t5);

						if (i.o == null || SettingVisible(setting, "AskEmContact"))
						{
							t5.AddCell("Emergency Contact:");
							t5.AddCell(rr.Emcontact);
							t5.AddCell("Emergency Phone:");
							t5.AddCell(rr.Emphone.FmtFone());
						}
						if (i.o == null || SettingVisible(setting, "AskInsurance"))
						{
							t5.AddCell("Health Insurance Carrier:");
							t5.AddCell(rr.Insurance);
							t5.AddCell("Policy #:");
							t5.AddCell(rr.Policy);
						}
						if (i.o == null || SettingVisible(setting, "AskDoctor"))
						{
							t5.AddCell("Family Physician Name:");
							t5.AddCell(rr.Doctor);
							t5.AddCell("Family Physician Phone:");
							t5.AddCell(rr.Docphone.FmtFone());
						}
						if (i.o == null || SettingVisible(setting, "AskParents"))
						{
							t5.AddCell("Mother's Name:");
							t5.AddCell(rr.Mname);
							t5.AddCell("Father's Name:");
							t5.AddCell(rr.Fname);
						}
						if (i.m != null && i.m.UserData != null)
						{
							var m = re.Match(i.m.UserData);
							while (m.Success)
							{
								var value0 = m.Groups["value0"];
								var value = m.Groups["value"];
								var label = m.Groups["label"];
								if (value0.Success)
								{
									t5.AddCell("");
									t5.AddCell(value0.Value);
								}
								else
								{
									t5.AddCell(label.Value);
									t5.AddCell(value.Value);
								}
								m = m.NextMatch();
							}
						}
						doc.Add(t5);
						if (i.m != null)
						{
							var groups = string.Join(", ", i.m.OrgMemMemTags.Select(om => om.MemberTag.Name).ToArray());
							doc.Add(new Paragraph("Groups: " + groups));
						}
						doc.Add(Chunk.NEXTPAGE);
					}
			}
			else
				doc.Add(new Phrase("no data"));
			pageEvents.EndPageSet();
			doc.Close();
		}
		private bool SettingVisible(Settings setting, string name)
		{
			if (setting != null)
				return setting.AskVisible(name);
			return false;
		}

		private RecReg GetRecRegOrTemp(Person p)
		{
			var rr = p.RecRegs.SingleOrDefault();
			if (rr == null)
				rr = new RecReg();
			return rr;
		}

		class PageEvent : PdfPageEventHelper
		{
			private PdfTemplate npages;
			private PdfWriter writer;
			private Document document;
			private PdfContentByte dc;
			private BaseFont font;
			private string HeadText;

			public override void OnOpenDocument(PdfWriter writer, Document document)
			{
				this.writer = writer;
				this.document = document;
				base.OnOpenDocument(writer, document);
				font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
				dc = writer.DirectContent;
			}
			public void EndPageSet()
			{
				if (npages == null)
					return;
				npages.BeginText();
				npages.SetFontAndSize(font, 8);
				npages.ShowText((writer.PageNumber + 1).ToString());
				npages.EndText();
			}
			public void StartPageSet(string header1)
			{
				EndPageSet();
				document.NewPage();
				document.ResetPageCount();
				this.HeadText = header1;
				npages = dc.CreateTemplate(50, 50);
			}
			public override void OnEndPage(PdfWriter writer, Document document)
			{
				base.OnEndPage(writer, document);

				string text;
				float len;

				//---Header left
				text = HeadText;
				const float HeadFontSize = 11f;
				len = font.GetWidthPoint(text, HeadFontSize);
				dc.BeginText();
				dc.SetFontAndSize(font, HeadFontSize);
				dc.SetTextMatrix(30, document.PageSize.Height - 30);
				dc.ShowText(text);
				dc.EndText();

				//---Column 1
				text = "Page " + (writer.PageNumber + 1) + " of ";
				len = font.GetWidthPoint(text, 8);
				dc.BeginText();
				dc.SetFontAndSize(font, 8);
				dc.SetTextMatrix(30, 30);
				dc.ShowText(text);
				dc.EndText();
				dc.AddTemplate(npages, 30 + len, 30);

				//---Column 2
				text = HeadText;
				len = font.GetWidthPoint(text, 8);
				dc.BeginText();
				dc.SetFontAndSize(font, 8);
				dc.SetTextMatrix(document.PageSize.Width / 2 - len / 2, 30);
				dc.ShowText(text);
				dc.EndText();

				//---Column 3
				text = Util.Now.ToShortDateString();
				len = font.GetWidthPoint(text, 8);
				dc.BeginText();
				dc.SetFontAndSize(font, 8);
				dc.SetTextMatrix(document.PageSize.Width - 30 - len, 30);
				dc.ShowText(text);
				dc.EndText();
			}
		}
	}
}

