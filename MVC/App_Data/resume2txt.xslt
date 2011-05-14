<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="text" indent="yes"/>

	<xsl:variable name="smallcase" select="'abcdefghijklmnopqrstuvwxyz'" />
	<xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />

	<!--
${developer.name}
${developer.city}, ${developer.state}
{{each contact}}
${method}: ${value}{{/each}}

${preamble.title.toUpperCase()}
${preamble.content}

AREAS OF EXPERTISE{{each expertise}}
  * ${this}{{/each}}

SKILL HIGHLIGHTS{{each catalog}}
  ${name.toUpperCase()}: {{each skills}}{{if !(typeof hidden == 'undefined' ? false : hidden)}}{{if $index > 0}} | {{/if}}${name}{{/if}}{{/each}}{{/each}}

{{each experienceTypes}}${name.toUpperCase()}
{{each experiences}}
  ${company.toUpperCase()} {{if timespan}}(${timespan}){{/if}}{{if locale}} ${locale}{{/if}}
{{if description}}  ${description}{{/if}}
{{each positions}}  ${title}{{if details}}
    ${details}{{/if}}
{{each accomplishments}}    * ${this}
{{/each}}{{/each}}{{/each}}
{{/each}}	-->
	<xsl:template match="/root">

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
			<xsl:value-of select="translate(title, $smallcase, $uppercase)"/>
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
			<xsl:value-of select="translate(name, $smallcase, $uppercase)"/>
			<xsl:call-template name="newline" />
			<xsl:for-each select="experiences">
				<xsl:text>  </xsl:text>
				<xsl:value-of select="translate(company, $smallcase, $uppercase)"/>
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
