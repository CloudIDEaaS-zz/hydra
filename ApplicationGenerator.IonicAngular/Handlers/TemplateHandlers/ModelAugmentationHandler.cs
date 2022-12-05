// file:	Handlers\TemplateHandlers\BusinessModelHandler.cs
//
// summary:	Implements the business model handler class

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using AbstraX;
using AbstraX.DataAnnotations;
using AbstraX.TemplateObjects;
using CodeInterfaces;
using Microsoft.VisualStudio.OLE.Interop;
using Newtonsoft.Json.Serialization;
using Utils;
using Utils.Hierarchies;
using Utils.Parsing.Nodes;

namespace AbstraX.Handlers.TemplateHandlers
{
    /// <summary>   The business model handler. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public class ModelAugmentationHandler : IModelAugmentationHandler
    {
        private IDynamicModuleBuilder dynamicModuleBuilder;

        /// <summary>   Gets the priority. </summary>
        ///
        /// <value> The priority. </value>

        public float Priority => 1.0f;

        /// <summary>   Process this. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityDomainModel">        The entity domain model. </param>
        /// <param name="businessModel">            The business model. </param>
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="projectFolderRoot">        The project folder root. </param>
        /// <param name="entitiesProject"></param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="appUIHierarchyNodeObject"></param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Process(EntityDomainModel entityDomainModel, BusinessModel businessModel, Guid projectType, string projectFolderRoot, IVSProject entitiesProject, IGeneratorConfiguration generatorConfiguration, out AppUIHierarchyNodeObject appUIHierarchyNodeObject)
        {
            var appSystemObject = businessModel.GetDescendants().Single(a => a.IsApp && a.Name == generatorConfiguration.AppName);
            var topLevelObject = businessModel.TopLevelObject;
            var topHierarchyNodeObjects = new List<UIHierarchyNodeObject>();
            var allHierarchyNodeObjects = new List<UIHierarchyNodeObject>();
            var appSettingsObjects = new Dictionary<AppSettingsKind, BusinessModelObject>();
            ModuleBuilder entititesModuleBuilder;
            UIHierarchyNodeObject topHierarchyNodeObject = null;
            IEntityContainerHandler entityContainerHandler;
            IModelTypeBuilderHandler modelTypeBuilderHandler;
            Assembly entitiesDynamicAssembly;
            EntityObject entityContainer;
            string debugUIHierarchy;

            // create the UI hierarchy

            appUIHierarchyNodeObject = null;

            topLevelObject.GetDescendantsAndSelf<BusinessModelObject, UIHierarchyNodeObject>(o => o.Children, (o, n) =>
            {
                var hierarchyNodeObject = new UIHierarchyNodeObject(o);

                if (topHierarchyNodeObject == null)
                {
                    topHierarchyNodeObject = hierarchyNodeObject;
                }

                if (n != null)
                {
                    n.AddChild(hierarchyNodeObject);
                }

                return hierarchyNodeObject;
            });

            // prune the tree

            topHierarchyNodeObject.GetDescendantsAndSelf(n => n.Children, n =>
            {
                if (n.Parent != null)
                {
                    var parent = n.Parent;

                    if (!parent.ShowInUI)
                    {
                        var newParent = parent.Parent;

                        if (newParent == null)
                        {
                            n.Parent = null;

                            if (n.ShowInUI)
                            {
                                topHierarchyNodeObjects.Add(n);
                            }
                        }
                        else
                        {
                            newParent.Children.Remove(parent);
                            newParent.AddChild(n);
                        }
                    }
                }
            });

            foreach (var hierarchyNodeObject in topHierarchyNodeObjects)
            {
                allHierarchyNodeObjects.AddRange(hierarchyNodeObject.GetDescendantsAndSelf());
            }

            // create the app UI Hierarchy from the system node of the business model

            appUIHierarchyNodeObject = allHierarchyNodeObjects.Single(n => n.Id == appSystemObject.Id).CreateCopy<AppUIHierarchyNodeObject>();
            appUIHierarchyNodeObject.AppSystemObject = appSystemObject;
            appUIHierarchyNodeObject.BusinessModel = businessModel;

            foreach (var hierarchyNodeObject in topHierarchyNodeObjects)
            {
                hierarchyNodeObject.Name = "Main";

                appUIHierarchyNodeObject.TopUIHierarchyNodeObjects.Add(hierarchyNodeObject);
            }

            // attach entities

            foreach (var entity in entityDomainModel.Entities)
            {
                if (entity.IsInherentDataItem)
                {
                    appUIHierarchyNodeObject.InherentEntities.Add(entity);
                }
                else
                {
                    var hierarchyNodeObject = allHierarchyNodeObjects.Single(n => n.Id == entity.ParentDataItem);

                    foreach (var shadowHierarchyNodeObject in allHierarchyNodeObjects.Where(n => n.ShadowItem == hierarchyNodeObject.Id))
                    {
                        shadowHierarchyNodeObject.Entities.Add(entity);
                    }

                    hierarchyNodeObject.Entities.Add(entity);
                }

                appUIHierarchyNodeObject.AllEntities.Add(entity);
            }

            // create new entities/attributes from app settings kind

            foreach (var appSettingsObject in topLevelObject.GetDescendants().Where(o => o.AppSettingsKind != null))
            {
                var appSettingsKind = EnumUtils.GetValue<AppSettingsKind>(appSettingsObject.AppSettingsKind);

                appSettingsObjects.Add(appSettingsKind, appSettingsObject);
            }

            foreach (var pair in appSettingsObjects)
            {
                var appSettingsKind = pair.Key;
                var handler = generatorConfiguration.GetAppSettingsKindHandler(appSettingsKind);

                if (handler != null)
                {
                    handler.Process(entityDomainModel, businessModel, appUIHierarchyNodeObject, appSettingsObjects, projectType, projectFolderRoot, generatorConfiguration);
                }
            }

            // create new entities/attributes from many-to-many containers
            
            foreach (var entity in appUIHierarchyNodeObject.AllEntities.ToList().Where(e => e.HasRelatedEntityAttributes()))
            {
                foreach (var containsManyToManyAttribute in entity.GetRelatedEntityAttributes().Where(a => a.Properties.Single(p => p.PropertyName == "RelatedEntity").ChildProperties.Any(p2 => p2.PropertyName == "RelationshipKind" && p2.PropertyValue == "ContainsManyToMany")))
                {
                    HandleManyToMany(appUIHierarchyNodeObject, entity, containsManyToManyAttribute);
                }
            }

            // create entity container

            entityContainerHandler = generatorConfiguration.GetEntityContainerHandler();
            entityContainerHandler.Process(appUIHierarchyNodeObject, projectType, projectFolderRoot, entitiesProject, generatorConfiguration, out entityContainer);
            appUIHierarchyNodeObject.EntityContainer = entityContainer;

            // create new entities/attributes from annotation pre-processing

            PreProcessAnnotations(appUIHierarchyNodeObject, generatorConfiguration);

            // add primary keys and indexes

            AddPrimaryKeysAndIndexes(appUIHierarchyNodeObject, generatorConfiguration);

            debugUIHierarchy = appUIHierarchyNodeObject.DebugPrintUIHierarchy();

            // build types

            dynamicModuleBuilder = generatorConfiguration.GetDynamicModuleBuilder();

            entititesModuleBuilder = dynamicModuleBuilder.CreateDynamicModuleBuilder(entitiesProject);

            appUIHierarchyNodeObject.EntitiesModuleBuilder = entititesModuleBuilder;

            // create type builders first so they can reference each other

            modelTypeBuilderHandler = generatorConfiguration.GetModelTypeBuilderHandler();

            foreach (var entity in appUIHierarchyNodeObject.AllEntities.Concat(new List<EntityObject> { appUIHierarchyNodeObject.EntityContainer }))
            {
                modelTypeBuilderHandler.CreateTypeBuilderForEntity(entititesModuleBuilder, entity, appUIHierarchyNodeObject, generatorConfiguration);
            }

            foreach (var entity in appUIHierarchyNodeObject.AllEntities.Concat(new List<EntityObject> { appUIHierarchyNodeObject.EntityContainer }))
            {
                modelTypeBuilderHandler.CreateTypeForEntity(entititesModuleBuilder, entity, appUIHierarchyNodeObject, generatorConfiguration);
            }

            entitiesDynamicAssembly = dynamicModuleBuilder.LoadAndAttachToAssembly(appUIHierarchyNodeObject);

            return true;
        }

        private void AddPrimaryKeysAndIndexes(AppUIHierarchyNodeObject appUIHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration)
        {
            foreach (var entity in appUIHierarchyNodeObject.AllEntities.ToList())
            {
                string identityEntityKind = null;

                if (entity.Properties.Any(p => p.PropertyName == "IdentityEntity"))
                {
                    var identityEntityProperty = entity.Properties.Single(p => p.PropertyName == "IdentityEntity");
                    var identityEntityKindProperty = identityEntityProperty.ChildProperties.Single(p => p.PropertyName == "IdentityEntityKind");

                    identityEntityKind = identityEntityKindProperty.PropertyValue;
                }

                if (identityEntityKind == "User")
                {
                    entity.Attributes.Insert(0, new AttributeObject
                    {
                        Name = entity.Name + "Id",
                        AttributeType = "Guid",
                        Properties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem
                            {
                                PropertyName = "Key"
                            },
                            new EntityPropertyItem
                            {
                                PropertyName = "IdentityField",
                                ChildProperties = new List<EntityPropertyItem>()
                                {
                                    new EntityPropertyItem
                                    {
                                        PropertyName = "UIHierarchyPath",
                                        PropertyValue = "/"
                                    },
                                    new EntityPropertyItem
                                    {
                                        PropertyName = "IdentityFieldKind",
                                        PropertyValue = "UserId"
                                    },
                                }
                            }
                        }
                    });
                }
                else
                {
                    entity.Attributes.Insert(0, new AttributeObject
                    {
                        Name = entity.Name + "Id",
                        AttributeType = "Guid",
                        Properties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem
                            {
                                PropertyName = "Key"
                            }
                        }
                    });
                }
            }
        }

        private void PreProcessAnnotations(AppUIHierarchyNodeObject appUIHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration)
        {
            foreach (var entity in appUIHierarchyNodeObject.AllEntities.ToList())
            {
                foreach (var property in entity.Properties.ToList())
                {
                    var annotationAtrributeType = generatorConfiguration.FindDataAnnotationType(property.PropertyName + "Attribute");
                    var handler = generatorConfiguration.GetDataAnnotationTypeHandler(property.PropertyName, annotationAtrributeType);

                    if (handler != null)
                    {
                        if (!handler.PreProcess(entity, property, annotationAtrributeType, appUIHierarchyNodeObject, generatorConfiguration))
                        {
                            throw new Exception($"Cannot process annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ property.PropertyName }");
                        }
                    }
                    else
                    {
                        throw new HandlerNotFoundException($"Cannot find annotation type handler for annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ property.PropertyName }");
                    }
                }

                foreach (var attribute in entity.Attributes.ToList())
                {
                    var propertyName = attribute.Name.RemoveText(" ");
                    var propertyTypeName = attribute.AttributeType;

                    if (propertyTypeName != "related entity")
                    {
                        foreach (var property in attribute.Properties.Where(p => !p.IsMarkerProperty()).ToList())
                        {
                            var annotationAtrributeType = generatorConfiguration.FindDataAnnotationType(property.PropertyName + "Attribute");
                            var handler = generatorConfiguration.GetDataAnnotationTypeHandler(property.PropertyName, annotationAtrributeType);

                            if (handler != null)
                            {
                                if (!handler.PreProcess(entity, attribute, property, annotationAtrributeType, appUIHierarchyNodeObject, generatorConfiguration))
                                {
                                    throw new Exception($"Cannot process annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ property.PropertyName }");
                                }
                            }
                            else
                            {
                                throw new HandlerNotFoundException($"annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ property.PropertyName }");
                            }
                        }
                    }
                }
            }
        }

        private void HandleManyToMany(AppUIHierarchyNodeObject appUIHierarchyNodeObject, EntityObject entity, AttributeObject containsManyToManyAttribute)
        {
            var relatedEntityProperty = containsManyToManyAttribute.Properties.Single(p => p.PropertyName == "RelatedEntity");
            var containsManyToManyProperty = relatedEntityProperty.ChildProperties.Single(p2 => p2.PropertyName == "RelationshipKind" && p2.PropertyValue == "ContainsManyToMany");
            var existingEntityProperty = containsManyToManyProperty.ChildProperties.Single(p3 => p3.PropertyName == "ExistingEntity");
            var existingEntityName = existingEntityProperty.PropertyValue;
            var existingEntity = appUIHierarchyNodeObject.AllEntities.Single(e => e.Name == existingEntityName);
            var containerNameProperty = containsManyToManyProperty.ChildProperties.Single(p3 => p3.PropertyName == "ContainerName");
            var containerName = containerNameProperty.PropertyValue;
            var associateEntityObject = new EntityObject()
            {
                Name = containerName,
                IsInherentDataItem = true,
                Attributes = new List<AttributeObject>()
                {
                    new AttributeObject
                    {
                        Name = entity.Name,
                        AttributeType = "related entity",
                        Properties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem()
                            {
                                PropertyName = "RelatedEntity",
                                ChildProperties =  new List<EntityPropertyItem>()
                                {
                                    new EntityPropertyItem()
                                    {
                                        PropertyName = "RelationshipKind",
                                        PropertyValue = "IsLookupOfManyToOne",
                                        ChildProperties = new List<EntityPropertyItem>()
                                        {
                                            new EntityPropertyItem()
                                            {
                                                PropertyName = "ExistingEntity",
                                                PropertyValue = entity.Name
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new AttributeObject
                    {
                        Name = existingEntityName,
                        AttributeType = "related entity",
                        Properties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem()
                            {
                                PropertyName = "RelatedEntity",
                                ChildProperties =  new List<EntityPropertyItem>()
                                {
                                    new EntityPropertyItem()
                                    {
                                        PropertyName = "RelationshipKind",
                                        PropertyValue = "IsLookupOfManyToOne",
                                        ChildProperties = new List<EntityPropertyItem>()
                                        {
                                            new EntityPropertyItem()
                                            {
                                                PropertyName = "ExistingEntity",
                                                PropertyValue = existingEntityName
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            if (entity.HasIdentityEntityKind(IdentityEntityKind.User) && existingEntity.HasIdentityEntityKind(IdentityEntityKind.Role))
            {
                associateEntityObject.Properties = new List<EntityPropertyItem>()
                {
                    new EntityPropertyItem()
                    {
                        PropertyName = "IdentityEntity",
                        ChildProperties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem()
                            {
                                PropertyName = "IdentityEntityKind",
                                PropertyValue = IdentityEntityKind.UserToRole.ToString()
                            }
                        }
                    }
                };
            }

            appUIHierarchyNodeObject.InherentEntities.Add(associateEntityObject);
            appUIHierarchyNodeObject.AllEntities.Add(associateEntityObject);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>

        public void Dispose()
        {
            dynamicModuleBuilder.Dispose();
        }
    }
}
  