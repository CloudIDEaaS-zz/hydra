USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderVerbatimBlockLineComment

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:46 PM
	Maps to class VisualStudioProvider.PDB.Headers.VerbatimBlockLineComment

	<References>
		<Reference>Comment</Reference>
		<Reference>HeaderFile</Reference>
		<Reference>Comment</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderVerbatimBlockLineComment](
		[VerbatimBlockLineCommentId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[CommentId] [uniqueidentifier] NULL,
		[OwningCommentId] [uniqueidentifier] NULL,
		[Text] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderVerbatimBlockLineComment] PRIMARY KEY CLUSTERED
(
	[VerbatimBlockLineCommentId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
