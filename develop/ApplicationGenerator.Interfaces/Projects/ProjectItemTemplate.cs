// file:	ProjectItemTemplates\ProjectItemTemplate.cs
//
// summary:	Implements the project item template class

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AbstraX.Projects
{
    /// <summary>   A project item template. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/10/2022. </remarks>

    public class ProjectItemTemplate
    {
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="templateFile"> The template file. </param>

        public ProjectItemTemplate(string templateFile)
        {
            this.TemplateSourcePath = templateFile;
        }

        /// <summary>   A template data. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

        [XmlRoot(ElementName = "TemplateData", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
		public class TemplateData
		{
            /// <summary>   Gets or sets the default name. </summary>
            ///
            /// <value> The default name. </value>

            [XmlElement(ElementName = "DefaultName", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
            public string DefaultName { get; set; }

            /// <summary>   Gets or sets the name. </summary>
            ///
            /// <value> The name. </value>

            [XmlElement(ElementName = "Name", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
			public string Name { get; set; }

            /// <summary>   Gets or sets the description. </summary>
            ///
            /// <value> The description. </value>

			[XmlElement(ElementName = "Description", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
			public string Description { get; set; }

            /// <summary>   Gets or sets the icon. </summary>
            ///
            /// <value> The icon. </value>

			[XmlElement(ElementName = "Icon", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
			public string Icon { get; set; }

            /// <summary>   Gets or sets the type of the project. </summary>
            ///
            /// <value> The type of the project. </value>

			[XmlElement(ElementName = "ProjectType", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
			public string ProjectType { get; set; }
		}

        /// <summary>   A project. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

		[XmlRoot(ElementName = "Project", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
		public class Project
		{
            /// <summary>   Gets or sets the project item. </summary>
            ///
            /// <value> The project item. </value>

			[XmlElement(ElementName = "ProjectItem", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
			public List<ProjectItem> ProjectItems { get; set; }

            /// <summary>   Gets or sets the file. </summary>
            ///
            /// <value> The file. </value>

			[XmlAttribute(AttributeName = "File")]
			public string File { get; set; }
		}

        /// <summary>   A project item. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

        [XmlRoot(ElementName = "ProjectItem", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
        public class ProjectItem
        {
            /// <summary>   Gets or sets the type of the sub. </summary>
            ///
            /// <value> The type of the sub. </value>

            [XmlAttribute(AttributeName = "SubType")]
            public string SubType { get; set; }

            /// <summary>   Gets or sets the filename of the target file. </summary>
            ///
            /// <value> The filename of the target file. </value>

            [XmlAttribute(AttributeName = "TargetFileName")]
            public string TargetFileName { get; set; }

            /// <summary>   Gets or sets options for controlling the replace. </summary>
            ///
            /// <value> Options that control the replace. </value>

            [XmlAttribute(AttributeName = "ReplaceParameters")]
            public string ReplaceParameters { get; set; }

            /// <summary>   Gets or sets the filename of the source file. </summary>
            ///
            /// <value> The filename of the source file. </value>

            [XmlText]
            public string SourceFileName { get; set; }
        }

        /// <summary>   A template content. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

		[XmlRoot(ElementName = "TemplateContent", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
		public class TemplateContent
		{
            /// <summary>   Gets or sets the project item. </summary>
            ///
            /// <value> The project item. </value>

            [XmlElement(ElementName = "ProjectItem", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
            public List<ProjectItem> ProjectItems { get; set; }

            /// <summary>   Gets or sets the project. </summary>
            ///
            /// <value> The project. </value>

            [XmlElement(ElementName = "Project", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
			public Project Project { get; set; }
		}

        /// <summary>   A wizard extension. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

        [XmlRoot(ElementName = "WizardExtension", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
        public class WizardExtension
        {
            /// <summary>   Gets or sets the assembly. </summary>
            ///
            /// <value> The assembly. </value>

            [XmlElement(ElementName = "Assembly", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
            public string Assembly { get; set; }

            /// <summary>   Gets or sets the name of the full class. </summary>
            ///
            /// <value> The name of the full class. </value>

            [XmlElement(ElementName = "FullClassName", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
            public string FullClassName { get; set; }
        }

        /// <summary>   The vs template. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

        [XmlRoot(ElementName = "VSTemplate", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
		public class VSTemplate
		{
            /// <summary>   Gets or sets information describing the template. </summary>
            ///
            /// <value> Information describing the template. </value>

			[XmlElement(ElementName = "TemplateData", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
			public TemplateData TemplateData { get; set; }

            /// <summary>   Gets or sets the template content. </summary>
            ///
            /// <value> The template content. </value>

			[XmlElement(ElementName = "TemplateContent", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
			public TemplateContent TemplateContent { get; set; }

            /// <summary>   Gets or sets the type. </summary>
            ///
            /// <value> The type. </value>

			[XmlAttribute(AttributeName = "Type")]
			public string Type { get; set; }

            /// <summary>   Gets or sets the version. </summary>
            ///
            /// <value> The version. </value>

			[XmlAttribute(AttributeName = "Version")]
			public string Version { get; set; }

            /// <summary>   Gets or sets the xmlns. </summary>
            ///
            /// <value> The xmlns. </value>

			[XmlAttribute(AttributeName = "xmlns")]
			public string Xmlns { get; set; }

            /// <summary>   Gets or sets the wizard extension. </summary>
            ///
            /// <value> The wizard extension. </value>

            [XmlElement(ElementName = "WizardExtension", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
            public WizardExtension WizardExtension { get; set; }
        }

        /// <summary>   Gets or sets the template. </summary>
        ///
        /// <value> The template. </value>

		public VSTemplate Template { get; set; }

        /// <summary>   Gets or sets the full pathname of the template source file. </summary>
        ///
        /// <value> The full pathname of the template source file. </value>

        public string TemplateSourcePath { get; set; }

        /// <summary>   Gets or sets the wizard. </summary>
        ///
        /// <value> The wizard. </value>

        public IProjectItemTypeWizard Wizard { get; set; }

        /// <summary>   Creates a file. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="userFileName"> Filename of the user file. </param>

        public void CreateFile(string userFileName)
        {
            var projectItem = this.Template.TemplateContent.ProjectItems.Single();
            var templateRootDirectory = new DirectoryInfo(Path.GetDirectoryName(this.TemplateSourcePath));
            var sourceFile = templateRootDirectory.GetFiles().Single(f => f.Name == projectItem.SourceFileName);

            using (var reader = new StreamReader(sourceFile.FullName))
            {
                using (var writer = new StreamWriter(userFileName))
                {
                    var contents = reader.ReadToEnd();

                    writer.Write(contents);
                    writer.Flush();
                }
            }
        }
    }
}
