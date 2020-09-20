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
    [DataContract, NodeImage("EntityProvider.Web.Images.EntityProperty.png"), DebuggerDisplay("{ DebugInfo }"), ClientCodeGeneration(typeof(AbstraXClientInterfaceGenerator))]
    public class DbEntityProperty : EntityProperty, IAttribute 
    {
        private IAttribute abstraXAttribute;
        private string databaseID;
        private string relatedID;
        private string documentationID;

        public DbEntityProperty()
        {
        }

        public DbEntityProperty(IParentBase parent, IDbAttribute abstraXAttribute, IAbstraXService service) : base(service)
        {
            this.abstraXAttribute = abstraXAttribute;
            this.parent = parent;
            this.name = abstraXAttribute.Name;
            this.dataType = abstraXAttribute.DataType;
            this.designComments = abstraXAttribute.DesignComments;
            id = this.MakeID("DbProperty='" + abstraXAttribute.DatabaseID + "'");

            providerEntityService = ((IBase)parent).ProviderEntityService;

            // TODO - shenkey - should not be a field in database
            hasDocumentation = this.HasDocumentation;

            // TODO - this will be retrieved dynamically
            //documentation = abstraXElement.Documentation;

            documentationSummary = abstraXAttribute.DocumentationSummary;
            childOrdinal = abstraXAttribute.ChildOrdinal;
            debugInfo = this.GetDebugInfo();
            databaseID = abstraXAttribute.DatabaseID;
            relatedID = abstraXAttribute.RelatedID;
       //     documentationID = abstraXAttribute.DocumentationID;
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
