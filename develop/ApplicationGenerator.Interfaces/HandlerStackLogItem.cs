// file:	HandlerStackLogItem.cs
//
// summary:	Implements the handler stack log item class

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
    /// <summary>   Values that represent handler stack events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

    public enum HandlerStackEvent
    {
        /// <summary>   An enum constant representing the created option. </summary>
        Created,
        /// <summary>   An enum constant representing the push option. </summary>
        Push,
        /// <summary>   An enum constant representing the pop option. </summary>
        Pop
    }

    /// <summary>   A handler stack log item. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

    [DebuggerDisplay(" { DebugInfo }")]
    public class HandlerStackLogItem
    {
        /// <summary>   Gets the handler stack item. </summary>
        ///
        /// <value> The handler stack item. </value>

        public HandlerStackItem HandlerStackItem { get; }

        /// <summary>   Gets the handler stack event. </summary>
        ///
        /// <value> The handler stack event. </value>

        public HandlerStackEvent HandlerStackEvent { get; }

        /// <summary>   Gets the start time offset. </summary>
        ///
        /// <value> The start time offset. </value>

        public TimeSpan StartTimeOffset { get; }

        /// <summary>   Gets the current pass. </summary>
        ///
        /// <value> The current pass. </value>

        public GeneratorPass CurrentPass { get; }

        /// <summary>   Gets the full pathname of the hierarchy file. </summary>
        ///
        /// <value> The full pathname of the hierarchy file. </value>

        public string UIHierarchyPath { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="handlerStackItem"> The handler stack item. </param>
        /// <param name="stackEvent">       The stack event. </param>
        /// <param name="offset">           The offset. </param>
        /// <param name="currentPass">      The current pass. </param>
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>

        public HandlerStackLogItem(HandlerStackItem handlerStackItem, HandlerStackEvent stackEvent, TimeSpan offset, GeneratorPass currentPass, string uiHierarchyPath)
        {
            this.HandlerStackItem = handlerStackItem;
            this.HandlerStackEvent = stackEvent;
            this.StartTimeOffset = offset;
            this.CurrentPass = currentPass;
            this.UIHierarchyPath = uiHierarchyPath;
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public string DebugInfo
        {
            get
            {
                return string.Format("{0}: {1}, "
                    + "{2}, "
                    + "Event: {3}, "
        			+ "StartTimeOffset: {4}, "
                    + "CurrentPass: {5}, "
        			+ "Path: '{6}'",
                    this.HandlerStackItem.BaseObject.GetType().Name,
                    this.HandlerStackItem.BaseObject.Name,
                    this.HandlerStackItem.ToString(),
        			this.HandlerStackEvent,
        			this.StartTimeOffset,
        			this.CurrentPass,
        			this.UIHierarchyPath
                );
            }
        }
    }
}
