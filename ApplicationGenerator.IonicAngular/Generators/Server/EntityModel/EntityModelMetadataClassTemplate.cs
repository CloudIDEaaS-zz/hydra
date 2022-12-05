﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace AbstraX.Generators.Server.EntityModel
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Utils;
    using AbstraX;
    using AbstraX.Generators;
    using AbstraX.DataAnnotations;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class EntityModelMetadataClassTemplate : AbstraX.Generators.Base.TemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utils;
using AbstraX.DataAnnotations;
using System.ComponentModel;
");
            
            #line 25 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"

    var entityType = this.EntityObject.DynamicEntityType;
    var metadataType = this.EntityObject.DynamicEntityMetadataType;

            
            #line default
            #line hidden
            this.Write("\r\nnamespace ");
            
            #line 30 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(entityType.Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    ");
            
            #line 32 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityObject.GetAttributeCode(1)));
            
            #line default
            #line hidden
            this.Write("\r\n    public partial class ");
            
            #line 33 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(entityType.Name));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n    }\r\n\r\n    ");
            
            #line 37 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityObject.GetMetadataAttributeCode(1)));
            
            #line default
            #line hidden
            this.Write("\r\n    public partial class ");
            
            #line 38 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(metadataType.Name));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n");
            
            #line 40 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"

    foreach (var property in entityType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly))
    {
        var entityAttribute = this.EntityObject.Attributes.Single(a => a.DynamicPropertyName == property.Name);

            
            #line default
            #line hidden
            this.Write("        ");
            
            #line 45 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(entityAttribute.GetMetadataAttributeCode(2)));
            
            #line default
            #line hidden
            this.Write("\r\n        ");
            
            #line 46 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.GetSignature(false, true)));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n");
            
            #line 48 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"

    }

            
            #line default
            #line hidden
            this.Write("    }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 1 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt"

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

private global::AbstraX.TemplateObjects.EntityObject _EntityObjectField;

/// <summary>
/// Access the EntityObject parameter of the template.
/// </summary>
private global::AbstraX.TemplateObjects.EntityObject EntityObject
{
    get
    {
        return this._EntityObjectField;
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
bool EntityObjectValueAcquired = false;
if (this.Session.ContainsKey("EntityObject"))
{
    this._EntityObjectField = ((global::AbstraX.TemplateObjects.EntityObject)(this.Session["EntityObject"]));
    EntityObjectValueAcquired = true;
}
if ((EntityObjectValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("EntityObject");
    if ((data != null))
    {
        this._EntityObjectField = ((global::AbstraX.TemplateObjects.EntityObject)(data));
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
