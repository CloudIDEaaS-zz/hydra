<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:uml="http://schemas.mindchemistry.com/UML.xsd" 
    xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:template match ="/">
    <uml:ObjectTypes id="UML">
      <uml:Type TypeName="object" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='string']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="bool" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='boolean']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="float" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='float']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="double" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='double']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="decimal" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='decimal']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="TimeSpan" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='duration']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="DateTime" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='dateTime']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="DateTime">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='time']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="DateTime">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='date']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="Guid">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='ID']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="int">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='gYearMonth']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="int">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='gYear']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="int">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='gMonthDay']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="int">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='gDay']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="int">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='gMonth']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="byte[]">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='hexBinary']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="byte[]" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='base64Binary']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='anyURI']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='QName']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='NOTATION']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='normalizedString']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='token']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='language']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='IDREFS']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='ENTITIES']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='NMTOKEN']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='NMTOKENS']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='Name']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='NCName']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="string">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='ENTITY']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="int">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='integer']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="int">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='nonPositiveInteger']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="int">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='negativeInteger']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="long" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='long']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="int" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='int']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="short" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='short']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="byte" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='byte']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="uint">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='nonNegativeInteger']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="ulong" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='unsignedLong']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="uint" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='unsignedInt']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="ushort" Preferred="true">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='unsignedShort']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="byte">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='unsignedByte']"/>
        </uml:MapTo>
      </uml:Type>
      <uml:Type TypeName="uint">
        <uml:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='positiveInteger']"/>
        </uml:MapTo>
      </uml:Type>
    </uml:ObjectTypes>
  </xsl:template>
</xsl:stylesheet>