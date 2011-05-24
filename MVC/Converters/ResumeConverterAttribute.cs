using System;

namespace Resume
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
	public class ResumeConverterAttribute : Attribute
	{
		public string Format { get; private set; }
		public string Name { get; private set; }

		public string MimeType { get; private set; }

		public ResumeConverterAttribute(string format, string name, string mimeType)
		{
			this.Format = format;
			this.Name = name;
			this.MimeType = mimeType;
		}
	}
}