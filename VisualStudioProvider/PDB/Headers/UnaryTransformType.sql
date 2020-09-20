USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderUnaryTransformType

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:43 PM
	Maps to class VisualStudioProvider.PDB.Headers.UnaryTransformType

	<References>
		<Reference>QualifiedType</Reference>
		<Reference>QualifiedType</Reference>
		<Reference>HeaderFile</Reference>
		<Reference>Type</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderUnaryTransformType](
		[UnaryTransformTypeId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[TypeId] [uniqueidentifier] NULL,
		[DesugardedQualifiedTypeId] [uniqueidentifier] NULL,
		[BaseTypeQualifiedTypeId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_tblSDKHeaderUnaryTransformType] PRIMARY KEY CLUSTERED
(
	[UnaryTransformTypeId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
