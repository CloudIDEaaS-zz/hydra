USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderParamCommandComment

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:29 PM
	Maps to class VisualStudioProvider.PDB.Headers.ParamCommandComment

	<References>
		<Reference>IntegerValue</Reference>
		<Reference>HeaderFile</Reference>
		<Reference>BlockCommandComment</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderParamCommandComment](
		[ParamCommandCommentId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[BlockCommandCommentId] [uniqueidentifier] NULL,
		[ParamIndexIntegerValueId] [uniqueidentifier] NULL,
		[Direction] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderParamCommandComment] PRIMARY KEY CLUSTERED
(
	[ParamCommandCommentId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
