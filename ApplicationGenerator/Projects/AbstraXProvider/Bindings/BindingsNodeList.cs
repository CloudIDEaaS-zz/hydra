using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.ServerInterfaces;

namespace AbstraX.Bindings
{
    public class BindingsNodeList : List<IBindingsTreeNode>
    {
        public IBindingsTreeNode BindingParent { get; set; }
        public IElement RootElement { get; private set; }
        public Stack<IElement> ElementStack { get; private set; }

        public BindingsNodeList(IElement rootElement)
        {
            RootElement = rootElement;
            ElementStack = new Stack<IElement>();
        }

        public BindingsNodeList Push(IElement element)
        {
            ElementStack.Push(element);

            return this;
        }

        public BindingsNodeList Pop()
        {
            ElementStack.Pop();

            return this;
        }

        public string OriginTrace
        {
            get
            {
                var trace = new StringBuilder();

                trace.AppendFormat("{0}:", BindingParent == null ? "Root" : BindingParent.Name);

                if (RootElement != null)
                {
                    trace.AppendFormat("/{0}", RootElement.Name);
                }

                foreach (var element in this.ElementStack.Reverse())
                {
                    trace.AppendFormat("/{0}", element.Name);
                }

                return trace.ToString();
            }
        }
    }
}
