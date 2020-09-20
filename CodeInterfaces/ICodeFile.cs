using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces
{
    public interface ICodeFile
    {
        Guid InstanceId { get; }
        string Hash { get; }
        string FileName { get; }
    }
}
