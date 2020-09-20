using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeInterfaces.CodeParsers
{
    public interface ICodeParser
    {
        ICodeEditPackage ParseFile(FileInfo file);
        ICodeEditPackage ParseFile(char[] chars, string path);
    }
}
