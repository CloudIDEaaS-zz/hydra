using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AbstraX.DataAnnotations;
using AbstraX.Models.Interfaces;
using Utils;
using System.Diagnostics;

namespace AbstraX
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public abstract class HandlerStackItem
    {
        public string HierarchyItemName { get; }
        public bool DoNotFollow { get; }
        public bool NoModules { get; protected set; }
        public string[] Roles { get; }
        public List<HandlerStackLogItem> Log { get; }
        public static FixedList<HandlerStackLogItem> GlobalLog { get; } = new FixedList<HandlerStackLogItem>(short.MaxValue);
        private static DateTime logStartTime { get; } = DateTime.Now;
        public AuthorizationState AuthorizationState { get; set; }
        public List<IViewLayoutHandler> ViewLayoutHandlers { get; }
        public abstract IEnumerable<IFacetHandler> AsEnumerable();
        public abstract string DebugInfo { get; }
        public IEntityWithFacets BaseObject { get; }

        public HandlerStackItem(IEntityWithFacets entityWithFacets, string hierarchyItemName, string[] roles)
        {
            this.BaseObject = entityWithFacets;
            this.HierarchyItemName = hierarchyItemName;
            this.ViewLayoutHandlers = new List<IViewLayoutHandler>();
            this.Roles = roles;
            this.Log = new List<HandlerStackLogItem>();
        }

        public HandlerStackItem(IEntityWithFacets entityWithFacets, bool doNotFollow, bool noModules)
        {
            this.BaseObject = entityWithFacets;
            this.DoNotFollow = doNotFollow;
            this.NoModules = noModules;
            this.Log = new List<HandlerStackLogItem>();
        }

        public void LogEvent(HandlerStackEvent stackEvent, string uiHierarchyPath, GeneratorPass currentPass)
        {
            var timestamp = DateTime.Now;
            var offset = timestamp - logStartTime;
            var logItem = new HandlerStackLogItem(this, stackEvent, offset, currentPass, uiHierarchyPath);
            var debugInfo = logItem.DebugInfo;

            this.Log.Add(logItem);
            GlobalLog.Add(logItem);
        }

        public static HandlerStackItem Create(IEntityWithFacets entityWithFacets, string hierarchyName, string[] roles, IFacetHandler handler)
        {
            return new SingleHandler(entityWithFacets, hierarchyName, roles, handler);
        }

        public static HandlerStackItem Create(IEntityWithFacets entityWithFacets, string hierarchyName, string[] roles, params IFacetHandler[] handlers)
        {
            if (handlers.Length == 1)
            {
                return new SingleHandler(entityWithFacets, hierarchyName, roles, handlers.Single());
            }
            else
            {
                return new MultipleHandlers(entityWithFacets, hierarchyName, roles, handlers);
            }
        }

        public static string GlobalLogText
        {
            get
            {
                var builder = new StringBuilder();

                foreach (var logItem in GlobalLog)
                {
                    var baseObject = logItem.HandlerStackItem.BaseObject;
                    var ancestorCount = baseObject.GetAncestors().Count();

                    builder.AppendLineSpaceIndent(ancestorCount * 2, logItem.DebugInfo);
                }

                return builder.ToString();
            }
        }

        public override string ToString()
        {
            return this.DebugInfo;
        }
    }

    public class DoNotFollowHandler : HandlerStackItem
    {
        public string Reason { get; }
        public List<StackFrame> StackTrace { get; }

        public DoNotFollowHandler(IEntityWithFacets entityWithFacets, string reason) : base(entityWithFacets, true, true)
        {
            this.Reason = reason;
            this.StackTrace = this.GetStack().ToList();
        }

        public override IEnumerable<IFacetHandler> AsEnumerable()
        {
            throw new NotImplementedException();
        }

        public override string DebugInfo
        {
            get
            {
                return "Handlers: [DoNotFollow]";
            }
        }
    }

    public class FollowHandler : HandlerStackItem
    {
        public string Reason { get; }
        public List<StackFrame> StackTrace { get; }

        public FollowHandler(IEntityWithFacets entityWithFacets, string reason) : base(entityWithFacets, false, true)
        {
            this.Reason = reason;
            this.StackTrace = this.GetStack().ToList();
        }

        public override IEnumerable<IFacetHandler> AsEnumerable()
        {
            return new List<IFacetHandler>();
        }

        public override string DebugInfo
        {
            get
            {
                return "Handlers: [Follow]";
            }
        }
    }

    public class SingleHandler : HandlerStackItem
    {
        public IFacetHandler Handler { get; }

        public override IEnumerable<IFacetHandler> AsEnumerable() => new List<IFacetHandler>() { this.Handler };

        public SingleHandler(IEntityWithFacets entityWithFacets, string hierarchyName, string[] roles, IFacetHandler handler) : base(entityWithFacets, hierarchyName, roles)
        {
            this.Handler = handler;
        }

        public override string DebugInfo
        {
            get
            {
                return string.Format("Handlers: '{0}'", this.Handler.GetType().Name);
            }
        }
    }

    public class MultipleHandlers : HandlerStackItem
    {
        public IEnumerable<IFacetHandler> Handlers { get; }
        public override IEnumerable<IFacetHandler> AsEnumerable() => this.Handlers;

        public MultipleHandlers(IEntityWithFacets entityWithFacets, string hierarchyName, string[] roles, IEnumerable<IFacetHandler> handlers) : base(entityWithFacets, hierarchyName, roles)
        {
            this.NoModules = !handlers.Any(h => h.FacetHandlerLayer == FacetHandlerLayer.Client);
            this.Handlers = handlers;
        }

        public override string DebugInfo
        {
            get
            {
                return string.Format("Handlers: '{0}'", this.Handlers.Select(h => h.GetType().Name).ToCommaDelimitedList());
            }
        }
    }
}
