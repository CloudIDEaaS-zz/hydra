USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderHTMLTagComment

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:23 PM
	Maps to class VisualStudioProvider.PDB.Headers.HTMLTagComment

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>InlineContentComment</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderHTMLTagComment](
		[HTMLTagCommentId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[InlineContentCommentId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_tblSDKHeaderHTMLTagComment] PRIMARY KEY CLUSTERED
(
	[HTMLTagCommentId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
