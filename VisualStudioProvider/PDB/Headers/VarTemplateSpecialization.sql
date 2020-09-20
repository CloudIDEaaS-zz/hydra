USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderVarTemplateSpecialization

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:45 PM
	Maps to class VisualStudioProvider.PDB.Headers.VarTemplateSpecialization

	<References>
		<Reference>VarTemplate</Reference>
		<Reference>HeaderFile</Reference>
		<Reference>Variable</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderVarTemplateSpecialization](
		[VarTemplateSpecializationId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[VariableId] [uniqueidentifier] NULL,
		[TemplateDeclVarTemplateId] [uniqueidentifier] NULL,
		[LocationIdentifier] [bigint] NOT NULL,
		[SpecializationKind] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderVarTemplateSpecialization] PRIMARY KEY CLUSTERED
(
	[VarTemplateSpecializationId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
