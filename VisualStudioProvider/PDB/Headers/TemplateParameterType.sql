USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderTemplateParameterType

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:35 PM
	Maps to class VisualStudioProvider.PDB.Headers.TemplateParameterType

	<References>
		<Reference>IntegerValue</Reference>
		<Reference>IntegerValue</Reference>
		<Reference>TypeTemplateParameter</Reference>
		<Reference>HeaderFile</Reference>
		<Reference>Type</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderTemplateParameterType](
		[TemplateParameterTypeId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[TypeId] [uniqueidentifier] NULL,
		[DepthIntegerValueId] [uniqueidentifier] NULL,
		[IndexIntegerValueId] [uniqueidentifier] NULL,
		[ParameterTypeTemplateParameterId] [uniqueidentifier] NULL,
		[IsParameterPack] [bit] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderTemplateParameterType] PRIMARY KEY CLUSTERED
(
	[TemplateParameterTypeId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
