USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderHTMLStartTagCommentAttribute

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:22 PM
	Maps to class VisualStudioProvider.PDB.Headers.HTMLStartTagCommentAttribute

	<References>
		<Reference>HTMLStartTagComment</Reference>
		<Reference>HeaderFile</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderHTMLStartTagCommentAttribute](
		[HTMLStartTagCommentAttributeId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[OwningHTMLStartTagCommentId] [uniqueidentifier] NULL,
		[Name] [nvarchar](255) NOT NULL,
		[Value] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderHTMLStartTagCommentAttribute] PRIMARY KEY CLUSTERED
(
	[HTMLStartTagCommentAttributeId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
