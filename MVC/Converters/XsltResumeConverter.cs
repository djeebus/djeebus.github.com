using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Web;

namespace Resume.Converters
{
	public abstract class XsltResumeConverter : IXmlResumeConverter
	{
		public abstract string XsltFilename { get; }
		XslCompiledTransform transformer;

		protected XsltResumeConverter()
		{
			string fullpath = HttpContext.Current.Server.MapPath(this.XsltFilename);

			transformer = new XslCompiledTransform(false);
			transformer.Load(fullpath);
		}

		public void WriteToStream(XmlDocument resume, Stream outputStream)
		{

			this.transformer.Transform(resume, null, outputStream);
		}
	}
}