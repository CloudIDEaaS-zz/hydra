﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace AbstraX.Generators.Modules.StandardModule
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Utils;
    using AbstraX.Generators;
    using AbstraX.Angular;
    using AbstraX;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class StandardModuleClassTemplate : AbstraX.Generators.Base.TemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("import { NgModule } from \'@angular/core\';\r\nimport { IonicPageModule } from \'ionic" +
                    "-angular\';\r\n");
            
            #line 17 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"
 
    var className = this.ModuleName;
    var builder = new StringBuilder();
    var component = this.Imports.GetComponent(this.AngularModule.UILoadKind);
    var indent4 = this.GetIndent(4);

    foreach (var import in this.Imports)
    {
        this.WriteLine(import.DeclarationCode);
    }

    // TODO - add module specific imports below this block

            
            #line default
            #line hidden
            this.Write("\r\n@NgModule({\r\n  declarations: [\r\n");
            
            #line 33 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"
  
    foreach (var declarable in this.AngularModule.Declarations)
    {
        builder.AppendWithLeadingIfLength(",\r\n", indent4 + declarable.Name);
    }

    this.WriteLine(builder.ToString());

            
            #line default
            #line hidden
            this.Write("  ],\r\n  imports: [\r\n    IonicPageModule.forChild(");
            
            #line 42 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(component.Name));
            
            #line default
            #line hidden
            this.Write(")\r\n");
            
            #line 43 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"
  
    builder = new StringBuilder();

    if (this.AngularModule.Imports.Count > 0)
    {
        foreach (var import in this.AngularModule.Imports)
        {
            builder.AppendWithLeadingIfLength(",\r\n", indent4 + import.Name);
        }

        this.WriteLine(builder.ToString());
    }

            
            #line default
            #line hidden
            this.Write("  ]");
            
            #line 55 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"
  
    
    if (this.AngularModule.Providers.Count > 0)
    {
        this.WriteLine(",");

            
            #line default
            #line hidden
            this.Write("  providers: [\r\n");
            
            #line 62 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"

        builder = new StringBuilder();

        foreach (var provider in this.AngularModule.Providers)
        {
            builder.AppendWithLeadingIfLength(",\r\n", indent4 + provider.Name);
        }

        this.WriteLine(builder.ToString());

            
            #line default
            #line hidden
            this.Write("  ]");
            
            #line 72 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"

    }

    if (this.AngularModule.Exports.Count > 0)
    {
        this.WriteLine(",");

            
            #line default
            #line hidden
            this.Write("  exports: [\r\n");
            
            #line 80 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"

        builder = new StringBuilder();

        foreach (var export in this.AngularModule.Exports)
        {
            builder.AppendWithLeadingIfLength(",\r\n", indent4 + export.Name);
        }

        this.WriteLine(builder.ToString());

            
            #line default
            #line hidden
            this.Write("  ]\r\n");
            
            #line 91 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"

    }
 
            
            #line default
            #line hidden
            this.Write("})\r\nexport class ");
            
            #line 95 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(className));
            
            #line default
            #line hidden
            this.Write(" {}\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 1 "D:\MC\CloudIDEaaS\root\ApplicationGenerator\Generators\Modules\StandardModule\StandardModuleClassTemplate.tt"

private global::System.EventHandler _DebugCallbackField;

/// <summary>
/// Access the DebugCallback parameter of the template.
/// </summary>
private global::System.EventHandler DebugCallback
{
    get
    {
        return this._DebugCallbackField;
    }
}

private global::System.Collections.Generic.IEnumerable<ModuleImportDeclaration> _ImportsField;

/// <summary>
/// Access the Imports parameter of the template.
/// </summary>
private global::System.Collections.Generic.IEnumerable<ModuleImportDeclaration> Imports
{
    get
    {
        return this._ImportsField;
    }
}

private global::AbstraX.Angular.AngularModule _AngularModuleField;

/// <summary>
/// Access the AngularModule parameter of the template.
/// </summary>
private global::AbstraX.Angular.AngularModule AngularModule
{
    get
    {
        return this._AngularModuleField;
    }
}

private string _ModuleNameField;

/// <summary>
/// Access the ModuleName parameter of the template.
/// </summary>
private string ModuleName
{
    get
    {
        return this._ModuleNameField;
    }
}


/// <summary>
/// Initialize the template
/// </summary>
public override void Initialize()
{
    base.Initialize();
    if ((this.Errors.HasErrors == false))
    {
bool DebugCallbackValueAcquired = false;
if (this.Session.ContainsKey("DebugCallback"))
{
    this._DebugCallbackField = ((global::System.EventHandler)(this.Session["DebugCallback"]));
    DebugCallbackValueAcquired = true;
}
if ((DebugCallbackValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("DebugCallback");
    if ((data != null))
    {
        this._DebugCallbackField = ((global::System.EventHandler)(data));
    }
}
bool ImportsValueAcquired = false;
if (this.Session.ContainsKey("Imports"))
{
    this._ImportsField = ((global::System.Collections.Generic.IEnumerable<ModuleImportDeclaration>)(this.Session["Imports"]));
    ImportsValueAcquired = true;
}
if ((ImportsValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("Imports");
    if ((data != null))
    {
        this._ImportsField = ((global::System.Collections.Generic.IEnumerable<ModuleImportDeclaration>)(data));
    }
}
bool AngularModuleValueAcquired = false;
if (this.Session.ContainsKey("AngularModule"))
{
    this._AngularModuleField = ((global::AbstraX.Angular.AngularModule)(this.Session["AngularModule"]));
    AngularModuleValueAcquired = true;
}
if ((AngularModuleValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("AngularModule");
    if ((data != null))
    {
        this._AngularModuleField = ((global::AbstraX.Angular.AngularModule)(data));
    }
}
bool ModuleNameValueAcquired = false;
if (this.Session.ContainsKey("ModuleName"))
{
    this._ModuleNameField = ((string)(this.Session["ModuleName"]));
    ModuleNameValueAcquired = true;
}
if ((ModuleNameValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("ModuleName");
    if ((data != null))
    {
        this._ModuleNameField = ((string)(data));
    }
}


    }
}


        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
}
