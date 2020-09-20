USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderBuiltinType

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:09 PM
	Maps to class VisualStudioProvider.PDB.Headers.BuiltinType

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>Type</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderBuiltinType](
		[BuiltinTypeId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[TypeId] [uniqueidentifier] NULL,
		[Type] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderBuiltinType] PRIMARY KEY CLUSTERED
(
	[BuiltinTypeId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
