using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Text;

namespace Resume.Converters
{
	public static class ResumeConversionFactory
	{
		static readonly Dictionary<string, ConversionInfo> converters =
				new Dictionary<string, ConversionInfo>();

		static ResumeConversionFactory()
		{
			Assembly a = Assembly.GetAssembly(typeof(ResumeConversionFactory));

			foreach (var t in a.GetTypes())
			{
				foreach (ResumeConverterAttribute attr in t.GetCustomAttributes(typeof(ResumeConverterAttribute), false))
				{
					converters.Add(attr.Format, new ConversionInfo(attr, t));
				}
			}
		}

		public static string GetJson()
		{
			var filename = HttpContext.Current.Server.MapPath("~/App_Data/resume.json");

			var jsonStream = System.IO.File.OpenRead(filename);

			string content;
			using (var reader = new StreamReader(jsonStream))
				content = reader.ReadToEnd();

			return content;
		}

		public static XmlDocument GetXmlDocument()
		{
			var json = GetJson();

			string content = string.Format("{{\"resume\": {0} }}", json);

			var doc = JsonConvert.DeserializeXmlNode(content);

			return doc;
		}

		private static Resume.Models.Resume GetResume()
		{
			return JsonConvert.DeserializeObject<Resume.Models.Resume>(GetJson());
		}

		public static ActionResult ConvertResume(string format)
		{
			if (string.IsNullOrEmpty(format))
				throw new ArgumentNullException("format");

			var conversionInfo = converters[format];
			if (conversionInfo == null)
				throw new ArgumentOutOfRangeException("format", format, "Unknown format");

			var ms = new MemoryStream();

			ConvertResumeToStream(conversionInfo, ms);

			return ConvertStreamToActionResult(conversionInfo, ms);
		}

		private static void ConvertResumeToStream(ConversionInfo conversionInfo, MemoryStream ms)
		{
			if (conversionInfo.Converter is IResumeConverter)
			{
				var conv = (IResumeConverter)conversionInfo.Converter;

				conv.WriteToStream(GetResume(), ms);
			}
			else if (conversionInfo.Converter is IXmlResumeConverter)
			{
				var conv = (IXmlResumeConverter)conversionInfo.Converter;

				conv.WriteToStream(GetXmlDocument(), ms);
			}
		}

		private static ActionResult ConvertStreamToActionResult(ConversionInfo conversionInfo, MemoryStream ms)
		{
			ms.Position = 0;

			var mimeType = conversionInfo.Metadata.MimeType;
			if (mimeType.StartsWith("text", StringComparison.CurrentCultureIgnoreCase))
			{
				using (ms)
				{
					using (var sr = new StreamReader(ms))
					{
						return new ContentResult
						{
							Content = sr.ReadToEnd(),
							ContentType = mimeType
						};
					}
				}
			}
			else
			{
				return new FileStreamResult(ms, mimeType)
				{
					FileDownloadName = string.Format("Joseph Lombrozo's Resume.{0}", conversionInfo.Metadata.Format)
				};
			}
		}

		private class ConversionInfo
		{
			public ResumeConverterAttribute Metadata { get; private set; }
			public object Converter { get; private set; }

			public ConversionInfo(ResumeConverterAttribute attribute, Type type)
			{
				this.Metadata = attribute;
				this.Converter = Activator.CreateInstance(type);
			}
		}
	}
}