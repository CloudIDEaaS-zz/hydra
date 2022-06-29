using Pdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudioProvider.PDB
{
    public interface IAddressResolutionPoint
    {
        void NotifyResolved(IProcedureBlock procedureBlock);
        void NotifyResolved();
    }
}
