using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Resume.Converters.Xml
{
	[ResumeConverter("xml", "XML", "text/xml")]
	public class XmlResumeConverter : IXmlResumeConverter
	{
		public void WriteToStream(System.Xml.XmlDocument resume, System.IO.Stream outputStream)
		{
			var sw = new StreamWriter(outputStream);
			sw.Write(resume.OuterXml);
			sw.Flush();
		}
	}
}