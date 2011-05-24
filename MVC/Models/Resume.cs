using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Resume.Models
{
	public class Resume
	{
		public Developer Developer { get; set; }
		public Link[] Links { get; set; }
		public Contact[] Contact { get; set; }
		public Preamble Preamble { get; set; }
		public string[] Expertise { get; set; }
		public SkillsCatalog[] Catalog { get; set; }
		public ExperienceCatalog[] ExperienceTypes { get; set; }
	}

	public class Developer
	{
		public string Name { get; set; }
		public string City { get; set; }
		public string State { get; set; }
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

	public class Preamble
	{
		public string Title { get; set; }
		public string Content { get; set; }
	}

	public class SkillsCatalog
	{
		public string Name { get; set; }
		public bool Hidden { get; set; }
		public Skill[] Skills { get; set; }
	}

	public class Skill
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public string Href { get; set; }
		public bool Hidden { get; set; }
		public bool Important { get; set; }
	}

	public class ExperienceCatalog
	{
		public string Name { get; set; }
		public Experience[] Experiences { get; set; }
	}

	public class Experience
	{
		public string Start { get; set; }
		public string End { get; set; }
		public string Company { get; set; }
		public string Locale { get; set; }
		public string Description { get; set; }
		public Position[] Positions { get; set; }

		public string Dates
		{
			get
			{
				if (!string.IsNullOrEmpty(this.Start) && !string.IsNullOrEmpty(this.End))
					return string.Format("{0} - {1}", this.Start, this.End);
				else if (!string.IsNullOrEmpty(this.Start))
					return string.Format("{0} - current", this.Start);
				else
					return string.Empty;
			}
		}
	}

	public class Position
	{
		public string Title { get; set; }
		public string[] Accomplishments { get; set; }
		public SkillKey[] SkillKeys { get; set; }
	}

	public class SkillKey
	{
		public string Key { get; set; }
	}
}