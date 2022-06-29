<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:script="http://schemas.mindchemistry.com/Script.xsd"
    xmlns:uml="http://schemas.mindchemistry.com/UML.xsd">

  <xsl:template match ="/">
    <script:ScriptTypes id="Script">
      <script:Type TypeName="object">
        <script:MapTo>
          <xsl:value-of select="any"/>
        </script:MapTo>
      </script:Type>
      <script:Type TypeName="string">
        <script:MapTo>
          <xsl:value-of select="string"/>
        </script:MapTo>
      </script:Type>
      <script:Type TypeName="bool">
        <script:MapTo>
          <xsl:value-of select="boolean"/>
        </script:MapTo>
        <script:TypeValidator>
          value == true || value == false
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="float">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
        <script:TypeValidator>
          !isNaN(parseFloat('value'))
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="double">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
        <script:TypeValidator>
          !isNaN(parseFloat('value'))
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="decimal">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
        <script:TypeValidator>
          !isNaN(parseFloat('value'))
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="Date">
        <script:MapTo>
          <xsl:value-of select="date"/>
        </script:MapTo>
      </script:Type>
      <script:Type TypeName="DateTime">
        <script:MapTo>
          <xsl:value-of select="date"/>
        </script:MapTo>
      </script:Type>
      <script:Type TypeName="Guid">
        <script:MapTo>
          <xsl:value-of select="string"/>
        </script:MapTo>
      </script:Type>
      <script:Type TypeName="DateAndTime">
        <script:MapTo>
          <xsl:value-of select="date"/>
        </script:MapTo>
      </script:Type>
      <script:Type TypeName="Array">
        <script:MapTo>
          <xsl:value-of select="Array"/>
        </script:MapTo>
        <script:TypeValidator>
          new Array(value)
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="ICollection">
        <script:MapTo>
          <xsl:value-of select="Array"/>
        </script:MapTo>
        <script:TypeValidator>
          new Array(value)
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="int">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
      </script:Type>
      <script:Type TypeName="uint">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
        <script:TypeValidator>
          !isNaN(parseInt('value'))
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="long">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
        <script:TypeValidator>
          !isNaN(parseInt('value'))
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="ulong">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
        <script:TypeValidator>
          !isNaN(parseInt('value'))
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="short">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
        <script:TypeValidator>
          !isNaN(parseInt('value'))
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="ushort">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
        <script:TypeValidator>
          !isNaN(parseInt('value'))
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="byte">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
        <script:TypeValidator>
          !isNaN(parseInt('value'))
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="sbyte">
        <script:MapTo>
          <xsl:value-of select="number"/>
        </script:MapTo>
        <script:TypeValidator>
          !isNaN(parseInt('value'))
        </script:TypeValidator>
      </script:Type>
      <script:Type TypeName="char">
        <script:MapTo>
          <xsl:value-of select="String"/>
        </script:MapTo>
      </script:Type>
    </script:ScriptTypes>
  </xsl:template>
</xsl:stylesheet>