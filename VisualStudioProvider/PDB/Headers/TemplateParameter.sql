USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderTemplateParameter

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:34 PM
	Maps to class VisualStudioProvider.PDB.Headers.TemplateParameter

	<References>
		<Reference>IntegerValue</Reference>
		<Reference>IntegerValue</Reference>
		<Reference>HeaderFile</Reference>
		<Reference>Declaration</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderTemplateParameter](
		[TemplateParameterId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[DeclarationId] [uniqueidentifier] NULL,
		[DepthIntegerValueId] [uniqueidentifier] NULL,
		[IndexIntegerValueId] [uniqueidentifier] NULL,
		[LocationIdentifier] [bigint] NOT NULL,
		[IsParameterPack] [bit] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderTemplateParameter] PRIMARY KEY CLUSTERED
(
	[TemplateParameterId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
