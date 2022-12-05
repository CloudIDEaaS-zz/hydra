// file:	Handlers\TemplateHandlers\ModelTypeBuilderHandler.cs
//
// summary:	Implements the model type builder handler class

using AbstraX.TemplateObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.TemplateHandlers
{
    /// <summary>   A model type builder handler. </summary>
    ///
    /// <remarks>   Ken, 10/12/2020. </remarks>

    public class ModelTypeBuilderHandler : IModelTypeBuilderHandler
    {
        /// <summary>   Gets the priority. </summary>
        ///
        /// <value> The priority. </value>

        public float Priority => 1.0f;

        /// <summary>   Creates type builder for entity. </summary>
        ///
        /// <remarks>   Ken, 10/12/2020. </remarks>
        ///
        /// <param name="moduleBuilder">            The module builder. </param>
        /// <param name="entity">                   The entity. </param>
        /// <param name="appUIHierarchyNodeObject"> The application hierarchy node object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>

        public void CreateTypeBuilderForEntity(ModuleBuilder moduleBuilder, EntityObject entity, AppUIHierarchyNodeObject appUIHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration)
        {
            var allEntities = appUIHierarchyNodeObject.AllEntities;
            var namespaceName = moduleBuilder.Assembly.GetName().Name;
            var entityName = entity.Name.RemoveSpaces();
            TypeBuilder typeBuilder;
            TypeBuilder metadataTypeBuilder;

            if (entity == appUIHierarchyNodeObject.EntityContainer)
            {
                typeBuilder = moduleBuilder.DefineType(namespaceName + ".Models." + entityName, TypeAttributes.Public | TypeAttributes.Class, typeof(DbContext));
            }
            else
            {
                typeBuilder = moduleBuilder.DefineType(namespaceName + ".Models." + entityName, TypeAttributes.Public | TypeAttributes.Class, typeof(object));
            }

            metadataTypeBuilder = moduleBuilder.DefineType(namespaceName + ".Metadata." + entityName + "Metadata", TypeAttributes.Public | TypeAttributes.Class, typeof(object));

            entity.DynamicEntityTypeBuilder = typeBuilder;
            entity.DynamicEntityMetadataTypeBuilder = metadataTypeBuilder;
        }

        /// <summary>   Creates type for entity. </summary>
        ///
        /// <remarks>   Ken, 10/12/2020. </remarks>
        ///
        /// <exception cref="Exception">                Thrown when an exception error condition occurs. </exception>
        /// <exception cref="HandlerNotFoundException"> Thrown when a Handler Not Found error condition
        ///                                             occurs. </exception>
        ///
        /// <param name="moduleBuilder">            The module builder. </param>
        /// <param name="entity">                   The entity. </param>
        /// <param name="appUIHierarchyNodeObject"> The application hierarchy node object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>

        public void CreateTypeForEntity(ModuleBuilder moduleBuilder, EntityObject entity, AppUIHierarchyNodeObject appUIHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration)
        {
            var allEntities = appUIHierarchyNodeObject.AllEntities;
            var namespaceName = moduleBuilder.Assembly.GetName().Name;
            var entityName = entity.Name.RemoveSpaces();
            var typeBuilder = entity.DynamicEntityTypeBuilder;
            var metadataTypeBuilder = entity.DynamicEntityMetadataTypeBuilder;
            var metadataTypeAttributeType = typeof(System.ComponentModel.DataAnnotations.MetadataTypeAttribute);
            var metadataTypeAttributeTypeConstructor = metadataTypeAttributeType.GetConstructor(new Type[] { typeof(Type) });
            Type entityType;
            Type metadataType;
            Type appResources;
            ILGenerator ilGenerator;
            string metadataCode;

            if (!generatorConfiguration.KeyValuePairs.ContainsKey("AppResources"))
            {
                var appResourcesTypeBuilder = moduleBuilder.DefineType(namespaceName + ".Resources." + "AppResources", TypeAttributes.Public | TypeAttributes.Class, typeof(object));

                appResources = appResourcesTypeBuilder.CreateType();
                generatorConfiguration.KeyValuePairs.Add("AppResources", appResources);
            }

            // flush out the metadata

            foreach (var property in entity.Properties.Where(p => !p.IsMarkerProperty()))
            {
                var annotationAtrributeType = generatorConfiguration.FindDataAnnotationType(property.PropertyName + "Attribute");
                var handler = generatorConfiguration.GetDataAnnotationTypeHandler(property.PropertyName, annotationAtrributeType);

                if (handler != null)
                {
                    if (!handler.Process(entity, property, annotationAtrributeType, metadataTypeBuilder, appUIHierarchyNodeObject, generatorConfiguration))
                    {
                        throw new Exception($"Cannot process annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ property.PropertyName }");
                    }
                }
                else
                {
                    throw new HandlerNotFoundException($"Cannot find annotation type handler for annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ property.PropertyName }");
                }
            }

            foreach (var attribute in entity.Attributes)
            {
                var propertyName = attribute.Name.RemoveText(" ");
                var propertyTypeName = attribute.AttributeType;
                Type propertyType;
                PropertyBuilder propertyBuilder;
                MethodBuilder getMethod;
                MethodBuilder setMethod;
                var regex = new Regex(@"EntitySet\[@Entity='(?<entity>.*?)'\]");
                var propertyMethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

                if (propertyTypeName == "related entity")
                {
                    var relatedEntityProperty = attribute.Properties.Single(p => p.PropertyName == "RelatedEntity");
                    var relationshipKindProperty = relatedEntityProperty.ChildProperties.Single(p => p.PropertyName == "RelationshipKind");
                    var relationshipKind = relationshipKindProperty.PropertyValue;

                    switch (relationshipKind)
                    {
                        case "ContainsManyToMany":
                            {
                                var entityNameProperty = relationshipKindProperty.ChildProperties.Single(p => p.PropertyName == "ContainerName");
                                var relatedEntityName = entityNameProperty.PropertyValue.RemoveSpaces();
                                var entityTypeArg = moduleBuilder.GetTypes().Single(t => t.Name == relatedEntityName);

                                propertyName = relatedEntityName.Pluralize();

                                propertyType = typeof(List<>).MakeGenericType(entityTypeArg);
                            }

                            break;

                        case "IsLookupOfManyToOne":
                            {
                                var entityNameProperty = relationshipKindProperty.ChildProperties.Single(p => p.PropertyName == "ExistingEntity");
                                var relatedEntityName = entityNameProperty.PropertyValue.RemoveSpaces();
                                var lookupEntityType = moduleBuilder.GetTypes().Single(t => t.Name == relatedEntityName);

                                propertyType = lookupEntityType;
                            }

                            break;

                        case "ContainsOneToMany":
                            {
                                var entityNameProperty = relationshipKindProperty.ChildProperties.Single(p => p.PropertyName == "ExistingEntity");
                                var relatedEntityName = entityNameProperty.PropertyValue.RemoveSpaces();
                                var entityTypeArg = moduleBuilder.GetTypes().Single(t => t.Name == relatedEntityName);

                                propertyType = typeof(List<>).MakeGenericType(entityTypeArg);
                            }

                            break;

                        default:
                            DebugUtils.Break();
                            propertyType = null;
                            break;
                    }

                    propertyMethodAttributes = EnumUtils.SetFlag<MethodAttributes>(propertyMethodAttributes, MethodAttributes.Virtual);
                }
                else
                {
                    if (regex.IsMatch(propertyTypeName))
                    {
                        var match = regex.Match(propertyTypeName);
                        var dbSetEntityName = match.GetGroupValue("entity").RemoveSpaces();
                        var entityTypeArg = moduleBuilder.GetTypes().Single(t => t.Name == dbSetEntityName);

                        propertyType = typeof(DbSet<>).MakeGenericType(entityTypeArg);
                    }
                    else if (propertyTypeName == "Guid")
                    {
                        propertyType = typeof(Guid);
                    }
                    else
                    {
                        propertyType = Type.GetType(TypeExtensions.GetPrimitiveTypeFullName(propertyTypeName));
                    }
                }

                propertyBuilder = metadataTypeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
                getMethod = metadataTypeBuilder.DefineMethod("get_" + propertyName, propertyMethodAttributes, propertyType, null);
                setMethod = metadataTypeBuilder.DefineMethod("set_" + propertyName, propertyMethodAttributes, typeof(void), new Type[] { propertyType });

                propertyBuilder.SetGetMethod(getMethod);
                propertyBuilder.SetSetMethod(setMethod);

                ilGenerator = getMethod.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldnull);
                ilGenerator.Emit(OpCodes.Ret);

                ilGenerator = setMethod.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ret);

                foreach (var property in attribute.Properties.Where(p => !p.IsMarkerProperty()).ToList())
                {
                    if (property.PropertyName != "RelatedEntity")
                    {
                        var annotationAtrributeType = generatorConfiguration.FindDataAnnotationType(property.PropertyName + "Attribute");
                        var handler = generatorConfiguration.GetDataAnnotationTypeHandler(property.PropertyName, annotationAtrributeType);

                        if (handler != null)
                        {
                            if (!handler.Process(entity, attribute, property, annotationAtrributeType, propertyBuilder, appUIHierarchyNodeObject, generatorConfiguration))
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

            metadataType = metadataTypeBuilder.CreateType();
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(metadataTypeAttributeTypeConstructor, new object[] { metadataType }));

            // flush out the entity type

            foreach (var attribute in entity.Attributes)
            {
                var propertyName = attribute.Name.RemoveText(" ");
                var propertyTypeName = attribute.AttributeType;
                Type propertyType;
                PropertyBuilder propertyBuilder;
                MethodBuilder getMethod;
                MethodBuilder setMethod;
                var regex = new Regex(@"EntitySet\[@Entity='(?<entity>.*?)'\]");
                var propertyMethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

                if (propertyTypeName == "related entity")
                {
                    var relatedEntityProperty = attribute.Properties.Single(p => p.PropertyName == "RelatedEntity");
                    var relationshipKindProperty = relatedEntityProperty.ChildProperties.Single(p => p.PropertyName == "RelationshipKind");
                    var relationshipKind = relationshipKindProperty.PropertyValue;

                    switch (relationshipKind)
                    {
                        case "ContainsManyToMany":
                            {
                                var entityNameProperty = relationshipKindProperty.ChildProperties.Single(p => p.PropertyName == "ContainerName");
                                var relatedEntityName = entityNameProperty.PropertyValue.RemoveSpaces();
                                var entityTypeArg = moduleBuilder.GetTypes().Single(t => t.Name == relatedEntityName);

                                propertyName = relatedEntityName.Pluralize();

                                propertyType = typeof(List<>).MakeGenericType(entityTypeArg);
                            }

                            break;

                        case "IsLookupOfManyToOne":
                            {
                                var entityNameProperty = relationshipKindProperty.ChildProperties.Single(p => p.PropertyName == "ExistingEntity");
                                var relatedEntityName = entityNameProperty.PropertyValue.RemoveSpaces();
                                var lookupEntityType = moduleBuilder.GetTypes().Single(t => t.Name == relatedEntityName);

                                propertyType = lookupEntityType;
                            }

                            break;

                        case "ContainsOneToMany":
                            {
                                var entityNameProperty = relationshipKindProperty.ChildProperties.Single(p => p.PropertyName == "ExistingEntity");
                                var relatedEntityName = entityNameProperty.PropertyValue.RemoveSpaces();
                                var entityTypeArg = moduleBuilder.GetTypes().Single(t => t.Name == relatedEntityName);

                                propertyType = typeof(List<>).MakeGenericType(entityTypeArg);
                            }

                            break;

                        default:
                            DebugUtils.Break();
                            propertyType = null;
                            break;
                    }

                    propertyMethodAttributes = EnumUtils.SetFlag<MethodAttributes>(propertyMethodAttributes, MethodAttributes.Virtual);
                }
                else
                {
                    if (regex.IsMatch(propertyTypeName))
                    {
                        var match = regex.Match(propertyTypeName);
                        var dbSetEntityName = match.GetGroupValue("entity").RemoveSpaces();
                        var entityTypeArg = moduleBuilder.GetTypes().Single(t => t.Name == dbSetEntityName);

                        propertyType = typeof(DbSet<>).MakeGenericType(entityTypeArg);
                    }
                    else if (propertyTypeName == "Guid")
                    {
                        propertyType = typeof(Guid);
                    }
                    else
                    {
                        propertyType = Type.GetType(TypeExtensions.GetPrimitiveTypeFullName(propertyTypeName));
                    }
                }

                propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
                getMethod = typeBuilder.DefineMethod("get_" + propertyTypeName, propertyMethodAttributes, propertyType, null);
                setMethod = typeBuilder.DefineMethod("set_" + propertyTypeName, propertyMethodAttributes, null, new Type[] { propertyType });

                ilGenerator = getMethod.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldnull);
                ilGenerator.Emit(OpCodes.Ret);

                ilGenerator = setMethod.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethod);
                propertyBuilder.SetSetMethod(setMethod);

                attribute.DynamicPropertyName = propertyName;
                attribute.DynamicPropertyType = propertyType;
            }

            entityType = typeBuilder.CreateType();
            metadataCode = entityType.GenerateCode(metadataType, moduleBuilder);

            foreach (var attribute in entity.Attributes)
            {
                attribute.DynamicEntityType = entityType;
                attribute.DynamicEntityMetadataType = metadataType;
            }

            entity.DynamicEntityType = entityType;
            entity.DynamicEntityMetadataType = metadataType;
        }
    }
}
