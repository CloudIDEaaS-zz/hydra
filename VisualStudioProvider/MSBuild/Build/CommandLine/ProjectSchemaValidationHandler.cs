namespace Microsoft.Build.CommandLine
{
    using Microsoft.Build.Shared;
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;

    internal sealed class ProjectSchemaValidationHandler
    {
        private bool syntaxError;

        private static string BuildStringFromResource(string projectFile, int fileLine, int fileEndLine, int fileColumn, int fileEndColumn, string resourceName, params object[] args)
        {
            string str;
            string str2;
            string message = Microsoft.Build.Shared.ResourceUtilities.FormatResourceString(out str, out str2, resourceName, args);
            return Microsoft.Build.Shared.EventArgsFormatting.FormatEventMessage("error", Microsoft.Build.Shared.AssemblyResources.GetString("SubCategoryForSchemaValidationErrors"), message, str, projectFile, fileLine, fileEndLine, fileColumn, fileEndColumn, 0);
        }

        private void OnSchemaValidationError(object sender, ValidationEventArgs args)
        {
            this.syntaxError = true;
            string projectFile = string.Empty;
            if (args.Exception.SourceUri.Length != 0)
            {
                projectFile = new Uri(args.Exception.SourceUri).LocalPath;
            }
            Console.WriteLine(BuildStringFromResource(projectFile, args.Exception.LineNumber, 0, args.Exception.LinePosition, 0, "SchemaValidationError", new object[] { args.Exception.Message }));
        }

        private static void ThrowInitializationExceptionWithResource(string projectFile, int fileLine, int fileEndLine, int fileColumn, int fileEndColumn, string resourceName, params object[] args)
        {
            InitializationException.Throw(BuildStringFromResource(projectFile, fileLine, fileEndLine, fileColumn, fileEndColumn, resourceName, args), null);
        }

        private void VerifyProjectSchema(string projectFile, string schemaFile)
        {
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrowArgumentNull(schemaFile, "schemaFile");
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrowArgumentNull(projectFile, "projectFile");
            XmlReaderSettings settings = new XmlReaderSettings {
                ValidationType = ValidationType.Schema,
                XmlResolver = null
            };
            settings.ValidationEventHandler += new ValidationEventHandler(this.OnSchemaValidationError);
            XmlTextReader schemaDocument = new XmlTextReader(schemaFile);
            XmlTextReader reader4 = schemaDocument;
            try
            {
                settings.Schemas.Add("http://schemas.microsoft.com/developer/msbuild/2003", schemaDocument);
                projectFile = Path.GetFullPath(projectFile);
                using (StreamReader reader2 = new StreamReader(projectFile))
                {
                    using (XmlReader reader3 = XmlReader.Create(reader2, settings, projectFile))
                    {
                        this.syntaxError = false;
                        bool flag = true;
                        while (flag)
                        {
                            try
                            {
                                flag = reader3.Read();
                                continue;
                            }
                            catch (XmlException)
                            {
                                continue;
                            }
                        }
                        VerifyThrowInitializationExceptionWithResource(!this.syntaxError, projectFile, 0, 0, 0, 0, "ProjectSchemaErrorHalt", new object[0]);
                    }
                }
            }
            catch (XmlException exception)
            {
                ThrowInitializationExceptionWithResource((exception.SourceUri.Length == 0) ? string.Empty : new Uri(exception.SourceUri).LocalPath, exception.LineNumber, 0, exception.LinePosition, 0, "InvalidSchemaFile", new object[] { schemaFile, exception.Message });
            }
            catch (XmlSchemaException exception2)
            {
                ThrowInitializationExceptionWithResource((exception2.SourceUri.Length == 0) ? string.Empty : new Uri(exception2.SourceUri).LocalPath, exception2.LineNumber, 0, exception2.LinePosition, 0, "InvalidSchemaFile", new object[] { schemaFile, exception2.Message });
            }
            finally
            {
                if (reader4 != null)
                {
                    //reader4.Dispose();
                }
            }
        }

        internal static void VerifyProjectSchema(string projectFile, string schemaFile, string binPath)
        {
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrowArgumentNull(projectFile, "projectFile");
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrowArgumentNull(binPath, "binPath");
            if ((schemaFile == null) || (schemaFile.Length == 0))
            {
                schemaFile = Path.Combine(binPath, "Microsoft.Build.xsd");
            }
            if (File.Exists(schemaFile))
            {
                Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("SchemaFileLocation"), schemaFile);
            }
            else
            {
                InitializationException.Throw(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("SchemaNotFoundErrorWithFile", new object[] { schemaFile }), null);
            }
            new ProjectSchemaValidationHandler().VerifyProjectSchema(projectFile, schemaFile);
        }

        private static void VerifyThrowInitializationExceptionWithResource(bool condition, string projectFile, int fileLine, int fileEndLine, int fileColumn, int fileEndColumn, string resourceName, params object[] args)
        {
            if (!condition)
            {
                ThrowInitializationExceptionWithResource(projectFile, fileLine, fileEndLine, fileColumn, fileEndColumn, resourceName, args);
            }
        }
    }
}

