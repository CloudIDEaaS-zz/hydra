USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderInlineCommandCommentArgument

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:24 PM
	Maps to class VisualStudioProvider.PDB.Headers.InlineCommandCommentArgument

	<References>
		<Reference>InlineCommandComment</Reference>
		<Reference>HeaderFile</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderInlineCommandCommentArgument](
		[InlineCommandCommentArgumentId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[OwningInlineCommandCommentId] [uniqueidentifier] NULL,
		[Text] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderInlineCommandCommentArgument] PRIMARY KEY CLUSTERED
(
	[InlineCommandCommentArgumentId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
