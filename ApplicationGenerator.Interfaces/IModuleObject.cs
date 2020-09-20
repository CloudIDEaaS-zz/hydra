using AbstraX.ClientFolderStructure;
using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public interface IModuleObject
    {
        IBase BaseObject { get; set; }
        string Name { get; set; }
        string RoutingName { get; set; }
        bool IsComponent { get; set; }
        UIAttribute GetUIAttribute(AbstraX.FolderStructure.Folder folder);
        bool ValidateModuleName(string name, out string error);
    }
}
