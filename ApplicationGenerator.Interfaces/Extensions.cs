// file:	Extensions.cs
//
// summary:	Implements the extensions class

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Utils;
using AbstraX.DataAnnotations;
using CodePlex.XPathParser;
using AbstraX.XPathBuilder;
using AbstraX.ServerInterfaces;
using System.Diagnostics;
using EntityProvider.Web.Entities;
using System.ComponentModel;
using System.Linq.Expressions;
using Assembly = System.Reflection.Assembly;
using System.ComponentModel.DataAnnotations;
using AbstraX.TypeMappings;
using System.IO;
using AbstraX.FolderStructure;
using Utils.Hierarchies;
using AbstraX.QueryPath;
using AbstraX.PackageCache;
using AbstraX.Models.Interfaces;
using System.Reflection;
using AbstraX.TemplateObjects;
using System.Reflection.Emit;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace AbstraX
{
    /// <summary>   An extensions. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    public static class Extensions
    {
        /// <summary>   A Type extension method that generates a code. </summary>
        ///
        /// <remarks>   Ken, 10/6/2020. </remarks>
        ///
        /// <param name="metadataType"> The type to act on. </param>
        /// <param name="entityType"></param>
        ///
        /// <returns>   The code. </returns>

        public static string GenerateCode(this Type metadataType, Type entityType)
        {
            CodeTypeDeclaration entityClass;
            CodeTypeDeclaration metaDataClass;
            ICodeGenerator codeGenerator;
            CodeCompileUnit unit;

            entityClass = entityType.GetCodeTypeDeclaration(metadataType.Name);
            metaDataClass = metadataType.GetCodeTypeDeclaration();

            unit = new CodeCompileUnit
            {
                Namespaces =
                {
                    new CodeNamespace(metadataType.Namespace)
                    {
                        Imports =
                        {
                            new CodeNamespaceImport("System"),
                            new CodeNamespaceImport("System.Collections.Generic"),
                            new CodeNamespaceImport("AbstraX.DataAnnotations"),
                            new CodeNamespaceImport("System.ComponentModel.DataAnnotations"),
                            new CodeNamespaceImport("System.ComponentModel")
                        },
                        Types =
                        {
                            entityClass, 
                            metaDataClass
                        }
                    }
                }
            };

            codeGenerator = new CSharpCodeProvider().CreateGenerator();

            using (var writer = new StringWriter())
            {
                codeGenerator.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions
                {
                    BracingStyle = "C"
                });
                return writer.ToString().Replace("{ get; set; };", "{ get; set; }");
            }
        }

        private static CodeTypeDeclaration GetCodeTypeDeclaration(this Type type, string metadataType = null)
        {
            var newClass = new CodeTypeDeclaration(type.Name)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                IsPartial = true
            };

            foreach (var attribute in type.GetCustomAttributesData())
            {
                if (attribute.AttributeType.Name == "MetadataTypeAttribute")
                {
                    newClass.CustomAttributes.Add(new CodeAttributeDeclaration(attribute.AttributeType.Name, new CodeAttributeArgument(new CodeSnippetExpression(string.Format("typeof({0})", metadataType)))));
                }
                else
                {
                    newClass.CustomAttributes.Add(new CodeAttributeDeclaration(attribute.AttributeType.Name, attribute.ConstructorArguments.Select(a => new CodeAttributeArgument(new CodeSnippetExpression(a.GetText()))).ToArray()));
                }
            }

            if (type.IsGenericType)
            {
                foreach (var genericArgumentType in type.GetGenericArguments())
                {
                    newClass.TypeParameters.Add(genericArgumentType.Name);
                }
            }

            foreach (var propertyInfo in type.GetProperties())
            {
                // this is a hack to get auto properties

                var property = new CodeMemberField
                {
                    Name = propertyInfo.Name + " { get; set; }",
                    Type = propertyInfo.PropertyType.MakeTypeReference(),
                    Attributes = MemberAttributes.Public,
                };

                foreach (var attribute in propertyInfo.GetCustomAttributesData())
                {
                    property.CustomAttributes.Add(new CodeAttributeDeclaration(attribute.AttributeType.Name, attribute.ConstructorArguments.Select(a => new CodeAttributeArgument(new CodeSnippetExpression(a.GetText()))).ToArray()));
                }

                newClass.Members.Add(property);
            }

            return newClass;
        }

        private static string GetText(this System.Reflection.CustomAttributeTypedArgument argument)
        {
            var type = argument.ArgumentType;
            var value = "null";

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:

                    value = "\"" + argument.Value.ToString() + "\"";
                    break;

                case TypeCode.Boolean:

                    value = argument.Value.ToString().ToLower();
                    break;

                case TypeCode.Int32:

                    if (type.IsEnum)
                    {
                        value = type.Name + "." + Enum.GetName(type, argument.Value);
                    }
                    else
                    {
                        value = argument.Value.ToString();
                    }

                    break;

                default:

                    if (type.IsAssignableFrom(typeof(Enum)))
                    {
                        value = type.Name + "." + Enum.GetName(type, argument.Value);
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

        private static CodeTypeReference MakeTypeReference(this Type type)
        {
            if (!type.IsGenericType)
            {
                return new CodeTypeReference(type);
            }
            else
            {
                return new CodeTypeReference(type.Name, type.GetGenericArguments().Select(MakeTypeReference).ToArray());
            }
        }

        /// <summary>   Gets the related entity attributes in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="entityObject"> The entityObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the related entity attributes in this
        /// collection.
        /// </returns>

        public static IEnumerable<AttributeObject> GetRelatedEntityAttributes(this EntityObject entityObject)
        {
            return entityObject.Attributes.Where(a => a.Properties.Any(p => p.PropertyName == "RelatedEntity"));
        }

        /// <summary>
        /// An EntityObject extension method that query if 'entityObject' has related entity
        /// attributes.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="entityObject"> The entityObject to act on. </param>
        ///
        /// <returns>   True if related entity attributes, false if not. </returns>

        public static bool HasRelatedEntityAttributes(this EntityObject entityObject)
        {
            return entityObject.Attributes.Any(a => a.Properties != null && a.Properties.Any(p => p.PropertyName == "RelatedEntity"));
        }


        /// <summary>
        /// An EntityObject extension method that query if 'entityObject' has identity kind.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="entityObject">         The entityObject to act on. </param>
        /// <param name="identityEntityKind">   The identity kind. </param>
        ///
        /// <returns>   True if identity kind, false if not. </returns>

        public static bool HasIdentityEntityKind(this EntityObject entityObject, IdentityEntityKind identityEntityKind)
        {
            return entityObject.Properties.Any(p => p.PropertyName == "IdentityEntity") && entityObject.Properties.Single(p => p.PropertyName == "IdentityEntity").ChildProperties.Any(p2 => p2.PropertyName == "IdentityEntityKind" && p2.PropertyValue == identityEntityKind.ToString());
        }

        /// <summary>
        /// An EntityObject extension method that query if 'entityObject' has identify field kind.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="attributeObject">         The entityObject to act on. </param>
        /// <param name="identityFieldKind">    The identity field kind. </param>
        ///
        /// <returns>   True if identify entity kind, false if not. </returns>

        public static bool HasIdentityFieldKind(this AttributeObject attributeObject, IdentityFieldKind identityFieldKind)
        {
            return attributeObject.Properties.Any(p => p.PropertyName == "IdentityEntity") && attributeObject.Properties.Single(p => p.PropertyName == "IdentityField").ChildProperties.Any(p2 => p2.PropertyName == "IdentityFieldKind" && p2.PropertyValue == identityFieldKind.ToString());
        }

        /// <summary>   An EntityObject extension method that shadows the given entity object. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="entityObject"> The entityObject to act on. </param>
        ///
        /// <returns>   An EntityObject. </returns>

        public static EntityObject Shadow(this EntityObject entityObject)
        {
            var shadow = entityObject.CreateCopy<EntityObject>();

            shadow.ShadowOfEntity = entityObject;

            return shadow;
        }

        /// <summary>   Gets the descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">  The hierarchyNodeObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants in this collection.
        /// </returns>

        public static IEnumerable<UIHierarchyNodeObject> GetDescendants(this UIHierarchyNodeObject hierarchyNodeObject)
        {
            var descendants = new List<UIHierarchyNodeObject>();

            hierarchyNodeObject.GetDescendants(o => o.Children, o =>
            {
                descendants.Add(o);
            });

            return descendants;
        }

        /// <summary>   Gets the descendants and selfs in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">  The hierarchyNodeObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants and selfs in this
        /// collection.
        /// </returns>

        public static IEnumerable<UIHierarchyNodeObject> GetDescendantsAndSelf(this UIHierarchyNodeObject hierarchyNodeObject)
        {
            var descendants = new List<UIHierarchyNodeObject>();

            hierarchyNodeObject.GetDescendantsAndSelf(o => o.Children, o =>
            {
                descendants.Add(o);
            });

            return descendants;
        }

        /// <summary>   Gets the ancestors in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">  The hierarchyNodeObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the ancestors in this collection.
        /// </returns>

        public static IEnumerable<UIHierarchyNodeObject> GetAncestors(this UIHierarchyNodeObject hierarchyNodeObject)
        {
            var ancestors = new List<UIHierarchyNodeObject>();

            while (hierarchyNodeObject.Parent != null)
            {
                ancestors.Add(hierarchyNodeObject);

                hierarchyNodeObject = hierarchyNodeObject.Parent;
            }

            return ancestors;
        }

        /// <summary>   Gets the descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="businessModel">    The business model. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants in this collection.
        /// </returns>

        public static IEnumerable<BusinessModelObject> GetDescendants(this BusinessModel businessModel)
        {
            var descendants = new List<BusinessModelObject>();
            var businessModelObject = businessModel.TopLevelObject;

            businessModelObject.GetDescendantsAndSelf(o => o.Children, o =>
            {
                descendants.Add(o);
            });

            return descendants;
        }

        /// <summary>   Gets the descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="businessModelObject">  The businessModelObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants in this collection.
        /// </returns>

        public static IEnumerable<BusinessModelObject> GetDescendants(this BusinessModelObject businessModelObject)
        {
            var descendants = new List<BusinessModelObject>();

            businessModelObject.GetDescendants(o => o.Children, o =>
            {
                descendants.Add(o);
            });

            return descendants;
        }

        /// <summary>   Gets the descendants and selfs in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="businessModelObject">  The businessModelObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants and selfs in this
        /// collection.
        /// </returns>

        public static IEnumerable<BusinessModelObject> GetDescendantsAndSelf(this BusinessModelObject businessModelObject)
        {
            var descendants = new List<BusinessModelObject>();

            businessModelObject.GetDescendantsAndSelf(o => o.Children, o =>
            {
                descendants.Add(o);
            });

            return descendants;
        }

        /// <summary>   Gets the ancestors in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="businessModelObject">  The businessModelObject to act on. </param>
        /// <param name="businessModel">        The business model. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the ancestors in this collection.
        /// </returns>

        public static IEnumerable<BusinessModelObject> GetAncestors(this BusinessModelObject businessModelObject, BusinessModel businessModel)
        {
            var ancestors = new List<BusinessModelObject>();
            var parentId = businessModelObject.ParentId;
            var allBusinessModelObjects = businessModel.GetDescendants();

            while (parentId != 0)
            {
                businessModelObject = allBusinessModelObjects.Single(o => o.Id == parentId);

                ancestors.Add(businessModelObject);

                parentId = businessModelObject.ParentId;
            }

            return ancestors;
        }

        /// <summary>   Gets the descendant properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityBaseObject"> The entityBaseObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendant properties in this
        /// collection.
        /// </returns>

        public static IEnumerable<EntityPropertyItem> GetDescendantProperties(this EntityBaseObject entityBaseObject)
        {
            var descendants = new List<EntityPropertyItem>();
            var properties = entityBaseObject.Properties;

            foreach (var property in properties)
            {
                foreach (var property2 in property.GetDescendantPropertiesAndSelf())
                {
                    descendants.Add(property2);
                }
            }

            return descendants;
        }

        /// <summary>   Gets the descendant properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityPropertyItem">   The entityPropertyItem to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendant properties in this
        /// collection.
        /// </returns>

        public static IEnumerable<EntityPropertyItem> GetDescendantProperties(this EntityPropertyItem entityPropertyItem)
        {
            var descendants = new List<EntityPropertyItem>();

            entityPropertyItem.GetDescendants(p => p.ChildProperties, (p) =>
            {
                descendants.Add(p);
            });

            return descendants;
        }

        /// <summary>   Gets the descendant properties and selfs in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityPropertyItem">   The entityPropertyItem to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendant properties and selfs
        /// in this collection.
        /// </returns>

        public static IEnumerable<EntityPropertyItem> GetDescendantPropertiesAndSelf(this EntityPropertyItem entityPropertyItem)
        {
            var descendants = new List<EntityPropertyItem>();

            entityPropertyItem.GetDescendantsAndSelf(p => p.ChildProperties, (p) =>
            {
                descendants.Add(p);
            });

            return descendants;
        }

        /// <summary>   Gets the ancestor properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityPropertyItem">   The entityPropertyItem to act on. </param>
        /// <param name="entityBaseObject">     The entityBaseObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the ancestor properties in this
        /// collection.
        /// </returns>

        public static IEnumerable<EntityPropertyItem> GetAncestorProperties(this EntityPropertyItem entityPropertyItem, EntityBaseObject entityBaseObject)
        {
            var ancestors = new List<EntityPropertyItem>();
            var allEntityProperties = entityBaseObject.GetDescendantProperties();

            while (entityPropertyItem != null)
            {
                entityPropertyItem = allEntityProperties.SingleOrDefault(i => i.ChildProperties.Contains(entityPropertyItem));

                ancestors.Add(entityPropertyItem);
            }

            return ancestors;
        }


        /// <summary>   A DirectoryInfo extension method that backups the given directory. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="directory">    The directory to act on. </param>

        public static void Backup(this DirectoryInfo directory)
        {
            var zipName = Path.Combine(directory.Parent.FullName, directory.Name + DateTime.Now.ToSortableDateTimeText() + ".zip");
            var exclusions = new List<string>
            {
                "debug",
                "debugPublic",
                "release",
                "releases",
                "x64",
                "x86",
                "arm",
                "arm64",
                "bld",
                "bin",
                "obj",
                "log",
                "logs",
                "node_modules",
                ".dll",
                ".exe",
                ".zip"
            };


            directory.ToZipFile(zipName, (f) =>
            {
                if (f.Directory.GetParts().Any(p => exclusions.Where(e => !e.StartsWith(".")).Any(e => e.AsCaseless() == p)))
                {
                    return false;
                }
                else if (exclusions.Where(e => e.StartsWith(".")).Any(e => e == f.Extension))
                {
                    return false;
                }

                return true;
            });
        }

        /// <summary>   A GetOverridesEventHandler extension method that gets override identifier. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handler">      The handler to act on. </param>
        /// <param name="baseObject">   The base object. </param>
        /// <param name="predicate">    The predicate. </param>
        /// <param name="generatedId">  Identifier for the generated. </param>
        ///
        /// <returns>   The override identifier. </returns>

        public static string GetOverrideId(this GetOverridesEventHandler handler, IBase baseObject, string predicate, string generatedId)
        {
            var overrideEventHandler = handler;
            var args = new GetOverridesEventArgs();
            IGeneratorOverrides generatorOverride;

            overrideEventHandler(baseObject, args);
            generatorOverride = args.Overrides.Select(p => p.Value).Last();

            return generatorOverride.GetOverrideId(baseObject, predicate, generatedId);
        }

        /// <summary>   A GetOverridesEventHandler extension method that skip process. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handler">                  The handler to act on. </param>
        /// <param name="facetHandler">             The facet handler. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="facet">                    The facet. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool SkipProcess(this GetOverridesEventHandler handler, IFacetHandler facetHandler, IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var overrideEventHandler = handler;
            var args = new GetOverridesEventArgs();
            IGeneratorOverrides generatorOverride;

            overrideEventHandler(baseObject, args);
            generatorOverride = args.Overrides.Select(p => p.Value).Last();

            return generatorOverride.SkipProcess(facetHandler, baseObject, facet, generatorConfiguration);
        }

        /// <summary>   An IEntityWithDataType extension method that gets original data type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityWithDataType">   The entityWithDataType to act on. </param>
        ///
        /// <returns>   The original data type. </returns>

        public static BaseType GetOriginalDataType(this IEntityWithDataType entityWithDataType)
        {
            var args = new GetOverridesEventArgs();
            var overrideEventHandler = entityWithDataType.OverrideEventHandler;
            var entityDataType = entityWithDataType.DataType;
            IGeneratorOverrides generatorOverride;
            BaseType type;
            string _namespace;

            overrideEventHandler(entityWithDataType, args);
            generatorOverride = args.Overrides.Select(p => p.Value).Last();

            if (generatorOverride.OverridesNamespace)
            {
                _namespace = generatorOverride.OriginalNamespace;

                type = new BaseType()
                {
                    FullyQualifiedName = _namespace + "." + entityWithDataType.DataType.Name,
                    Name = entityDataType.Name,
                    ID = entityDataType.ID,
                    ParentID = entityDataType.ParentID
                };
            }
            else
            {
                type = entityWithDataType.DataType;
            }

            return type;
        }

        /// <summary>
        /// A Dictionary&lt;string,PackageWorkingInstallFromCache&gt; extension method that query if
        /// 'dependencies' contains install key.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="dependencies"> The dependencies to act on. </param>
        /// <param name="root">         The root. </param>
        /// <param name="install">      The install. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool ContainsInstallKey(this Dictionary<string, PackageWorkingInstallFromCache> dependencies, PackageWorkingInstallFromCache root, string install)
        {
            return dependencies.Concat(new List<KeyValuePair<string, PackageWorkingInstallFromCache>>() { new KeyValuePair<string, PackageWorkingInstallFromCache>(root.Install, root) }).Where(p => !p.Value.InstallToLocalModules).ToDictionary().ContainsKey(install);
        }

        /// <summary>   A PackageWorkingInstallFromCache extension method that creates the status. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="installFromCache"> The installFromCache to act on. </param>
        /// <param name="status">           The status. </param>
        /// <param name="mode">             The mode. </param>
        ///
        /// <returns>   The new status. </returns>

        public static PackageInstallFromCacheStatus CreateStatus(this PackageWorkingInstallFromCache installFromCache, string status, StatusMode mode)
        {
            return new PackageInstallFromCacheStatus(installFromCache.Install, installFromCache.CachePath, installFromCache.PackagePath, status, mode);
        }

        /// <summary>   A PackageWorkingInstallFromCache extension method that creates the status. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="installFromCache"> The installFromCache to act on. </param>
        /// <param name="ex">               The exception. </param>
        ///
        /// <returns>   The new status. </returns>

        public static PackageInstallFromCacheStatus CreateStatus(this PackageWorkingInstallFromCache installFromCache, Exception ex)
        {
            return new PackageInstallFromCacheStatus(installFromCache.Install, installFromCache.CachePath, installFromCache.PackagePath, ex.Message, StatusMode.Error);
        }

        /// <summary>   A string extension method that fix dots. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   A string. </returns>

        public static string FixDots(this string path)
        {
            return path.RegexReplace(@"^\.\./", "./");
        }

        /// <summary>   A TimeSpan extension method that converts a timeSpan to a decimal seconds. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="timeSpan"> The timeSpan to act on. </param>
        ///
        /// <returns>   TimeSpan as a float. </returns>

        public static float ToDecimalSeconds(this TimeSpan timeSpan)
        {
            return timeSpan.GetDecimalTimeComponent((t) => t.Milliseconds, 2);
        }

        /// <summary>   An UIAttribute extension method that gets feature kind. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="uIAttribute">  The uIAttribute to act on. </param>
        ///
        /// <returns>   The feature kind. </returns>

        public static UIFeatureKind GetFeatureKind(this UIAttribute uIAttribute)
        {
            var uiKind = uIAttribute.UIKind;
            var field = EnumUtils.GetField<UIKind>(uiKind);
            var kindGuidAttribute = field.GetCustomAttribute<KindGuidAttribute>();

            return kindGuidAttribute.FeatureKind;
        }

        /// <summary>
        /// An UIFeatureKind extension method that query if 'uiFeatureKind' is component.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="uiFeatureKind">    The uiFeatureKind to act on. </param>
        ///
        /// <returns>   True if component, false if not. </returns>

        public static bool IsComponent(this UIFeatureKind uiFeatureKind)
        {
            return uiFeatureKind.IsOneOf(UIFeatureKind.CustomComponent, UIFeatureKind.StandardComponent);
        }

        /// <summary>   An XPathOperator extension method that gets a text. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="_operator">    The _operator to act on. </param>
        ///
        /// <returns>   The text. </returns>

        public static string GetText(this XPathOperator _operator)
        {
            return XPathElement.GetCSharpOperatorString(_operator);
        }

        /// <summary>
        /// A QueryDictionary extension method that adds to the dictionary list create if not exist.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="dictionary">   The dictionary to act on. </param>
        /// <param name="key">          The key. </param>
        /// <param name="queryInfo">    Information describing the query. </param>

        public static void AddToDictionaryListCreateIfNotExist(this QueryDictionary dictionary, IBase key, QueryInfo queryInfo)
        {
            List<QueryInfo> items;

            if (dictionary.ContainsKey(key))
            {
                items = dictionary[key];
            }
            else
            {
                items = new List<QueryInfo>();

                dictionary.Add(key, items);
            }

            if (!items.Contains(queryInfo))
            {
                items.Add(queryInfo);
            }
        }

        /// <summary>   An IElement extension method that gets a key. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="element">  The element to act on. </param>
        ///
        /// <returns>   The key. </returns>

        public static IAttribute GetKey(this IElement element)
        {
            if (element is IElementWithKeyAttribute)
            {
                var elementWithKeyAttribute = (IElementWithKeyAttribute)element;

                return elementWithKeyAttribute.GetKeyAttribute();
            }

            return element.Attributes.Single(a => a.HasFacetAttribute<KeyAttribute>());
        }

        /// <summary>
        /// A List&lt;IBuiltInImportHandler&gt; extension method that clears the declarations described
        /// by builtInImportHandlerList.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="builtInImportHandlerList"> The builtInImportHandlerList to act on. </param>

        public static void ClearDeclarations(this List<IBuiltInImportHandler> builtInImportHandlerList)
        {
            builtInImportHandlerList.ForEach(b => b.ClearDeclarations());
        }

        /// <summary>   Enumerates exclude in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="importGroups"> The importGroups to act on. </param>
        /// <param name="moduleName">   Name of the module. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process exclude in this collection.
        /// </returns>

        public static IDictionary<string, IEnumerable<ModuleImportDeclaration>> Exclude(this IDictionary<string, IEnumerable<ModuleImportDeclaration>> importGroups, string moduleName)
        {
            return importGroups.Where(g => !g.Value.Any(m => m.ModuleNames.Any(n => n == moduleName))).ToDictionary(g => g.Key, g => g.Value);
        }

        /// <summary>   Enumerates exclude in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="importDeclarations">   The importDeclarations to act on. </param>
        /// <param name="moduleName">           Name of the module. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process exclude in this collection.
        /// </returns>

        public static IEnumerable<ModuleImportDeclaration> Exclude(this IEnumerable<ModuleImportDeclaration> importDeclarations, string moduleName)
        {
            return importDeclarations.Where(m => !m.ModuleNames.Any(n => n == moduleName));
        }

        /// <summary>
        /// An IDictionary&lt;string,IEnumerable&lt;ModuleImportDeclaration&gt;&gt; extension method that
        /// adds an index import.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="importGroups">     The importGroups to act on. </param>
        /// <param name="groupName">        Name of the group. </param>
        /// <param name="moduleName">       Name of the module. </param>
        /// <param name="subFolderCount">   Number of sub folders. </param>

        public static void AddIndexImport(this IDictionary<string, IEnumerable<ModuleImportDeclaration>> importGroups, string groupName, string moduleName, int subFolderCount)
        {
            var folderBuilder = new StringBuilder();
            var declarations = (List<ModuleImportDeclaration>) importGroups[groupName];
            ModuleImportDeclaration declaration;
            string path;

            subFolderCount.Countdown((n) =>
            {
                folderBuilder.Append(@"../");
            });

            folderBuilder.Append("..");
            path = folderBuilder.ToString();

            declaration = new ModuleImportDeclaration(path, moduleName);

            declarations.Add(declaration);
        }

        /// <summary>   An IModuleAssembly extension method that creates import declaration. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="module">           The module to act on. </param>
        /// <param name="folder">           Pathname of the folder. </param>
        /// <param name="subFolderCount">   Number of sub folders. </param>
        ///
        /// <returns>   The new import declaration. </returns>

        public static ModuleImportDeclaration CreateImportDeclaration(this Module module, Folder folder, int subFolderCount)
        {
            var file = module.File;
            var fileName = FileSystemObject.PathCombine(file.FolderName, Path.GetFileNameWithoutExtension(file.Name));
            var fileInfo = new FileInfo(fileName);
            var folderBuilder = new StringBuilder(folder.FullName.BackSlashes());
            string path;
            ModuleImportDeclaration declaration;

            subFolderCount.Countdown((n) =>
            {
                folderBuilder.AppendFormat(@"\subFolder{0}", n);
            });

            if (module.ReferencedByIndex)
            {
                path = Path.GetDirectoryName(fileInfo.GetRelativePath(folderBuilder.ToString()).ForwardSlashes());
            }
            else
            {
                path = fileInfo.GetRelativePath(folderBuilder.ToString()).ForwardSlashes();
            }

            if (!path.StartsWith(@"../"))
            {
                path = path.Prepend("./");
            }

            declaration = new ModuleImportDeclaration(path, module, module.Name);

            return declaration;
        }

        /// <summary>   An IModuleAssembly extension method that creates import declaration. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="moduleAssembly">   The moduleAssembly to act on. </param>
        /// <param name="folder">           Pathname of the folder. </param>
        /// <param name="subFolderCount">   Number of sub folders. </param>
        ///
        /// <returns>   The new import declaration. </returns>

        public static ModuleImportDeclaration CreateImportDeclaration(this IModuleAssembly moduleAssembly, Folder folder, int subFolderCount)
        {
            var file = moduleAssembly.File;
            var fileName = FileSystemObject.PathCombine(file.FolderName, Path.GetFileNameWithoutExtension(file.Name));
            var fileInfo = new FileInfo(fileName);
            var folderBuilder = new StringBuilder(folder.FullName.BackSlashes());
            string path;
            ModuleImportDeclaration declaration;

            subFolderCount.Countdown((n) =>
            {
                folderBuilder.AppendFormat(@"\subFolder{0}", n);
            });

            path = fileInfo.GetRelativePath(folderBuilder.ToString()).ForwardSlashes();

            if (!path.StartsWith(@"../"))
            {
                path = path.Prepend("./");
            }

            declaration = new ModuleImportDeclaration(path, moduleAssembly, moduleAssembly.Name);

            return declaration;
        }

        /// <summary>   An IAttribute extension method that query if 'attribute' is key. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="attribute">    The attribute to act on. </param>
        ///
        /// <returns>   True if key, false if not. </returns>

        public static bool IsKey(this IAttribute attribute)
        {
            return attribute.HasFacetAttribute<KeyAttribute>();
        }

        /// <summary>   An Enum extension method that gets module import declaration attribute. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="moduleId"> The moduleId to act on. </param>
        ///
        /// <returns>   The module import declaration attribute. </returns>

        public static ModuleImportDeclarationAttribute GetModuleImportDeclarationAttribute(this Enum moduleId)
        {
            var field = EnumUtils.GetField(moduleId);

            return field.GetCustomAttribute<ModuleImportDeclarationAttribute>();
        }

        /// <summary>
        /// An IdentityFieldKind extension method that query if 'userFieldKind' has identity field
        /// category attribute.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="userFieldKind">    The userFieldKind to act on. </param>
        ///
        /// <returns>   True if identity field category attribute, false if not. </returns>

        public static bool HasIdentityFieldCategoryAttribute(this IdentityFieldKind userFieldKind)
        {
            var field = EnumUtils.GetField(userFieldKind);

            return field.HasCustomAttribute<IdentityFieldCategoryAttribute>();
        }

        /// <summary>
        /// An IdentityFieldKind extension method that gets identity field category attribute.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="userFieldKind">    The userFieldKind to act on. </param>
        ///
        /// <returns>   The identity field category attribute. </returns>

        public static IdentityFieldCategoryAttribute GetIdentityFieldCategoryAttribute(this IdentityFieldKind userFieldKind)
        {
            var field = EnumUtils.GetField(userFieldKind);

            return field.GetCustomAttribute<IdentityFieldCategoryAttribute>();
        }

        /// <summary>   An IAttribute extension method that gets script type name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="attribute">    The attribute to act on. </param>
        ///
        /// <returns>   The script type name. </returns>

        public static string GetScriptTypeName(this IAttribute attribute)
        {
            var shortTypeName = attribute.DataType.UnderlyingType.GetShortName();
            var mappings = TypeMapper.Mappings["ScriptTypes"];
            var entry = mappings.Map[shortTypeName];

            return entry.MappedToType;
        }

        /// <summary>   An IAttribute extension method that gets dot net type name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="attribute">    The attribute to act on. </param>
        ///
        /// <returns>   The dot net type name. </returns>

        public static string GetDotNetTypeName(this IAttribute attribute)
        {
            var shortTypeName = attribute.DataType.UnderlyingType.GetShortName();

            return shortTypeName;
        }

        /// <summary>   An IAttribute extension method that gets short type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="attribute">    The attribute to act on. </param>
        ///
        /// <returns>   The short type. </returns>

        public static string GetShortType(this IAttribute attribute)
        {
            return attribute.DataType.UnderlyingType.GetShortName();
        }

        /// <summary>   A string[] extension method that splits the imports. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="importStrings">    The importStrings to act on. </param>
        ///
        /// <returns>   A Guid[]. </returns>

        public static Guid[] SplitImports(this string[] importStrings)
        {
            var list = new List<Guid>();

            if (importStrings != null)
            {
                foreach (var str in importStrings)
                {
                    foreach (var str2 in str.Split(",").Select(s => s.Trim()))
                    {
                        list.Add(Guid.Parse(str2));
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>   A string extension method that converts this  to a constant name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="value">    The value to act on. </param>
        /// <param name="suffix">   (Optional) The suffix. </param>
        ///
        /// <returns>   The given data converted to a string. </returns>

        public static string ToConstantName(this string value, string suffix = "")
        {
            return value.Replace(" ", "_").ToUpper() + suffix;
        }

        /// <summary>   An Expression extension method that visits the given node. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="node"> The node to act on. </param>
        ///
        /// <returns>   An AbstraXVisitor. </returns>

        public static AbstraXVisitor Visit(this Expression node)
        {
            var visitor = new AbstraXVisitor();

            visitor.Visit(node);

            return visitor;
        }

        /// <summary>   An IBase extension method that gets display name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The display name. </returns>

        public static string GetDisplayName(this IBase baseObject)
        {
            DisplayNameAttribute displayNameAttribute;
            string name;

            if (baseObject.NoUIConfigOrFollow())
            {
                return baseObject.Name.ToTitleCase();
            }

            displayNameAttribute = baseObject.GetFacetAttribute<DisplayNameAttribute>();

            if (displayNameAttribute != null)
            {
                name = displayNameAttribute.DisplayName;
            }
            else
            {
                name = baseObject.Name;
            }

            return name;
        }

        /// <summary>   An IBase extension method that gets navigation name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="loadKind">     The load kind. </param>
        ///
        /// <returns>   The navigation name. </returns>

        public static string GetNavigationName(this IBase baseObject, UILoadKind loadKind)
        {
            int count;
            UINavigationNameAttribute nameAttribute;
            string name;

            if (baseObject.NoUIConfigOrFollow())
            {
                return baseObject.Name.ToTitleCase();
            }
            count = baseObject.GetFacetAttributes<UINavigationNameAttribute>().Count();

            if (count == 1)
            {
                nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>();
            }
            else
            {
                nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>(u => u.UILoadKind == loadKind);
            }

            if (nameAttribute != null)
            {
                name = nameAttribute.Name;
            }
            else if (baseObject is NavigationProperty)
            {
                name = baseObject.Name;
            }
            else
            {
                name = baseObject.Name;
            }

            return name;
        }

        /// <summary>   An IBase extension method that gets navigation name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="kind">         The kind. </param>
        ///
        /// <returns>   The navigation name. </returns>

        public static string GetNavigationName(this IBase baseObject, UIKind kind)
        {
            int count;
            UINavigationNameAttribute nameAttribute;
            string name;

            if (baseObject.NoUIConfigOrFollow())
            {
                return baseObject.Name.ToTitleCase();
            }
            count = baseObject.GetFacetAttributes<UINavigationNameAttribute>().Count();

            if (count == 1)
            {
                nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>();
            }
            else
            {
                nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>(u => u.UIKind == kind);
            }

            if (nameAttribute != null)
            {
                name = nameAttribute.Name;
            }
            else if (baseObject is NavigationProperty)
            {
                name = baseObject.Name;
            }
            else
            {
                name = baseObject.Name;
            }

            return name;
        }

        /// <summary>   An IBase extension method that gets navigation name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The navigation name. </returns>

        public static string GetNavigationName(this IBase baseObject)
        {
            UINavigationNameAttribute nameAttribute;
            string name;

            if (baseObject.NoUIConfigOrFollow())
            {
                return baseObject.Name.ToTitleCase();
            }

            nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>();

            if (nameAttribute != null)
            {
                name = nameAttribute.Name;
            }
            else if (baseObject is NavigationProperty)
            {
                name = baseObject.Name;
            }
            else
            {
                name = baseObject.Name;
            }

            return name;
        }

        /// <summary>   Gets the following childrens in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">           The base object. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        /// <param name="noScalar">             (Optional) True to no scalar. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the following childrens in this
        /// collection.
        /// </returns>

        public static IEnumerable<IBase> GetFollowingChildren(this IBase baseObject, PartsAliasResolver partsAliasResolver, bool noScalar = false)
        {
            var parentObject = (IParentBase)baseObject;

            if (parentObject.Kind == DefinitionKind.Model)
            {
                foreach (var descendantObject in baseObject.GetDescendants())
                {
                    if (descendantObject.HasFacetAttribute<UIAttribute>())
                    {
                        yield return descendantObject;
                    }
                }
            }
            else
            {
                if (parentObject.ChildNodes != null)
                {
                    foreach (var childObject in parentObject.ChildNodes)
                    {
                        if (childObject.Follows(parentObject, partsAliasResolver, noScalar))
                        {
                            yield return childObject;
                        }
                    }
                }
            }
        }

        /// <summary>   Gets the ancestors in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="includeSelf">  (Optional) True to include, false to exclude the self. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the ancestors in this collection.
        /// </returns>

        public static IEnumerable<IBase> GetAncestors(this IBase baseObject, bool includeSelf = false)
        {
            var parent = baseObject.Parent;
            var ancestors = new List<IBase>();

            if (includeSelf)
            {
                ancestors.Add(baseObject);
            }

            while (parent != null)
            {
                ancestors.Add(parent);

                parent = parent.Parent;
            }

            return ancestors;
        }

        /// <summary>   Gets the following descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">           The base object. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the following descendants in this
        /// collection.
        /// </returns>

        public static IEnumerable<IBase> GetFollowingDescendants(this IBase baseObject, PartsAliasResolver partsAliasResolver)
        {
            var descendants = new List<IBase>();

            baseObject.GetDescendants(obj => obj.GetFollowingChildren(partsAliasResolver), obj =>
            {
                descendants.Add(obj);
            });

            return descendants;
        }

        /// <summary>   Gets the descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants in this collection.
        /// </returns>

        public static IEnumerable<IBase> GetDescendants(this IBase baseObject)
        {
            var descendants = new Dictionary<string, IBase>();

            baseObject.GetDescendants(obj => (obj is IParentBase && ((IParentBase)obj).ChildNodes != null) ? ((IParentBase)obj).ChildNodes : new List<IBase>(), obj =>
            {
                var name = obj.Name;

                if (!descendants.ContainsKey(name))
                {
                    descendants.Add(name, obj);
                    return true;
                }
                else
                {
                    return false;
                }
            });

            return descendants.Values;
        }

        /// <summary>   Gets the following navigation childrens in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">           The base object. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the following navigation childrens in
        /// this collection.
        /// </returns>

        public static IEnumerable<IBase> GetFollowingNavigationChildren(this IBase baseObject, PartsAliasResolver partsAliasResolver)
        {
            var parentObject = (IParentBase)baseObject;

            foreach (var childObject in parentObject.ChildNodes.OfType<NavigationProperty>())
            {
                if (childObject.Follows(parentObject, partsAliasResolver))
                {
                    yield return childObject;
                }
            }
        }

        /// <summary>   Gets the grid columns in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="element">              The element to act on. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the grid columns in this collection.
        /// </returns>

        public static IEnumerable<IBase> GetGridColumns(this IElement element, PartsAliasResolver partsAliasResolver)
        {
            foreach (var attribute in element.Attributes.Where(a => a.HasFacetAttribute<GridColumnAttribute>()))
            {
                if (attribute.Follows(element, partsAliasResolver))
                {
                    yield return attribute;
                }
            }
        }

        /// <summary>   Gets the form fields in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="element">              The element to act on. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the form fields in this collection.
        /// </returns>

        public static IEnumerable<IBase> GetFormFields(this IElement element, PartsAliasResolver partsAliasResolver)
        {
            foreach (var attribute in element.Attributes.Where(a => a.HasFacetAttribute<FormFieldAttribute>()))
            {
                if (attribute.Follows(element, partsAliasResolver))
                {
                    yield return attribute;
                }
            }
        }

        /// <summary>   Gets the identity fields in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="element">  The element to act on. </param>
        /// <param name="category"> The category. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the identity fields in this
        /// collection.
        /// </returns>

        public static IEnumerable<IBase> GetIdentityFields(this IElement element, IdentityFieldCategory category)
        {
            foreach (var attribute in element.Attributes.Where(a => a.HasFacetAttribute<IdentityFieldAttribute>()))
            {
                var userFieldAttribute = attribute.GetFacetAttribute<IdentityFieldAttribute>();
                var userFieldKind = userFieldAttribute.IdentityFieldKind;

                if (userFieldKind.HasIdentityFieldCategoryAttribute())
                {
                    var categoryAttribute = userFieldKind.GetIdentityFieldCategoryAttribute();

                    if (categoryAttribute.IdentityFieldCategoryFlags.HasAnyFlag(category))
                    {
                        yield return attribute;
                    }
                }
            }
        }

        /// <summary>   Gets the parent navigation properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="element">              The element to act on. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the parent navigation properties in
        /// this collection.
        /// </returns>

        public static IEnumerable<IRelationProperty> GetParentNavigationProperties(this IElement element, PartsAliasResolver partsAliasResolver)
        {
            var grandParent = (IElement)element.Parent.Parent;

            if (grandParent.Kind == DefinitionKind.StaticContainer)
            {
                var entityContainer = (IEntityContainer)grandParent;

                foreach (var childElement in entityContainer.EntitySets)
                {
                    if (childElement.ChildElements.Single().ID == element.ID && childElement.Follows(element, partsAliasResolver))
                    {
                        yield return childElement;
                    }
                }
            }
            else
            {
                var container = grandParent.GetContainer();

                foreach (var childElement in container.ChildElements.OfType<IRelationProperty>())
                {
                    if (childElement.ChildElements.Single().Name == element.Name)
                    {
                        yield return childElement;
                    }
                }

                foreach (var childElement in grandParent.ChildElements.OfType<IRelationProperty>())
                {
                    if (childElement.ChildElements.Single().ID == element.ID && childElement.Follows(element, partsAliasResolver))
                    {
                        yield return childElement;
                    }
                }
            }
        }

        /// <summary>   An IBase extension method that gets a container. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The container. </returns>

        public static IEntityContainer GetContainer(this IBase baseObject)
        {
            while (baseObject != null)
            {
                baseObject = baseObject.Parent;

                if (baseObject is IEntityContainer)
                {
                    return (IEntityContainer)baseObject;
                }
            }

            return null;
        }

        /// <summary>   A QueryPathFunctionKind extension method that gets expected cardinality. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="functionKind"> The functionKind to act on. </param>
        ///
        /// <returns>   The expected cardinality. </returns>

        public static QueryExpectedCardinality GetExpectedCardinality(this QueryPathFunctionKind functionKind)
        {
            var field = EnumUtils.GetField<QueryPathFunctionKind>(functionKind);

            if (field.HasCustomAttribute<QueryExpectedCardinalityAttribute>())
            {
                var expectedCardinalityAttribute = field.GetCustomAttribute<QueryExpectedCardinalityAttribute>();

                return expectedCardinalityAttribute.Cardinality;
            }

            return QueryExpectedCardinality.Unknown;
        }

        /// <summary>
        /// A QueryPathFunctionKind extension method that gets function code expression.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="functionKind"> The functionKind to act on. </param>
        ///
        /// <returns>   The function code expression. </returns>

        public static string GetFunctionCodeExpression(this QueryPathFunctionKind functionKind)
        {
            var field = EnumUtils.GetField<QueryPathFunctionKind>(functionKind);

            if (field.HasCustomAttribute<QueryFunctionCodeAttribute>())
            {
                var codeExpression = field.GetCustomAttribute<QueryFunctionCodeAttribute>();

                return codeExpression.CodeExpression;
            }

            return "<Node Code>";
        }

        /// <summary>   A QueryPathAttribute extension method that gets query path queue. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="loadParentPath">   The loadParentPath to act on. </param>
        /// <param name="baseObject">       The base object. </param>
        ///
        /// <returns>   The query path queue. </returns>

        public static QueryPathQueue GetQueryPathQueue(this QueryPathAttribute loadParentPath, IBase baseObject)
        {
            return new QueryPathQueue(loadParentPath, baseObject);
        }

        /// <summary>   An IBase extension method that gets container set. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The container set. </returns>

        public static IEntitySet GetContainerSet(this IBase baseObject)
        {
            var container = baseObject.GetContainer();

            return container.EntitySets.Single(s => s.ChildElements.Single().Name == baseObject.Name);
        }

        /// <summary>   A ProcessFacetsHandler extension method that raises. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handler">  The handler to act on. </param>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="types">    A variable-length parameters list containing types. </param>

        public static void Raise(this ProcessFacetsHandler handler, object sender, params Type[] types)
        {
            if (handler != null)
            {
                handler.Invoke(sender, new ProcessFacetsEventArgs(types));
            }
        }

        /// <summary>   A ProcessFacetsHandler extension method that raises. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="handler">  The handler to act on. </param>
        /// <param name="sender">   Source of the event. </param>

        public static void Raise<T>(this ProcessFacetsHandler handler, object sender)
        {
            if (handler != null)
            {
                handler.Invoke(sender, new ProcessFacetsEventArgs(new Type[] { typeof(T) }));
            }
        }

        /// <summary>   An IBase extension method that gets navigation leaf. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The navigation leaf. </returns>

        public static string GetNavigationLeaf(this IBase baseObject)
        {
            if (baseObject is Entity_Set)
            {
                return ((Entity_Set)baseObject).Name;
            }
            else
            {
                var paths = baseObject.GetUIHierarchyPaths();
                var queue = ParseHierarchyPath(paths.First());
                var parts = queue.SplitElementParts();

                return parts.Last();
            }
        }

        /// <summary>   An IBase extension method that follows. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="childObject">          The childObject to act on. </param>
        /// <param name="parentObject">         The parent object. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        /// <param name="noScalar">             (Optional) True to no scalar. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool Follows(this IBase childObject, IBase parentObject, PartsAliasResolver partsAliasResolver, bool noScalar = false)
        {
            if (parentObject.Kind == DefinitionKind.Model)
            {
                return true;
            }
            else if (!noScalar || childObject is NavigationProperty)
            {
                var parentPaths = parentObject.GetUIHierarchyPaths();
                var childPaths = childObject.GetUIHierarchyPaths();

                if (parentPaths.Length == 0 && childPaths.Length == 0)
                {
                    if (parentObject is IEntityWithOptionalFacets)
                    {
                        return ((IEntityWithOptionalFacets)parentObject).FollowWithout;
                    }
                }
                else
                {
                    foreach (var parentPath in parentPaths)
                    {
                        var parentQueue = ParseHierarchyPath(parentPath, partsAliasResolver);
                        var parentParts = parentQueue.SplitElementParts();

                        foreach (var childPath in childPaths)
                        {
                            var childQueue = ParseHierarchyPath(childPath, partsAliasResolver);
                            var childParts = childQueue.SplitElementParts();
                            var nextParentPath = false;
                            var x = 0;

                            if (childParts.Count() > 0)
                            {
                                foreach (var part in childParts.Take(childParts.Count() - 1))
                                {
                                    var part2 = parentParts.ElementAt(x);

                                    if (part != part2)
                                    {
                                        nextParentPath = true;
                                        break;
                                    }

                                    x++;
                                }

                                if (nextParentPath)
                                {
                                    continue;
                                }

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>   Enumerates with facets in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="baseObjects">  The baseObjects to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process with facets in this collection.
        /// </returns>

        public static IEnumerable<IBase> WithFacets<T>(this IEnumerable<IBase> baseObjects) where T : Attribute
        {
            return baseObjects.Where(o => o.HasFacetAttribute<T>());
        }

        /// <summary>   An IBase extension method that gets facet attribute. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The facet attribute. </returns>

        public static T GetFacetAttribute<T>(this IBase baseObject) where T : Attribute
        {
            if (baseObject is IEntityWithFacets)
            {
                var entityWithFacets = (IEntityWithFacets)baseObject;
                var facet = entityWithFacets.Facets.SingleOrDefault(f => f.Attribute.IsOfType<T>());

                if (facet != null)
                {
                    return (T)facet.Attribute;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>   An IBase extension method that gets facet attribute. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="baseObject">   The base object. </param>
        /// <param name="filter">       Specifies the filter. </param>
        ///
        /// <returns>   The facet attribute. </returns>

        public static T GetFacetAttribute<T>(this IBase baseObject, Func<T, bool> filter) where T : Attribute
        {
            if (baseObject is IEntityWithFacets)
            {
                var entityWithFacets = (IEntityWithFacets)baseObject;
                var facet = entityWithFacets.Facets.SingleOrDefault(f => f.Attribute.IsOfType<T>() && filter((T) f.Attribute));

                if (facet != null)
                {
                    return (T)facet.Attribute;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>   An IBase extension method that gets facet attributes. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   An array of t. </returns>

        public static T[] GetFacetAttributes<T>(this IBase baseObject) where T : Attribute
        {
            if (baseObject is IEntityWithFacets)
            {
                var entityWithFacets = (IEntityWithFacets)baseObject;
                var facets = entityWithFacets.Facets.Where(f => f.Attribute.IsOfType<T>());

                if (facets.Count() > 0)
                {
                    var attributes = new List<T>();

                    foreach (var facet in facets)
                    {
                        attributes.Add((T)facet.Attribute);
                    }

                    return attributes.ToArray();
                }
                else
                {
                    return new T[0];
                }
            }

            return new T[0];
        }

        /// <summary>   An IBase extension method that gets validation attributes. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   An array of attribute. </returns>

        public static Attribute[] GetValidationAttributes(this IBase baseObject)
        {
            if (baseObject is IEntityWithFacets)
            {
                var entityWithFacets = (IEntityWithFacets)baseObject;
                return entityWithFacets.Facets.Where(f => f.Attribute.GetType().IsOneOf<ValidationAttribute, DataTypeAttribute>()).Select(f => f.Attribute).ToArray();
            }

            return new Attribute[0];
        }

        /// <summary>   An IBase extension method that query if 'baseObject' has facet attribute. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   True if facet attribute, false if not. </returns>

        public static bool HasFacetAttribute<T>(this IBase baseObject) where T : Attribute
        {
            if (baseObject is IEntityWithOptionalFacets && baseObject.Facets.Length == 0)
            {
                return false;
            }
            else if (baseObject is IEntityWithFacets)
            {
                var entityWithFacets = (IEntityWithFacets)baseObject;
                return entityWithFacets.Facets.Any(f => f.Attribute.IsOfType<T>());
            }

            return false;
        }

        /// <summary>   A Facet[] extension method that gets user interface hierarchy path list. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="facets">       The facets to act on. </param>
        /// <param name="printMode">    The print mode. </param>
        ///
        /// <returns>   The user interface hierarchy path list. </returns>

        public static string GetUIHierarchyPathList(this Facet[] facets, PrintMode printMode)
        {
            return facets.Select(f => f.Attribute).OfType<UIAttribute>().Select(a =>
            {
                if (a.UILoadKind.IsOneOf(UILoadKind.RootPage, UILoadKind.MainPage))
                {
                    var facetTypes = facets.Select(f => f.GetType());
                    var nameAttributes = facets.Select(f => f.Attribute).OfType<UINavigationNameAttribute>().ToList();

                    if (nameAttributes.Count == 1)
                    {
                        return nameAttributes.Single().Name;
                    }
                    else
                    {
                        return nameAttributes.Single(n => n.UIKind == a.UIKind).Name;
                    }
                }
                else if (printMode.HasFlag(PrintMode.PrintUIHierarchyPathOnly) && a.PathRootAlias != null)
                {
                    return a.UIHierarchyPath + " = " + a.PathRootAlias;
                }
                else
                {
                    return a.UIHierarchyPath;
                }

            }).ToMultiLineList();
        }

        /// <summary>   An IBase extension method that gets user interface hierarchy paths. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   An array of string. </returns>

        public static string[] GetUIHierarchyPaths(this IBase baseObject)
        {
            var componentAttributes = baseObject.GetFacetAttributes<UIAttribute>();
            var navigableAttributes = baseObject.GetFacetAttributes<NavigableAttribute>();
            var paths = new List<string>();
            var facets = baseObject.Facets;

            if (componentAttributes.Count() > 0)
            {
                foreach (var componentAttribute in componentAttributes)
                {
                    if (componentAttribute.UILoadKind.IsOneOf(UILoadKind.RootPage, UILoadKind.MainPage))
                    {
                        var facetTypes = facets.Select(f => f.GetType());
                        var nameAttributes = facets.Select(f => f.Attribute).OfType<UINavigationNameAttribute>().ToList();

                        if (nameAttributes.Count == 1)
                        {
                            paths.Add(nameAttributes.Single().Name);
                        }
                        else
                        {
                            paths.Add(nameAttributes.Single(n => n.UIKind == componentAttribute.UIKind).Name);
                        }
                    }
                    else
                    {
                        paths.Add(componentAttribute.UIHierarchyPath);
                    }
                }

                return paths.ToArray();
            }
            else if (navigableAttributes.Count() > 0)
            {
                foreach (var navigableAttribute in navigableAttributes)
                {
                    paths.Add(navigableAttribute.UIHierarchyPath);
                }

                return paths.ToArray();
            }

            return new string[0];
        }

        /// <summary>   An IEnumerable&lt;HandlerStackItem&gt; extension method that pre process. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handlerStackItems">        The handlerStackItems to act on. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool PreProcess(this IEnumerable<HandlerStackItem> handlerStackItems, IBase baseObject, IGeneratorConfiguration generatorConfiguration, IViewLayoutHandler currentHandler)
        {
            foreach (var handlerStackItem in handlerStackItems)
            {
                foreach (var handler in handlerStackItem.ViewLayoutHandlers)
                {
                    if (!handler.PreProcess(baseObject, generatorConfiguration, currentHandler))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// An IEnumerable&lt;HandlerStackItem&gt; extension method that posts the process.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handlerStackItems">        The handlerStackItems to act on. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool PostProcess(this IEnumerable<HandlerStackItem> handlerStackItems, IBase baseObject, IGeneratorConfiguration generatorConfiguration, IViewLayoutHandler currentHandler)
        {
            foreach (var handlerStackItem in handlerStackItems)
            {
                foreach (var handler in handlerStackItem.ViewLayoutHandlers)
                {
                    if (!handler.PostProcess(baseObject, generatorConfiguration, currentHandler))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>   A HandlerStackItem extension method that logs a create. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="handlerStackItem"> The handlerStackItem to act on. </param>
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="currentPass">      The current pass. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        public static T LogCreate<T>(this HandlerStackItem handlerStackItem, string uiHierarchyPath, GeneratorPass currentPass) where T : HandlerStackItem
        {
            handlerStackItem.LogEvent(HandlerStackEvent.Created, uiHierarchyPath, currentPass);

            return (T) handlerStackItem;
        }

        /// <summary>   A HandlerStackItem extension method that logs a create. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handlerStackItem"> The handlerStackItem to act on. </param>
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="currentPass">      The current pass. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        public static HandlerStackItem LogCreate(this HandlerStackItem handlerStackItem, string uiHierarchyPath, GeneratorPass currentPass)
        {
            handlerStackItem.LogEvent(HandlerStackEvent.Created, uiHierarchyPath, currentPass);

            return handlerStackItem;
        }

        /// <summary>   An IEnumerable&lt;HandlerStackItem&gt; extension method that pre process. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handlerStackItems">        The handlerStackItems to act on. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool PreProcess(this IEnumerable<HandlerStackItem> handlerStackItems, IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            foreach (var handler in handlerStackItems.SelectMany(h => h.AsEnumerable()))
            {
                if (!handler.PreProcess(baseObject, generatorConfiguration, currentHandler))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>   An IBase extension method that no user interface configuration or follow. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool NoUIConfigOrFollow(this IBase baseObject)
        {
            var noUIConfigOrFollow = false;
             
            if (baseObject is IEntityWithOptionalFacets)
            {
                var entityWithOptionalFacets = (IEntityWithOptionalFacets)baseObject;

                noUIConfigOrFollow = entityWithOptionalFacets.NoUIOrConfig;

                if (entityWithOptionalFacets.FollowWithout)
                {
                    noUIConfigOrFollow = false;
                }
            }

            return noUIConfigOrFollow;
        }

        /// <summary>
        /// An IEnumerable&lt;HandlerStackItem&gt; extension method that posts the process.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handlerStackItems">        The handlerStackItems to act on. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool PostProcess(this IEnumerable<HandlerStackItem> handlerStackItems, IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            foreach (var handler in handlerStackItems.SelectMany(h => h.AsEnumerable()))
            {
                if (!handler.PostProcess(baseObject, generatorConfiguration, currentHandler))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>   Parse hierarchy path. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="componentAttribute">   The componentAttribute to act on. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>   A Queue&lt;IXPathPart&gt; </returns>

        public static Queue<IXPathPart> ParseHierarchyPath(this UIAttribute componentAttribute, PartsAliasResolver partsAliasResolver)
        {
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();
            var id = string.Empty;
            var path = componentAttribute.UIHierarchyPath;

            path = partsAliasResolver.Resolve(path);

            parser.Parse(path, builder);

            return builder.PartQueue;
        }

        /// <summary>   Parse hierarchy path. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="componentAttribute">   The componentAttribute to act on. </param>
        ///
        /// <returns>   A Queue&lt;IXPathPart&gt; </returns>

        public static Queue<IXPathPart> ParseHierarchyPath(this UIAttribute componentAttribute)
        {
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();
            var id = string.Empty;
            var path = componentAttribute.UIHierarchyPath;

            parser.Parse(path, builder);

            return builder.PartQueue;
        }

        /// <summary>   Parse hierarchy path. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="path">                 Full pathname of the file. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>   A Queue&lt;IXPathPart&gt; </returns>

        public static Queue<IXPathPart> ParseHierarchyPath(string path, PartsAliasResolver partsAliasResolver)
        {
            if (!path.IsNullWhiteSpaceOrEmpty())
            {
                var parser = new XPathParser<string>();
                var builder = new XPathStringBuilder();
                var id = string.Empty;

                path = partsAliasResolver.Resolve(path);

                parser.Parse(path, builder);

                return builder.PartQueue;
            }
            else
            {
                return new Queue<IXPathPart>();
            }
        }

        /// <summary>   Parse hierarchy path. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   A Queue&lt;IXPathPart&gt; </returns>

        public static Queue<IXPathPart> ParseHierarchyPath(string path)
        {
            if (!path.IsNullWhiteSpaceOrEmpty())
            {
                var parser = new XPathParser<string>();
                var builder = new XPathStringBuilder();
                var id = string.Empty;

                parser.Parse(path, builder);

                return builder.PartQueue;
            }
            else
            {
                return new Queue<IXPathPart>();
            }
        }

        /// <summary>   Enumerates split element parts in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="partsQueue">   The partsQueue to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process split element parts in this
        /// collection.
        /// </returns>

        public static IEnumerable<string> SplitElementParts(this Queue<IXPathPart> partsQueue)
        {
            foreach (var part in partsQueue.OfType<XPathElement>())
            {
                if (part is XPathElement)
                {
                    var element = (XPathElement)part;

                    yield return element.Text;

                    foreach (var predicate in element.Predicates)
                    {
                        yield return predicate.Text;
                    }
                }
                else
                {
                    DebugUtils.Break();
                }
            }
        }

        /// <summary>   Gets all types in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="assembly"> The assembly to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all types in this collection.
        /// </returns>

        public static IEnumerable<Type> GetAllTypes(this Assembly assembly)
        {
            var thisAssembly = Assembly.GetEntryAssembly();

            foreach (var type in assembly.GetTypes().Concat(assembly.GetExportedTypes()).Distinct())
            {
                yield return type;
            }

            foreach (var assemblyName in assembly.GetReferencedAssemblies())
            {
                var refAssembly = Assembly.Load(assemblyName);

                if (refAssembly.HasCustomAttribute<EntityMetadataSourceAssemblyAttribute>())
                {
                    foreach (var type in refAssembly.GetTypes())
                    {
                        yield return type;
                    }
                }
            }

            foreach (var type in thisAssembly.GetTypes().Concat(thisAssembly.GetExportedTypes()).Distinct())
            {
                yield return type;
            }

            foreach (var assemblyName in thisAssembly.GetReferencedAssemblies().Where(a => a.Name == "ApplicationGenerator.Interfaces"))
            {
                var refAssembly = Assembly.Load(assemblyName);

                foreach (var assemblyName2 in thisAssembly.GetReferencedAssemblies().Where(a => a.Name == "System.ComponentModel.DataAnnotations" || a.Name == "System"))
                {
                    var refAssembly2 = Assembly.Load(assemblyName2);

                    foreach (var type in refAssembly2.GetTypes().Concat(refAssembly2.GetExportedTypes()).Distinct())
                    {
                        yield return type;
                    }
                }

                foreach (var type in refAssembly.GetTypes().Concat(refAssembly.GetExportedTypes()).Distinct())
                {
                    yield return type;
                }
            }
        }

        /// <summary>   A Type extension method that query if 'type' is view layout handler type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">     The type to act on. </param>
        /// <param name="layout">   The layout. </param>
        ///
        /// <returns>   True if view layout handler type, false if not. </returns>

        public static bool IsViewLayoutHandlerType(this Type type, string layout)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<ViewLayoutHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ViewLayoutHandlerAttribute>();

                return Path.GetFileNameWithoutExtension(attribute.ViewLayout) == Path.GetFileNameWithoutExtension(layout);
            }

            return false;
        }

        /// <summary>   A Type extension method that query if this  is facet handler type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">                 The type to act on. </param>
        /// <param name="entityDefinitionKind"> The entityDefinitionKind to act on. </param>
        ///
        /// <returns>   True if facet handler type, false if not. </returns>

        public static bool IsFacetHandlerType(this Type type, DefinitionKind entityDefinitionKind)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<FacetHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<FacetHandlerAttribute>();

                if (entityDefinitionKind.Matches(attribute.Kind))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>   A Type extension method that query if this  is facet handler type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">                 The type to act on. </param>
        /// <param name="facetType">            Type of the facet. </param>
        /// <param name="entityDefinitionKind"> The entityDefinitionKind to act on. </param>
        /// <param name="componentKind">        The component kind. </param>
        ///
        /// <returns>   True if facet handler type, false if not. </returns>

        public static bool IsFacetHandlerType(this Type type, Type facetType, DefinitionKind entityDefinitionKind, Guid componentKind)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<FacetHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<FacetHandlerAttribute>();

                if (attribute.FacetType == facetType && (attribute.Kind == componentKind || entityDefinitionKind.Matches(attribute.Kind)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>   A Type extension method that query if this  is module kind type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="moduleKind">       An enum constant representing the module kind option. </param>
        /// <param name="definitionKind">   The definition kind. </param>
        /// <param name="featureKind">      The feature kind. </param>
        ///
        /// <returns>   True if module kind type, false if not. </returns>

        public static bool IsModuleKindType(this Type type, Enum moduleKind, DefinitionKind definitionKind, UIFeatureKind featureKind)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<ModuleKindHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ModuleKindHandlerAttribute>();

                return attribute.ModuleKind.Equals(moduleKind) && attribute.DefinitionKind == definitionKind && attribute.FeatureKind == featureKind;
            }

            return false;
        }

        /// <summary>   A Type extension method that query if this  is module kind type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="moduleKind">       An enum constant representing the module kind option. </param>
        /// <param name="definitionKind">   The definition kind. </param>
        /// <param name="loadKind">         The load kind. </param>
        /// <param name="uiKind">           The kind. </param>
        /// <param name="featureKind">      The feature kind. </param>
        ///
        /// <returns>   True if module kind type, false if not. </returns>

        public static bool IsModuleKindType(this Type type, Enum moduleKind, DefinitionKind definitionKind, UILoadKind loadKind, Guid uiKind, UIFeatureKind featureKind)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<ModuleKindHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ModuleKindHandlerAttribute>();

                if (attribute.ModuleKind.Equals(moduleKind) && attribute.DefinitionKind == definitionKind && attribute.FeatureKind == featureKind)
                {
                    return true;
                }
                else if (attribute.ModuleKind.Equals(moduleKind) && (attribute.UIKind == uiKind || ((attribute.DefinitionKind == DefinitionKind.NotApplicable || attribute.DefinitionKind == definitionKind) && attribute.FeatureKind == featureKind)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>   A DefinitionKind extension method that matches. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityDefinitionKind"> The entityDefinitionKind to act on. </param>
        /// <param name="componentKind">        The component kind. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool Matches(this DefinitionKind entityDefinitionKind, Guid componentKind)
        {
            if (componentKind == Guid.Parse(UIKindGuids.Any))
            {
                return true;
            }
            else
            {
                switch (entityDefinitionKind)
                {
                    case DefinitionKind.Class:
                    case DefinitionKind.Interface:
                    case DefinitionKind.Structure:
                        return componentKind == Guid.Parse(UIKindGuids.Element);
                    case DefinitionKind.StaticContainer:
                        return componentKind == Guid.Parse(UIKindGuids.StaticContainer);
                }
            }

            return false;
        }

        /// <summary>   A Type extension method that query if 'type' is validation handler type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="validationType">   Type of the validation. </param>
        ///
        /// <returns>   True if validation handler type, false if not. </returns>

        public static bool IsValidationHandlerType(this Type type, Type validationType)
        {
            if (type.HasCustomAttribute<ValidationHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ValidationHandlerAttribute>();

                return attribute.ValidationType == validationType;
            }

            return false;
        }

        /// <summary>   A Type extension method that query if 'type' is import handler type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">         The type to act on. </param>
        /// <param name="handlerId">    Identifier for the handler. </param>
        ///
        /// <returns>   True if import handler type, false if not. </returns>

        public static bool IsImportHandlerType(this Type type, ulong handlerId)
        {
            if (type.HasCustomAttribute<ImportHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ImportHandlerAttribute>();

                return attribute.HandlerId == handlerId;
            }

            return false;
        }
    }
}
