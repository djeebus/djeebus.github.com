(function ($) {
	var myResume;

	$(document).ready(function () {
		// initialize the status bar
		var status = $('#status');

		reportMessage('Precompiling templates ... ');
		$('#html-template').template("html");
		$('#text-template').template("text");

		reportMessage('Binding commands ... ');
		$('.change-template').click(function () {
			buildResumeUi($(this).attr('template-name'));
		});
		status.html('Fetching resume ... ');

		$.ajax({
			type: "GET",
			url: "resume.json",
			dataType: "json",
			success: function (data) {
				try {
					myResume = data;
					buildViewModel(myResume);
					buildResumeUi('html');

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

	function reportMessage(msg) {
		$('#status').html(msg);
	}

	function buildResumeUi(template) {
		$('#content').empty();

		$.tmpl(template, myResume).appendTo("#content");

		if (template == 'html') {
			$('#content').delegate('.skill-define,.skill-use', 'hover', function () {
				var key = $(this).attr('skill-key');
				var skills = $('[skill-key="' + key + '"],.experience:has([skill-key="' + key + '"])')
				skills.toggleClass('skill-highlight');
			})
			.delegate('.toggle-skills', 'click', function () {
				var parents = $(this).parents('.position-info')
				parents.toggleClass('show-skills');
			})
			.delegate('.toggle-skills-catalog', 'click', function () {
				$('.skills-category').toggle();
			});
		}
	}

	function sortByName(a, b) {
		var xa = a.name.toUpperCase(), xb = b.name.toUpperCase();

		if (xa == xb) {
			return 0;
		} else if (xa < xb) {
			return -1;
		} else {
			return 1;
		}
	}

	function buildViewModel(resume) {
		var skillsCatalog = resume.catalog;

		skillsCatalog.sort(sortByName);

		for (var x = 0; x < skillsCatalog.length; x++) {
			skillsCatalog[x].skills.sort(sortByName);
		}

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

		resume.importantSkills = [];

		for (var cIndex = 0; cIndex < resume.catalog.length; cIndex++) {
			var c = resume.catalog[cIndex];
			for (var sIndex = 0; sIndex < c.skills.length; sIndex++) {
				var s = c.skills[sIndex];
				if (s.important) {
					resume.importantSkills[resume.importantSkills.length++] = s;
				}
			}
		}

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

							category.skills.sort(function (a, b) {
								if (a.name < b.name) {
									return -1;
								} else if (a.name == b.name) {
									return 0;
								} else {
									return 1;
								}
							});
						}
					}
				}
			}
		}

		return resume;
	}
})(jQuery);