USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderBlockCommandCommentArgument

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:08 PM
	Maps to class VisualStudioProvider.PDB.Headers.BlockCommandCommentArgument

	<References>
		<Reference>BlockCommandComment</Reference>
		<Reference>HeaderFile</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderBlockCommandCommentArgument](
		[BlockCommandCommentArgumentId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[OwningBlockCommandCommentId] [uniqueidentifier] NULL,
		[Text] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderBlockCommandCommentArgument] PRIMARY KEY CLUSTERED
(
	[BlockCommandCommentArgumentId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
