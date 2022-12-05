﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace AbstraX.Generators.Pages.AboutPage
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Utils;
    using AbstraX.Generators;
    using AbstraX.Angular;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\CloudIDEaaS\develop\ApplicationGenerator.IonicAngular\Generators\Pages\AboutPage\AboutClassTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class AboutClassTemplate : AbstraX.Generators.Base.TemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            
            #line 19 "C:\CloudIDEaaS\develop\ApplicationGenerator.IonicAngular\Generators\Pages\AboutPage\AboutClassTemplate.tt"

    // gets info from the AppDataProvider (see Generators\Client\AppDataProvider\AppDataProviderClassTemplate.tt)

    var pageName = this.PageName.ToCamelCase();
    var className = this.PageName + "Page";
    var component = new Page(className, this.UILoadKind, this.UIKind);

    this.Declarations.Add(component);
    this.Exports.Add(component);

    foreach (var import in this.Imports)
    {
        this.WriteLine(import.DeclarationCode);
    }

            
            #line default
            #line hidden
            this.Write(@"import { UserDataProvider } from '../../providers/userdata.provider';
import { AboutData, AppStore, AppDataProvider } from '../../providers/appdata.provider';
import { LaunchReview } from '@ionic-native/launch-review';
const { version } = require('../../../../package.json');

");
            
            #line 39 "C:\CloudIDEaaS\develop\ApplicationGenerator.IonicAngular\Generators\Pages\AboutPage\AboutClassTemplate.tt"

    if (this.Authorize.Length > 0)
    {

            
            #line default
            #line hidden
            this.Write("@Authorize(\"");
            
            #line 43 "C:\CloudIDEaaS\develop\ApplicationGenerator.IonicAngular\Generators\Pages\AboutPage\AboutClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.Authorize));
            
            #line default
            #line hidden
            this.Write("\")\r\n");
            
            #line 44 "C:\CloudIDEaaS\develop\ApplicationGenerator.IonicAngular\Generators\Pages\AboutPage\AboutClassTemplate.tt"
 
    }

            
            #line default
            #line hidden
            this.Write("@Component({\r\n  selector: \'page-");
            
            #line 48 "C:\CloudIDEaaS\develop\ApplicationGenerator.IonicAngular\Generators\Pages\AboutPage\AboutClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pageName));
            
            #line default
            #line hidden
            this.Write("\',\r\n  templateUrl: \'");
            
            #line 49 "C:\CloudIDEaaS\develop\ApplicationGenerator.IonicAngular\Generators\Pages\AboutPage\AboutClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pageName));
            
            #line default
            #line hidden
            this.Write(".html\',\r\n  styleUrls: [\'./");
            
            #line 50 "C:\CloudIDEaaS\develop\ApplicationGenerator.IonicAngular\Generators\Pages\AboutPage\AboutClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pageName));
            
            #line default
            #line hidden
            this.Write(".scss\']\r\n})\r\nexport class ");
            
            #line 52 "C:\CloudIDEaaS\develop\ApplicationGenerator.IonicAngular\Generators\Pages\AboutPage\AboutClassTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(className));
            
            #line default
            #line hidden
            this.Write(@" implements OnInit {
  aboutData: AboutData;
  appStoreLinks: AppStore[];
  version: string;

  constructor(
    public router: Router,
    public routerOutlet: IonRouterOutlet,
    public toastCtrl: ToastController,
    public user: UserDataProvider,
    public app: AppDataProvider,
    public config: Config
  ) {
        this.version = version;
  }

  ionViewDidEnter() {
    this.app.getAboutData().subscribe((aboutData: AboutData) => {
      this.aboutData = aboutData;
      this.appStoreLinks = aboutData.details.appStoreLinks;
    });
  }

  async presentPopover(event: Event) {
  }
  
  ngOnInit() {
  }
}

");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 1 "C:\CloudIDEaaS\develop\ApplicationGenerator.IonicAngular\Generators\Pages\AboutPage\AboutClassTemplate.tt"

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

private global::System.Collections.Generic.List<object> _InputField;

/// <summary>
/// Access the Input parameter of the template.
/// </summary>
private global::System.Collections.Generic.List<object> Input
{
    get
    {
        return this._InputField;
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

private global::System.Collections.Generic.List<ESModule> _ExportsField;

/// <summary>
/// Access the Exports parameter of the template.
/// </summary>
private global::System.Collections.Generic.List<ESModule> Exports
{
    get
    {
        return this._ExportsField;
    }
}

private global::System.Collections.Generic.List<IDeclarable> _DeclarationsField;

/// <summary>
/// Access the Declarations parameter of the template.
/// </summary>
private global::System.Collections.Generic.List<IDeclarable> Declarations
{
    get
    {
        return this._DeclarationsField;
    }
}

private global::AbstraX.DataAnnotations.UILoadKind _UILoadKindField;

/// <summary>
/// Access the UILoadKind parameter of the template.
/// </summary>
private global::AbstraX.DataAnnotations.UILoadKind UILoadKind
{
    get
    {
        return this._UILoadKindField;
    }
}

private global::AbstraX.DataAnnotations.UIKind _UIKindField;

/// <summary>
/// Access the UIKind parameter of the template.
/// </summary>
private global::AbstraX.DataAnnotations.UIKind UIKind
{
    get
    {
        return this._UIKindField;
    }
}

private string _PageNameField;

/// <summary>
/// Access the PageName parameter of the template.
/// </summary>
private string PageName
{
    get
    {
        return this._PageNameField;
    }
}

private string _AuthorizeField;

/// <summary>
/// Access the Authorize parameter of the template.
/// </summary>
private string Authorize
{
    get
    {
        return this._AuthorizeField;
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
bool InputValueAcquired = false;
if (this.Session.ContainsKey("Input"))
{
    this._InputField = ((global::System.Collections.Generic.List<object>)(this.Session["Input"]));
    InputValueAcquired = true;
}
if ((InputValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("Input");
    if ((data != null))
    {
        this._InputField = ((global::System.Collections.Generic.List<object>)(data));
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
bool ExportsValueAcquired = false;
if (this.Session.ContainsKey("Exports"))
{
    this._ExportsField = ((global::System.Collections.Generic.List<ESModule>)(this.Session["Exports"]));
    ExportsValueAcquired = true;
}
if ((ExportsValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("Exports");
    if ((data != null))
    {
        this._ExportsField = ((global::System.Collections.Generic.List<ESModule>)(data));
    }
}
bool DeclarationsValueAcquired = false;
if (this.Session.ContainsKey("Declarations"))
{
    this._DeclarationsField = ((global::System.Collections.Generic.List<IDeclarable>)(this.Session["Declarations"]));
    DeclarationsValueAcquired = true;
}
if ((DeclarationsValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("Declarations");
    if ((data != null))
    {
        this._DeclarationsField = ((global::System.Collections.Generic.List<IDeclarable>)(data));
    }
}
bool UILoadKindValueAcquired = false;
if (this.Session.ContainsKey("UILoadKind"))
{
    this._UILoadKindField = ((global::AbstraX.DataAnnotations.UILoadKind)(this.Session["UILoadKind"]));
    UILoadKindValueAcquired = true;
}
if ((UILoadKindValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("UILoadKind");
    if ((data != null))
    {
        this._UILoadKindField = ((global::AbstraX.DataAnnotations.UILoadKind)(data));
    }
}
bool UIKindValueAcquired = false;
if (this.Session.ContainsKey("UIKind"))
{
    this._UIKindField = ((global::AbstraX.DataAnnotations.UIKind)(this.Session["UIKind"]));
    UIKindValueAcquired = true;
}
if ((UIKindValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("UIKind");
    if ((data != null))
    {
        this._UIKindField = ((global::AbstraX.DataAnnotations.UIKind)(data));
    }
}
bool PageNameValueAcquired = false;
if (this.Session.ContainsKey("PageName"))
{
    this._PageNameField = ((string)(this.Session["PageName"]));
    PageNameValueAcquired = true;
}
if ((PageNameValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("PageName");
    if ((data != null))
    {
        this._PageNameField = ((string)(data));
    }
}
bool AuthorizeValueAcquired = false;
if (this.Session.ContainsKey("Authorize"))
{
    this._AuthorizeField = ((string)(this.Session["Authorize"]));
    AuthorizeValueAcquired = true;
}
if ((AuthorizeValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("Authorize");
    if ((data != null))
    {
        this._AuthorizeField = ((string)(data));
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
