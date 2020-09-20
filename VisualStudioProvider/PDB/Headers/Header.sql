USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderHeader

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:21 PM
	Maps to class VisualStudioProvider.PDB.Headers.Header

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>Namespace</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderHeader](
		[HeaderId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[NamespaceId] [uniqueidentifier] NULL,
		[LocationIdentifier] [bigint] NOT NULL,
		[Name] [nvarchar](255) NOT NULL,
		[FileName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderHeader] PRIMARY KEY CLUSTERED
(
	[HeaderId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
