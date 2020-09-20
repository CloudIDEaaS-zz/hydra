USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderTypeDefDecl

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:40 PM
	Maps to class VisualStudioProvider.PDB.Headers.TypeDefDecl

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>TypeDefNameDecl</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderTypeDefDecl](
		[TypeDefDeclId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[TypeDefNameDeclId] [uniqueidentifier] NULL,
		[LocationIdentifier] [bigint] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderTypeDefDecl] PRIMARY KEY CLUSTERED
(
	[TypeDefDeclId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
