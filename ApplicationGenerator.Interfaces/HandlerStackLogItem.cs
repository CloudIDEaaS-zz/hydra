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
    public enum HandlerStackEvent
    {
        Created,
        Push,
        Pop
    }

    [DebuggerDisplay(" { DebugInfo }")]
    public class HandlerStackLogItem
    {
        public HandlerStackItem HandlerStackItem { get; }
        public HandlerStackEvent HandlerStackEvent { get; }
        public TimeSpan StartOffset { get; }
        public GeneratorPass CurrentPass { get; }
        public string UIHierarchyPath { get; }

        public HandlerStackLogItem(HandlerStackItem handlerStackItem, HandlerStackEvent stackEvent, TimeSpan offset, GeneratorPass currentPass, string uiHierarchyPath)
        {
            this.HandlerStackItem = handlerStackItem;
            this.HandlerStackEvent = stackEvent;
            this.StartOffset = offset;
            this.CurrentPass = currentPass;
            this.UIHierarchyPath = uiHierarchyPath;
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("{0}: {1}, "
                    + "{2}, "
                    + "Event: {3}, "
        			+ "Offset: {4}, "
        			+ "CurrentPass: {5}, "
        			+ "Path: '{6}'",
                    this.HandlerStackItem.BaseObject.GetType().Name,
                    this.HandlerStackItem.BaseObject.Name,
                    this.HandlerStackItem.ToString(),
        			this.HandlerStackEvent,
        			this.StartOffset,
        			this.CurrentPass,
        			this.UIHierarchyPath
                );
            }
        }
    }
}
