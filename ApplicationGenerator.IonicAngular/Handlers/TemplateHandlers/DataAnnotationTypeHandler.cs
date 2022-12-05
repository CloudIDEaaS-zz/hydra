// file:	InternalHandlers\DataAnnotationTypeHandler.cs
//
// summary:	Implements the data annotation type handler class

using AbstraX.DataAnnotations;
using AbstraX.TemplateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using Utils.XPath;

namespace AbstraX.Handlers.TemplateHandlers
{
    /// <summary>   A data annotation type handler. </summary>
    ///
    /// <remarks>   Ken, 10/5/2020. </remarks>

    public class DataAnnotationTypeHandler : IDataAnnotationTypeHandler
    {
        /// <summary>   Gets the priority. </summary>
        ///
        /// <value> The priority. </value>

        public float Priority => 1.0f;

        /// <summary>   Determine if we can handle. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <param name="propertyName"> Name of the property. </param>
        /// <param name="type">         The type. </param>
        ///
        /// <returns>   True if we can handle, false if not. </returns>

        public bool CanHandle(string propertyName, Type type)
        {
            return type != null || propertyName.IsOneOf("UIGroup", "DefaultSortBy", "AppSettingsKind");
        }

        /// <summary>   Process this. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <param name="entityObject">             The entity object. </param>
        /// <param name="entityPropertyItem">       The entity property item. </param>
        /// <param name="annotationType"></param>
        /// <param name="typeBuilder">              The type builder. </param>
        /// <param name="appUIHierarchyNodeObject">   The application hierarchy node object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Process(EntityObject entityObject, EntityPropertyItem entityPropertyItem, Type annotationType, TypeBuilder typeBuilder, AppUIHierarchyNodeObject appUIHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration)
        {
            if (entityPropertyItem.PropertyName == "UIGroup")
            {
                foreach (var childProperty in entityPropertyItem.ChildProperties)
                {
                    var annotationAtrributeType = generatorConfiguration.FindDataAnnotationType(childProperty.PropertyName + "Attribute");
                    var handler = generatorConfiguration.GetDataAnnotationTypeHandler(childProperty.PropertyName, annotationAtrributeType);

                    if (handler != null)
                    {
                        if (!handler.Process(entityObject, childProperty, annotationAtrributeType, typeBuilder, appUIHierarchyNodeObject, generatorConfiguration))
                        {
                            throw new Exception($"Cannot process annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ childProperty.PropertyName }");
                        }
                    }
                    else
                    {
                        throw new HandlerNotFoundException($"Cannot find annotation type handler for annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ childProperty.PropertyName }");
                    }
                }

                return true;
            }
            else if (entityPropertyItem.PropertyName == "DefaultSortBy")
            {
                return true;
            }
            else if (annotationType != null)
            {
                return Process(entityObject, null, entityPropertyItem, appUIHierarchyNodeObject, annotationType, generatorConfiguration, (c) => typeBuilder.SetCustomAttribute(c));
            }
            else
            {
                throw new Exception($"Cannot find Attribute type or custom handler to process property '{ entityPropertyItem.PropertyName }");
            }
        }

        /// <summary>   Process this.  </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <param name="entityObject">             The entity object. </param>
        /// <param name="attributeObject">          The attribute object. </param>
        /// <param name="entityPropertyItem">       The entity property item. </param>
        /// <param name="annotationType"></param>
        /// <param name="propertyBuilder">          The property builder. </param>
        /// <param name="appUIHierarchyNodeObject">   The application hierarchy node object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Process(EntityObject entityObject, AttributeObject attributeObject, EntityPropertyItem entityPropertyItem, Type annotationType, PropertyBuilder propertyBuilder, AppUIHierarchyNodeObject appUIHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration)
        {
            if (annotationType != null)
            {
                return Process(entityObject, attributeObject, entityPropertyItem, appUIHierarchyNodeObject, annotationType, generatorConfiguration, (c) => propertyBuilder.SetCustomAttribute(c));
            }
            else
            {
                throw new Exception($"Cannot find Attribute type or custom handler to process property '{ entityPropertyItem.PropertyName }");
            }
        }

