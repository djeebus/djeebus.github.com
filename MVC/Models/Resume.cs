using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Resume.Models
{
	public class Resume
	{
		public Developer developer { get; set; }
		public Link[] links { get; set; }
		public Contact[] contact { get; set; }
	}

	public class Developer
	{
		public string name { get; set; }
		public string city { get; set; }
		public string state { get; set; }
	}

	public class Link
	{
		public string Href { get; set; }
		public string Title { get; set; }
	}

	public class Contact
	{
		public string Method { get; set; }
		public string Value { get; set; }
	}
}