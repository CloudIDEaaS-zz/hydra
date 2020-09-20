using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using AbstraX.AssemblyInterfaces;
using AbstraX.ServerInterfaces;
using AbstraX.Bindings.Interfaces;
using Microsoft.Practices.Unity;
using log4net;

namespace AbstraX.Contracts
{
    public interface IProviderEntityService
    {
        ILog Log { get; }
        IProgrammingLanguage SelectedSourceLanguage { get; }
        IAbstraXDatabaseProxy DatabaseProxy { get; }
        bool IncludePublicMemberVariablesAsProperties { get; }
    }

    public interface IDomainHostApplication : IProviderEntityService
    {
        void PostEventShowUI(string uiComponentURL, string parms);
        void PostEventShowUI(string uiComponentURL, string parms, IBuildUIDaemon buildUIDaemon, TimeSpan waitTimeout);
        void PostEventBuildMessage(string message);
        void PostEventBuildComplete();
        void PostEventError(string errorMessage);
        void PostEventPercentComplete(float percentComplete);
        void ClearEventQueue();
        Queue<BuildUIQueueEntry> BuildUIQueue { get; }
        ILog Log { get; }
        IEventsService EventsService { get; set; }
        IAssemblyProviderService AssemblyProviderService { get; set;  }
        IBindingsTreeCache BindingsTreeCache { get; }
        IBindingsGenerator BindingsGenerator { get; }
        IBindingsBuilder BindingsBuilder { get; }
        string ImageEditorUrl { get; set; }
        string GetImageEditorUrl();
        List<IVSSolution> Solutions { get; }
        void SetStatus(string status);
        List<string> LatestStatus { get; }
        IUnityContainer PipelineContainer { get; }
        IPipelineService PipelineService { get; }
        bool FinishBuildWithDefaults { get; set; }
        Dictionary<string, IAbstraXProviderService> RegisteredServices { get; }
        Dictionary<string, ICodeTemplate> ProjectGroupTemplates { get; }
        Dictionary<string, ICodeProjectTemplate> ProjectTemplates { get; }
        Dictionary<string, ICodeItemTemplate> ItemTemplates { get; }
        Dictionary<string, IBuildFilter> BuildFilters { get; }
        IAbstraXDatabaseProxy DatabaseProxy { get; }
        List<IProgrammingLanguage> AvailableLanguages { get; }
        IList<ProviderListItem> ProviderList { get; }
        string RootPath { get; }
        string WorkspacePath { get; }
    }
}
