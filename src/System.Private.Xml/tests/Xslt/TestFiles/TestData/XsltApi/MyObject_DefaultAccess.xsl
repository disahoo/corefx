<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:myObj="urn:my-object">


    <xsl:template match="/">

	<result>
		Default:<xsl:value-of select="myObj:DefaultFunction()"/>
	</result>
	
    </xsl:template>
</xsl:stylesheet>