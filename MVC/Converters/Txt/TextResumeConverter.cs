using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Resume.Converters.Txt
{
	[ResumeConverter("txt", "Plain Text", "text/plain")]
	public class TextResumeConverter : XsltResumeConverter
	{
		public override string XsltFilename
		{
			get { return "~/App_Data/resume2txt.xslt"; }
		}
	}
}