using ImpromptuInterface.Dynamic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Utils;

namespace ApplicationGeneratorBuildTasks
{
    public static class ComponentFinder
    {
        public static List<Component> GetComponents(string binariesPath, string projectPath, string targetFramework, string targetAssembly, string productFilePath)
        {
            var packageFiles = new List<FileInfo>();
            var components = new List<Component>();
            var document = XDocument.Load(projectPath);
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            var projectName = XName.Get("p", "http://schemas.microsoft.com/developer/msbuild/2003");
            var projectFolder = Path.GetDirectoryName(projectPath);
            var packagesDirectory = new DirectoryInfo(Path.GetFullPath(Path.Combine(projectFolder, @"..\packages")));
            var packageSubDirectories = packagesDirectory.GetDirectories().ToList();
            var regex = new Regex(@"(?<framework>[a-z]*)?(?<major>\d)\.?(?<minor>\d)");
            var dotNetVersion = Version.Parse(Assembly.GetExecutingAssembly().GetFrameworkVersion());
            var binariesDirectory = new DirectoryInfo(binariesPath);
            IEnumerable<XElement> elementReferences;
            IEnumerable<XElement> elementPackageReferences;
            FrameworkVersion targetFrameworkVersion;

            if (regex.IsMatch(targetFramework))
            {
                var match = regex.Match(targetFramework);
                var major = match.GetGroupValue("major");
                var minor = match.GetGroupValue("minor");
                var framework = match.GetGroupValue("framework");

                targetFrameworkVersion = new FrameworkVersion(framework, Version.Parse(major + "." + minor));
            }
            else
            {
                targetFrameworkVersion = null;
                DebugUtils.Break();
            }

            packagesDirectory = new DirectoryInfo(Path.GetFullPath(Path.Combine(projectFolder, @"..\..\packages")));
            packageSubDirectories.AddRange(packagesDirectory.GetDirectories());

            packagesDirectory = new DirectoryInfo(Environment.ExpandEnvironmentVariables(@"%userprofile%\.nuget\packages"));
            packageSubDirectories.AddRange(packagesDirectory.GetDirectories());

            packageSubDirectories = packageSubDirectories.OrderBy(p => p.Name).ToList();

            namespaceManager.AddNamespace(projectName.LocalName, projectName.NamespaceName);

            elementReferences = document.XPathSelectElements("/p:Project/p:ItemGroup/p:Reference", namespaceManager);
            elementPackageReferences = document.XPathSelectElements("/p:Project/p:ItemGroup/p:PackageReference", namespaceManager);

            foreach (var elementReference in elementReferences)
            {
                var hintPath = elementReference.Element(projectName.Namespace + "HintPath");

                if (hintPath != null)
                {
                    var componentFile = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath), hintPath.Value)));

                    if (componentFile.Exists)
                    {
                        packageFiles.Add(componentFile);
                    }
                }
            }

            foreach (var elementPackageReference in elementPackageReferences)
            {
                var include = elementPackageReference.Attribute("Include");
                var versionElement = elementPackageReference.Element(projectName.Namespace + "Version");
                DirectoryInfo packageDirectory;
                DirectoryInfo packageLibDirectory;

                if (packageSubDirectories.Any(d => d.Name.AsCaseless() == include.Value + "." + versionElement.Value))
                {
                    packageDirectory = packageSubDirectories.Single(d => d.Name.AsCaseless() == include.Value + "." + versionElement.Value);
                }
                else if (packageSubDirectories.Any(d => d.Name.AsCaseless() == include.Value && d.GetDirectories().Any(d2 => d2.Name == versionElement.Value)))
                {
                    packageDirectory = packageSubDirectories.Single(d => d.Name.AsCaseless() == include.Value && d.GetDirectories().Any(d2 => d2.Name == versionElement.Value));
                    packageDirectory = new DirectoryInfo(Path.Combine(packageDirectory.FullName, versionElement.Value));
                }
                else
                {
                    packageDirectory = null;
                    DebugUtils.Break();
                }

                packageLibDirectory = new DirectoryInfo(Path.Combine(packageDirectory.FullName, "lib"));

                if (packageLibDirectory.Exists)
                {
                    var frameworkVersions = new List<FrameworkVersion>();
                    FrameworkVersion bestFrameworkVersion;
                    DirectoryInfo componentDirectory;

                    foreach (var frameworkDirectory in packageLibDirectory.GetDirectories())
                    {
                        regex = new Regex(@"net(?<framework>[a-z]*)?(?<major>\d)\.?(?<minor>\d)");

                        if (regex.IsMatch(frameworkDirectory.Name))
                        {
                            var match = regex.Match(frameworkDirectory.Name);
                            var major = match.GetGroupValue("major");
                            var minor = match.GetGroupValue("minor");
                            var framework = match.GetGroupValue("framework");
                            var frameworkVersion = new FrameworkVersion(framework, Version.Parse(major + "." + minor));

                            if (framework == string.Empty)
                            {
                                framework = "net";
                            }
                            
                            if (framework == targetFrameworkVersion.Framework)
                            {
                                frameworkVersions.Add(frameworkVersion);
                            }
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    bestFrameworkVersion = frameworkVersions.FirstOrDefault(v => v.Framework == targetFrameworkVersion.Framework && v.Version <= dotNetVersion);
                    componentDirectory = new DirectoryInfo(Path.Combine(packageLibDirectory.FullName, string.Format("net{0}{1}", bestFrameworkVersion.Version.Major, bestFrameworkVersion.Version.Minor)));

                    if (!componentDirectory.Exists)
                    {
                        DebugUtils.Break();
                    }

                    foreach (var componentDll in componentDirectory.GetFiles("*.dll"))
                    {
                        packageFiles.Add(componentDll);
                    }
                }
            }

            foreach (var binary in binariesDirectory.GetFiles().Where(b => b.Extension.IsOneOf(".dll", ".exe")))
            {
                if (packageFiles.Any(f => f.Name == binary.Name))
                {
                    var packageFile = packageFiles.Single(f => f.Name == binary.Name);

                    packageFiles.Remove(packageFile);
                }

                if (Path.GetFileNameWithoutExtension(binary.Name).AsCaseless() != targetAssembly)
                {
                    components.Add(new Component(binary, projectFolder, productFilePath));
                }
            }

            return components;
        }
    }
}
