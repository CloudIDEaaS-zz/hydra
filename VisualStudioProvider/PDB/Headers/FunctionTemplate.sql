USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderFunctionTemplate

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:19 PM
	Maps to class VisualStudioProvider.PDB.Headers.FunctionTemplate

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>Template</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderFunctionTemplate](
		[FunctionTemplateId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[TemplateId] [uniqueidentifier] NULL,
		[LocationIdentifier] [bigint] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderFunctionTemplate] PRIMARY KEY CLUSTERED
(
	[FunctionTemplateId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
