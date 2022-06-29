using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualStudioProvider;
using Utils;
using System.IO;
using System.Windows.Forms;

namespace CreateSharedReferences
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var builder = new StringBuilder();
            var targetProject = new VSProject(@"D:\MC\CloudIDEaaS\root\HydraDebugAssistant\HydraDebugAssistant.csproj");
            var sourceProjects = new List<VSProject>
            {
                new VSProject(@"D:\MC\CloudIDEaaS\root\Utils\Utils.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\ColorMine\ColorMine.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\CppParser\CppParser.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\ProcessDiagnosticsLibrary\ProcessDiagnosticsLibrary.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\SDKInterfaceLibrary.Entities\SDKInterfaceLibrary.Entities.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\SharpSerializer\SharpSerializer.Library\SharpSerializer.Library.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\Rtf2Html\Converter\Html\ConverterHtml2010.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\Rtf2Html\Interpreter\Interpreter2010.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\Rtf2Html\Parser\Parser2010.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\Rtf2Html\Rtf2Html\Rtf2Html2010.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\Rtf2Html\Sys\Sys2010.csproj"),
                new VSProject(@"D:\MC\CloudIDEaaS\root\NamedPipeWrapper\NamedPipeWrapper.csproj")
            };

            foreach (var sourceProject in sourceProjects)
            {

                builder.AppendLine($"  <!-- { sourceProject.Name } -->");
                builder.AppendLine("  <ItemGroup>");

                foreach (var sourceItem in sourceProject.CompileItems)
                {
                    var filePath = sourceItem.FilePath;
                    var linkedReference = targetProject.CompileItems.Where(c => c.IsLink && c.FilePath.AsCaseless() == filePath).FirstOrDefault();

                    if (linkedReference == null)
                    {
                        var targetProjectDirectory = new DirectoryInfo(Path.GetDirectoryName(targetProject.FileName));
                        var file = new FileInfo(filePath);
                        var relativePath = file.GetRelativePath(targetProjectDirectory.FullName);
                        var link = relativePath.RemoveStartIfMatches(@"..\");

                        builder.AppendLine($"    <Compile Include=\"{ relativePath }\">");
                        builder.AppendLine($"      <Link>{ link }</Link>");
                        builder.AppendLine($"    </Compile>");
                    }
                }

                builder.AppendLine("  </ItemGroup>");
            }

            Clipboard.SetText(builder.ToString());
        }
    }
}

