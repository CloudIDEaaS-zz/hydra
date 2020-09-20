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

namespace AbstraX
{
    public static class Extensions
    {
        public static string GetOverrideId(this GetOverridesEventHandler handler, IBase baseObject, string predicate, string generatedId)
        {
            var overrideEventHandler = handler;
            var args = new GetOverridesEventArgs();
            IGeneratorOverrides generatorOverride;

            overrideEventHandler(baseObject, args);
            generatorOverride = args.Overrides.Select(p => p.Value).Last();

            return generatorOverride.GetOverrideId(baseObject, predicate, generatedId);
        }

        public static bool SkipProcess(this GetOverridesEventHandler handler, IFacetHandler facetHandler, IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var overrideEventHandler = handler;
            var args = new GetOverridesEventArgs();
            IGeneratorOverrides generatorOverride;

            overrideEventHandler(baseObject, args);
            generatorOverride = args.Overrides.Select(p => p.Value).Last();

            return generatorOverride.SkipProcess(facetHandler, baseObject, facet, generatorConfiguration);
        }

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

        public static bool ContainsInstallKey(this Dictionary<string, PackageWorkingInstallFromCache> dependencies, PackageWorkingInstallFromCache root, string install)
        {
            return dependencies.Concat(new List<KeyValuePair<string, PackageWorkingInstallFromCache>>() { new KeyValuePair<string, PackageWorkingInstallFromCache>(root.Install, root) }).Where(p => !p.Value.InstallToLocalModules).ToDictionary().ContainsKey(install);
        }

        public static PackageInstallFromCacheStatus CreateStatus(this PackageWorkingInstallFromCache installFromCache, string status, StatusMode mode)
        {
            return new PackageInstallFromCacheStatus(installFromCache.Install, installFromCache.CachePath, installFromCache.PackagePath, status, mode);
        }

        public static PackageInstallFromCacheStatus CreateStatus(this PackageWorkingInstallFromCache installFromCache, Exception ex)
        {
            return new PackageInstallFromCacheStatus(installFromCache.Install, installFromCache.CachePath, installFromCache.PackagePath, ex.Message, StatusMode.Error);
        }

        public static string FixDots(this string path)
        {
            return path.RegexReplace(@"^\.\./", "./");
        }

        public static float ToDecimalSeconds(this TimeSpan timeSpan)
        {
            return timeSpan.GetDecimalTimeComponent((t) => t.Milliseconds, 2);
        }

        public static UIFeatureKind GetFeatureKind(this UIAttribute uIAttribute)
        {
            var uiKind = uIAttribute.UIKind;
            var field = EnumUtils.GetField<UIKind>(uiKind);
            var kindGuidAttribute = field.GetCustomAttribute<KindGuidAttribute>();

            return kindGuidAttribute.FeatureKind;
        }

        public static bool IsComponent(this UIFeatureKind uiFeatureKind)
        {
            return uiFeatureKind.IsOneOf(UIFeatureKind.CustomComponent, UIFeatureKind.StandardComponent);
        }

        public static string GetText(this XPathOperator _operator)
        {
            return XPathElement.GetCSharpOperatorString(_operator);
        }

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

        public static IAttribute GetKey(this IElement element)
        {
            if (element is IElementWithKeyAttribute)
            {
                var elementWithKeyAttribute = (IElementWithKeyAttribute)element;

                return elementWithKeyAttribute.GetKeyAttribute();
            }

            return element.Attributes.Single(a => a.HasFacetAttribute<KeyAttribute>());
        }

        public static void ClearDeclarations(this List<IBuiltInImportHandler> builtInImportHandlerList)
        {
            builtInImportHandlerList.ForEach(b => b.ClearDeclarations());
        }

        public static IDictionary<string, IEnumerable<ModuleImportDeclaration>> Exclude(this IDictionary<string, IEnumerable<ModuleImportDeclaration>> importGroups, string moduleName)
        {
            return importGroups.Where(g => !g.Value.Any(m => m.ModuleNames.Any(n => n == moduleName))).ToDictionary(g => g.Key, g => g.Value);
        }

        public static IEnumerable<ModuleImportDeclaration> Exclude(this IEnumerable<ModuleImportDeclaration> importDeclarations, string moduleName)
        {
            return importDeclarations.Where(m => !m.ModuleNames.Any(n => n == moduleName));
        }

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

        public static bool IsKey(this IAttribute attribute)
        {
            return attribute.HasFacetAttribute<KeyAttribute>();
        }

        public static ModuleImportDeclarationAttribute GetModuleImportDeclarationAttribute(this Enum moduleId)
        {
            var field = EnumUtils.GetField(moduleId);

            return field.GetCustomAttribute<ModuleImportDeclarationAttribute>();
        }

        public static bool HasIdentityFieldCategoryAttribute(this IdentityFieldKind userFieldKind)
        {
            var field = EnumUtils.GetField(userFieldKind);

            return field.HasCustomAttribute<IdentityFieldCategoryAttribute>();
        }

