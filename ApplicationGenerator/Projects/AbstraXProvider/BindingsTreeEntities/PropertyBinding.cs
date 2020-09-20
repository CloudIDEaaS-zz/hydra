using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX.Bindings;
using AbstraX.ServerInterfaces;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;

namespace AbstraX.BindingsTreeEntities
{
    [DataContract, DebuggerDisplay("{ DebugInfo }")]
    public class PropertyBinding : BaseBindingsTreeNode, IPropertyBinding
    {
        private IPropertyBinding propertyBindingInternal;
        private AttributeProperty attribute;
        private Folder bindingSourceFolder;
        private NodeProperty bindingModeProperty;

        public PropertyBinding()
        {
        }

        public PropertyBinding(IPropertyBinding propertyBinding, BaseBindingsTreeNode parentNode)
        {
            propertyBindingInternal = propertyBinding;
            this.ParentNode = parentNode;
            this.name = propertyBinding.PropertyBindingName;
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return propertyBindingInternal;
            }
        }

        public Folder BindingSourceFolder
        {
            get
            {
                if (bindingSourceFolder == null)
                {
                    bindingSourceFolder = new Folder(this, BindingsFolderNames.BindingsSource);

                    if (propertyBindingInternal.BindingSource is IAbstraXBindingSource)
                    {
                        bindingSourceFolder.ChildNodes.Add(new AbstraXBindingSource((IAbstraXBindingSource) propertyBindingInternal.BindingSource, bindingSourceFolder));
                    }
                    else if (propertyBindingInternal.BindingSource is IQueryBindingSource)
                    {
                        bindingSourceFolder.ChildNodes.Add(new QueryBindingSource((IQueryBindingSource)propertyBindingInternal.BindingSource, bindingSourceFolder));
                    }
                    else
                    {
                        Debugger.Break();
                    }
                }

                return bindingSourceFolder;
            }
        }

        public AttributeProperty PropertyAttribute
        {
            get
            {
                if (attribute == null)
                {
                    attribute = new AttributeProperty(propertyBindingInternal.Property, this);
                }

                return attribute;
            }
        }

        [Exclude]
        public IAttribute Property
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [Exclude]
        public IBindingSource BindingSource
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public NodeProperty BindingModeProperty
        {
            get
            {
                if (bindingModeProperty == null)
                {
                    bindingModeProperty = new NodeProperty("BindingMode", Enum.GetName(typeof(BindingMode), propertyBindingInternal.BindingMode), this);
                }

                return bindingModeProperty;
            }
        }

        [Exclude]
        public BindingMode BindingMode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [DataMember, Key]
        public override string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        [DataMember]
        public override string ParentID
        {
            get
            {
                return this.ParentNode.ID;
            }
        }

        [DataMember]
        public override string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        [DataMember]
        public override float ChildOrdinal
        {
            get
            {
                return 0;
            }
        }

        [DataMember]
        public override bool HasChildren
        {
            get 
            {
                return true;
            }
        }

        [DataMember]
        public override string DebugInfo
        {
            get 
            {
                return this.GetDebugInfo(propertyBindingInternal.PropertyBindingName);
            }
        }

        public string PropertyBindingName
        {
            get
            {
                return propertyBindingInternal.PropertyBindingName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
