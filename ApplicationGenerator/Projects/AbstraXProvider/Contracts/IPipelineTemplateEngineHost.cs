using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using AbstraX.TypeMappings;
using AbstraX.ServerInterfaces;
using AbstraX.Bindings;
using AbstraX.AssemblyInterfaces;

namespace AbstraX.Contracts
{
    public class StepAbortionException : Exception
    {
        public FileInfo LastFile { get; set; }

        public StepAbortionException(FileInfo lastFile) : base("Step aborted!")
        {
            this.LastFile = lastFile;
        }
    }

    public class StepAbortionEventArgs : EventArgs
    {
        public FileInfo LastFile { get; set; }

        public StepAbortionEventArgs(FileInfo lastFile)
        {
            this.LastFile = lastFile;
        }
    }

    public enum EmitDebugInfoLevel
    {
        None,
        Simple,
        Diagnostics
    }

    public delegate void AbortStepHandler(object sender, StepAbortionEventArgs e); 
    public delegate void DefineConstructHandler(IElementBuild build, ConstructType constructType, out BaseType baseType);
    public delegate void DefineTypeHandler(IElementBuild build, BaseType type, out BaseType baseType);
    public delegate void GetElementNameHandler(IElement element, ref string name);
    public delegate void GetBuildNameHandler(IElementBuild build, ref string name);
    public delegate void GetTypeNameHandler(BaseType type, ref string name);
    public delegate void ElementIsToBeBuiltHandler(IElement element, out bool isToBeBuilt);
    public delegate void TypeIsToBeBuiltHandler(BaseType type, out bool isToBeBuilt);
    public delegate void GetNavigationPropertyNameHandler(NavigationItem navigationItem, ref string name);
    public delegate void GetPropertyBindingPropertyNameHandler(IPropertyBinding binding, ref string name);
    public delegate void GetMethodOperationNameHandler(IMethodOperation operation, ref string name);
    public delegate void GetAttributeNameHandler(IAttribute attribute, ref string name);

    public interface IPipelineTemplateEngineHost
    {
        Stack<IGenerator> GeneratorStack { get; }
        EmitDebugInfoLevel EmitDebugInfoLevel { get; set; }
        event EventHandler DebugCallback;
        event DefineConstructHandler DefineConstruct;
        event DefineTypeHandler DefineType;
        event AbortStepHandler AbortBuild;
        event GetBuildNameHandler GetBuildName;
        event GetElementNameHandler GetElementName;
        event GetTypeNameHandler GetTypeName;
        event GetAttributeNameHandler GetAttributeName;
        event GetBuildNameHandler GetReferencedBuildName;
        event GetElementNameHandler GetReferencedElementName;
        event GetTypeNameHandler GetReferencedTypeName;
        event ElementIsToBeBuiltHandler ElementIsToBeBuilt;
        event TypeIsToBeBuiltHandler TypeIsToBeBuilt;
        event GetNavigationPropertyNameHandler GetNavigationPropertyName;
        event GetPropertyBindingPropertyNameHandler GetPropertyBindingPropertyName;
        event GetMethodOperationNameHandler GetMethodOperationName;
        event OnGeneratorFileWithBuildPostBuildHandler OnGeneratorFileWithBuildPostBuild;
        event OnGeneratorFileWithBuildPreBuildHandler OnGeneratorFileWithBuildPreBuild;
        event OnGeneratorFileWithListPostBuildHandler OnGeneratorFileWithListPostBuild;
        event OnGeneratorFileWithListPreBuildHandler OnGeneratorFileWithListPreBuild;
        void GenerateFile(IElementBuild build, FileInfo fileInfo, string _namespace, Dictionary<string, object> extraValues = null);
        void GenerateFile(FileInfo fileInfo, string _namespace, Dictionary<string, object> values = null);
        void GenerateFile(FileInfo fileInfo, string _namespace, IEnumerable list = null);
        void GenerateFile<TGeneratorType>(IElementBuild build, FileInfo fileInfo, string _namespace, Dictionary<string, object> extraValues = null);
        void GenerateFile<TGeneratorType>(FileInfo fileInfo, string _namespace, Dictionary<string, object> values = null);
        void GenerateFile<TGeneratorType>(FileInfo fileInfo, string _namespace, IEnumerable list = null);
    }
}
