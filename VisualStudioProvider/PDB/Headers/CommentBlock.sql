USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderCommentBlock

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/5/2016 7:30:43 PM
	Maps to class VisualStudioProvider.PDB.Headers.CommentBlock

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>Comment</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderCommentBlock](
		[CommentBlockId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[CommentId] [uniqueidentifier] NULL,
		[Kind] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderCommentBlock] PRIMARY KEY CLUSTERED
(
	[CommentBlockId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
