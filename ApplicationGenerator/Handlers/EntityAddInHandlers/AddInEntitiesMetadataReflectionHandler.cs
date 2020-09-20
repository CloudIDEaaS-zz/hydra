using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AbstraX;
using AbstraX.Models.Interfaces;
using EntityProvider.Web.Entities;

namespace AbstraX.Handlers.EntityAddInHandlers
{

    [AddInHandler(AddInHandlerKind.MetadataReflection, AbstraXProviderGuids.Entities)]
    public class AddInEntitiesMetadataReflectionHandler : IAddInMetadataReflectionHandler
    {
        public void Handle(GetAddInEntitiesEventArgs args, Type metadataClassType)
        {
            switch (args.DefinitionKind)
            {
                case ServerInterfaces.DefinitionKind.ComplexSetProperty:

                    var entityContainer = (Entity_Container) args.BaseObject;
                    var children = args.GetCurrentChildren();
                    var addInSets = metadataClassType.GetProperties().Where(p => !children.Any((o) => o.Name == p.Name));

                    foreach (var addInSet in addInSets)
                    {
                        XmlElement element = CreateEntitySet(entityContainer, entityContainer.Name, addInSet);

                        args.Entities.Add(args.EntityFactory.Call(addInSet.Name, element));
                    }

                    break;
            }
        }

        private XmlElement CreateEntitySet(EntityBase entitiesBase, string containerName, PropertyInfo addInSet)
        {
            var fragment = entitiesBase.EdmxDocument.CreateDocumentFragment();
            var containerElement = (XmlElement) entitiesBase.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema/e:EntityContainer[@Name='" + containerName + "']", entitiesBase.NamespaceManager);
            var type = addInSet.PropertyType.GetGenericArguments()[0];
            XmlElement element;

            fragment.InnerXml = $"<EntitySet Name=\"{ addInSet.Name }\" EntityType =\"{ type.Namespace.Split('.').Last() + "." + type.Name }\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm\" />";

            element = (XmlElement)containerElement.AppendChild(fragment);

            CreateEntity(entitiesBase, type);

            return element;
        }

        private XmlElement CreateEntity(EntityBase entitiesBase, Type addInEntity)
        {
            var fragment = entitiesBase.EdmxDocument.CreateDocumentFragment();
            XmlElement element;
            XmlElement schemaElement = (XmlElement)entitiesBase.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema", entitiesBase.NamespaceManager);

            fragment.InnerXml = $"<EntityType Name=\"{ addInEntity.Name }\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm\" />";

            element = (XmlElement)schemaElement.AppendChild(fragment);

            foreach (var property in addInEntity.GetProperties())
            {
                XmlElement propertyElement;
                string propertyTypeName;

                fragment = entitiesBase.EdmxDocument.CreateDocumentFragment();

                if (property.PropertyType.Name == "Nullable`1")
                {
                    propertyTypeName = property.PropertyType.GetGenericArguments().Single().Name;
                    fragment.InnerXml = $"<Property Name=\"{ property.Name }\" Type=\"{ propertyTypeName }\" Nullable=\"true\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm\" />";
                }
                else
                {
                    propertyTypeName = property.PropertyType.Name;
                    fragment.InnerXml = $"<Property Name=\"{ property.Name }\" Type=\"{ propertyTypeName }\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm\" />";
                }

                propertyElement = (XmlElement) element.AppendChild(fragment);
            }

            return element;
        }
    }
}
