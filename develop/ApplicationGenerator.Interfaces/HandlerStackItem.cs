// file:	HandlerStackItem.cs
//
// summary:	Implements the handler stack item class

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
    /// <summary>   A handler stack item. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

    [DebuggerDisplay(" { DebugInfo } ")]
    public abstract class HandlerStackItem
    {
        /// <summary>   Gets the name of the hierarchy item. </summary>
        ///
        /// <value> The name of the hierarchy item. </value>

        public string EntityObjectName { get; }

        /// <summary>   Gets a value indicating whether the do not follow. </summary>
        ///
        /// <value> True if do not follow, false if not. </value>

        public bool DoNotFollow { get; }

        /// <summary>   Gets or sets a value indicating whether the no modules. </summary>
        ///
        /// <value> True if no modules, false if not. </value>

        public bool NoModules { get; protected set; }

        /// <summary>   Gets the roles. </summary>
        ///
        /// <value> The roles. </value>

        public string[] Roles { get; }

        /// <summary>   Gets the log. </summary>
        ///
        /// <value> The log. </value>

        public List<HandlerStackLogItem> Log { get; }

        /// <summary>   Gets the global log. </summary>
        ///
        /// <value> The global log. </value>

        public static FixedList<HandlerStackLogItem> GlobalLog { get; } = new FixedList<HandlerStackLogItem>(short.MaxValue);

        /// <summary>   Gets the log start time. </summary>
        ///
        /// <value> The log start time. </value>

        private static DateTime logStartTime { get; } = DateTime.Now;

        /// <summary>   Gets or sets the state of the authorization. </summary>
        ///
        /// <value> The authorization state. </value>

        public AuthorizationState AuthorizationState { get; set; }

        /// <summary>   Gets the view layout handlers. </summary>
        ///
        /// <value> The view layout handlers. </value>

        public List<IViewLayoutHandler> ViewLayoutHandlers { get; }

        /// <summary>   Enumerates as enumerable in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process as enumerable in this collection.
        /// </returns>

        public abstract IEnumerable<IFacetHandler> AsEnumerable();

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public abstract string DebugInfo { get; }

        /// <summary>   Gets the base object. </summary>
        ///
        /// <value> The base object. </value>

        public IEntityObjectWithFacets BaseObject { get; }

        /// <summary>   Gets or sets the indentation. </summary>
        ///
        /// <value> The indentation. </value>

        public int Indentation { get; set; }

        /// <summary>   Gets or sets a value indicating whether the popped. </summary>
        ///
        /// <value> True if popped, false if not. </value>

        public bool Popped { get; set; }

        /// <summary>   Gets or sets the number of modules. </summary>
        ///
        /// <value> The number of modules. </value>

        public int ModuleCount { get; internal set; }

        /// <summary>   Gets or sets the facet modules. </summary>
        ///
        /// <value> The facet modules. </value>

        public List<FacetPartsModules> FacetPartsModules { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="entityWithFacets">     The entity with facets. </param>
        /// <param name="roles">                The roles. </param>

        public HandlerStackItem(IEntityObjectWithFacets entityWithFacets, string[] roles)
        {
            this.BaseObject = entityWithFacets;
            this.EntityObjectName = entityWithFacets.Name;
            this.ViewLayoutHandlers = new List<IViewLayoutHandler>();
            this.Roles = roles;
            this.FacetPartsModules = new List<FacetPartsModules>();
            this.Log = new List<HandlerStackLogItem>();
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="doNotFollow">      True to do not follow. </param>
        /// <param name="noModules">        True to no modules. </param>

        public HandlerStackItem(IEntityObjectWithFacets entityWithFacets, bool doNotFollow, bool noModules)
        {
            this.BaseObject = entityWithFacets;
            this.DoNotFollow = doNotFollow;
            this.NoModules = noModules;
            this.Log = new List<HandlerStackLogItem>();
        }

        /// <summary>   Logs an event. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="stackEvent">       The stack event. </param>
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="currentPass">      The current pass. </param>
        /// <param name="logger">           The logger. </param>

        public void LogEvent(HandlerStackEvent stackEvent, string uiHierarchyPath, GeneratorPass currentPass, Serilog.ILogger logger)
        {
            var timestamp = DateTime.Now;
            var offset = timestamp - logStartTime;
            var logItem = new HandlerStackLogItem(this, stackEvent, offset, currentPass, uiHierarchyPath);
            var debugInfo = logItem.DebugInfo;

            this.Log.Add(logItem);
            GlobalLog.Add(logItem);

            logger.Information(logItem.DebugInfo);
        }

        /// <summary>   Creates a new HandlerStackItem. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="roles">            The roles. </param>
        /// <param name="handler">          The handler. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        public static HandlerStackItem Create(IEntityObjectWithFacets entityWithFacets, string[] roles, IFacetHandler handler)
        {
            return new SingleHandler(entityWithFacets, roles, handler);
        }

        /// <summary>   Creates a new HandlerStackItem. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="roles">            The roles. </param>
        /// <param name="handlers">         A variable-length parameters list containing handlers. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        public static HandlerStackItem Create(IEntityObjectWithFacets entityWithFacets, string[] roles, params IFacetHandler[] handlers)
        {
            if (handlers.Length == 1)
            {
                return new SingleHandler(entityWithFacets, roles, handlers.Single());
            }
            else
            {
                return new MultipleHandlers(entityWithFacets, roles, handlers);
            }
        }

        /// <summary>   Gets the global log text. </summary>
        ///
        /// <value> The global log text. </value>

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

        /// <summary>   Returns a string that represents the current object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <returns>   A string that represents the current object. </returns>

        public override string ToString()
        {
            return this.DebugInfo;
        }
    }

    /// <summary>   A do not follow handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

    public class DoNotFollowHandler : HandlerStackItem
    {
        /// <summary>   Gets the reason. </summary>
        ///
        /// <value> The reason. </value>

        public string Reason { get; }

        /// <summary>   Gets the stack trace. </summary>
        ///
        /// <value> The stack trace. </value>

        public List<StackFrame> StackTrace { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="reason">           The reason. </param>

        public DoNotFollowHandler(IEntityObjectWithFacets entityWithFacets, string reason) : base(entityWithFacets, true, true)
        {
            this.Reason = reason;
            this.StackTrace = this.GetStack().ToList();
        }

        /// <summary>   Enumerates as enumerable in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process as enumerable in this collection.
        /// </returns>

        public override IEnumerable<IFacetHandler> AsEnumerable()
        {
            throw new NotImplementedException();
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public override string DebugInfo
        {
            get
            {
                return "Handlers: [DoNotFollow]";
            }
        }
    }

    /// <summary>   A follow handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

    public class FollowHandler : HandlerStackItem
    {
        /// <summary>   Gets the reason. </summary>
        ///
        /// <value> The reason. </value>

        public string Reason { get; }

        /// <summary>   Gets the stack trace. </summary>
        ///
        /// <value> The stack trace. </value>

        public List<StackFrame> StackTrace { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="reason">           The reason. </param>

        public FollowHandler(IEntityObjectWithFacets entityWithFacets, string reason) : base(entityWithFacets, false, true)
        {
            this.Reason = reason;
            this.StackTrace = this.GetStack().ToList();
        }

        /// <summary>   Enumerates as enumerable in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process as enumerable in this collection.
        /// </returns>

        public override IEnumerable<IFacetHandler> AsEnumerable()
        {
            return new List<IFacetHandler>();
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public override string DebugInfo
        {
            get
            {
                return "Handlers: [Follow]";
            }
        }
    }

    /// <summary>   A single handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

    public class SingleHandler : HandlerStackItem
    {
        /// <summary>   Gets the handler. </summary>
        ///
        /// <value> The handler. </value>

        public IFacetHandler Handler { get; }

        /// <summary>   Enumerates as enumerable in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process as enumerable in this collection.
        /// </returns>

        public override IEnumerable<IFacetHandler> AsEnumerable() => new List<IFacetHandler>() { this.Handler };

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="hierarchyName">    Name of the hierarchy. </param>
        /// <param name="roles">            The roles. </param>
        /// <param name="handler">          The handler. </param>

        public SingleHandler(IEntityObjectWithFacets entityWithFacets, string[] roles, IFacetHandler handler) : base(entityWithFacets, roles)
        {
            this.Handler = handler;
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public override string DebugInfo
        {
            get
            {
                return string.Format("Handlers: '{0}'", this.Handler.GetType().Name);
            }
        }
    }

    /// <summary>   A multiple handlers. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

    public class MultipleHandlers : HandlerStackItem
    {
        /// <summary>   Gets the handlers. </summary>
        ///
        /// <value> The handlers. </value>

        public IEnumerable<IFacetHandler> Handlers { get; }

        /// <summary>   Enumerates as enumerable in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process as enumerable in this collection.
        /// </returns>

        public override IEnumerable<IFacetHandler> AsEnumerable() => this.Handlers;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="roles">            The roles. </param>
        /// <param name="handlers">         The handlers. </param>

        public MultipleHandlers(IEntityObjectWithFacets entityWithFacets, string[] roles, IEnumerable<IFacetHandler> handlers) : base(entityWithFacets, roles)
        {
            this.NoModules = !handlers.Any(h => h.FacetHandlerLayer == FacetHandlerLayer.Client);
            this.Handlers = handlers;
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public override string DebugInfo
        {
            get
            {
                return string.Format("Handlers: '{0}'", this.Handlers.Select(h => h.GetType().Name).ToCommaDelimitedList());
            }
        }
    }
}
