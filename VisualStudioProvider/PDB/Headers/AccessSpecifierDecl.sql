USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderAccessSpecifierDecl

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:04 PM
	Maps to class VisualStudioProvider.PDB.Headers.AccessSpecifierDecl

	<References>
		<Reference>Class</Reference>
		<Reference>HeaderFile</Reference>
		<Reference>Declaration</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderAccessSpecifierDecl](
		[AccessSpecifierDeclId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[DeclarationId] [uniqueidentifier] NULL,
		[OwningClassId] [uniqueidentifier] NULL,
		[LocationIdentifier] [bigint] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderAccessSpecifierDecl] PRIMARY KEY CLUSTERED
(
	[AccessSpecifierDeclId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
