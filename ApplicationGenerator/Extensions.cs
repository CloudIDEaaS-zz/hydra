using AbstraX.Angular;
using AbstraX.Generators;
using AbstraX.Generators.Pages.RepeaterPage;
using AbstraX.Generators.Pages.EditPopupPage;
using AbstraX.Generators.Pages.NavigationGridPage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using AbstraX.DataAnnotations;
using AbstraX.Generators.Base;
using AbstraX.ServerInterfaces;
using AbstraX.Angular.Routes;
using System.IO;
using AbstraX.Models.Interfaces;
using System.Reflection;
using System.Configuration;
using AbstraX.Handlers.CommandHandlers;

namespace AbstraX
{
    public static class Extensions
    {
        public static IEnumerable<KeyValuePair<string, IGeneratorOverrides>> GetOverrides(this IGeneratorEngine engine)
        {
            return GetOverrides();
        }

        public static IEnumerable<KeyValuePair<string, IGeneratorOverrides>> GetOverrides(this GeneratorHandler handler)
        {
            return GetOverrides();
        }

        public static IEnumerable<KeyValuePair<string, IGeneratorOverrides>> GetOverrides()
        {
            var overrides = (List<Override>)ConfigurationManager.GetSection("generatorOverridesSection");

            foreach (var _override in overrides)
            {
                var overrideAssembly = _override.Assembly;
                var types = Assembly.Load(overrideAssembly.AssemblyName).GetTypes().Where(t => t.Implements<IGeneratorOverrides>());

                foreach (var type in types)
                {
                    var generatorOverride = (IGeneratorOverrides)Activator.CreateInstance(type);

                    yield return new KeyValuePair<string, IGeneratorOverrides>(_override.ArgumentsKind, generatorOverride);
                }
            }
        }

        public static bool IsKey(this IEntityProperty property)
        {
            return property.HasFacetAttribute<KeyAttribute>();
        }

        public static void AddImportsAndRoutes(this AngularModule module, IEnumerable<ModuleImportDeclaration> imports)
        {
            foreach (var import in imports.Select(i => i.ModuleOrAssembly).OfType<AngularModule>())
            {
                module.Routes.AddRange(module.CreateRoutesTo(import));
                module.Imports.Add(import);
            }
        }

        public static string GetRoutesArrayCode(this List<Route> routes)
        {
            var builder = new StringBuilder();

            builder.AppendLine("[");

            foreach (var route in routes)
            {
                builder.AppendLineFormat("{0},", route.Code);
            }

            builder.Append("]");

            return builder.ToString();
        }

        public static IEnumerable<Route> CreateRoutesTo(this AngularModule module, AngularModule routeToModule)
        {
            var routes = new List<Route>();
            var path = Path.GetFileNameWithoutExtension(routeToModule.ForChildFile.FullName);
            Route route;
            string code;

            if (routeToModule.UILoadKind == UILoadKind.RootPage)
            {
                route = new Route
                {
                    Path = string.Empty,
                    RedirectTo = path,
                    PathMatch = "full"
                };

                code = route.Code;
                routes.Add(route);
            }

            route = new Route
            {
                Path = path,
                LoadChildren = new LoadChildren(module, routeToModule)
            };

            code = route.Code;
            routes.Add(route);

            return routes;
        }

        public static QueryInfo GetQueryForKind(this IEnumerable<AbstraX.QueryInfo> queries, QueryKind kind)
        {
            return queries.Single(q => q.QueryKind == kind);
        }

        public static bool HasQueryForKind(this IEnumerable<AbstraX.QueryInfo> queries, QueryKind kind)
        {
            if (queries == null)
            {
                return false;
            }

            return queries.Any(q => q.QueryKind == kind);
        }

        public static Module GetComponent(this IEnumerable<ModuleImportDeclaration> imports)
        {
            var import = imports.Single(d => d.ModuleOrAssembly is Page);

            return (Module) import.ModuleOrAssembly;
        }

