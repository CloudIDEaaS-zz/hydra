// file:	Extensions.cs
//
// summary:	Implements the extensions class

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
using Microsoft.Win32;
using System.Net;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Moq;
using static Utils.ControlExtensions;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System.Threading;
using System.Security;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utils.InProcessTransactions;
using System.Net.Http.Headers;
using System.Net.Http;
using AbstraX.DataModels;

namespace AbstraX
{
    /// <summary>   An extensions. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    public static class Extensions
    {
        public static Image imageNotStarted;
        public static Image imageInProcess;
        public static Image imageComplete;

        static Extensions()
        {
            var type = typeof(Extensions);

            imageNotStarted = type.ReadResource<Bitmap>(@"Images\WizardCompletion\CompletionStepIcons\NotStarted.png");
            imageInProcess = type.ReadResource<Bitmap>(@"Images\WizardCompletion\CompletionStepIcons\InProcess.png");
            imageComplete = type.ReadResource<Bitmap>(@"Images\WizardCompletion\CompletionStepIcons\Complete.png");
        }

        public static void ToNotStarted(this PictureBox pictureBox)
        {
            pictureBox.ChangeImage(imageNotStarted);
        }

        public static void ToInProcess(this PictureBox pictureBox)
        {
            pictureBox.ChangeImage(imageInProcess);
        }

        public static void ToComplete(this PictureBox pictureBox)
        {
            pictureBox.ChangeImage(imageComplete);
        }

        /// <summary>   An IEntityProperty extension method that query if 'property' is key. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="property"> The property to act on. </param>
        ///
        /// <returns>   True if key, false if not. </returns>

        public static bool IsKey(this IEntityProperty property)
        {
            return property.HasFacetAttribute<KeyAttribute>();
        }

        /// <summary>   An IGeneratorConfiguration extension method that gets resource data. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/19/2021. </remarks>
        ///
        /// <param name="config">   The config to act on. </param>
        ///
        /// <returns>   The resource data. </returns>

        public static IResourceData GetResourceData(this IGeneratorConfiguration config)
        {
            var assemblyLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var localThemeFolder = System.IO.Path.Combine(assemblyLocation, @"Themes");
            var localThemePath = System.IO.Path.Combine(localThemeFolder, @"variables.scss");
            var sassContent = System.IO.File.ReadAllText(localThemePath);
            var root = System.IO.Path.Combine(config.ProjectFolderRoot, "hydra_resources");
            ResourceData resourceData;

            using (var resourceManager = new ResourceManager(root, sassContent))
            {
                resourceData = (ResourceData)resourceManager.ResourceData;

                if (resourceData.AppName == null)
                {
                    resourceData.AppName = config.AppName;
                }

                if (resourceData.AppDescription == null)
                {
                    resourceData.AppDescription = config.AppDescription;
                }

                if (resourceData.OrganizationName == null)
                {
                    var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                    var organization = (string)key.GetValue("RegisteredOrganization");

                    if (organization != null)
                    {
                        resourceData.OrganizationName = organization;
                    }
                    else
                    {
                        organization = Dns.GetHostName();
                        resourceData.OrganizationName = organization;
                    }
                }
            }

            return resourceData;
        }

        public static void AddHydraResources(this IGeneratorConfiguration config)
        {
            var assemblyLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var localThemeFolder = System.IO.Path.Combine(assemblyLocation, @"Themes");
            var localThemePath = System.IO.Path.Combine(localThemeFolder, @"variables.scss");
            var sassContent = System.IO.File.ReadAllText(localThemePath);
            var resourceDefaults = AbstraXExtensions.GetResourceDefaults();
            string root;
            ResourceData resourceData;

            if (!config.KeyValuePairs.ContainsKey("ResourceData"))
            {
                root = System.IO.Path.Combine(config.ProjectFolderRoot, "hydra_resources");

                using (var resourceManager = new ResourceManager(root, sassContent))
                {
                    resourceData = (ResourceData)resourceManager.ResourceData;

                    if (resourceData.AppName == null)
                    {
                        resourceData.AppName = config.AppName;
                    }

                    if (resourceData.AppDescription == null)
                    {
                        resourceData.AppDescription = config.AppDescription;
                    }

                    if (resourceData.OrganizationName == null)
                    {
                        if (config.OrganizationName == null)
                        {
                            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                            var organization = (string)key.GetValue("RegisteredOrganization");

                            if (organization != null)
                            {
                                resourceData.OrganizationName = organization;
                            }
                            else
                            {
                                organization = Dns.GetHostName();
                                resourceData.OrganizationName = organization;
                            }
                        }
                        else
                        {
                            resourceData.OrganizationName = config.OrganizationName;
                        }
                    }

                    if (resourceData.AboutBanner == null)
                    {
                        resourceManager.AddFileResource(nameof(resourceData.AboutBanner), resourceDefaults.AboutBannerFile);
                    }

                    if (resourceData.Logo == null)
                    {
                        resourceManager.AddFileResource(nameof(resourceData.Logo), resourceDefaults.LogoFile);
                    }

                    if (resourceData.SplashScreen == null)
                    {
                        resourceManager.AddFileResource(nameof(resourceData.SplashScreen), resourceDefaults.SplashScreenFile);
                    }

                    resourceData.Save();
                }

                using (resourceData)
                {
                    var resourceDataClone = new ResourceDataClone(resourceData.RootPath, resourceData.ColorGroupsLookup, resourceData.PropertiesLookup, resourceData.ThemeToSelectors, resourceData.KeyValues);

                    config.KeyValuePairs.Add("ResourceData", resourceDataClone);
                }
            }
        }

        /// <summary>
        /// An AngularModule extension method that adds the imports and routes to 'imports'.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="module">   The module to act on. </param>
        /// <param name="imports">  The imports to act on. </param>

        public static void AddImportsAndRoutes(this AngularModule module, IEnumerable<ModuleImportDeclaration> imports, Page page = null)
        {
            if (page != null)
            {
                foreach (var import in imports.Select(i => i.ModuleOrAssembly).OfType<Page>().Where(p => p.Name == page.Name))
                {
                    module.Routes.AddRange(module.CreateRoutesTo(import));
                    module.Imports.Add(import);
                }
            }
            else
            {
                if (module.UIKind != UIKind.NotSpecified)
                {
                    Route route;

                    page = module.Declarations.OfType<Page>().Single();

                    if (module.BaseRoute != null)
                    {
                        var parts = module.BaseRoute.Split("/").Skip(2);
                        var path = string.Join("/", parts);

                        route = new Route
                        {
                            Path = path,
                            Component = page,
                        };
                    }
                    else
                    {
                        route = new Route
                        {
                            Path = "",
                            Component = page,
                        };
                    }

                    module.Routes.Add(route);

                    foreach (var import in imports.Select(i => i.ModuleOrAssembly).OfType<AngularModule>())
                    {
                        string code;

                        module.Imports.Add(import);

                        if (!import.Is<RoutingModule>())
                        {
                            route.Children.AddRange(module.CreateRoutesTo(import));
                        }

                        code = route.Code;
                    }
                }
                else
                {
                    foreach (var import in imports.Select(i => i.ModuleOrAssembly).OfType<AngularModule>())
                    {
                        if (!import.Is<RoutingModule>())
                        {
                            module.Routes.AddRange(module.CreateRoutesTo(import));
                        }

                        module.Imports.Add(import);
                    }
                }
            }
        }

        public static void AddRouteGuards(this AngularModule module, IGeneratorConfiguration configuration, IBase baseObject)
        {
            if (configuration.KeyValuePairs.ContainsKey("RouteGuards"))
            {
                var routeGuards = (Dictionary<string, List<RouteGuard>>)configuration.KeyValuePairs["RouteGuards"];
                var routeGuardList = routeGuards[baseObject.ID];

                if (routeGuardList != null)
                {
                    foreach (var routeGuard in routeGuardList)
                    {
                        module.RouteGuards.Add(routeGuard);
                    }
                }
            }
        }

        /// <summary>   A List&lt;Route&gt; extension method that gets routes array code. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="routes">   The routes to act on. </param>
        ///
        /// <returns>   The routes array code. </returns>

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

        /// <summary>   Enumerates create routes to in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="module">           The module to act on. </param>
        /// <param name="routeToModule">    The route to module. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process create routes to in this collection.
        /// </returns>

        public static IEnumerable<Route> CreateRoutesTo(this AngularModule module, AngularModule routeToModule)
        {
            var routes = new List<Route>();
            var path = Path.GetFileNameWithoutExtension(routeToModule.ForChildFile.FullName);
            Route route;
            string code;

            if (routeToModule.UILoadKind == UILoadKind.HomePage)
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
            else if (routeToModule.UILoadKind == UILoadKind.MainPage)
            {
                path = "app";
            }

            route = new Route
            {
                Path = path,
                LoadChildren = new LoadChildren(module, routeToModule),
            };

            if (routeToModule.RouteGuards != null)
            {
                foreach (var routeGuard in routeToModule.RouteGuards.OfType<RouteGuard>())
                {
                    if (routeGuard.Kind == RouteGuardKind.CanLoad)
                    {
                        route.CanLoad.Add(routeGuard);
                    }
                    else if (routeGuard.Kind == RouteGuardKind.CanActivate)
                    {
                        route.CanActivate.Add(routeGuard);
                    }
                    else
                    {
                        DebugUtils.Break();
                    }
                }
            }

            code = route.Code;
            routes.Add(route);

            return routes;
        }

        public static IEnumerable<Route> CreateRoutesTo(this AngularModule module, Page page)
        {
            var routes = new List<Route>();
            var path = Path.GetFileNameWithoutExtension(page.File.FullName);
            Route route;
            string code;

            route = new Route
            {
                Path = string.Empty,
                Component = page
            };

            code = route.Code;
            routes.Add(route);

            return routes;
        }

        /// <summary>
        /// An IEnumerable&lt;AbstraX.QueryInfo&gt; extension method that gets query for kind.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="queries">  The queries. </param>
        /// <param name="kind">     The kind. </param>
        ///
        /// <returns>   The query for kind. </returns>

        public static QueryInfo GetQueryForKind(this IEnumerable<AbstraX.QueryInfo> queries, QueryKind kind)
        {
            return queries.Single(q => q.QueryKind == kind);
        }

        /// <summary>
        /// An IEnumerable&lt;AbstraX.QueryInfo&gt; extension method that query if 'queries' has query
        /// for kind.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="queries">  The queries. </param>
        /// <param name="kind">     The kind. </param>
        ///
        /// <returns>   True if query for kind, false if not. </returns>

        public static bool HasQueryForKind(this IEnumerable<AbstraX.QueryInfo> queries, QueryKind kind)
        {
            if (queries == null)
            {
                return false;
            }

            return queries.Any(q => q.QueryKind == kind);
        }

        /// <summary>
        /// An IEnumerable&lt;ModuleImportDeclaration&gt; extension method that gets a component.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="imports">  The imports to act on. </param>
        ///
        /// <returns>   The component. </returns>

        //public static Module GetComponent(this IEnumerable<ModuleImportDeclaration> imports)
        //{
        //    var import = imports.Single(d => d.ModuleOrAssembly is Page);

        //    return (Module) import.ModuleOrAssembly;
        //}

        /// <summary>
        /// An IEnumerable&lt;ModuleImportDeclaration&gt; extension method that gets a component.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <param name="imports">  The imports to act on. </param>
        /// <param name="uiKind">   The kind. </param>
        ///
        /// <returns>   The component. </returns>

        public static Module GetComponent(this IEnumerable<ModuleImportDeclaration> imports, UIKind uiKind)
        {
            var import = imports.Single(d => d.ModuleOrAssembly is Page);

            return (Module)import.ModuleOrAssembly;
        }

        /// <summary>
        /// An IEnumerable&lt;ModuleImportDeclaration&gt; extension method that gets a component.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="imports">  The imports to act on. </param>
        /// <param name="loadKind"> The load kind. </param>
        ///
        /// <returns>   The component. </returns>

        public static Module GetComponent(this IEnumerable<ModuleImportDeclaration> imports, UILoadKind loadKind, UIKind uiKind)
        {
            var import = imports.Single(d => d.ModuleOrAssembly is Page && ((Page)d.ModuleOrAssembly).UILoadKind == loadKind && ((Page)d.ModuleOrAssembly).UIKind == uiKind);

            return (Module)import.ModuleOrAssembly;
        }

        /// <summary>
        /// A Dictionary&lt;string,object&gt; extension method that adds a module assembly properties to
        /// 'moduleAssemblyProperties'.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="sessionVariables">         The session variables. </param>
        /// <param name="moduleAssemblyProperties"> The module assembly properties. </param>

        public static void AddModuleAssemblyProperties(this Dictionary<string, object> sessionVariables, AngularModuleAssemblyProperties moduleAssemblyProperties)
        {
            sessionVariables.Add("Imports", moduleAssemblyProperties.Imports);
            sessionVariables.Add("Exports", moduleAssemblyProperties.Exports);
            sessionVariables.Add("Declarations", moduleAssemblyProperties.Declarations);
            sessionVariables.Add("Providers", moduleAssemblyProperties.Providers);
            sessionVariables.Add("Modules", moduleAssemblyProperties.Modules);
        }

        /// <summary>
        /// A Dictionary&lt;string,object&gt; extension method that gets exported components.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="sessionVariables"> The session variables. </param>
        ///
        /// <returns>   The exported components. </returns>

        public static List<Module> GetExportedComponents(this Dictionary<string, object> sessionVariables)
        {
            var exports = (List<ESModule>)sessionVariables["Exports"];

            return exports.Cast<Module>().ToList();
        }

        /// <summary>   A List&lt;Module&gt; extension method that adds to the file. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="modules">  The modules to act on. </param>
        /// <param name="file">     The file. </param>

        public static void AddToFile(this List<Module> modules, FolderStructure.File file)
        {
            foreach (var export in modules)
            {
                export.File = file;
            }
        }

        /// <summary>   A List&lt;Provider&gt; extension method that adds a file to 'file'. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="modules">  The modules to act on. </param>
        /// <param name="file">     The file. </param>

        public static void AddFile(this List<ESModule> modules, FolderStructure.File file)
        {
            foreach (var export in modules)
            {
                export.File = file;
            }
        }

        /// <summary>   A List&lt;Provider&gt; extension method that adds a file to 'file'. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="providers">    The providers to act on. </param>
        /// <param name="file">         The file. </param>

        public static void AddFile(this List<Provider> providers, FolderStructure.File file)
        {
            foreach (var provider in providers)
            {
                provider.File = file;
            }
        }

        /// <summary>   An IBase extension method that query if 'baseObject' is client identity. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="field">    The field to act on. </param>
        ///
        /// <returns>   True if client identity, false if not. </returns>

        public static bool IsClientIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Client);
        }

        /// <summary>
        /// A HandlerObjectBase extension method that query if 'field' is login identity.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="field">    The field to act on. </param>
        ///
        /// <returns>   True if login identity, false if not. </returns>

        public static bool IsLoginIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Login);
        }

        /// <summary>
        /// A HandlerObjectBase extension method that query if 'field' is register identity.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="field">    The field to act on. </param>
        ///
        /// <returns>   True if register identity, false if not. </returns>

        public static bool IsRegisterIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Register);
        }

        /// <summary>
        /// A HandlerObjectBase extension method that query if 'field' is server identity.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="field">    The field to act on. </param>
        ///
        /// <returns>   True if server identity, false if not. </returns>

        public static bool IsServerIdentity(this HandlerObjectBase field)
        {
            return field.HasIdentityCategoryFlag(IdentityFieldCategory.Server);
        }

        /// <summary>
        /// An IBase extension method that query if 'baseObject' is client identity.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        ///
        /// <returns>   True if client identity, false if not. </returns>

        public static bool IsClientIdentity(this IBase baseObject)
        {
            return baseObject.HasIdentityCategoryFlag(IdentityFieldCategory.Client);
        }

        /// <summary>   Gets the client identity properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="entityProperties"> The entity properties. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the client identity properties in
        /// this collection.
        /// </returns>

        public static IEnumerable<Generators.EntityProperty> GetClientIdentityProperties(this List<Generators.EntityProperty> entityProperties)
        {
            return entityProperties.Where(p => p.IsClientIdentity());
        }

        /// <summary>   Gets the login identity properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="entityProperties"> The entity properties. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the login identity properties in this
        /// collection.
        /// </returns>

        public static IEnumerable<Generators.EntityProperty> GetLoginIdentityProperties(this List<Generators.EntityProperty> entityProperties)
        {
            return entityProperties.Where(p => p.IsLoginIdentity());
        }

        /// <summary>   Gets the register identity properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="entityProperties"> The entity properties. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the register identity properties in
        /// this collection.
        /// </returns>

        public static IEnumerable<Generators.EntityProperty> GetRegisterIdentityProperties(this List<Generators.EntityProperty> entityProperties)
        {
            return entityProperties.Where(p => p.IsRegisterIdentity());
        }

        /// <summary>   Gets the server identity properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="entityProperties"> The entity properties. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the server identity properties in
        /// this collection.
        /// </returns>

        public static IEnumerable<Generators.EntityProperty> GetServerIdentityProperties(this List<Generators.EntityProperty> entityProperties)
        {
            return entityProperties.Where(p => p.IsServerIdentity());
        }

        /// <summary>
        /// An IBase extension method that query if 'baseObject' has identity category flag.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="field">                        The field to act on. </param>
        /// <param name="identityFieldCategoryFlag">    The identity field category flag. </param>
        ///
        /// <returns>   True if identity category flag, false if not. </returns>

        public static bool HasIdentityCategoryFlag(this HandlerObjectBase field, IdentityFieldCategory identityFieldCategoryFlag)
        {
            return field.BaseObject.HasIdentityCategoryFlag(identityFieldCategoryFlag);
        }

        /// <summary>
        /// An IBase extension method that query if 'baseObject' has identity category flag.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="baseObject">                   The baseObject to act on. </param>
        /// <param name="identityFieldCategoryFlag">    The identity field category flag. </param>
        ///
        /// <returns>   True if identity category flag, false if not. </returns>

        public static bool HasIdentityCategoryFlag(this IBase baseObject, IdentityFieldCategory identityFieldCategoryFlag)
        {
            var identityFieldAttribute = baseObject.GetFacetAttribute<IdentityFieldAttribute>();

            if (identityFieldAttribute != null)
            {
                var identityFieldKind = identityFieldAttribute.IdentityFieldKind;
                var identityFieldCategoryAttribute = identityFieldKind.GetIdentityFieldCategoryAttribute();
                var identityFieldCategory = identityFieldCategoryAttribute.IdentityFieldCategoryFlags;

                return identityFieldCategory.HasAnyFlag(identityFieldCategoryFlag);
            }

            return false;
        }

        /// <summary>
        /// A List&lt;Generators.EntityProperty&gt; extension method that gets identity field.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="fields">   The fields to act on. </param>
        /// <param name="kind">     The kind. </param>
        ///
        /// <returns>   The identity field. </returns>

        public static IdentityField GetIdentityField(this List<IdentityField> fields, IdentityFieldKind kind)
        {
            return fields.Single(f => f.BaseObject.HasFacetAttribute<IdentityFieldAttribute>() && f.BaseObject.GetFacetAttribute<IdentityFieldAttribute>().IdentityFieldKind == kind);
        }

        /// <summary>
        /// A List&lt;Generators.EntityProperty&gt; extension method that gets identity field.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="fields">   The fields to act on. </param>
        /// <param name="kind">     The kind. </param>
        ///
        /// <returns>   The identity field. </returns>

        public static FormField GetIdentityField(this List<FormField> fields, IdentityFieldKind kind)
        {
            return fields.Single(f => f.BaseObject.HasFacetAttribute<IdentityFieldAttribute>() && f.BaseObject.GetFacetAttribute<IdentityFieldAttribute>().IdentityFieldKind == kind);
        }

        /// <summary>
        /// A List&lt;Generators.EntityProperty&gt; extension method that gets identity field.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="entityProperties"> The entity properties. </param>
        /// <param name="kind">             The kind. </param>
        ///
        /// <returns>   The identity field. </returns>

        public static Generators.EntityProperty GetIdentityField(this List<Generators.EntityProperty> entityProperties, IdentityFieldKind kind)
        {
            return entityProperties.Single(f => f.BaseObject.HasFacetAttribute<IdentityFieldAttribute>() && f.BaseObject.GetFacetAttribute<IdentityFieldAttribute>().IdentityFieldKind == kind);
        }

        /// <summary>
        /// A Generators.EntityProperty extension method that query if 'entityProperty' has identity
        /// field.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="entityProperty">   The entity property. </param>
        ///
        /// <returns>   True if identity field, false if not. </returns>

        public static bool HasIdentityField(this Generators.EntityProperty entityProperty)
        {
            return entityProperty.BaseObject.HasFacetAttribute<IdentityFieldAttribute>();
        }

        /// <summary>   Gets the displayed in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="fields">   The fields to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the displayed in this collection.
        /// </returns>

        public static IEnumerable<FormField> GetDisplayed(this List<FormField> fields)
        {
            return fields.Where(f => !f.BaseObject.HasFacetAttribute<KeyAttribute>() && !f.BaseObject.HasFacetAttribute<ScaffoldColumnAttribute>());
        }

        /// <summary>   Gets the key or displayed in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="fields">   The fields to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the key or displayed in this
        /// collection.
        /// </returns>

        public static IEnumerable<FormField> GetKeyOrDisplayed(this List<FormField> fields)
        {
            return fields.Where(f => f.BaseObject.HasFacetAttribute<KeyAttribute>() || !f.BaseObject.HasFacetAttribute<ScaffoldColumnAttribute>());
        }

        /// <summary>   A List&lt;GridColumn&gt; extension method that gets a key. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="fields">   The fields to act on. </param>
        ///
        /// <returns>   The key. </returns>

        public static FormField GetKey(this List<FormField> fields)
        {
            return fields.Single(f => f.BaseObject.HasFacetAttribute<KeyAttribute>());
        }

        /// <summary>   Gets the masked in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="fields">   The fields to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the masked in this collection.
        /// </returns>

        public static IEnumerable<FormField> GetMasked(this List<FormField> fields)
        {
            return fields.GetDisplayed().Where(f => f.ValidationSet.PreferredValidationMask != null);
        }

        /// <summary>   A List&lt;GridColumn&gt; extension method that gets text identifier. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="gridColumns">  The gridColumns to act on. </param>
        ///
        /// <returns>   The text identifier. </returns>

        public static GridColumn GetTextIdentifier(this List<GridColumn> gridColumns)
        {
            return gridColumns.Single(c => c.IsTextIdentifier);
        }

        /// <summary>   A List&lt;GridColumn&gt; extension method that gets a key. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="gridColumns">  The gridColumns to act on. </param>
        ///
        /// <returns>   The key. </returns>

        public static GridColumn GetKey(this List<GridColumn> gridColumns)
        {
            return gridColumns.Single(c => c.IsKey);
        }

        /// <summary>   An int extension method that creates test string of length. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="length">   The length to act on. </param>
        ///
        /// <returns>   The new test string of length. </returns>

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

        /// <summary>
        /// An IGeneratorConfiguration extension method that generates an information.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="generatorConfiguration">   The generatorConfiguration to act on. </param>
        /// <param name="sessionVariables">         The session variables. </param>
        /// <param name="fileType">                 (Optional) Type of the file. </param>
        ///
        /// <returns>   The information. </returns>

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
