USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderMacroDefinition

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/16/2016 12:33:13 PM
	Maps to class VisualStudioProvider.PDB.Headers.MacroDefinition

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>PreprocessedEntity</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderMacroDefinition](
		[MacroDefinitionId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[PreprocessedEntityId] [uniqueidentifier] NULL,
		[Expression] [nvarchar](255) NOT NULL,
		[Name] [nvarchar](255) NOT NULL,
		[LineNumberStart] [int] NOT NULL,
		[LineNumberEnd] [int] NOT NULL,
		[ColumnNumberStart] [int] NOT NULL,
		[ColumnNumberEnd] [int] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderMacroDefinition] PRIMARY KEY CLUSTERED
(
	[MacroDefinitionId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
