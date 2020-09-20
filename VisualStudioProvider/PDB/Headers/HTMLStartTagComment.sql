USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderHTMLStartTagComment

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:22 PM
	Maps to class VisualStudioProvider.PDB.Headers.HTMLStartTagComment

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>HTMLTagComment</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderHTMLStartTagComment](
		[HTMLStartTagCommentId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[HTMLTagCommentId] [uniqueidentifier] NULL,
		[TagName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderHTMLStartTagComment] PRIMARY KEY CLUSTERED
(
	[HTMLStartTagCommentId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
