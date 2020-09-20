USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderExpression

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:17 PM
	Maps to class VisualStudioProvider.PDB.Headers.Expression

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>Statement</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderExpression](
		[ExpressionId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[StatementId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_tblSDKHeaderExpression] PRIMARY KEY CLUSTERED
(
	[ExpressionId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
