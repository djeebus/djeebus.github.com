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
using Resume.Converters;

namespace Resume.Controllers
{
	public class RenderController : Controller
	{
		public ActionResult Index()
		{
			return this.View();
		}

		public ActionResult Generic(string format)
		{
			switch (format)
			{
				case "json":
					return this.File("~/App_Data/resume.json", "application/json");

				default:
					return ResumeConversionFactory.ConvertResume(format);
			}
		}
	}
}