        /// <summary>   Pre process. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <exception cref="Exception">                Thrown when an exception error condition occurs. </exception>
        /// <exception cref="HandlerNotFoundException"> Thrown when a Handler Not Found error condition
        ///                                             occurs. </exception>
        ///
        /// <param name="entityObject">             The entity object. </param>
        /// <param name="entityPropertyItem">       The entity property item. </param>
        /// <param name="annotationType">           Type of the annotation. </param>
        /// <param name="appUIHierarchyNodeObject">   The application hierarchy node object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool PreProcess(EntityObject entityObject, EntityPropertyItem entityPropertyItem, Type annotationType, AppUIHierarchyNodeObject appUIHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration)
        {
            if (entityPropertyItem.PropertyName == "UIGroup")
            {
                foreach (var childProperty in entityPropertyItem.ChildProperties)
                {
                    var annotationAtrributeType = generatorConfiguration.FindDataAnnotationType(childProperty.PropertyName + "Attribute");
                    var handler = generatorConfiguration.GetDataAnnotationTypeHandler(childProperty.PropertyName, annotationAtrributeType);

                    if (handler != null)
                    {
                        if (!handler.PreProcess(entityObject, childProperty, annotationAtrributeType, appUIHierarchyNodeObject, generatorConfiguration))
                        {
                            throw new Exception($"Cannot process annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ childProperty.PropertyName }");
                        }
                    }
                    else
                    {
                        throw new HandlerNotFoundException($"Cannot find annotation type handler for annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ childProperty.PropertyName }");
                    }
                }

                return true;
            }
            else if (entityPropertyItem.PropertyName == "DefaultSortBy")
            {
                var regex = new Regex(string.Format(@"EntitySet\[@Entity='{0}'\]", entityObject.Name));

                foreach (var attribute in appUIHierarchyNodeObject.EntityContainer.Attributes)
                {
                    if (regex.IsMatch(attribute.AttributeType))
                    {
                        if (!attribute.Properties.Any(p => p.PropertyName == "SortBy"))
                        {
                            attribute.Properties.Add(new EntityPropertyItem
                            {
                                PropertyName = "SortBy",
                                PropertyValue = entityPropertyItem.PropertyValue.RemoveSpaces()
                            });
                        }

                        entityObject.Properties.Remove(entityPropertyItem);  // not translated to an Attribute type

                        break;
                    }
                }

                return true;
            }
            else if (annotationType != null)
            {
                return Process(entityObject, null, entityPropertyItem, appUIHierarchyNodeObject, annotationType, generatorConfiguration);
            }
            else
            {
                throw new Exception($"Cannot find Attribute type or custom handler to process property '{ entityPropertyItem.PropertyName }");
            }
        }

        /// <summary>   Pre process. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="entityObject">             The entity object. </param>
        /// <param name="attributeObject">          The attribute object. </param>
        /// <param name="entityPropertyItem">       The entity property item. </param>
        /// <param name="annotationType">           Type of the annotation. </param>
        /// <param name="appUIHierarchyNodeObject">   The application hierarchy node object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool PreProcess(EntityObject entityObject, AttributeObject attributeObject, EntityPropertyItem entityPropertyItem, Type annotationType, AppUIHierarchyNodeObject appUIHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration)
        {
            if (annotationType != null)
            {
                return Process(entityObject, attributeObject, entityPropertyItem, appUIHierarchyNodeObject, annotationType, generatorConfiguration);
            }
            else
            {
                throw new Exception($"Cannot find Attribute type or custom handler to process property '{ entityPropertyItem.PropertyName }");
            }
        }

