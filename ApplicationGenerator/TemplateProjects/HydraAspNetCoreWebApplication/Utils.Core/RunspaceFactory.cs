using System;
using System.Collections.Generic;
using System.Management.Automation.Runspaces;
using System.Text;
using Utils.Wrappers.Implementations;
using Utils.Wrappers.Interfaces;

namespace Utils
{
    public class RunspaceFactory : IRunspaceFactory
    {
        public RunspaceFactory()
        {
        }

        public IRunspace CreateRunspace(InitialSessionState initialSessionState)
        {
            return new Wrappers.Implementations.Runspace(System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(initialSessionState));
        }

        public IRunspace CreateRunspace(RunspaceConnectionInfo connectionInfo)
        {
            return new Wrappers.Implementations.Runspace(System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(connectionInfo));
        }

        public IRunspace CreateOutOfProcessRunspace(TypeTable typeTable, PowerShellProcessInstance processInstance)
        {
            return new Wrappers.Implementations.Runspace(System.Management.Automation.Runspaces.RunspaceFactory.CreateOutOfProcessRunspace(typeTable, processInstance));
        }
    }
}
