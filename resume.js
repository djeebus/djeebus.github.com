// enclosure to prevent dirtying up the global namespace
(function ($) {
	// local field to keep track of the resume view model
	var myResume;

	// provides a simple syntax for pulling querystring variables
	// copied from ... somewhere, don't remember
	var urlParams = {};
	(function () {
		var e,
			a = /\+/g,  // Regex for replacing addition symbol with a space
			r = /([^&=]+)=?([^&]*)/g,
			d = function (s) { return decodeURIComponent(s.replace(a, " ")); },
			q = window.location.search.substring(1);

		while (e = r.exec(q))
			urlParams[d(e[1])] = d(e[2]);
	})();

	$(document).ready(function () {
		reportMessage('Precompiling templates ... ');
		$('.resume-template').each(function () {
			var t = $(this);
			t.template(t.attr('id'));
		});

		reportMessage('Binding commands ... ');
		$('a[template-name]').each(function () {
			var a = $(this)
			var templateName = a.attr('template-name');
			a.attr('href', '?template=' + templateName);
			a.html(templateName);
		});
		$('a[template-name]').click(function () {
			var a = $(this)

			var templateName = a.attr('template-name');

			a.attr('href', 'javascript:void(0)');
			a.html(templateName);

			buildResumeUi(templateName);
		});
		$('#content').delegate('.toggle-skills', 'hover', function () {
			var parents = $(this).parents('.position-info')
			parents.toggleClass('show-skills-hover');
		});
		$('#content').delegate('.toggle-skills', 'click', function () {
			var parents = $(this).parents('.position-info')
			parents.toggleClass('show-skills-click');
		});
		$('#content').delegate('.skills-used-container .close-command a', 'click', function () {
			var parents = $(this).parents('.position-info')
			parents.removeClass('show-skills-click');
		});


		reportMessage('Fetching resume ... ');
		$.ajax({
			type: "GET",
			url: "resume.json",
			dataType: "json",
			cache: false,
			success: function (data) {
				try {
					myResume = data;

					reportMessage('Building view model ... ');
					buildViewModel(myResume);

					var t = urlParams.template; 	// get requested template type
					if (typeof t == 'undefined') {	// if no specific template is requested
						t = 'html'; 					// set default template type
					}

					reportMessage('Build resume');
					buildResumeUi(t);

					$('#status').hide();
				}
				catch (e) {
					reportMessage(e.toString());
				}
			},
			error: function (a, msg, c) {
				reportMessage(msg + ': ' + c);
			}
		});
	});

	function getParameterByName(name) {
		name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
		var regexS = "[\\?&]" + name + "=([^&#]*)";
		var regex = new RegExp(regexS);
		var results = regex.exec(window.location.href);
		if (results == null)
			return "";
		else
			return decodeURIComponent(results[1].replace(/\+/g, " "));
	}

	function reportMessage(msg) {
		$('#status').html(msg);
	}

	function buildResumeUi(templateName) {
		var content = $('#content');

		// clear original content
		content.empty();

		if (templateName == 'xml') {
			// convert json to xml
			var xml = $.json2xml(myResume, {
				rootTagName: 'resume',
				formatOutput: true
			});

			// escape invalid xml characters
			var escapedXml = xml.replace(/</g, '&lt;').replace(/>/g, '&gt;');

			// render xml in <pre /> tag to preserve spacing
			content.html('<pre>' + escapedXml + '</pre>');
		} else {
			// all other templates are simply jquery.tmpl templates
			var template = $.tmpl(templateName, myResume)
			template.appendTo("#content");

			if (templateName == 'text') {
				var version = getInternetExplorerVersion();
				if (version != -1) {
					// hack: internet explorer seems to double every breakline, 
					// so remove them and replace the broken content w/ the updated version
					var originalHtml = content.html();
					var updatedHtml = originalHtml.replace(/\r\n\r\n/g, '\r\n');
					content.html(updatedHtml);
				}
			}
		}
	}

	function buildViewModel(resume) {
		var skillsCatalog = resume.catalog;

		// utility function for finding full skill information
		skillsCatalog.findSkill = function (key) {
			for (var x = 0; x < this.length; x++) {
				var category = this[x];

				for (var y = 0; y < category.skills.length; y++) {
					var skill = category.skills[y];

					if (skill.key == key) {
						return {
							"key": skill.key,
							"name": skill.name,
							"category": category.name,
							"href": skill.href
						}
					}
				}
			}

			return { key: key, name: key, category: "Skills" };
		};

		var types = resume.experienceTypes;
		for (var tIndex = 0; tIndex < types.length; tIndex++) {
			var experiences = types[tIndex].experiences;
			for (var eIndex = 0; eIndex < experiences.length; eIndex++) {
				var experience = experiences[eIndex];

				if (experience.start || experience.end) {
					if (experience.start == experience.end) {
						experience.timespan = '' + experience.start;
					} else if (experience.end) {
						experience.timespan = '' + experience.start + ' - ' + experience.end;
					} else {
						experience.timespan = '' + experience.start + ' - current';
					}
				} else {
					experience.timespan = '';
				}

				if (typeof experience.positions != 'undefined') {
					for (var pIndex = 0; pIndex < experience.positions.length; pIndex++) {
						var position = experience.positions[pIndex];

						if (typeof position.skillKeys == 'undefined') {
							continue;
						}
						var skillKeys = position.skillKeys;
						position.categories = new Array();

						for (var sIndex = 0; sIndex < skillKeys.length; sIndex++) {
							var key = skillKeys[sIndex].key;
							var skill = skillsCatalog.findSkill(key);

							var category = null;
							for (var categoryIndex = 0; categoryIndex < position.categories.length; categoryIndex++) {
								if (position.categories[categoryIndex].name == skill.category) {
									category = position.categories[categoryIndex];
									break;
								}
							}

							if (category == null) {
								var category = { skills: new Array(), name: skill.category };
								position.categories[position.categories.length] = category;
							}

							category.skills[category.skills.length] = skill;
						}
					}
				}
			}
		}

		return resume;
	}

	// copied from http://msdn.microsoft.com/en-us/library/ms537509%28v=vs.85%29.aspx
	function getInternetExplorerVersion()
	// Returns the version of Internet Explorer or a -1
	// (indicating the use of another browser).
	{
		var rv = -1; // Return value assumes failure.
		if (navigator.appName == 'Microsoft Internet Explorer') {
			var ua = navigator.userAgent;
			var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
			if (re.exec(ua) != null)
				rv = parseFloat(RegExp.$1);
		}
		return rv;
	}
})(jQuery);