USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderTypeAlias

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:39 PM
	Maps to class VisualStudioProvider.PDB.Headers.TypeAlias

	<References>
		<Reference>DeclarationContext</Reference>
		<Reference>QualifiedType</Reference>
		<Reference>Template</Reference>
		<Reference>HeaderFile</Reference>
		<Reference>TypeDefNameDecl</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderTypeAlias](
		[TypeAliasId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[TypeDefNameDeclId] [uniqueidentifier] NULL,
		[OwningDeclarationContextId] [uniqueidentifier] NULL,
		[QualifiedTypeId] [uniqueidentifier] NULL,
		[DescribedAliasTemplateId] [uniqueidentifier] NULL,
		[LocationIdentifier] [bigint] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderTypeAlias] PRIMARY KEY CLUSTERED
(
	[TypeAliasId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
