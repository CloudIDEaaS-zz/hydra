using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.ServerInterfaces;
using System.ServiceModel.DomainServices.Server;
using AbstraX.Contracts;
using AbstraX.TypeMappings;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX;
using AbstraX.Templates;

namespace EntityProvider.Web.Entities.DatabaseEntities
{
    [DataContract, NodeImage("EntityProvider.Web.Images.EntitySet.png"), DebuggerDisplay("{ DebugInfo }"), ClientCodeGeneration(typeof(AbstraXClientInterfaceGenerator))]
    public class DbEntitySet : Entity_Set, IElement 
    {
        private IElement abstraXElement;
        private string databaseID;
        private string relatedID;
        private string documentationID;

        public DbEntitySet()
        {
        }

        public DbEntitySet(IParentBase parent, IDbElement abstraXElement, IAbstraXService  service) : base(service)
        {
            this.abstraXElement = abstraXElement;
            this.parent = parent;
            this.name  = abstraXElement.Name;
            id = this.MakeID("DbEntitySet='" + abstraXElement.DatabaseID + "'");

            providerEntityService = ((IBase)parent).ProviderEntityService;

            designComments = abstraXElement.DesignComments;

            // TODO - shenkey - should not be a field in database
            hasDocumentation = abstraXElement.HasDocumentation;
            
            // TODO - this will be retrieved dynamically
            //documentation = abstraXElement.Documentation;

            documentationSummary = abstraXElement.DocumentationSummary;
            childOrdinal = abstraXElement.ChildOrdinal;
            debugInfo = this.GetDebugInfo() + "*******HasDocumentation=  " + abstraXElement.HasDocumentation.ToString();
            databaseID = abstraXElement.DatabaseID;
            relatedID = abstraXElement.RelatedID;
         //   documentationID = abstraXElement.DocumentationID;
        }

        [DataMember]
        public string DatabaseID
        {
            get { return this.databaseID; }
        }

        [DataMember]
        public string RelatedID
        {
            get { return this.relatedID; }
        }

        [DataMember]
        public string DocumentationID
        {
            get { return this.documentationID; }
        }
    }
}
