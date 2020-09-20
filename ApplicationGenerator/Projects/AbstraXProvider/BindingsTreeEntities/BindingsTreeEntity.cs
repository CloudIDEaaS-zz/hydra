using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.Bindings;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
using AbstraX.ServerInterfaces;
using AbstraX.TypeMappings;

namespace AbstraX.BindingsTreeEntities
{
    [DataContract, DebuggerDisplay("{ DebugInfo }")]
    public class BindingsTreeEntity : BaseBindingsTreeNode, IBindingsTree
    {
        private IBindingsTree bindingsTreeInternal;
        private string parentID;
        private List<BindingsTreeNode> rootBindingEntities;

        public BindingsTreeEntity()
        {
        }

        public BindingsTreeEntity(IBindingsTree bindingsTree, string parentID)
        {
            bindingsTreeInternal = bindingsTree;
            this.parentID = parentID;
            this.BindingsTreeName = bindingsTree.BindingsTreeName;
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return bindingsTreeInternal;
            }
        }

        [Exclude]
        public IEnumerable<IBindingsTreeNode> RootBindings
        {
            get 
            {
                return bindingsTreeInternal.RootBindings;
            }
        }

        [Association("BindingsTree_BindingsTreeNodes", "ID", "ParentID")]
        public List<BindingsTreeNode> RootBindingEntities
        {
            get 
            {
                if (rootBindingEntities == null)
                {
                    rootBindingEntities = new List<BindingsTreeNode>();

                    this.RootBindings.ToList().ForEach(n => rootBindingEntities.Add(new BindingsTreeNode(n, this)));
                }

                return rootBindingEntities;
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
                return parentID;
            }
        }

        [DataMember]
        public override string Name
        {
            get
            {
                return bindingsTreeInternal.BindingsTreeName;
            }
            set
            {
                bindingsTreeInternal.BindingsTreeName = value;
            }
        }

        [DataMember]
        public override bool HasChildren
        {
            get
            {
                return bindingsTreeInternal.RootBindings.Count() > 0;
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
        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo(bindingsTreeInternal.BindingsTreeName);
            }
        }

        [DataMember]
        public string BindingsTreeName { get; set; }

        public IAbstraXProviderService AbstraXProviderService
        {
            get
            {
                return bindingsTreeInternal.AbstraXProviderService;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [DataMember]
        public bool GenerateStatelessConstructs { get; set; }
    }
}
