using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Resume.Converters.Html
{
	[ResumeConverter("html", "Webpage", "text/html")]
	public class HtmlResumeConverter : XsltResumeConverter
	{
		public override string XsltFilename
		{
			get { return "~/App_Data/resume2html.xslt"; }
		}
	}
}