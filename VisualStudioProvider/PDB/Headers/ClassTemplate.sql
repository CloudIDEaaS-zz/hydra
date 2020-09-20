USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderClassTemplate

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:12 PM
	Maps to class VisualStudioProvider.PDB.Headers.ClassTemplate

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>Template</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderClassTemplate](
		[ClassTemplateId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[TemplateId] [uniqueidentifier] NULL,
		[LocationIdentifier] [bigint] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderClassTemplate] PRIMARY KEY CLUSTERED
(
	[ClassTemplateId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
