// file:	InternalHandlers\DataAnnotationTypeHandler.cs
//
// summary:	Implements the data annotation type handler class

using AbstraX.TemplateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AbstraX.InternalHandlers
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
            return type != null || propertyName == "UIGroup";
        }

        /// <summary>   Process this. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <param name="entityObject">             The entity object. </param>
        /// <param name="entityPropertyItem">       The entity property item. </param>
        /// <param name="annotationType"></param>
        /// <param name="typeBuilder">              The type builder. </param>
        /// <param name="appHierarchyNodeObject">   The application hierarchy node object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Process(EntityObject entityObject, EntityPropertyItem entityPropertyItem, Type annotationType, TypeBuilder typeBuilder, UIHierarchyNodeObject appHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration)
        {
            if (entityPropertyItem.PropertyName == "UIGroup")
            {
                foreach (var childProperty in entityPropertyItem.ChildProperties)
                {
                    var annotationAtrributeType = generatorConfiguration.FindDataAnnotationType(childProperty.PropertyName + "Attribute");
                    var handler = generatorConfiguration.GetDataAnnotationTypeHandler(childProperty.PropertyName, annotationAtrributeType);

                    if (handler != null)
                    {
                        if (!handler.Process(entityObject, childProperty, annotationAtrributeType, typeBuilder, appHierarchyNodeObject, generatorConfiguration))
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
            else if (annotationType != null)
            {
                return Process(entityPropertyItem, annotationType, (c) => typeBuilder.SetCustomAttribute(c));
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
        /// <param name="appHierarchyNodeObject">   The application hierarchy node object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Process(EntityObject entityObject, AttributeObject attributeObject, EntityPropertyItem entityPropertyItem, Type annotationType, PropertyBuilder propertyBuilder, UIHierarchyNodeObject appHierarchyNodeObject, IGeneratorConfiguration generatorConfiguration)
        {
            if (annotationType != null)
            {
                return Process(entityPropertyItem, annotationType, (c) => propertyBuilder.SetCustomAttribute(c));
            }
            else
            {
                throw new Exception($"Cannot find Attribute type or custom handler to process property '{ entityPropertyItem.PropertyName }");
            }
        }

        private bool Process(EntityPropertyItem entityPropertyItem, Type annotationType, Action<CustomAttributeBuilder> builderAction)
        {
            var argValues = new List<object>();
            var namedFields = new List<FieldInfo>();
            var fieldValues = new List<object>();
            var namedProperties = new List<PropertyInfo>();
            var propertyValues = new List<object>();

            var constructor = annotationType.GetConstructors().FirstOrDefault(c =>
            {
                var hasParmsFieldsOrProperties = false;
                var hasParm = false;
                var hasField = false;
                var hasProperty = false;
                var parameters = c.GetParameters();
                var x = 0;

                if (entityPropertyItem.ChildProperties == null)
                {
                    if (parameters.Length == 1 && !entityPropertyItem.PropertyValue.IsNullOrEmpty())
                    {
                        var parm = parameters.FirstOrDefault();

                        if (parm == parameters.ElementAt(x))
                        {
                            var value = this.GetValue(parm.ParameterType, entityPropertyItem);
                            argValues.Add(value);
                            hasParmsFieldsOrProperties = true;
                            x++;
                        }
                    }
                }
                else
                {
                    foreach (var childProperty in entityPropertyItem.ChildProperties)
                    {
                        var propertyName = childProperty.PropertyName;
                        var propertyNameCamelCase = propertyName.ToCamelCase();
                        object value;

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
                                value = this.GetValue(parm.ParameterType, childProperty);
                                argValues.Add(value);
                                hasParmsFieldsOrProperties = true;
                                x++;
                            }

                            continue;
                        }

                        hasField = annotationType.GetFields().Any(f => f.Name.IsOneOf(propertyName, propertyNameCamelCase));

                        if (hasField)
                        {
                            var field = annotationType.GetFields().Single(f => f.Name.IsOneOf(propertyName, propertyNameCamelCase));

                            value = this.GetValue(field.FieldType, childProperty);
                            argValues.Add(value);
                            namedFields.Add(field);
                            hasParmsFieldsOrProperties = true;

                            continue;
                        }

                        hasProperty = annotationType.GetProperties().Any(p => p.Name.IsOneOf(propertyName, propertyNameCamelCase));

                        if (hasProperty)
                        {
                            var property = annotationType.GetProperties().Single(p => p.Name.IsOneOf(propertyName, propertyNameCamelCase));

                            value = this.GetValue(property.PropertyType, childProperty);
                            argValues.Add(value);
                            namedProperties.Add(property);
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
                                argValues.Add(remainingParm.DefaultValue);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }

                return hasParmsFieldsOrProperties;
            });

            if (constructor != null)
            {
                builderAction(new CustomAttributeBuilder(constructor, argValues.ToArray(), namedProperties.ToArray(), propertyValues.ToArray(), namedFields.ToArray(), fieldValues.ToArray()));

                return true;
            }
            else
            {
                throw new Exception($"Cannot find appropriate constructor for annotation type '{ annotationType.AsDisplayText() }' matching property '{ entityPropertyItem.PropertyName }");
            }
        }

        private object GetValue(Type type, EntityPropertyItem property)
        {
            var propertyName = property.PropertyName;
            var propertyValue = property.PropertyValue;
            object value = null;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:

                    value = propertyValue;
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
