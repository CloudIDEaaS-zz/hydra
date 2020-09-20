USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderClassLayout

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:11 PM
	Maps to class VisualStudioProvider.PDB.Headers.ClassLayout

	<References>
		<Reference>VTableLayout</Reference>
		<Reference>HeaderFile</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderClassLayout](
		[ClassLayoutId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[LayoutVTableLayoutId] [uniqueidentifier] NULL,
		[ABI] [nvarchar](255) NOT NULL,
		[Alignment] [int] NOT NULL,
		[DataSize] [int] NOT NULL,
		[HasOwnVFPtr] [bit] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderClassLayout] PRIMARY KEY CLUSTERED
(
	[ClassLayoutId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