        public static Module GetComponent(this IEnumerable<ModuleImportDeclaration> imports, UILoadKind loadKind)
        {
            var import = imports.Single(d => d.ModuleOrAssembly is Page && ((Page) d.ModuleOrAssembly).UILoadKind == loadKind);

            return (Module)import.ModuleOrAssembly;
        }

        public static void AddModuleAssemblyProperties(this Dictionary<string, object> sessionVariables, AngularModuleAssemblyProperties moduleAssemblyProperties)
        {
            sessionVariables.Add("Imports", moduleAssemblyProperties.Imports);
            sessionVariables.Add("Exports", moduleAssemblyProperties.Exports);
            sessionVariables.Add("Declarations", moduleAssemblyProperties.Declarations);
            sessionVariables.Add("Providers", moduleAssemblyProperties.Providers);
            sessionVariables.Add("Modules", moduleAssemblyProperties.Modules);
        }

        public static List<Module> GetExportedComponents(this Dictionary<string, object> sessionVariables)
        {
            var exports = (List<ESModule>) sessionVariables["Exports"];

            return exports.Cast<Module>().ToList();
        }

        public static void AddToFile(this List<Module> modules, FolderStructure.File file)
        {
            foreach (var export in modules)
            {
                export.File = file;
            }
        }

        public static void AddFile(this List<ESModule> modules, FolderStructure.File file)
        {
            foreach (var export in modules)
            {
                export.File = file;
            }
        }

        public static void AddFile(this List<Provider> providers, FolderStructure.File file)
        {
            foreach (var provider in providers)
            {
                provider.File = file;
            }
        }

