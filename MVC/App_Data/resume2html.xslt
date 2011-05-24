<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
								xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
								xmlns="http://www.w3.org/1999/xhtml"
>
	<xsl:output method="html" indent="yes" />

	<xsl:template match="/root">
		<html>
			<head>
				<title>
					<xsl:value-of select="developer/name"/>
					<xsl:text>'s Resume</xsl:text>
				</title>
				<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.5.1/jquery.js" xml:space="preserve"> </script>
				<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.10/jquery-ui.js" xml:space="preserve"> </script>
				<!--<script type="text/javascript" src="resume.js"></script>-->
				<link type="text/css" rel="Stylesheet" href="/Content/site.css" />
			</head>

			<body>
				<div id="content" class="curved">
					<div class="header section">
						<ul class="contact-info">
							<xsl:for-each select="contact">
								<li>
									<span class="method">
										<xsl:choose>
											<xsl:when test="title">
												<xsl:value-of select="title"/>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="method"/>
											</xsl:otherwise>
										</xsl:choose>
									</span>

									<span class="value">
										<xsl:choose>
											<xsl:when test="method = 'email'">
												<a href="mailto:{value}">
													<xsl:value-of select="value"/>
												</a>
											</xsl:when>
											<xsl:when test="method = 'gtalk'">
												<a href="gtalk:chat?jid={value}">
													<xsl:value-of select="value"/>
												</a>
											</xsl:when>
											<xsl:when test="method = 'skype'">
												<xsl:value-of select="value"/>
												<span class="skype-commands">
													(<a href="skype:{value}?call">call</a> or <a href="skype:{value}?chat">chat</a>)
												</span>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="value"/>
											</xsl:otherwise>
										</xsl:choose>
									</span>
								</li>
							</xsl:for-each>
						</ul>

						<ul class="links">
							<xsl:for-each select="links">
								<li>
									<a href="{href}" target="_blank">
										<xsl:value-of select="title"/>
									</a>
								</li>
							</xsl:for-each>
						</ul>

						<ul class="basic-info">
							<li class="name">
								<xsl:value-of select="developer/name" />
							</li>
							<li class="location">
								<xsl:value-of select="developer/city" />
								<xsl:text>, </xsl:text>
								<xsl:value-of select="developer/state"/>
							</li>
						</ul>
					</div>

					<div class="preamble section">
						<h1>
							<xsl:value-of select="preamble/title" />
						</h1>
						<p>
							<xsl:value-of select="preamble/content" />
						</p>

						<h1>Areas of Expertise</h1>
						<ul id="important-accomplishments">
							<xsl:for-each select="expertise">
								<li>
									<xsl:value-of select="."/>
								</li>
							</xsl:for-each>
						</ul>
					</div>

					<div class="skills-container section">
						<h1>Skill Highlights</h1>
						<xsl:for-each select="catalog">
							<div>
								<xsl:attribute name="class">
									<xsl:text>skills-category</xsl:text>
									<xsl:if test="hidden">
										<xsl:text> deprecated</xsl:text>
									</xsl:if>
								</xsl:attribute>
								<h2>
									<xsl:value-of select="name"/>
								</h2>
								<ul>
									<xsl:for-each select="skills">
										<li skill-key="{key}">
											<xsl:attribute name="class">
												<xsl:text>skill-define </xsl:text>
												<xsl:value-of select="key"/>
												<xsl:if test="important">
													<xsl:text> important</xsl:text>
												</xsl:if>
												<xsl:if test="hidden">
													<xsl:text> deprecated</xsl:text>
												</xsl:if>
											</xsl:attribute>
											<xsl:choose>
												<xsl:when test="href">
													<a href="{href}" target="_blank">
														<xsl:value-of select="name"/>
													</a>
												</xsl:when>
												<xsl:otherwise>
													<xsl:value-of select="name"/>
												</xsl:otherwise>
											</xsl:choose>
										</li>
									</xsl:for-each>
								</ul>
							</div>
						</xsl:for-each>
					</div>

					<div class="experiences section">
						<xsl:for-each select="experienceTypes">
							<h1>
								<xsl:value-of select="name" />
							</h1>
							<xsl:for-each select="experiences">
								<div class="{type} experience">
									<h3>
										<span class="company">
											<xsl:value-of select="company" />
										</span>
										<xsl:if test="start or end">
											<span class="timespan">
												<xsl:value-of select="start"/>
												<xsl:text> - </xsl:text>
												<xsl:choose>
													<xsl:when test="end">
														<xsl:value-of select="end"/>
													</xsl:when>
													<xsl:otherwise>
														<xsl:text>current</xsl:text>
													</xsl:otherwise>
												</xsl:choose>
											</span>
										</xsl:if>
									</h3>
									<div class="company-details">
										<xsl:if test="locale">
											<span class="locale">
												<xsl:value-of select="locale" />
											</span>
										</xsl:if>
									</div>
									<p class="description">
										<xsl:value-of select="description" />
									</p>

									<xsl:for-each select="positions">
										<div class="position-info">
											<span class="title">
												<xsl:value-of select="title" />
												{{if categories}}<a class="toggle-skills">skills used</a>{{/if}}
											</span>
											<div class="skills-used-container curved">
												<div class="close-command">
													[<a>x</a>]
												</div>
												{{each categories}}
												<div class="skills-category">
													<h2>${name}</h2>
													<ul class="skills-used">
														{{each skills}}
														<li class="skill-use ${key}" skill-key="${key}">${name}</li>
														{{/each}}
													</ul>
												</div>
												{{/each}}
											</div>
											<p class="description">
												<xsl:value-of select="details" />
											</p>
											<ul class="accomplishments">
												<xsl:for-each select="accomplishments">
													<li>
														<xsl:value-of select="."/>
													</li>
												</xsl:for-each>
											</ul>
										</div>
									</xsl:for-each>
								</div>
							</xsl:for-each>
						</xsl:for-each>
					</div>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
