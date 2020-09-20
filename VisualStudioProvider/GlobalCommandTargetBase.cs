using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using System.Diagnostics;
using GuidAttribute = System.Runtime.InteropServices.GuidAttribute;
using Utils;
using System.Threading;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace VisualStudioProvider
{
    public class GlobalCommandTargetBase : OleComponent, IOleCommandTarget
    {
        private IVsRegisterPriorityCommandTarget registerPriorityCommandTarget;
        private uint commandTargetCookie;
        private bool catalogBuilt;
        private bool capturing;
        public Dictionary<Guid, CommandSet> CommandSets { get; private set; }
        public event EventHandler OnStop;

        public GlobalCommandTargetBase()
        {
            registerPriorityCommandTarget = (IVsRegisterPriorityCommandTarget)Package.GetGlobalService(typeof(IVsRegisterPriorityCommandTarget));
            CommandSets = new Dictionary<Guid, CommandSet>();
        }

        internal virtual void Start()
        {
            Debug.Assert(!capturing);

            if (!catalogBuilt)
            {
                OnStatus("Building global command target catalog");

                BuildCatalog();
            }

            if (ErrorHandler.Failed(registerPriorityCommandTarget.RegisterPriorityCommandTarget(0, this, out commandTargetCookie)))
            {
                OnStatus("Attempt to register priority command target failed");
            }

            capturing = true;
        }

        internal virtual void Stop()
        {
            if (OnStop != null)
            {
                OnStop(this, EventArgs.Empty);
            }

            if (capturing)
            {
                try
                {
                    if (ErrorHandler.Failed(registerPriorityCommandTarget.UnregisterPriorityCommandTarget(commandTargetCookie)))
                    {
                        OnStatus("Attempt to stop priority command target failed");
                    }
                }
                catch
                {
                }

                capturing = false;
            }

            base.Unregister();
        }

        protected virtual void OnStatus(string text)
        {
        }

        public CommandSet GetCommandSet<T>()
        {
            var valuePair = CommandSets.SingleOrDefault(s => s.Value.Name == typeof(T).Name);

            return valuePair.Value;
        }

        protected void BuildCatalog()
        {
            var type = typeof(VSConstants);
            var enums = type.GetNestedTypes().Where(t => t.IsEnum && t.GetCustomAttributes(true).Cast<Attribute>().Any(a => a.GetType() == typeof(GuidAttribute)));

            foreach (var _enum in enums)
            {
                var guidAttribute = (GuidAttribute)_enum.GetCustomAttributes(true).Cast<Attribute>().Single(a => a.GetType() == typeof(GuidAttribute));
                var guid = Guid.Parse(guidAttribute.Value);
                var name = _enum.Name;

                CommandSets.Add(guid, new CommandSet(guid, name, _enum));
            }

            catalogBuilt = true;
        }

        protected virtual void WriteLine(string format, params object[] args)
        {
        }

        public virtual int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            return (int) Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
        }

        public virtual int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
        }
    }
}
