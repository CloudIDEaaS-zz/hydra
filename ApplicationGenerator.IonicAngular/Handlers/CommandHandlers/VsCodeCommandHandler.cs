// file:	Handlers\CommandHandlers\NugetCommandHandler.cs
//
// summary:	Implements the nuget command handler class

using AbstraX.Handlers.CommandHandlers.DotNetTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers
{
    /// <summary>   A nuget command handler. </summary>
    ///
    /// <remarks>   Ken, 10/12/2020. </remarks>

    public class VsCodeCommandHandler : BaseWindowsCommandHandler 
    {
        public VsCodeCommandHandler() : base("code.exe")
        {
            this.NoWait = true;
        }

        public void OpenFile(string filePath)
        {
            Debug.Assert(File.Exists(filePath));

            base.RunCommand(filePath, Path.GetDirectoryName(filePath));
        }

        public void OpenFolder(string folderPath)
        {
            Debug.Assert(Directory.Exists(folderPath));

            base.RunCommand(folderPath, folderPath);
        }
    }
}
