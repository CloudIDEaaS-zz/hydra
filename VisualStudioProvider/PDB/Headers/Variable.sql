USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderVariable

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:43 PM
	Maps to class VisualStudioProvider.PDB.Headers.Variable

	<References>
		<Reference>DeclarationContext</Reference>
		<Reference>QualifiedType</Reference>
		<Reference>HeaderFile</Reference>
		<Reference>Declaration</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderVariable](
		[VariableId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[DeclarationId] [uniqueidentifier] NULL,
		[OwningDeclarationContextId] [uniqueidentifier] NULL,
		[QualifiedTypeId] [uniqueidentifier] NULL,
		[LocationIdentifier] [bigint] NOT NULL,
		[Mangled] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderVariable] PRIMARY KEY CLUSTERED
(
	[VariableId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
