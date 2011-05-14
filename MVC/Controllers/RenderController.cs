using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Xsl;
using System.Text;
using Newtonsoft.Json.Linq;
using Resume.Models;

namespace Resume.Controllers
{
	public class RenderController : Controller
	{
		private string GetJson()
		{
			var filename = this.Server.MapPath("~/App_Data/resume.json");

			var jsonStream = System.IO.File.OpenRead(filename);

			string content;
			using (var reader = new StreamReader(jsonStream))
				content = reader.ReadToEnd();

			return content;
		}

		private XmlDocument GetXmlDocument()
		{
			var json = this.GetJson();

			string content = string.Format("{{\"root\": {0} }}", json);

			var doc = JsonConvert.DeserializeXmlNode(content);

			return doc;
		}

		public ActionResult Index()
		{
			return this.View();
		}

		public ActionResult ToJson()
		{
			var json = this.GetJson();

			return this.Content(json, "text/plain");
		}

		public ActionResult ToXml()
		{
			var doc = GetXmlDocument();

			return this.Content(doc.InnerXml, "text/xml");
		}

		public ActionResult ToText()
		{
			string content = Transform("~/App_Data/resume2txt.xslt");

			return this.Content(content, "text/plain");
		}

		private string Transform(string xsltFilename)
		{
			string content;
			var xml = this.GetXmlDocument();

			StringBuilder sb = new StringBuilder();
			var writer = new StringWriter(sb);

			XslCompiledTransform t = new XslCompiledTransform();
			t.Load(this.Server.MapPath(xsltFilename));
			t.Transform(xml, null, writer);
			content = sb.ToString();
			return content;
		}

		public ActionResult ToHtml()
		{
			string content = Transform("~/App_Data/resume2html.xslt");

			return this.Content(content, "text/html");
		}

		//public ActionResult ToPdf()
		//{

		//}

		//public ActionResult ToDoc()
		//{
		//    return this.File(stream, "application/msword", "resume.doc");
		//}

		public ActionResult ToDocx()
		{
			var jsonText = this.GetJson();

			var result = JsonConvert.DeserializeObject<Resume.Models.Resume>(jsonText);

			var converter = new ResumeToDocx(result);

			var ms = converter.ToDocxStream();

			return this.File(ms, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "resume.docx");
		}
	}
}
