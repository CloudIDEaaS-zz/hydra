
namespace AbstraX.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using AbstraX.Contracts;
    using System.Threading;
using log4net;

    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class EventsService : DomainService, IEventsService
    {
        private static Queue<EventMessage> messageQueue = new Queue<EventMessage>();
        private static AutoResetEvent uiWaitEvent = new AutoResetEvent(false);
        private IBuildUIDaemon lastUIDaemon;
        public ILog Log { get; set; }

        [Invoke]
        public EventMessage[] GetMessages()
        {
            Monitor.Enter(messageQueue);

            var messages = messageQueue.ToArray();
            messageQueue.Clear();

            Monitor.Exit(messageQueue);

            return messages;
        }

        [Ignore]
        public void PostMessage(EventMessage message)
        {
            Monitor.Enter(messageQueue);

            messageQueue.Enqueue(message);

            Monitor.Exit(messageQueue);
        }

        [Invoke]
        public void ClearQueue()
        {
            messageQueue.Clear();
        }

        [Ignore]
        public void PostMessage(EventMessage message, IBuildUIDaemon daemon, TimeSpan waitTimout)
        {
            Monitor.Enter(messageQueue);

            messageQueue.Enqueue(message);

            Monitor.Exit(messageQueue);

            if (!uiWaitEvent.WaitOne(waitTimout))
            {
                this.PostMessage(new EventMessage(Message.GeneralError, "Timeout in waiting for Wizard Frame UI component"));
            }
        }

        [Ignore]
        public void PostMessage(Message message, params object[] parms)
        {
            PostMessage(new EventMessage(message, parms));
        }

        [Ignore]
        public void ClearWait()
        {
            uiWaitEvent.Set();
        }
    }
}


