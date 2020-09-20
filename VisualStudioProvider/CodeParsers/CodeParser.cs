using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Metaspec;
using CodeInterfaces.CodeParsers;

namespace VisualStudioProvider.CodeParsers
{
    public class CodeParser : ICodeParser
    {
        public ICodeEditPackage ParseFile(FileInfo file)
        {
            return null;
        }

        public ICodeEditPackage ParseFile(char[] chars, string path)
        {
            ICsProject project = ICsProjectFactory.create(project_namespace.pn_project_namespace);
            ICsFile file = ICsFileFactory.create(chars, path);

            project.setBuildEntityModel(true);
            project.addFiles(file);
            project.parse(true, true);

            var unit = file.getCompilationUnit();

            return null;
        }
    }
}
