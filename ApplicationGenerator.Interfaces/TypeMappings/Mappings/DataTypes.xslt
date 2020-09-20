<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:data="http://schemas.mindchemistry.com/Data.xsd" 
    xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:template match ="/">
    <data:SQLDataTypes id="SQLDataTypes">
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='string']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="bit">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='boolean']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="float">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='float']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="real">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='double']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="decimal">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='decimal']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="datetime">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='duration']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="datetime">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='dateTime']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="datetime">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='time']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="datetime">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='date']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='gYearMonth']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='gYear']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='gMonthDay']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='gDay']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='gMonth']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="binary" NeedSize="true" DefaultSize="8000">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='hexBinary']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="binary" NeedSize="true" DefaultSize="8000">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='base64Binary']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='anyURI']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='QName']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='NOTATION']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='normalizedString']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='token']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='language']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='IDREFS']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='ENTITIES']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='NMTOKEN']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='NMTOKENS']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='Name']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='NCName']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='ID']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='IDREF']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="varchar" NeedSize="true" DefaultSize="255">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='ENTITY']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='integer']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='nonPositiveInteger']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='negativeInteger']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="bigint">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='long']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='int']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="smallint">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='short']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="tinyint">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='byte']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='nonNegativeInteger']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="long">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='unsignedLong']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='unsignedInt']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="smallint">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='unsignedShort']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="tinyint">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='unsignedByte']"/>
        </data:MapTo>
      </data:Type>
      <data:Type TypeName="int">
        <data:MapTo>
          <xsl:value-of select="/xs:schema/xs:simpleType[@name='positiveInteger']"/>
        </data:MapTo>
      </data:Type>
    </data:SQLDataTypes>
  </xsl:template>
</xsl:stylesheet>