        private bool Process(EntityObject entityObject, AttributeObject attributeObject, EntityPropertyItem entityPropertyItem, AppUIHierarchyNodeObject appUIHierarchyNodeObject, Type annotationType, IGeneratorConfiguration generatorConfiguration, Action<CustomAttributeBuilder> builderAction = null)
        {
            var annotationTypeName = annotationType.Name;
            var customAttributeArgsLookup = new Dictionary<ConstructorInfo, CustomAttributeArgs>();

            var constructor = annotationType.GetConstructors().FirstOrDefault(c =>
            {
                var customAttributeArgs = new CustomAttributeArgs();
                var hasParmsFieldsOrProperties = false;
                var hasParm = false;
                var hasField = false;
                var hasProperty = false;
                var parameters = c.GetParameters();
                var x = 0;

                if (entityPropertyItem.ChildProperties == null)
                {
                    if (parameters.Length == 0)
                    {
                        if (entityPropertyItem.PropertyValue.IsNullOrEmpty())
                        {
                            customAttributeArgsLookup.Add(c, customAttributeArgs);

                            return true;
                        }
                    }
                    else if (!entityPropertyItem.PropertyValue.IsNullOrEmpty())
                    {
                        var parm = parameters.FirstOrDefault();

                        if (parm == parameters.ElementAt(x))
                        {
                            var value = this.GetValue(parm.ParameterType, entityPropertyItem, generatorConfiguration, (w) => HandleWildcard(w, entityObject, attributeObject, entityPropertyItem, appUIHierarchyNodeObject, builderAction == null));
                            customAttributeArgs.ArgValues.Add(value);
                            hasParmsFieldsOrProperties = true;
                            x++;
                        }
                    }
                }
                else
                {
                    foreach (var childProperty in entityPropertyItem.ChildProperties.ToList())
                    {
                        var propertyName = childProperty.PropertyName;
                        var propertyNameCamelCase = propertyName.ToCamelCase();
                        object value;

                        hasParmsFieldsOrProperties = false;

                        if (propertyName.StartsWith("UI"))
                        {
                            propertyNameCamelCase = "ui" + propertyName.RemoveStartIfMatches("UI");
                        }

                        hasParm = parameters.Any(p => p.Name.IsOneOf(propertyName, propertyNameCamelCase));

                        if (hasParm)
                        {
                            var parm = parameters.Single(p => p.Name.IsOneOf(propertyName, propertyNameCamelCase));

                            if (parm == parameters.ElementAt(x))
                            {
                                value = this.GetValue(parm.ParameterType, childProperty, generatorConfiguration, (w) => HandleWildcard(w, entityObject, attributeObject, childProperty, appUIHierarchyNodeObject, builderAction == null));
                                customAttributeArgs.ArgValues.Add(value);
                                hasParmsFieldsOrProperties = true;
                                x++;
                            }

                            continue;
                        }

                        hasField = annotationType.GetFields().Any(f => f.IsPublic && f.Name.IsOneOf(propertyName, propertyNameCamelCase));

                        if (!hasParm && hasField)
                        {
                            var field = annotationType.GetFields().Single(f => f.Name.IsOneOf(propertyName, propertyNameCamelCase));

                            value = this.GetValue(field.FieldType, childProperty, generatorConfiguration, (w) => HandleWildcard(w, entityObject, attributeObject, childProperty, appUIHierarchyNodeObject, builderAction == null));
                            customAttributeArgs.FieldValues.Add(value);
                            customAttributeArgs.NamedFields.Add(field);
                            hasParmsFieldsOrProperties = true;

                            continue;
                        }

                        hasProperty = annotationType.GetProperties().Any(p => p.CanWrite && p.Name.IsOneOf(propertyName, propertyNameCamelCase));

                        if (!hasParm && !hasField && hasProperty)
                        {
                            var property = annotationType.GetProperties().Single(p => p.Name.IsOneOf(propertyName, propertyName));

                            value = this.GetValue(property.PropertyType, childProperty, generatorConfiguration, (w) => HandleWildcard(w, entityObject, attributeObject, childProperty, appUIHierarchyNodeObject, builderAction == null));
                            customAttributeArgs.PropertyValues.Add(value);
                            customAttributeArgs.NamedProperties.Add(property);
                            hasParmsFieldsOrProperties = true;

                            continue;
                        }

                        if (!hasParmsFieldsOrProperties)
                        {
                            break;
                        }
                    }
                }

                if (x != parameters.Length)
                {
                    var remainingParms = parameters.Skip(x).ToArray();

                    if (remainingParms.Length == 0)
                    {
                        return false;
                    }
                    else
                    {
                        foreach (var remainingParm in remainingParms)
                        {
                            if (remainingParm.HasDefaultValue)
                            {
                                customAttributeArgs.ArgValues.Add(remainingParm.DefaultValue);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }

                customAttributeArgsLookup.Add(c, customAttributeArgs);

                return hasParmsFieldsOrProperties;
            });

            if (constructor != null)
            {
                var customAttributeArgs = customAttributeArgsLookup[constructor];
                var argValues = customAttributeArgs.ArgValues;
                var namedFields = customAttributeArgs.NamedFields;
                var fieldValues = customAttributeArgs.FieldValues;
                var namedProperties = customAttributeArgs.NamedProperties;
                var propertyValues = customAttributeArgs.PropertyValues;

                if (builderAction != null)
                {
                    builderAction(new CustomAttributeBuilder(constructor, argValues.ToArray(), namedProperties.ToArray(), propertyValues.ToArray(), namedFields.ToArray(), fieldValues.ToArray()));
                }
                else
                {
                    HandleAnnotationType(entityObject, attributeObject, entityPropertyItem, appUIHierarchyNodeObject, annotationType, constructor, argValues.ToArray(), namedProperties.ToArray(), propertyValues.ToArray(), namedFields.ToArray(), fieldValues.ToArray());
                }

                return true;
            }
            else
            {
                throw new Exception($"Cannot find appropriate constructor for annotation type '{ annotationType.AsDisplayText() }' matching property '{ entityPropertyItem.PropertyName }");
            }
        }

        private void HandleAnnotationType(EntityObject entityObject, AttributeObject attributeObject, EntityPropertyItem entityPropertyItem, AppUIHierarchyNodeObject appUIHierarchyNodeObject, Type annotationType, ConstructorInfo constructor, object[] argValues, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields, object[] fieldValues)
        {
            // todo - handle more robustly, i.e. named properties or fields

            switch (annotationType.Name)
            {
                case "IdentityEntityAttribute":
                    
                    var identityEntityKind = (IdentityEntityKind)argValues[0];

                    if (identityEntityKind == IdentityEntityKind.User)
                    {
                        HandleIdentityEntityUser(entityObject, entityPropertyItem, appUIHierarchyNodeObject);
                    }

                    break;

                case "AppSettingsKindAttribute":

                    var appSettingsKind = (AppSettingsKind)argValues[0];

                    if (appSettingsKind == AppSettingsKind.GlobalSettings)
                    {
                        HandleGlobalSettings(entityObject, entityPropertyItem, appUIHierarchyNodeObject);
                    }

                    break;

            }
        }

        private void HandleGlobalSettings(EntityObject entityObject, EntityPropertyItem entityPropertyItem, AppUIHierarchyNodeObject appUIHierarchyNodeObject)
        {
            var appSystemObject = appUIHierarchyNodeObject.AppSystemObject;
            var businessModel = appUIHierarchyNodeObject.BusinessModel;
            var ancestors = appSystemObject.GetAncestors(businessModel);
            var internalApp = ancestors.Any(a => a.StakeholderKind == "InternalRoot");

            foreach (var hierarchyNode in appUIHierarchyNodeObject.GetDescendants().Where(n => n.ContainsEntity(entityObject, appUIHierarchyNodeObject)))
            {
                var uiPath = hierarchyNode.GetUIPath();
                Regex regex;

                switch (hierarchyNode.AppSettingsKind)
                {
                    case AppSettingsKind.GlobalSettings:

                        regex = new Regex(string.Format(@"EntitySet\[@Entity='{0}'\]", entityObject.Name));

                        foreach (var attribute in appUIHierarchyNodeObject.EntityContainer.Attributes)
                        {
                            if (regex.IsMatch(attribute.AttributeType))
                            {
                                string propertyUiPath;

                                propertyUiPath = uiPath.RemoveEndAtLastChar('/').MergeAttributes(string.Format("@{0}", attribute.Name));

                                attribute.Properties.AddRange(new List<EntityPropertyItem>()
                                {
                                    new EntityMarkerUIGroup(propertyUiPath, UIKind.NavigationGridPageWithEdits),
                                    new EntityPropertyItem
                                    {
                                        PropertyName = "UI",
                                        ChildProperties = new List<EntityPropertyItem>
                                        {
                                            new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = propertyUiPath},
                                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "NavigationGridPageWithEdits"}
                                        }
                                    }
                                });

                                uiPath = uiPath.MergeAttributes(string.Format("@Name='{0}'", "App Settings"));
                                hierarchyNode.ConstructedUIPath = uiPath;

                                entityObject.Properties.AddRange(new List<EntityPropertyItem>()
                                {
                                    new EntityMarkerUIGroup(propertyUiPath, UIKind.EditPopup),
                                    new EntityPropertyItem
                                    {
                                        PropertyName = "UI",
                                        ChildProperties = new List<EntityPropertyItem>
                                        {
                                            new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath},
                                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "EditPopup"}
                                        }
                                    }
                                });

                                break;
                            }
                        }

                        foreach (var attributeObject in entityObject.Attributes)
                        {
                            var isTextIdentifier = false; // isTextIdentifier

                            // todo - kn, figure out how to inject script to disable SettingName for anything in a list of UI settings

                            if (entityObject.Name == appUIHierarchyNodeObject.Name + "Settings" && attributeObject.Name == "Setting")
                            {
                                isTextIdentifier = true;
                            }
                            else if (entityObject.Name == "User" && attributeObject.Name == "Email Address")
                            {
                                isTextIdentifier = true;
                            }

                            if (isTextIdentifier)
                            {
                                attributeObject.Properties.AddRange(new List<EntityPropertyItem>()
                                {
                                    new EntityMarkerUIGroup(uiPath.Append("/" + attributeObject.GetUIPathSegment())),
                                    new EntityPropertyItem
                                    {
                                        PropertyName = "GridColumn",
                                        ChildProperties = new List<EntityPropertyItem>
                                        {
                                            new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath.Append("/" + attributeObject.GetUIPathSegment()) },
                                            new EntityPropertyItem { PropertyName = "IsTextIdentifier", PropertyValue = "true" }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                attributeObject.Properties.AddRange(new List<EntityPropertyItem>()
                                {
                                    new EntityMarkerUIGroup(uiPath.Append("/" + attributeObject.GetUIPathSegment())),
                                    new EntityPropertyItem
                                    {
                                        PropertyName = "GridColumn",
                                        ChildProperties = new List<EntityPropertyItem>
                                        {
                                            new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath.Append("/" + attributeObject.GetUIPathSegment()) }
                                        }
                                    }
                                });
                            }

                            attributeObject.Properties.AddRange(new List<EntityPropertyItem>()
                            {
                                new EntityMarkerUIGroup(uiPath.Append("/" + attributeObject.GetUIPathSegment())),
                                new EntityPropertyItem
                                {
                                    PropertyName = "FormField",
                                    ChildProperties = new List<EntityPropertyItem>
                                    {
                                        new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath.Append("/" + attributeObject.GetUIPathSegment()) }
                                    }
                                }
                            });
                        }

                        break;

                    case AppSettingsKind.UserPreferences:

                        // todo - kn, figure out how to filter the grid based on the AllowUserCustomize field

                        regex = new Regex(string.Format(@"EntitySet\[@Entity='{0}'\]", entityObject.Name));

                        foreach (var attribute in appUIHierarchyNodeObject.EntityContainer.Attributes)
                        {
                            if (regex.IsMatch(attribute.AttributeType))
                            {
                                var propertyUiPath = uiPath.RemoveEndAtLastChar('/').MergeAttributes(string.Format("@{0}", attribute.Name));

                                attribute.Properties.AddRange(new List<EntityPropertyItem>()
                                {
                                    new EntityMarkerUIGroup(propertyUiPath, UIKind.NavigationGridPageWithEdits),
                                    new EntityPropertyItem
                                    {
                                        PropertyName = "UI",
                                        ChildProperties = new List<EntityPropertyItem>
                                        {
                                            new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = propertyUiPath},
                                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "NavigationGridPageWithEdits"}
                                        }
                                    }
                                });

                                uiPath = uiPath.MergeAttributes(string.Format("@Name='{0}'", "App Settings"));
                                hierarchyNode.ConstructedUIPath = uiPath;

                                entityObject.Properties.AddRange(new List<EntityPropertyItem>()
                                {
                                    new EntityMarkerUIGroup(propertyUiPath, UIKind.EditPage),
                                    new EntityPropertyItem
                                    {
                                        PropertyName = "UI",
                                        ChildProperties = new List<EntityPropertyItem>
                                        {
                                            new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath},
                                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "EditPage"}
                                        }
                                    },
                                    new EntityPropertyItem
                                    {
                                        PropertyName = "UINavigationName",
                                        ChildProperties = new List<EntityPropertyItem>
                                        {
                                            new EntityPropertyItem { PropertyName = "Name", PropertyValue = hierarchyNode.Name},
                                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "EditPage"}
                                        }
                                    }
                                });

