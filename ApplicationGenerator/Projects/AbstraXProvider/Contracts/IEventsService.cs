using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace AbstraX.Contracts
{
    public interface IEventsService
    {
        EventMessage[] GetMessages();
        void PostMessage(EventMessage message);
        void PostMessage(EventMessage message, IBuildUIDaemon daemon, TimeSpan waitTimout);
        void PostMessage(Message message, params object[] parms);
        void ClearQueue();
        void ClearWait();
        ILog Log { get; set; }
    }
}
