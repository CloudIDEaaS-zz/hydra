using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public enum AddInHandlerKind
    {
        MetadataReflection
    }

    public class AddInHandlerAttribute : Attribute
    {
        public AddInHandlerKind AddInHandlerKind { get; }
        public Guid AbstraXProviderGuid { get; }

        public AddInHandlerAttribute(AddInHandlerKind kind, string abstraXProviderGuid)
        {
            this.AddInHandlerKind = kind;
            this.AbstraXProviderGuid = Guid.Parse(abstraXProviderGuid);
        }
    }

    public interface IAddInMetadataReflectionHandler
    {
        void Handle(GetAddInEntitiesEventArgs addInEntitiesEventArgs, Type metadataClassType);
    }
}