                                break;
                            }
                        }

                        foreach (var attributeObject in entityObject.Attributes)
                        {
                            var globalSettingsFieldKind = EnumUtils.GetValue<GlobalSettingsFieldKind>(attributeObject.Properties.Where(p => p.PropertyName == "GlobalSettingsFieldKind").SelectMany(p => p.ChildProperties.Where(p2 => p2.PropertyName == "GlobalSettingsFieldKind").Select(p2 => p2.PropertyValue)).Single());

                            attributeObject.Properties.AddRange(new List<EntityPropertyItem>()
                            {
                                new EntityMarkerUIGroup(uiPath.Append("/" + attributeObject.Name)),
                                new EntityPropertyItem
                                {
                                    PropertyName = "GridColumn",
                                    ChildProperties = new List<EntityPropertyItem>
                                    {
                                        new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath.Append("/" + attributeObject.Name) }
                                    }
                                }
                            });

                            switch (globalSettingsFieldKind)
                            {
                                case GlobalSettingsFieldKind.SettingName:

                                    attributeObject.Properties.AddRange(new List<EntityPropertyItem>()
                                    {
                                        new EntityMarkerUIGroup(uiPath.Append("/" + attributeObject.Name)),
                                        new EntityPropertyItem
                                        {
                                            PropertyName = "FormField",
                                            ChildProperties = new List<EntityPropertyItem>
                                            {
                                                new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath.Append("/" + attributeObject.Name) },
                                                new EntityPropertyItem { PropertyName = "FormFieldKind", PropertyValue = "Label" }
                                            }
                                        }
                                    });

                                    break;

                                case GlobalSettingsFieldKind.SettingValue:

                                    attributeObject.Properties.AddRange(new List<EntityPropertyItem>()
                                    {
                                        new EntityMarkerUIGroup(uiPath.Append("/" + attributeObject.Name)),
                                        new EntityPropertyItem
                                        {
                                            PropertyName = "FormField",
                                            ChildProperties = new List<EntityPropertyItem>
                                            {
                                                new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath.Append("/" + attributeObject.Name) }
                                            }
                                        }
                                    });

                                    break;
                            }
                        }

                        break;
                }
            }
        }

        private void HandleIdentityEntityUser(EntityObject entityObject, EntityPropertyItem entityPropertyItem, AppUIHierarchyNodeObject appUIHierarchyNodeObject)
        {
            var appSystemObject = appUIHierarchyNodeObject.AppSystemObject;
            var businessModel = appUIHierarchyNodeObject.BusinessModel;
            var ancestors = appSystemObject.GetAncestors(businessModel);
            var internalApp = ancestors.Any(a => a.StakeholderKind == "InternalRoot");

            foreach (var hierarchyNode in appUIHierarchyNodeObject.GetDescendants().Where(n => n.ContainsEntity(entityObject, appUIHierarchyNodeObject)))
            {
                var uiPath = hierarchyNode.GetUIPath();
                var parent = hierarchyNode.Parent;

                switch (hierarchyNode.AppSettingsKind)
                {
                    case AppSettingsKind.ProfilePreferences:
                        {
                            var containerAttribute = appUIHierarchyNodeObject.FindEntityContainerAttribute(hierarchyNode, entityObject);
                            var propertyUiPath = uiPath.RemoveEndAtLastChar('/').MergeAttributes(string.Format("@{0}", containerAttribute.Name));

                            uiPath = string.Format("{0}/{1}", propertyUiPath, entityObject.Name);
                            hierarchyNode.ConstructedUIPath = uiPath;

                            containerAttribute.Properties.AddRange(new List<EntityPropertyItem>()
                            {
                                new EntityMarkerUIGroup(propertyUiPath, UIKind.NavigationGridPageWithEdits),
                                new EntityMarkerIdentityEntityUserKind(propertyUiPath, TaskCapabilities.List),
                                new EntityPropertyItem
                                {
                                    PropertyName = "UI",
                                    ChildProperties = new List<EntityPropertyItem>
                                    {
                                        new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = propertyUiPath},
                                        new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "None"}
                                    }
                                }
                            });

                            entityObject.CustomQueries.Add("GetCurrentUser", new CustomQuery("GetCurrentUser", "container.Users.Single(u => u.UserName == identity.Name)"));

                            entityObject.Properties.AddRange(new List<EntityPropertyItem>()
                            {
                                new EntityMarkerUIGroup(uiPath, UIKind.EditPage),
                                new EntityMarkerAppSettingsKind(uiPath, hierarchyNode.AppSettingsKind),
                                new EntityPropertyItem
                                {
                                    PropertyName = "UI",
                                    ChildProperties = new List<EntityPropertyItem>
                                    {
                                        new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath},
                                        new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "EditPage"}
                                    }
                                },
                                new EntityPropertyItem
                                {
                                    PropertyName = "UINavigationName",
                                    ChildProperties = new List<EntityPropertyItem>
                                    {
                                        new EntityPropertyItem { PropertyName = "Name", PropertyValue = hierarchyNode.Name},
                                        new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "EditPage"}
                                    }
                                },
                                new EntityPropertyItem
                                {
                                    PropertyName = "CustomQuery",
                                    ChildProperties = new List<EntityPropertyItem>
                                    {
                                        new EntityPropertyItem { PropertyName = "ResourcesType", PropertyValue = "AppResources"},
                                        new EntityPropertyItem { PropertyName = "ControllerMethodName", PropertyValue = "GetCurrentUser"},
                                        new EntityPropertyItem { PropertyName = "QueryKind", PropertyValue = "LoadParentReference"}
                                    }
                                }
                            });
                        }

                        break;

                    default:

                        switch (hierarchyNode.UIKind)
                        {
                            case UIKind.NotSpecified:

                                if (hierarchyNode.Parent.Level.IsOneOf(BusinessModelLevel.Task, BusinessModelLevel.Responsibility))
                                {
                                    var capabilities = hierarchyNode.Parent.TaskCapabilities;

                                    if (capabilities.HasFlag(TaskCapabilities.List))
                                    {
                                        var containerAttribute = appUIHierarchyNodeObject.FindEntityContainerAttribute(hierarchyNode, entityObject);
                                        var propertyUiPath = uiPath.RemoveEndAtLastChar('/').MergeAttributes(string.Format("@{0}", containerAttribute.Name));

                                        uiPath = propertyUiPath;

                                        containerAttribute.Properties.AddRange(new List<EntityPropertyItem>()
                                        {
                                            new EntityMarkerUIGroup(propertyUiPath, UIKind.NavigationGridPageWithEdits),
                                            new EntityMarkerIdentityEntityUserKind(propertyUiPath, TaskCapabilities.List),
                                            new EntityPropertyItem
                                            {
                                                PropertyName = "UI",
                                                ChildProperties = new List<EntityPropertyItem>
                                                {
                                                    new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = propertyUiPath},
                                                    new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "NavigationGridPageWithEdits"}
                                                }
                                            }
                                        });

                                        if (entityObject.Properties.Any(p => p.PropertyName == "DefaultSortBy"))
                                        {
                                            var defaultSortByProperty = entityObject.Properties.Single(p => p.PropertyName == "DefaultSortBy");

                                            containerAttribute.Properties.Add(new EntityPropertyItem
                                            {
                                                PropertyName = "SortBy",
                                                PropertyValue = defaultSortByProperty.PropertyValue.RemoveSpaces()
                                            });
                                        }
                                    }

                                    if (capabilities.HasFlag(TaskCapabilities.Update))
                                    {
                                        uiPath = string.Format("{0}/{1}", uiPath, entityObject.Name);
                                        hierarchyNode.ConstructedUIPath = uiPath;

                                        entityObject.Properties.AddRange(new List<EntityPropertyItem>()
                                        {
                                            new EntityMarkerUIGroup(uiPath, UIKind.EditPopup),
                                            new EntityMarkerIdentityEntityUserKind(uiPath, TaskCapabilities.Update),
                                            new EntityPropertyItem
                                            {
                                                PropertyName = "UI",
                                                ChildProperties = new List<EntityPropertyItem>
                                                {
                                                    new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath},
                                                    new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "EditPopup"}
                                                }
                                            },
                                            new EntityPropertyItem
                                            {
                                                PropertyName = "UINavigationName",
                                                ChildProperties = new List<EntityPropertyItem>
                                                {
                                                    new EntityPropertyItem { PropertyName = "Name", PropertyValue = hierarchyNode.Name},
                                                    new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "EditPopup"}
                                                }
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }

                                break;

                            default:
                                break;
                        }

                        break;
                }
            }

            if (internalApp)
            {
                entityObject.Properties.AddRange(new List<EntityPropertyItem>()
                {
                    new EntityMarkerUIGroup("/Login", UIKind.LoginPage),
                    new EntityPropertyItem
                    {
                        PropertyName = "UI",
                        ChildProperties = new List<EntityPropertyItem>
                        {
                            new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = "/Login"},
                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "LoginPage"}
                        }
                    },
                    new EntityPropertyItem
                    {
                        PropertyName = "UINavigationName",
                        ChildProperties = new List<EntityPropertyItem>
                        {
                            new EntityPropertyItem { PropertyName = "Name", PropertyValue = "Login"},
                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "LoginPage"}
                        }
                    },
                    new EntityPropertyItem
                    {
                        PropertyName = "Authorize",
                        ChildProperties = new List<EntityPropertyItem>
                        {
                            new EntityPropertyItem { PropertyName = "AuthorizationState", PropertyValue = "SkipAuthorization"},
                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "LoginPage"}
                        }
                    }
                });
            }
            else
            {
                entityObject.Properties.AddRange(new List<EntityPropertyItem>()
                {
                    new EntityMarkerUIGroup("/Register", UIKind.RegisterPage),
                    new EntityPropertyItem
                    {
                        PropertyName = "UI",
                        ChildProperties = new List<EntityPropertyItem>
                        {
                            new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = "/Register"},
                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "RegisterPage"}
                        }
                    },
                    new EntityPropertyItem
                    {
                        PropertyName = "UINavigationName",
                        PropertyValue = "Register"
                    },
                    new EntityPropertyItem
                    {
                        PropertyName = "Authorize",
                        ChildProperties = new List<EntityPropertyItem>
                        {
                            new EntityPropertyItem { PropertyName = "AuthorizationState", PropertyValue = "SkipAuthorization"},
                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "RegisterPage"},
                        }
                    },
                    new EntityMarkerUIGroup("/Login", UIKind.LoginPage),
                    new EntityPropertyItem
                    {
                        PropertyName = "UI",
                        ChildProperties = new List<EntityPropertyItem>
                        {
                            new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = "/Login"},
                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "LoginPage"}
                        }
                    },
                    new EntityPropertyItem
                    {
                        PropertyName = "Authorize",
                        ChildProperties = new List<EntityPropertyItem>
                        {
                            new EntityPropertyItem { PropertyName = "AuthorizationState", PropertyValue = "SkipAuthorization"},
                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "LoginPage"},
                        }
                    }
                });
            }
        }

        private object HandleWildcard(string wildcard, EntityObject entityObject, AttributeObject attributeObject, EntityPropertyItem entityPropertyItem, AppUIHierarchyNodeObject appUIHierarchyNodeObject, bool isPreProcess)
        {
            IEnumerable<EntityPropertyItem> hierarchy;
            EntityPropertyItem parent;
            var uiHierarchyNodes = appUIHierarchyNodeObject.GetDescendants().Where(n => n.ContainsEntity(entityObject));
            var uiHierarchyPathProperties = entityObject.GetDescendantProperties().Where(p => p.PropertyName == "UIHierarchyPath").ToList();
            EntityBaseObject entityBaseObject;

            if (attributeObject != null)
            {
                hierarchy = entityPropertyItem.GetAncestorProperties(attributeObject);
                parent = hierarchy.First();
                entityBaseObject = attributeObject;
            }
            else
            {
                hierarchy = entityPropertyItem.GetAncestorProperties(entityObject);
                parent = hierarchy.First();
                entityBaseObject = entityObject;
            }

            if (entityPropertyItem.PropertyName == "UIHierarchyPath")
            {
                switch (parent.PropertyName)
                {
                    case "IdentityField":
                        {
                            var identityEntityKind = EnumUtils.GetValue<IdentityEntityKind>(entityObject.Properties.Where(p => p.PropertyName == "IdentityEntity").SelectMany(p => p.ChildProperties.Where(p2 => p2.PropertyName == "IdentityEntityKind").Select(p2 => p2.PropertyValue)).Single());
                            var identityFieldKind = EnumUtils.GetValue<IdentityFieldKind>(parent.ChildProperties.Where(p => p.PropertyName == "IdentityFieldKind").Select(p => p.PropertyValue).Single());

                            foreach (var hierarchyNode in uiHierarchyNodes)
                            {
                                var uiPath = hierarchyNode.GetUIPath();
                                var appSettingsKind = hierarchyNode.AppSettingsKind;
                                var matchingProperties = uiHierarchyPathProperties.Where(p => p.PropertyValue.StartsWith(uiPath) || p.IsMatchingProperty(identityFieldKind, hierarchyNode));
                                var parentHierarchyNode = hierarchyNode.Parent;
                                var taskCapabilities = parentHierarchyNode.TaskCapabilities;

                                foreach (var matchingProperty in matchingProperties)
                                {
                                    var parentProperty = matchingProperty.GetAncestorProperties(entityObject).First();
                                    var uiPathSegment = attributeObject.GetUIPathSegment();
                                    UIKind uiKind;

                                    if (parentProperty.PropertyName == "UI")
                                    {
                                        uiKind = EnumUtils.GetValue<UIKind>(parentProperty.ChildProperties.Where(p => p.PropertyName == "UIKind").Select(p => p.PropertyValue).Single());

                                        switch (identityFieldKind)
                                        {
                                            case IdentityFieldKind.PasswordHash:

                                                if (!uiKind.IsOneOf(UIKind.LoginPage, UIKind.RegisterPage) && hierarchyNode.AppSettingsKind != AppSettingsKind.ProfilePreferences)
                                                {
                                                    continue;
                                                }

                                                break;

                                            case IdentityFieldKind.UserName:

                                                if (!uiKind.IsOneOf(UIKind.LoginPage, UIKind.RegisterPage))
                                                {
                                                    continue;
                                                }

                                                break;
                                        }
                                    }

                                    if (matchingProperty.PropertyValue != uiPath)
                                    {
                                        uiPath = matchingProperty.PropertyValue;
                                        taskCapabilities = TaskCapabilities.Enter;
                                    }

                                    switch (hierarchyNode.AppSettingsKind)
                                    {
                                        case AppSettingsKind.None:

                                            var isTextIdentifier = false;

                                            // todo - kn, figure out how to inject script to disable SettingName for anything in a list of UI settings

                                            if (entityObject.Name == appUIHierarchyNodeObject.Name + "Settings" && attributeObject.Name == "Setting")
                                            {
                                                isTextIdentifier = true;
                                            }
                                            else if (entityObject.Name == "User" && attributeObject.Name == "Email Address")
                                            {
                                                isTextIdentifier = true;
                                            }

                                            attributeObject.Properties.Add(new EntityMarkerUIGroup(uiPath.Append("/" + uiPathSegment)));

                                            if (taskCapabilities.HasFlag(TaskCapabilities.List))
                                            {
                                                if (isTextIdentifier)
                                                {
                                                    attributeObject.Properties.AddRange(new List<EntityPropertyItem>()
                                                    {
                                                        new EntityPropertyItem
                                                        {
                                                            PropertyName = "GridColumn",
                                                            ChildProperties = new List<EntityPropertyItem>
                                                            {
                                                                new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath.Append("/" + uiPathSegment) },
                                                                new EntityPropertyItem { PropertyName = "IsTextIdentifier", PropertyValue = "true" }
                                                            }
                                                        }
                                                    });
                                                }
                                                else
                                                {
                                                    attributeObject.Properties.AddRange(new List<EntityPropertyItem>()
                                                    {
                                                        new EntityPropertyItem
                                                        {
                                                            PropertyName = "GridColumn",
                                                            ChildProperties = new List<EntityPropertyItem>
                                                            {
                                                                new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath.Append("/" + uiPathSegment) }
                                                            }
                                                        }
                                                    });
                                                }
                                            }

                                            if (taskCapabilities.HasAnyFlag(TaskCapabilities.Update | TaskCapabilities.Enter))
                                            {
                                                attributeObject.Properties.AddRange(new List<EntityPropertyItem>()
                                                {
                                                    new EntityPropertyItem
                                                    {
                                                        PropertyName = "FormField",
                                                        ChildProperties = new List<EntityPropertyItem>
                                                        {
                                                            new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath.Append("/" + uiPathSegment) }
                                                        }
                                                    }
                                                });
                                            }

                                            break;

                                        case AppSettingsKind.ProfilePreferences:

                                            attributeObject.Properties.AddRange(new List<EntityPropertyItem>()
                                            {
                                                new EntityMarkerUIGroup(uiPath.Append("/" + uiPathSegment)),
                                                new EntityPropertyItem
                                                {
                                                    PropertyName = "FormField",
                                                    ChildProperties = new List<EntityPropertyItem>
                                                    {
                                                        new EntityPropertyItem { PropertyName = "UIHierarchyPath", PropertyValue = uiPath.Append("/" + uiPathSegment) }
                                                    }
                                                }
                                            });

                                            break;
                                    }
                                }
                            }
                        }

                        entityPropertyItem.PropertyValue = "/";

                        break;
                }
            }

            return null;
        }

        private object GetValue(Type type, EntityPropertyItem property, IGeneratorConfiguration generatorConfiguration, Func<string, object> wildcardCallback)
        {
            var propertyName = property.PropertyName;
            var propertyValue = property.PropertyValue;
            object value = null;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:

                    value = propertyValue;

                    if (((string) value) == "*")
                    {
                        value = wildcardCallback((string) value);
                    }

                    break;

                case TypeCode.Boolean:

                    value = bool.Parse(propertyValue);
                    break;

                case TypeCode.Int32:

                    if (type.IsEnum)
                    {
                        if (propertyName == type.Name)
                        {
                            value = EnumUtils.GetValue(type, propertyValue);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }
                    else
                    {
                        value = int.Parse(propertyValue);
                    }

                    break;

                default:

                    if (type.IsAssignableFrom(typeof(Enum)))
                    {
                        if (propertyName == type.Name)
                        {
                            value = EnumUtils.GetValue(type, propertyValue);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }
                    else if (type == typeof(Type))
                    {
                        return generatorConfiguration.KeyValuePairs[propertyValue];
                    }
                    else
                    {
                        DebugUtils.Break();
                    }

                    break;
            }

            return value;
        }
    }
}
