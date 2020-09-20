USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderTextComment

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:37 PM
	Maps to class VisualStudioProvider.PDB.Headers.TextComment

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>InlineContentComment</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderTextComment](
		[TextCommentId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[InlineContentCommentId] [uniqueidentifier] NULL,
		[Text] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderTextComment] PRIMARY KEY CLUSTERED
(
	[TextCommentId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
