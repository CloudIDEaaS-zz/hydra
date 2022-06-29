using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Angular.Routes
{
    [DebuggerDisplay(" { FunctionCode } ")]
    public class LoadChildren
    {
        public AngularModule RoutedToModule { get; }
        public AngularModule Module { get; }

        public LoadChildren(AngularModule module, AngularModule routedToModule)
        {
            this.RoutedToModule = routedToModule;
            this.Module = module;
        }

        public string FunctionCode
        {
            get
            {
                var builder = new StringBuilder();

                if (this.Module.File != null)
                {
                    var modulePath = this.Module.File.SystemLocalFile.FullName;
                    var importPath = Path.Combine(this.RoutedToModule.File.SystemLocalFile.Directory.GetRelativePath(modulePath), Path.GetFileNameWithoutExtension(this.RoutedToModule.File.SystemLocalFile.Name)).ReverseSlashes();

                    importPath = importPath.FixDots();

                    builder.AppendFormat("() => import(\"{0}\").then(m => m.{1})", importPath, this.RoutedToModule.Name);

                    return builder.ToString();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
