USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderIntegerValue

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:25 PM
	Maps to class VisualStudioProvider.PDB.Headers.IntegerValue

	<References>
		<Reference>HeaderFile</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderIntegerValue](
		[IntegerValueId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[Value] [bigint] NOT NULL,
		[StringValue] [nvarchar](255) NOT NULL,
		[ValueType] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderIntegerValue] PRIMARY KEY CLUSTERED
(
	[IntegerValueId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