        public static bool IsClientIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Client);
        }

        public static bool IsLoginIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Login);
        }

        public static bool IsRegisterIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Register);
        }

        public static bool IsServerIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Server);
        }

        public static bool IsClientIdentity(this IBase baseObject)
        {
            return baseObject.HasIdentityCategoryFlag(IdentityFieldCategory.Client);
        }

        public static IEnumerable<Generators.EntityProperty> GetClientIdentityProperties(this List<Generators.EntityProperty> entityProperties)
        {
            return entityProperties.Where(p => p.IsClientIdentity());
        }

        public static IEnumerable<Generators.EntityProperty> GetLoginIdentityProperties(this List<Generators.EntityProperty> entityProperties)
        {
            return entityProperties.Where(p => p.IsLoginIdentity());
        }

        public static IEnumerable<Generators.EntityProperty> GetRegisterIdentityProperties(this List<Generators.EntityProperty> entityProperties)
        {
            return entityProperties.Where(p => p.IsRegisterIdentity());
        }

        public static IEnumerable<Generators.EntityProperty> GetServerIdentityProperties(this List<Generators.EntityProperty> entityProperties)
        {
            return entityProperties.Where(p => p.IsServerIdentity());
        }

        public static bool HasIdentityCategoryFlag(this HandlerObjectBase field, IdentityFieldCategory identityFieldCategoryFlag)
        {
            return field.BaseObject.HasIdentityCategoryFlag(identityFieldCategoryFlag);
        }

        public static bool HasIdentityCategoryFlag(this IBase baseObject, IdentityFieldCategory identityFieldCategoryFlag)
        {
            var identityFieldAttribute = baseObject.GetFacetAttribute<IdentityFieldAttribute>();
            var identityFieldKind = identityFieldAttribute.IdentityFieldKind;
            var identityFieldCategoryAttribute = identityFieldKind.GetIdentityFieldCategoryAttribute();
            var identityFieldCategory = identityFieldCategoryAttribute.IdentityFieldCategoryFlags;

            return identityFieldCategory.HasAnyFlag(identityFieldCategoryFlag);
        }

        public static IdentityField GetIdentityField(this List<IdentityField> fields, IdentityFieldKind kind)
        {
            return fields.Single(f => f.BaseObject.HasFacetAttribute<IdentityFieldAttribute>() && f.BaseObject.GetFacetAttribute<IdentityFieldAttribute>().IdentityFieldKind == kind);
        }

        public static FormField GetIdentityField(this List<FormField> fields, IdentityFieldKind kind)
        {
            return fields.Single(f => f.BaseObject.HasFacetAttribute<IdentityFieldAttribute>() && f.BaseObject.GetFacetAttribute<IdentityFieldAttribute>().IdentityFieldKind == kind);
        }

        public static Generators.EntityProperty GetIdentityField(this List<Generators.EntityProperty> entityProperties, IdentityFieldKind kind)
        {
            return entityProperties.Single(f => f.BaseObject.HasFacetAttribute<IdentityFieldAttribute>() && f.BaseObject.GetFacetAttribute<IdentityFieldAttribute>().IdentityFieldKind == kind);
        }

        public static bool HasIdentityField(this Generators.EntityProperty entityProperty)
        {
            return entityProperty.BaseObject.HasFacetAttribute<IdentityFieldAttribute>();
        }

        public static IEnumerable<FormField> GetDisplayed(this List<FormField> fields)
        {
            return fields.Where(f => !f.BaseObject.HasFacetAttribute<KeyAttribute>() && !f.BaseObject.HasFacetAttribute<ScaffoldColumnAttribute>());
        }

        public static IEnumerable<FormField> GetKeyOrDisplayed(this List<FormField> fields)
        {
            return fields.Where(f => f.BaseObject.HasFacetAttribute<KeyAttribute>() || !f.BaseObject.HasFacetAttribute<ScaffoldColumnAttribute>());
        }

        public static FormField GetKey(this List<FormField> fields)
        {
            return fields.Single(f => f.BaseObject.HasFacetAttribute<KeyAttribute>());
        }

        public static IEnumerable<FormField> GetMasked(this List<FormField> fields)
        {
            return fields.GetDisplayed().Where(f => f.ValidationSet.PreferredValidationMask != null);
        }

        public static GridColumn GetTextIdentifier(this List<GridColumn> gridColumns)
        {
            return gridColumns.Single(c => c.IsTextIdentifier);
        }

        public static GridColumn GetKey(this List<GridColumn> gridColumns)
        {
            return gridColumns.Single(c => c.IsKey);
        }

        public static string CreateTestStringOfLength(this int length)
        {
            var builder = new StringBuilder();

            1.Loop(length, (n) =>
            {
                var value = n % 10;

                if (n == 1)
                {
                    builder.Append("\"");
                }

                if (value == 0)
                {
                    builder.AppendFormat("0\" /* {0} */ + \"", n);
                }
                else
                {
                    builder.Append(value);
                }
            });

            builder.Append("\"");

            return builder.ToString();
        }
        
        public static StringBuilder GenerateInfo(this IGeneratorConfiguration generatorConfiguration, Dictionary<string, object> sessionVariables, string fileType = null)
        {
            var builder = new StringBuilder();

            builder.AppendLineFormat("FileType: \"{0}\"", fileType);

            if (sessionVariables.ContainsKey("Output"))
            {
                var output = (string)sessionVariables["Output"];

                builder.Append(output);
            }
            else
            {
                if (sessionVariables.ContainsKey("PageName"))
                {
                    var pageName = (string)sessionVariables["PageName"];

                    builder.AppendLineFormat("PageName: \"{0}\"", pageName);
                }

                if (sessionVariables.ContainsKey("Imports"))
                {
                    var imports = (IEnumerable<ModuleImportDeclaration>)sessionVariables["Imports"];

                    builder.AppendLine("-".Repeat(255));

                    foreach (var import in imports)
                    {
                        builder.AppendLineSpaceIndent(2, import.DeclarationCode);
                    }

                    if (sessionVariables.ContainsKey("Exports"))
                    {
                        var exports = (IEnumerable<Module>)sessionVariables["Exports"];

                        builder.AppendLine();

                        foreach (var export in exports)
                        {
                            builder.AppendLine(export.GetDummyCode(2));
                        }
                    }

                    builder.AppendLine("-".Repeat(255));
                }
            }

            return builder;
        }
    }
}
