USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderComment

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:13 PM
	Maps to class VisualStudioProvider.PDB.Headers.Comment

	<References>
		<Reference>Declaration</Reference>
		<Reference>HeaderFile</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderComment](
		[CommentId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[OwningDeclarationId] [uniqueidentifier] NULL,
		[Kind] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderComment] PRIMARY KEY CLUSTERED
(
	[CommentId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
