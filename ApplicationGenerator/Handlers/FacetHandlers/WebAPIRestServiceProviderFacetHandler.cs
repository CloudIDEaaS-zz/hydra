using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Server.WebAPIRestServiceProvider;
using AbstraX.Handlers.ExpressionHandlers;
using AbstraX.ServerInterfaces;
using ApplicationGenerator.Data;
using EntityProvider.Web.Entities;
using RestEntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.FacetHandlers
{
    public class WebAPIRestServiceProviderFacetHandler : ISingletonForLifeFacetHandler
    {
        private RestEntityContainer container;

        public float Priority => 3.0f;
        public bool ForLife => true;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.WebService;

        public WebAPIRestServiceProviderFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var name = baseObject.Name;
            var container = (RestEntityContainer)baseObject;
            var variables = container.Variables;
            var parentPath = (string) variables["parentPath"];
            var parentObject = baseObject.EntityDictionary.Single(p => p.Value.GetCondensedID() == parentPath).Value;
            var rootObject = (object)container.JsonRootObject;
            var expressionHandler = generatorConfiguration.GetExpressionHandler(Guid.Parse(AbstraXProviderGuids.RestService));
            var title = (string)container.Variables["title"];
            var serviceName = title + "ServiceProvider";
            List<ServiceMethod> serviceMethods;

            this.container = container;

            serviceMethods = new List<ServiceMethod>();
            generatorConfiguration.KeyValuePairs["RestServiceMethods"] = serviceMethods;

            foreach (var entitySet in container.EntitySets.Cast<RestEntitySet>())
            {
                var additionalProperties = (object)entitySet.SelectOriginal("$.additionalProperties");

                if (additionalProperties != null)
                {
                    var operations = additionalProperties.JsonSelect("$.operations");
                    var entity = (RestEntityType)entitySet.Entities.Single();
                    var entityObject = (object)entity.JsonObject;
                    var entityId = entityObject.JsonSelect("$.properties..uniqueId");

                    // kn todo future - flush out entitySet.Operations

                    foreach (var operationPair in operations.GetDynamicMemberNameValueDictionary())
                    {
                        var operationName = operationPair.Key.ToTitleCase();
                        var serviceMethod = new ServiceMethod(entitySet, operationName, serviceName, "string id", generatorConfiguration);
                        var operation = operationPair.Value;
                        var urlExpression = additionalProperties.GetDynamicMemberValue<string>("url");
                        var resultPath = operation.GetDynamicMemberValue<string>("resultPath");
                        Dictionary<string, string> methodVariables;
                        Dictionary<string, string> returnProperties;
                        string parms;
                        string test;

                        if (operation.HasDynamicMember("queryString"))
                        {
                            var queryString = operation.GetDynamicMemberValue<string>("queryString");

                            parms = expressionHandler.Handle<string>(container, ExpressionResultLocation.Provider, ExpressionType.QueryString, ExpressionReturnType.QueryString, queryString);
                            methodVariables = expressionHandler.Handle<Dictionary<string, string>>(container, ExpressionResultLocation.Provider, ExpressionType.Url, ExpressionReturnType.MethodVariables, urlExpression, parms);
                        }
                        else
                        {
                            methodVariables = expressionHandler.Handle<Dictionary<string, string>>(container, ExpressionResultLocation.Provider, ExpressionType.Url, ExpressionReturnType.MethodVariables, urlExpression);
                        }

                        returnProperties = expressionHandler.Handle<Dictionary<string, string>>(container, ExpressionResultLocation.Provider, ExpressionType.Url, ExpressionReturnType.ReturnProperties, urlExpression);

                        serviceMethod.MethodVariables = methodVariables;
                        serviceMethod.ReturnProperties = returnProperties;
                        serviceMethod.ResultPath = resultPath.RemoveSurrounding("{", "}");

                        serviceMethod.UniqueIdFactory = expressionHandler.Handle<Func<string[], string>>(entity, ExpressionResultLocation.Provider, ExpressionType.UniqueId, ExpressionReturnType.UniqueIdFactory, entityId);

                        //test = serviceMethod.UniqueIdFactory(new string[] { "a", "b" });

                        if (generatorConfiguration.CurrentPass == GeneratorPass.Files)
                        {
                            serviceMethods.Add(serviceMethod);
                        }
                    }
                }
            }

            return true;
        }

        public void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, IEnumerable<Module> addModules, ModuleAddType moduleAddType)
        {
            modules.AddRange(addModules);
        }

        public bool PreProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }

        public bool PostProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }

        public bool Terminate(IGeneratorConfiguration generatorConfiguration)
        {
            var providersPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.WebAPIProviders];
            var providersFolder = (Folder)generatorConfiguration.FileSystem[providersPath];
            var title = (string)container.Variables["title"];
            var serviceName = title + "ServiceProvider";
            var configPathString = $"@\"{ title }Config\\Config.json\"";
            var namingConvention = EnumUtils.GetValue<NamingConvention>((string)container.Variables["namingConvention"]);
            List<ServiceMethod> serviceMethods;

            serviceMethods = (List<ServiceMethod>)generatorConfiguration.KeyValuePairs["RestServiceMethods"];

            WebAPIRestServiceProviderGenerator.GenerateProvider(container, providersPath, title, serviceName, configPathString, namingConvention, generatorConfiguration, serviceMethods);

            return true;
        }
    }
}
