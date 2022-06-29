using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualStudioProvider;
using Utils;

namespace FindGenerators
{
    class Program
    {
        static void Main(string[] args)
        {
            var project = new VSProject(@"D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\ApplicationGenerator.IonicAngular.csproj");
            var generators = project.CompileItems.Where(i => i.FileName.EndsWith("Generator.cs")).ToList();
            var directory = new DirectoryInfo(Path.GetDirectoryName(project.FileName));

            foreach (var generator in generators)
            {
                var contents = generator.GetFileContents<string>();
                var syntaxTree = CSharpSyntaxTree.ParseText(contents);
                var root = syntaxTree.GetCompilationUnitRoot();
                var generatorClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
                var generateMethod = generatorClass.Members.OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text.StartsWith("Generate"));

                Console.WriteLine(generatorClass.Identifier.Text);

                foreach (var genericName in generateMethod.Body.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Where(m => m.Name.Identifier.Text.StartsWith("Generate")).Select(m => m.Name).OfType<GenericNameSyntax>()) 
                {
                    var typeArgName = genericName.TypeArgumentList.Arguments.OfType<IdentifierNameSyntax>().Single().Identifier.Text;
                    var templateFile = directory.GetFiles("*.tt", SearchOption.AllDirectories).Single(f => !f.FullName.Contains(@"\bin\") && Path.GetFileNameWithoutExtension(f.Name) == typeArgName);
                    var relativePath = templateFile.FullName.RemoveStart(directory.FullName + @"\");
                    var identifier = typeArgName;

                    if (typeArgName.Contains("Page"))
                    {
                        identifier = "Page";
                    }
                    else if (typeArgName.Contains("Class"))
                    {
                        identifier = "Class";
                    }
                    else if (typeArgName.Contains("Sass"))
                    {
                        identifier = "Sass";
                    }
                    
                    Console.WriteLine($"\t[GeneratorTemplate(\"{ identifier }\", @\"{ relativePath }\")]");
                }

                foreach (var genericName in generateMethod.Body.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Where(m => m.Name.Identifier.Text.StartsWith("Generate")).Select(m => m.Name).OfType<GenericNameSyntax>())
                {
                    var typeArgName = genericName.TypeArgumentList.Arguments.OfType<IdentifierNameSyntax>().Single().Identifier.Text;
                    var templateFile = directory.GetFiles("*.tt", SearchOption.AllDirectories).Single(f => !f.FullName.Contains(@"\bin\") && Path.GetFileNameWithoutExtension(f.Name) == typeArgName);
                    var relativePath = templateFile.FullName.RemoveStart(directory.FullName + @"\");
                    var identifier = typeArgName;

                    if (typeArgName.Contains("Page"))
                    {
                        identifier = "Page";
                    }
                    else if (typeArgName.Contains("Class"))
                    {
                        identifier = "Class";
                    }
                    else if (typeArgName.Contains("Sass"))
                    {
                        identifier = "Sass";
                    }

                    Console.WriteLine($"\t\thost.SetGenerator(typeof({ generatorClass.Identifier.Text }), \"{ identifier }\");");
                }

                Console.WriteLine();
            }
        }
    }
}
