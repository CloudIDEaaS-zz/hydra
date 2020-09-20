USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderType

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:38 PM
	Maps to class VisualStudioProvider.PDB.Headers.Type

	<References>
		<Reference>HeaderFile</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderType](
		[TypeId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[Kind] [nvarchar](255) NOT NULL,
		[IsDependent] [bit] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderType] PRIMARY KEY CLUSTERED
(
	[TypeId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
