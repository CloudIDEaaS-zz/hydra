<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:uml="http://schemas.mindchemistry.com/UML.xsd" 
    xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:template match ="/">
    <uml:ObjectTypes id="UML">
      <uml:Type TypeName="bool" MacroType="Bool">
        <uml:MapTo>
          <xsl:value-of select="Boolean"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="byte" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="Byte"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="sbyte" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="SByte"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="char" MacroType="Char">
        <uml:MapTo>
          <xsl:value-of select="Char"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="decimal" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="Decimal"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="double" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="Double"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="float" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="Single"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="int" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="Int32"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="uint" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="UInt32"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="long" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="Int64"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="ulong" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="UInt64"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="object" MacroType="Unknown">
        <uml:MapTo>
          <xsl:value-of select="Object"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="short" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="Int16"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="ushort" MacroType="Number">
        <uml:MapTo>
          <xsl:value-of select="UInt16"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string" MacroType="Text">
        <uml:MapTo>
          <xsl:value-of select="String"/>
        </uml:MapTo>
      </uml:Type>
    </uml:ObjectTypes>
  </xsl:template>
</xsl:stylesheet>