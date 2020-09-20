using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.ServerInterfaces;
using System.Diagnostics;
using AbstraX.Contracts;
using AbstraX.AssemblyInterfaces;

namespace AbstraX
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class NavigationItem
    {
        public object NavigationProperty { get; set; }
        public IElement NavigationResultElement { get; private set; }
        public IElementBuild NavigationResultBuild { get; set; }
        public BaseType PropertyType { get; set; }

        public NavigationItem(IElement navigationProperty, IElement navigationResultElement, BaseType propertyType)
        {
            this.NavigationProperty = navigationProperty;
            this.NavigationResultElement = navigationResultElement;
            this.PropertyType = propertyType;
        }

        public NavigationItem(string navigationPropertyName, IElement navigationResultElement, BaseType propertyType)
        {
            this.NavigationProperty = navigationPropertyName;
            this.NavigationResultElement = navigationResultElement;
            this.PropertyType = propertyType;
        }

        public string PropertyName
        {
            get
            {
                if (this.NavigationProperty is string)
                {
                    return (string)(object) this.NavigationProperty;
                }
                else
                {
                    var element = (IElement)this.NavigationProperty;

                    return element is IGetSetProperty ? ((IGetSetProperty) element).PropertyName : element.Name;
                }
            }
        }

        public bool IsLocal
        {
            get
            {
                if (this.NavigationProperty is string)
                {
                    if (NavigationResultElement is ISurrogateElement)
                    {
                        var surrogate = (ISurrogateElement)NavigationResultElement;

                        return surrogate.ReferencedFrom.Parent.Modifiers.HasFlag(Modifiers.IsLocal);
                    }
                    else
                    {
                        return NavigationResultElement.Parent.Modifiers.HasFlag(Modifiers.IsLocal);
                    }
                }
                else
                {
                    var element = (IElement)this.NavigationProperty;

                    if (element is ISurrogateElement)
                    {
                        var surrogate = (ISurrogateElement)element;

                        return surrogate.ReferencedFrom.Parent.IsLocal();
                    }
                    else
                    {
                        return element.IsLocal();
                    }
                }
            }
        }

        public bool CanRead
        {
            get
            {
                if (this.NavigationProperty is string)
                {
                    return NavigationResultElement.Parent.Modifiers.HasFlag(Modifiers.CanRead);
                }
                else
                {
                    var element = (IElement)this.NavigationProperty;

                    return element.Modifiers.HasFlag(Modifiers.CanRead);
                }
            }
        }

        public bool CanWrite
        {
            get
            {
                if (this.NavigationProperty is string)
                {
                    return NavigationResultElement.Parent.Modifiers.HasFlag(Modifiers.CanWrite);
                }
                else
                {
                    var element = (IElement)this.NavigationProperty;

                    return element.Modifiers.HasFlag(Modifiers.CanWrite);
                }
            }
        }

        public string DebugInfo
        {
            get
            {
                var builder = new StringBuilder();

                if (this.NavigationResultBuild != null)
                {
                    builder.AppendFormat("Property: '{0}' returns type: '{1}'", this.PropertyName, this.NavigationResultBuild.Element.DataType.FullyQualifiedName);
                }
                else if (this.PropertyType != null)
                {
                    builder.AppendFormat("Property: '{0}' returns type: '{1}'", this.PropertyName, this.PropertyType.FullyQualifiedName);
                }
                else
                {
                    builder.AppendFormat("Property: '{0}' returns type: '{1}'", this.PropertyName, "unknown");
                }

                return builder.ToString();
            }
        }
    }
}
