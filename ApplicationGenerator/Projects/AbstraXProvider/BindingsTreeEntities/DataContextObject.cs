using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX.Bindings;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
using AbstraX.TypeMappings;
using AbstraX.ServerInterfaces;

namespace AbstraX.BindingsTreeEntities
{
    [DataContract, DebuggerDisplay("{ DebugInfo }")]
    public class DataContextObject : BaseBindingsTreeNode
    {
        private IDataContextObject dataContextObjectInternal;
        private Folder dataContextObjectFolder;
        private Folder supportedOperationsFolder;
        private Folder remoteCallsFolder;

        public DataContextObject()
        {
        }

        public DataContextObject(IDataContextObject dataContextObject, BaseBindingsTreeNode parentNode)
        {
            this.dataContextObjectInternal = dataContextObject;
            this.ParentNode = parentNode;

            if (dataContextObject.ContextObject != null)
            {
                this.name = dataContextObject.ContextObject.Name;
            }
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return dataContextObjectInternal;
            }
        }

        [Exclude]
        public IDataContextObject DataContextObjectInternal
        {
            get 
            { 
                return dataContextObjectInternal; 
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

        public Folder DataContextObjectFolder
        {
            get
            {
                if (dataContextObjectFolder == null)
                {
                    dataContextObjectFolder = new Folder(this, BindingsFolderNames.ContextObject);
                    dataContextObjectFolder.ChildNodes.Add(new Element(dataContextObjectInternal.ContextObject, dataContextObjectFolder));
                }

                return dataContextObjectFolder;
            }
        }

        public Folder RemoteCallsFolder
        {
            get
            {
                if (remoteCallsFolder == null)
                {
                    remoteCallsFolder = new Folder(this, BindingsFolderNames.RemoteCalls);
                    remoteCallsFolder.ChildNodes.AddRange(dataContextObjectInternal.RemoteCalls.Select(o => new Operation(o, remoteCallsFolder)));
                }

                return remoteCallsFolder;
            }
        }

        public Folder SupportedOperationsFolder
        {
            get
            {
                if (supportedOperationsFolder == null)
                {
                    supportedOperationsFolder = new Folder(this, BindingsFolderNames.SupportedOperations);

                    foreach (OptionalDataContextOperation operation in Enum.GetValues(typeof(OptionalDataContextOperation)))
                    {
                        if ((operation & dataContextObjectInternal.SupportedOperations) == operation)
                        {
                            var name = Enum.GetName(typeof(OptionalDataContextOperation), operation);
                            supportedOperationsFolder.ChildNodes.Add(new NodeProperty(name, supportedOperationsFolder));
                        }
                    }
                }

                return supportedOperationsFolder;
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
        public override bool HasChildren
        {
            get
            {
                return true;
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
                return this.GetDebugInfo(dataContextObjectInternal.UpdatedName);
            }
        }
    }
}
