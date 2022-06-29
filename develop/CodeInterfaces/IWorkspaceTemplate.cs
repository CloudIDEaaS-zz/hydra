using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterfaces
{
    public interface IWorkspaceTemplate
    {
        string FileName { get; set; }
        bool ReplaceParameters { get; set; }
    }
}
