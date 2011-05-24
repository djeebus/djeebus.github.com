using System.IO;
using System.Xml;

namespace Resume.Converters
{
	public interface IXmlResumeConverter
	{
		void WriteToStream(XmlDocument resume, Stream outputStream);
	}
}
