USE [SDKInterfaceLibrary]
GO

/*
	Table: tblSDKHeaderClassTemplatePartialSpecialization

	Generated automatically by DiaHeadersSqlTransformer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=09006a76b6b1ba0d, Author: u164225, Date: 10/13/2016 4:38:12 PM
	Maps to class VisualStudioProvider.PDB.Headers.ClassTemplatePartialSpecialization

	<References>
		<Reference>HeaderFile</Reference>
		<Reference>ClassTemplateSpecialization</Reference>
	</References>
*/


CREATE TABLE [dbo].[tblSDKHeaderClassTemplatePartialSpecialization](
		[ClassTemplatePartialSpecializationId] [uniqueidentifier] NOT NULL,
		[HeaderFileId] [uniqueidentifier] NOT NULL,
		[ClassTemplateSpecializationId] [uniqueidentifier] NULL,
		[LocationIdentifier] [bigint] NOT NULL,
 CONSTRAINT [PK_tblSDKHeaderClassTemplatePartialSpecialization] PRIMARY KEY CLUSTERED
(
	[ClassTemplatePartialSpecializationId] ASC
)

WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO
