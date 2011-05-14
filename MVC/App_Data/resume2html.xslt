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
				<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.5.1/jquery.js"></script>
				<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.10/jquery-ui.js"></script>
				<script type="text/javascript" src="resume.js"></script>
				<link type="text/css" rel="Stylesheet" href="resume.css" />
			</head>
		</html>
		<!-- name information -->
		<xsl:for-each select="developer">
			<xsl:value-of select="name"/>
			<xsl:call-template name="newline" />
			<xsl:value-of select="city"/>
			<xsl:text>, </xsl:text>
			<xsl:value-of select="state"/>
			<xsl:call-template name="newline" />
		</xsl:for-each>

		<xsl:call-template name="newline" />

		<!-- contact information -->
		<xsl:for-each select="contact">
			<xsl:value-of select="method"/>
			<xsl:text>: </xsl:text>
			<xsl:value-of select="value"/>
			<xsl:text>
</xsl:text>
		</xsl:for-each>

		<xsl:call-template name="newline" />

		<xsl:for-each select="links">
			<xsl:value-of select="title"/>
			<xsl:text>: </xsl:text>
			<xsl:value-of select="href"/>
			<xsl:call-template name="newline" />
		</xsl:for-each>

		<xsl:call-template name="newline" />

		<xsl:for-each select="preamble">
			<xsl:value-of select="title"/>
			<xsl:call-template name="newline" />
			<xsl:value-of select="content"/>
			<xsl:call-template name="newline" />
		</xsl:for-each>

		<xsl:call-template name="newline" />
		<xsl:text>AREAS OF EXPERTISE</xsl:text>
		<xsl:call-template name="newline" />

		<xsl:for-each select="expertise">
			<xsl:text>  * </xsl:text>
			<xsl:value-of select="."/>
			<xsl:call-template name="newline" />
		</xsl:for-each>

		<xsl:call-template name="newline" />
		<xsl:text>SKILL HIGHLIGHTS</xsl:text>
		<xsl:call-template name="newline" />

		<xsl:for-each select="catalog">
			<xsl:text>  </xsl:text>
			<xsl:value-of select="name"/>
			<xsl:text>: </xsl:text>

			<xsl:for-each select="skills[not(hidden = 'true')]">
				<xsl:if test="position() != 1">
					<xsl:text> | </xsl:text>
				</xsl:if>

				<xsl:value-of select="name"/>
			</xsl:for-each>

			<xsl:call-template name="newline" />
		</xsl:for-each>

		<xsl:call-template name="newline" />
		<xsl:for-each select="experienceTypes">
			<xsl:value-of select="name"/>
			<xsl:call-template name="newline" />
			<xsl:for-each select="experiences">
				<xsl:text>  </xsl:text>
				<xsl:value-of select="company"/>
				<xsl:if test="start">
					<xsl:text> (</xsl:text>
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
					<xsl:text>)</xsl:text>
				</xsl:if>
				<xsl:if test="locale">
					<xsl:text>     </xsl:text>
					<xsl:value-of select="locale"/>
				</xsl:if>
				<xsl:call-template name="newline" />
				<xsl:text>  </xsl:text>
				<xsl:value-of select="description"/>
				<xsl:for-each select="positions">
					<xsl:text>    </xsl:text>
					<xsl:value-of select="name"/>
					<xsl:call-template name="newline"/>
					<xsl:for-each select="accomplishments">
						<xsl:text>    * </xsl:text>
						<xsl:value-of select="."/>
						<xsl:call-template name="newline" />
					</xsl:for-each>
					<xsl:call-template name="newline" />
				</xsl:for-each>
			</xsl:for-each>
			<xsl:call-template name="newline" />
		</xsl:for-each>
	</xsl:template>





	<xsl:template name="newline">
		<xsl:text xml:space="preserve">
</xsl:text>


	</xsl:template>
</xsl:stylesheet>