        public static IdentityFieldCategoryAttribute GetIdentityFieldCategoryAttribute(this IdentityFieldKind userFieldKind)
        {
            var field = EnumUtils.GetField(userFieldKind);

            return field.GetCustomAttribute<IdentityFieldCategoryAttribute>();
        }

        public static string GetScriptTypeName(this IAttribute attribute)
        {
            var shortTypeName = attribute.DataType.UnderlyingType.GetShortName();
            var mappings = TypeMapper.Mappings["ScriptTypes"];
            var entry = mappings.Map[shortTypeName];

            return entry.MappedToType;
        }

        public static string GetDotNetTypeName(this IAttribute attribute)
        {
            var shortTypeName = attribute.DataType.UnderlyingType.GetShortName();

            return shortTypeName;
        }

        public static string GetShortType(this IAttribute attribute)
        {
            return attribute.DataType.UnderlyingType.GetShortName();
        }

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

        public static string ToConstantName(this string value, string suffix = "")
        {
            return value.Replace(" ", "_").ToUpper() + suffix;
        }

        public static AbstraXVisitor Visit(this Expression node)
        {
            var visitor = new AbstraXVisitor();

            visitor.Visit(node);

            return visitor;
        }

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

        public static IEnumerable<IBase> GetFollowingDescendants(this IBase baseObject, PartsAliasResolver partsAliasResolver)
        {
            var descendants = new List<IBase>();

            baseObject.GetDescendants(obj => obj.GetFollowingChildren(partsAliasResolver), obj =>
            {
                descendants.Add(obj);
            });

            return descendants;
        }

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

        public static QueryPathQueue GetQueryPathQueue(this QueryPathAttribute loadParentPath, IBase baseObject)
        {
            return new QueryPathQueue(loadParentPath, baseObject);
        }

        public static IEntitySet GetContainerSet(this IBase baseObject)
        {
            var container = baseObject.GetContainer();

            return container.EntitySets.Single(s => s.ChildElements.Single().Name == baseObject.Name);
        }

        public static void Raise(this ProcessFacetsHandler handler, object sender, params Type[] types)
        {
            if (handler != null)
            {
                handler.Invoke(sender, new ProcessFacetsEventArgs(types));
            }
        }

        public static void Raise<T>(this ProcessFacetsHandler handler, object sender)
        {
            if (handler != null)
            {
                handler.Invoke(sender, new ProcessFacetsEventArgs(new Type[] { typeof(T) }));
            }
        }

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

        public static IEnumerable<IBase> WithFacets<T>(this IEnumerable<IBase> baseObjects) where T : Attribute
        {
            return baseObjects.Where(o => o.HasFacetAttribute<T>());
        }

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

        public static Attribute[] GetValidationAttributes(this IBase baseObject)
        {
            if (baseObject is IEntityWithFacets)
            {
                var entityWithFacets = (IEntityWithFacets)baseObject;
                return entityWithFacets.Facets.Where(f => f.Attribute.GetType().IsOneOf<ValidationAttribute, DataTypeAttribute>()).Select(f => f.Attribute).ToArray();
            }

            return new Attribute[0];
        }

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

        public static T LogCreate<T>(this HandlerStackItem handlerStackItem, string uiHierarchyPath, GeneratorPass currentPass) where T : HandlerStackItem
        {
            handlerStackItem.LogEvent(HandlerStackEvent.Created, uiHierarchyPath, currentPass);

            return (T) handlerStackItem;
        }


        public static HandlerStackItem LogCreate(this HandlerStackItem handlerStackItem, string uiHierarchyPath, GeneratorPass currentPass)
        {
            handlerStackItem.LogEvent(HandlerStackEvent.Created, uiHierarchyPath, currentPass);

            return handlerStackItem;
        }

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

        public static Queue<IXPathPart> ParseHierarchyPath(this UIAttribute componentAttribute)
        {
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();
            var id = string.Empty;
            var path = componentAttribute.UIHierarchyPath;

            parser.Parse(path, builder);

            return builder.PartQueue;
        }

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

        public static IEnumerable<Type> GetAllTypes(this Assembly assembly)
        {
            var thisAssembly = Assembly.GetEntryAssembly();

            foreach (var type in assembly.GetTypes())
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

            foreach (var type in thisAssembly.GetTypes())
            {
                yield return type;
            }

            foreach (var assemblyName in thisAssembly.GetReferencedAssemblies().Where(a => a.Name == "ApplicationGenerator.Interfaces"))
            {
                var refAssembly = Assembly.Load(assemblyName);

                foreach (var type in refAssembly.GetTypes())
                {
                    yield return type;
                }
            }
        }

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

        public static bool IsValidationHandlerType(this Type type, Type validationType)
        {
            if (type.HasCustomAttribute<ValidationHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ValidationHandlerAttribute>();

                return attribute.ValidationType == validationType;
            }

            return false;
        }

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
