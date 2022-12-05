using AbstraX.Angular;
using AbstraX.Angular.Routes;
using AbstraX.DataAnnotations;
using AbstraX.Generators.Client.EntityProvider;
using AbstraX.Generators.Client.RouteGuardProvider;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Client.EntityProvider
{
    [GeneratorTemplate("Class", @"Generators\Client\RouteGuardProvider\RouteGuardProviderClassTemplate.tt")]
    public static class RouteGuardProviderGenerator
    {
        public static void GenerateProvider(IBase baseObject, string providersFolderPath, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var uiNavigationNameAttributes = baseObject.GetFacetAttributes<UINavigationNameAttribute>();
            var uiPathTree = generatorConfiguration.GetUIPathTree();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;
            string routeName = null;
            string defaultRouteVariable = null;
            bool checkStorage = false;
            bool isHome = false;
            RouteGuardKind routeGuardKind = RouteGuardKind.CanLoad;
            string storageVariable = null;
            Action<UINavigationNameAttribute> generate;
            Dictionary<string, List<RouteGuard>> routeGuards;

            if (generatorConfiguration.KeyValuePairs.ContainsKey("RouteGuards"))
            {
                routeGuards = (Dictionary<string, List<RouteGuard>>) generatorConfiguration.KeyValuePairs["RouteGuards"];
            }
            else
            {
                routeGuards = new Dictionary<string, List<RouteGuard>>();

                generatorConfiguration.KeyValuePairs.Add("RouteGuards", routeGuards);
            }

            generate = new Action<UINavigationNameAttribute>((a) =>
            {
                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("RouteName", routeName);
                sessionVariables.Add("DefaultRouteVariable", defaultRouteVariable);
                sessionVariables.Add("CheckStorage", checkStorage);
                sessionVariables.Add("StorageVariable", storageVariable);

                fileLocation = providersFolderPath;
                filePath = PathCombine(fileLocation, "check-" + routeName.ToLower() + ".provider.ts");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(RouteGuardProviderGenerator), "Class");

                do
                {
                    var routeGuard = new RouteGuard(baseObject, a, routeName, defaultRouteVariable, routeGuardKind, checkStorage, storageVariable, isHome, routeGuards);
                    FolderStructure.File file;

                    output = host.Generate<RouteGuardProviderClassTemplate>(sessionVariables, false);

                    file = generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Route Guard Provider Class"));

                    routeGuard.File = file;

                    routeGuards.AddToDictionaryListCreateIfNotExist(baseObject.ID, routeGuard);
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);
            });

            try
            {
                // Route guard provider
                
                foreach (var uiNavigationNameAttribute in uiNavigationNameAttributes)
                {
                    routeName = null;
                    defaultRouteVariable = null;
                    checkStorage = false;
                    storageVariable = null;
                    isHome = false;

                    if (uiNavigationNameAttribute.UILoadKind == UILoadKind.HomePage)
                    {
                        routeName = uiNavigationNameAttribute.Name;
                        checkStorage = true;
                        storageVariable = "ion_did_homepage";
                        isHome = true;
                        routeGuardKind = RouteGuardKind.CanLoad;

                        defaultRouteVariable = "defaultroute";

                        generate(uiNavigationNameAttribute);
                    }
                }
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
