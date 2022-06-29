
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public enum NgxTranslateModules : ulong
    {
        [ModuleImportDeclaration("TranslateService", "@ngx-translate/core")]
        TranslateService = 0xad77e842bbd0c111,
    }

    public enum AgGridAngularModules : ulong
    {
        [ModuleImportDeclaration("AgGridNg2", "ag-grid-angular")]
        AgGridNg2 = 0x9c06471288453ea5,
    }

    public enum AgGridModules : ulong
    {
        [ModuleImportDeclaration("ICellEditorComp", "ag-grid")]
        ICellEditorComp = 0x9d427aa902a8505f,
    }

    public enum AngularTextMaskModules : ulong
    {
        [ModuleImportDeclaration("TextMaskModule", "angular2-text-mask")]
        TextMaskModule = 0xbad0956e15f07477,
    }

    public enum SuperTabsModules : ulong
    {
        [ModuleImportDeclaration("SuperTabsModule", "ionic2-super-tabs")]
        SuperTabsModule = 0xa4aeb275b6e9f912,
    }

    public enum LinqCollectionsModules : ulong
    {
        [ModuleImportDeclaration("List", "linq-collections")]
        List = 0xab4786ed4ababaec,
    }

    public enum RxJsMapModules : ulong
    {
        [ModuleImportDeclaration("rxjs/add/operator/map")]
        Default = 0xb6a0dfd958e21d7a,
    }

    public enum RxJsObservableModules : ulong
    {
        [ModuleImportDeclaration("Observable", "rxjs")]
        Observable = 0x8ebc7a0b93556f14,
    }

    public enum AngularProviderModules : ulong
    {
        [ModuleImportDeclaration("Injectable", "@angular/core")]
        Injectable = 0xa96014aaf94db40a,
        [ModuleImportDeclaration("Component", "@angular/core")]
        Component = 0xbb7a0ff1a7aed1d8,
    }

    public enum AngularModules : ulong
    {
        [ModuleImportDeclaration("Component", "@angular/core")]
        Component = 0x9a2996d9339f6472,
        [ModuleImportDeclaration("NgZone", "@angular/core")]
        NgZone = 0x8daee62163e4bb01,
        [ModuleImportDeclaration("ViewChild", "@angular/core")]
        ViewChild = 0xb491481845506a70,
        [ModuleImportDeclaration("OnInit", "@angular/core")]
        OnInit = 0x83145c72de11be8c,
    }

    public enum AngularRouterModules : ulong
    {
        [ModuleImportDeclaration("Router", "@angular/router")]
        Router = 0x80ad91f99502c8c4,
    }

    public enum AngularValidatorModules : ulong
    {
        [ModuleImportDeclaration("Validators", "@angular/forms")]
        Validators = 0x8e9ae43261c4369b,
        [ModuleImportDeclaration("FormGroup", "@angular/forms")]
        FormGroup = 0xa505f54d399a3786,
        [ModuleImportDeclaration("FormControl", "@angular/forms")]
        FormControl = 0x92420ce5fb65c1d7,
        [ModuleImportDeclaration("AbstractControl", "@angular/forms")]
        AbstractControl = 0xb26366b00995666a,
        [ModuleImportDeclaration("ValidatorFn", "@angular/forms")]
        ValidatorFn = 0xb17ede735dde5cca,
    }

    public enum AngularFormsModules : ulong
    {
        [ModuleImportDeclaration("Validators", "@angular/forms")]
        Validators = 0x970876f53eaf4a31,
        [ModuleImportDeclaration("FormGroup", "@angular/forms")]
        FormGroup = 0x84039b6efd096dca,
        [ModuleImportDeclaration("FormControl", "@angular/forms")]
        FormControl = 0x94dde3e51a4b38ee,
        [ModuleImportDeclaration("AbstractControl", "@angular/forms")]
        AbstractControl = 0xaf43f346209ca356,
        [ModuleImportDeclaration("ValidatorFn", "@angular/forms")]
        ValidatorFn = 0xbaf525cf62d20478,
    }

    public enum IonicModules : ulong
    {
        [ModuleImportDeclaration("NavController", "@ionic/angular")]
        NavController = 0xbfea7f68c6080f6c,
        [ModuleImportDeclaration("LoadingController", "@ionic/angular")]
        LoadingController = 0xaf6fc0d19865e8c1,
        [ModuleImportDeclaration("IonRouterOutlet", "@ionic/angular")]
        IonRouterOutlet = 0xb59eba10ea4ea684,
        [ModuleImportDeclaration("ToastController", "@ionic/angular")]
        ToastController = 0x80b92a021e19cc77,
        [ModuleImportDeclaration("Config", "@ionic/angular")]
        Config = 0xb69f3e468c752b12,
    }

    public enum IonicGridPageModules : ulong
    {
        [ModuleImportDeclaration("NavController", "@ionic/angular")]
        NavController = 0x815aaa0208cc6be2,
        [ModuleImportDeclaration("LoadingController", "@ionic/angular")]
        LoadingController = 0x99d288978992f8b7,
        [ModuleImportDeclaration("NavParams", "@ionic/angular")]
        NavParams = 0xab397486273ccb92,
        [ModuleImportDeclaration("Toast", "@ionic/angular")]
        Toast = 0xaea38aed203f6b0e,
        [ModuleImportDeclaration("AlertController", "@ionic/angular")]
        AlertController = 0x8dc8722b0975407b,
        [ModuleImportDeclaration("PopoverController", "@ionic/angular")]
        PopoverController = 0xa42237555ca43eb0,
    }

    public enum IonicEditPageModules : ulong
    {
        [ModuleImportDeclaration("NavController", "@ionic/angular")]
        NavController = 0xa3c1bd3afa08c25b,
        [ModuleImportDeclaration("LoadingController", "@ionic/angular")]
        LoadingController = 0xa0f3728e05a99b82,
        [ModuleImportDeclaration("NavParams", "@ionic/angular")]
        NavParams = 0xb313714f6b147770,
        [ModuleImportDeclaration("ViewController", "@ionic/angular")]
        ViewController = 0x9fd3b934b70da956,
    }

    public enum IonicGridPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("GridPage", "BuiltIn")]
        GridPage = 0xba5ff22fc2f0a8b6,
        [ModuleImportDeclaration("PageName", "BuiltIn")]
        PageName = 0x89263ed2ffeef369,
        [ModuleImportDeclaration("RecordExpression", "BuiltIn")]
        RecordExpression = 0x9ce4406bbdcc3f10,
        [ModuleImportDeclaration("EditDeleteButtons", "BuiltIn")]
        EditDeleteButtons = 0xbd2e9c84f466eb94,
    }

    public enum IonicEditPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("UsablePopover", "BuiltIn")]
        UsablePopover = 0x87e641bb9dc9dcad,
    }

    public enum IonicPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("Authorize", "BuiltIn")]
        Authorize = 0xadafa305d85ce92e,
    }

    public enum AngularValidationPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("ValidationMap", "BuiltIn")]
        ValidationMap = 0x9759fbad6c2e21ca,
    }

    [ModuleImports()]
    public static class ModuleImports
    {
        public const ulong APPLICATION_IMPORT_HANDLER_ID = ulong.MinValue;
        public const ulong NGXTRANSLATE_HANDLER_ID = 0xea394bc261764722;
        public const string NGXTRANSLATE_HANDLER_ID_STRING = "ea394bc2-6176-4722";
        public const ulong NGXTRANSLATE_TRANSLATESERVICE_ID = 0xad77e842bbd0c111;
        public const string NGXTRANSLATE_TRANSLATESERVICE_ID_STRING = "ad77-e842bbd0c111";
        public const string NGXTRANSLATE_TranslateService = "{" + NGXTRANSLATE_HANDLER_ID_STRING + "-" + NGXTRANSLATE_TRANSLATESERVICE_ID_STRING + "}";
        public const ulong AGGRIDANGULAR_HANDLER_ID = 0x98f036d8770a4dac;
        public const string AGGRIDANGULAR_HANDLER_ID_STRING = "98f036d8-770a-4dac";
        public const ulong AGGRIDANGULAR_AGGRIDNG2_ID = 0x9c06471288453ea5;
        public const string AGGRIDANGULAR_AGGRIDNG2_ID_STRING = "9c06-471288453ea5";
        public const string AGGRIDANGULAR_AgGridNg2 = "{" + AGGRIDANGULAR_HANDLER_ID_STRING + "-" + AGGRIDANGULAR_AGGRIDNG2_ID_STRING + "}";
        public const ulong AGGRID_HANDLER_ID = 0xa02c65c0294f469a;
        public const string AGGRID_HANDLER_ID_STRING = "a02c65c0-294f-469a";
        public const ulong AGGRID_ICELLEDITORCOMP_ID = 0x9d427aa902a8505f;
        public const string AGGRID_ICELLEDITORCOMP_ID_STRING = "9d42-7aa902a8505f";
        public const string AGGRID_ICellEditorComp = "{" + AGGRID_HANDLER_ID_STRING + "-" + AGGRID_ICELLEDITORCOMP_ID_STRING + "}";
        public const ulong ANGULARTEXTMASK_HANDLER_ID = 0xd9d5ce49376846c5;
        public const string ANGULARTEXTMASK_HANDLER_ID_STRING = "d9d5ce49-3768-46c5";
        public const ulong ANGULARTEXTMASK_TEXTMASKMODULE_ID = 0xbad0956e15f07477;
        public const string ANGULARTEXTMASK_TEXTMASKMODULE_ID_STRING = "bad0-956e15f07477";
        public const string ANGULARTEXTMASK_TextMaskModule = "{" + ANGULARTEXTMASK_HANDLER_ID_STRING + "-" + ANGULARTEXTMASK_TEXTMASKMODULE_ID_STRING + "}";
        public const ulong SUPERTABS_HANDLER_ID = 0xc2502e8378e545f0;
        public const string SUPERTABS_HANDLER_ID_STRING = "c2502e83-78e5-45f0";
        public const ulong SUPERTABS_SUPERTABSMODULE_ID = 0xa4aeb275b6e9f912;
        public const string SUPERTABS_SUPERTABSMODULE_ID_STRING = "a4ae-b275b6e9f912";
        public const string SUPERTABS_SuperTabsModule = "{" + SUPERTABS_HANDLER_ID_STRING + "-" + SUPERTABS_SUPERTABSMODULE_ID_STRING + "}";
        public const ulong LINQCOLLECTIONS_HANDLER_ID = 0x54e2568e051442b8;
        public const string LINQCOLLECTIONS_HANDLER_ID_STRING = "54e2568e-0514-42b8";
        public const ulong LINQCOLLECTIONS_LIST_ID = 0xab4786ed4ababaec;
        public const string LINQCOLLECTIONS_LIST_ID_STRING = "ab47-86ed4ababaec";
        public const string LINQCOLLECTIONS_List = "{" + LINQCOLLECTIONS_HANDLER_ID_STRING + "-" + LINQCOLLECTIONS_LIST_ID_STRING + "}";
        public const ulong RXJSMAP_HANDLER_ID = 0x58ca451b39054c0c;
        public const string RXJSMAP_HANDLER_ID_STRING = "58ca451b-3905-4c0c";
        public const ulong RXJSMAP_DEFAULT_ID = 0xb6a0dfd958e21d7a;
        public const string RXJSMAP_DEFAULT_ID_STRING = "b6a0-dfd958e21d7a";
        public const string RXJSMAP_Default = "{" + RXJSMAP_HANDLER_ID_STRING + "-" + RXJSMAP_DEFAULT_ID_STRING + "}";
        public const ulong RXJSOBSERVABLE_HANDLER_ID = 0xce62b5b0ffec4ba7;
        public const string RXJSOBSERVABLE_HANDLER_ID_STRING = "ce62b5b0-ffec-4ba7";
        public const ulong RXJSOBSERVABLE_OBSERVABLE_ID = 0x8ebc7a0b93556f14;
        public const string RXJSOBSERVABLE_OBSERVABLE_ID_STRING = "8ebc-7a0b93556f14";
        public const string RXJSOBSERVABLE_Observable = "{" + RXJSOBSERVABLE_HANDLER_ID_STRING + "-" + RXJSOBSERVABLE_OBSERVABLE_ID_STRING + "}";
        public const ulong ANGULARPROVIDER_HANDLER_ID = 0x314e041b53c94481;
        public const string ANGULARPROVIDER_HANDLER_ID_STRING = "314e041b-53c9-4481";
        public const ulong ANGULARPROVIDER_INJECTABLE_ID = 0xa96014aaf94db40a;
        public const string ANGULARPROVIDER_INJECTABLE_ID_STRING = "a960-14aaf94db40a";
        public const string ANGULARPROVIDER_Injectable = "{" + ANGULARPROVIDER_HANDLER_ID_STRING + "-" + ANGULARPROVIDER_INJECTABLE_ID_STRING + "}";
        public const ulong ANGULARPROVIDER_COMPONENT_ID = 0xbb7a0ff1a7aed1d8;
        public const string ANGULARPROVIDER_COMPONENT_ID_STRING = "bb7a-0ff1a7aed1d8";
        public const string ANGULARPROVIDER_Component = "{" + ANGULARPROVIDER_HANDLER_ID_STRING + "-" + ANGULARPROVIDER_COMPONENT_ID_STRING + "}";
        public const ulong ANGULAR_HANDLER_ID = 0xfdc57789ee4748c4;
        public const string ANGULAR_HANDLER_ID_STRING = "fdc57789-ee47-48c4";
        public const ulong ANGULAR_COMPONENT_ID = 0x9a2996d9339f6472;
        public const string ANGULAR_COMPONENT_ID_STRING = "9a29-96d9339f6472";
        public const string ANGULAR_Component = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_COMPONENT_ID_STRING + "}";
        public const ulong ANGULAR_NGZONE_ID = 0x8daee62163e4bb01;
        public const string ANGULAR_NGZONE_ID_STRING = "8dae-e62163e4bb01";
        public const string ANGULAR_NgZone = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_NGZONE_ID_STRING + "}";
        public const ulong ANGULAR_VIEWCHILD_ID = 0xb491481845506a70;
        public const string ANGULAR_VIEWCHILD_ID_STRING = "b491-481845506a70";
        public const string ANGULAR_ViewChild = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_VIEWCHILD_ID_STRING + "}";
        public const ulong ANGULAR_ONINIT_ID = 0x83145c72de11be8c;
        public const string ANGULAR_ONINIT_ID_STRING = "8314-5c72de11be8c";
        public const string ANGULAR_OnInit = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_ONINIT_ID_STRING + "}";
        public const ulong ANGULARROUTER_HANDLER_ID = 0xc8c531bfd78c46eb;
        public const string ANGULARROUTER_HANDLER_ID_STRING = "c8c531bf-d78c-46eb";
        public const ulong ANGULARROUTER_ROUTER_ID = 0x80ad91f99502c8c4;
        public const string ANGULARROUTER_ROUTER_ID_STRING = "80ad-91f99502c8c4";
        public const string ANGULARROUTER_Router = "{" + ANGULARROUTER_HANDLER_ID_STRING + "-" + ANGULARROUTER_ROUTER_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_HANDLER_ID = 0x7063777132ad40ca;
        public const string ANGULARVALIDATOR_HANDLER_ID_STRING = "70637771-32ad-40ca";
        public const ulong ANGULARVALIDATOR_VALIDATORS_ID = 0x8e9ae43261c4369b;
        public const string ANGULARVALIDATOR_VALIDATORS_ID_STRING = "8e9a-e43261c4369b";
        public const string ANGULARVALIDATOR_Validators = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_VALIDATORS_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_FORMGROUP_ID = 0xa505f54d399a3786;
        public const string ANGULARVALIDATOR_FORMGROUP_ID_STRING = "a505-f54d399a3786";
        public const string ANGULARVALIDATOR_FormGroup = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_FORMGROUP_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_FORMCONTROL_ID = 0x92420ce5fb65c1d7;
        public const string ANGULARVALIDATOR_FORMCONTROL_ID_STRING = "9242-0ce5fb65c1d7";
        public const string ANGULARVALIDATOR_FormControl = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_FORMCONTROL_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_ABSTRACTCONTROL_ID = 0xb26366b00995666a;
        public const string ANGULARVALIDATOR_ABSTRACTCONTROL_ID_STRING = "b263-66b00995666a";
        public const string ANGULARVALIDATOR_AbstractControl = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_ABSTRACTCONTROL_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_VALIDATORFN_ID = 0xb17ede735dde5cca;
        public const string ANGULARVALIDATOR_VALIDATORFN_ID_STRING = "b17e-de735dde5cca";
        public const string ANGULARVALIDATOR_ValidatorFn = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_VALIDATORFN_ID_STRING + "}";
        public const ulong ANGULARFORMS_HANDLER_ID = 0x1f3bd1a9d03745ef;
        public const string ANGULARFORMS_HANDLER_ID_STRING = "1f3bd1a9-d037-45ef";
        public const ulong ANGULARFORMS_VALIDATORS_ID = 0x970876f53eaf4a31;
        public const string ANGULARFORMS_VALIDATORS_ID_STRING = "9708-76f53eaf4a31";
        public const string ANGULARFORMS_Validators = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_VALIDATORS_ID_STRING + "}";
        public const ulong ANGULARFORMS_FORMGROUP_ID = 0x84039b6efd096dca;
        public const string ANGULARFORMS_FORMGROUP_ID_STRING = "8403-9b6efd096dca";
        public const string ANGULARFORMS_FormGroup = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_FORMGROUP_ID_STRING + "}";
        public const ulong ANGULARFORMS_FORMCONTROL_ID = 0x94dde3e51a4b38ee;
        public const string ANGULARFORMS_FORMCONTROL_ID_STRING = "94dd-e3e51a4b38ee";
        public const string ANGULARFORMS_FormControl = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_FORMCONTROL_ID_STRING + "}";
        public const ulong ANGULARFORMS_ABSTRACTCONTROL_ID = 0xaf43f346209ca356;
        public const string ANGULARFORMS_ABSTRACTCONTROL_ID_STRING = "af43-f346209ca356";
        public const string ANGULARFORMS_AbstractControl = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_ABSTRACTCONTROL_ID_STRING + "}";
        public const ulong ANGULARFORMS_VALIDATORFN_ID = 0xbaf525cf62d20478;
        public const string ANGULARFORMS_VALIDATORFN_ID_STRING = "baf5-25cf62d20478";
        public const string ANGULARFORMS_ValidatorFn = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_VALIDATORFN_ID_STRING + "}";
        public const ulong IONIC_HANDLER_ID = 0x718b6d26498b4498;
        public const string IONIC_HANDLER_ID_STRING = "718b6d26-498b-4498";
        public const ulong IONIC_NAVCONTROLLER_ID = 0xbfea7f68c6080f6c;
        public const string IONIC_NAVCONTROLLER_ID_STRING = "bfea-7f68c6080f6c";
        public const string IONIC_NavController = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_NAVCONTROLLER_ID_STRING + "}";
        public const ulong IONIC_LOADINGCONTROLLER_ID = 0xaf6fc0d19865e8c1;
        public const string IONIC_LOADINGCONTROLLER_ID_STRING = "af6f-c0d19865e8c1";
        public const string IONIC_LoadingController = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_LOADINGCONTROLLER_ID_STRING + "}";
        public const ulong IONIC_IONROUTEROUTLET_ID = 0xb59eba10ea4ea684;
        public const string IONIC_IONROUTEROUTLET_ID_STRING = "b59e-ba10ea4ea684";
        public const string IONIC_IonRouterOutlet = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_IONROUTEROUTLET_ID_STRING + "}";
        public const ulong IONIC_TOASTCONTROLLER_ID = 0x80b92a021e19cc77;
        public const string IONIC_TOASTCONTROLLER_ID_STRING = "80b9-2a021e19cc77";
        public const string IONIC_ToastController = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_TOASTCONTROLLER_ID_STRING + "}";
        public const ulong IONIC_CONFIG_ID = 0xb69f3e468c752b12;
        public const string IONIC_CONFIG_ID_STRING = "b69f-3e468c752b12";
        public const string IONIC_Config = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_CONFIG_ID_STRING + "}";
        public const string IONIC_BASIC_PAGE_IMPORTS = IONIC_NavController + ", " + IONIC_LoadingController + ", " + IONIC_IonRouterOutlet + ", " + IONIC_ToastController + ", " + IONIC_Config + ", " + NGXTRANSLATE_TranslateService;
        public const ulong IONICGRIDPAGE_HANDLER_ID = 0x8b957f53a49f4fb4;
        public const string IONICGRIDPAGE_HANDLER_ID_STRING = "8b957f53-a49f-4fb4";
        public const ulong IONICGRIDPAGE_NAVCONTROLLER_ID = 0x815aaa0208cc6be2;
        public const string IONICGRIDPAGE_NAVCONTROLLER_ID_STRING = "815a-aa0208cc6be2";
        public const string IONICGRIDPAGE_NavController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_NAVCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_LOADINGCONTROLLER_ID = 0x99d288978992f8b7;
        public const string IONICGRIDPAGE_LOADINGCONTROLLER_ID_STRING = "99d2-88978992f8b7";
        public const string IONICGRIDPAGE_LoadingController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_LOADINGCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_NAVPARAMS_ID = 0xab397486273ccb92;
        public const string IONICGRIDPAGE_NAVPARAMS_ID_STRING = "ab39-7486273ccb92";
        public const string IONICGRIDPAGE_NavParams = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_NAVPARAMS_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_TOAST_ID = 0xaea38aed203f6b0e;
        public const string IONICGRIDPAGE_TOAST_ID_STRING = "aea3-8aed203f6b0e";
        public const string IONICGRIDPAGE_Toast = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_TOAST_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_ALERTCONTROLLER_ID = 0x8dc8722b0975407b;
        public const string IONICGRIDPAGE_ALERTCONTROLLER_ID_STRING = "8dc8-722b0975407b";
        public const string IONICGRIDPAGE_AlertController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_ALERTCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_POPOVERCONTROLLER_ID = 0xa42237555ca43eb0;
        public const string IONICGRIDPAGE_POPOVERCONTROLLER_ID_STRING = "a422-37555ca43eb0";
        public const string IONICGRIDPAGE_PopoverController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_POPOVERCONTROLLER_ID_STRING + "}";
        public const string IONIC_GRID_PAGE_IMPORTS = IONICGRIDPAGE_NavController + ", " + IONICGRIDPAGE_LoadingController + ", " + IONICGRIDPAGE_NavParams + ", " + IONICGRIDPAGE_Toast + ", " + IONICGRIDPAGE_AlertController + ", " + IONICGRIDPAGE_PopoverController + ", " + NGXTRANSLATE_TranslateService + ", " + AGGRIDANGULAR_AgGridNg2 + ", " + AGGRID_ICellEditorComp + ", " + RXJSMAP_Default;
        public const ulong IONICEDITPAGE_HANDLER_ID = 0xe5335495105e4463;
        public const string IONICEDITPAGE_HANDLER_ID_STRING = "e5335495-105e-4463";
        public const ulong IONICEDITPAGE_NAVCONTROLLER_ID = 0xa3c1bd3afa08c25b;
        public const string IONICEDITPAGE_NAVCONTROLLER_ID_STRING = "a3c1-bd3afa08c25b";
        public const string IONICEDITPAGE_NavController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_NAVCONTROLLER_ID_STRING + "}";
        public const ulong IONICEDITPAGE_LOADINGCONTROLLER_ID = 0xa0f3728e05a99b82;
        public const string IONICEDITPAGE_LOADINGCONTROLLER_ID_STRING = "a0f3-728e05a99b82";
        public const string IONICEDITPAGE_LoadingController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_LOADINGCONTROLLER_ID_STRING + "}";
        public const ulong IONICEDITPAGE_NAVPARAMS_ID = 0xb313714f6b147770;
        public const string IONICEDITPAGE_NAVPARAMS_ID_STRING = "b313-714f6b147770";
        public const string IONICEDITPAGE_NavParams = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_NAVPARAMS_ID_STRING + "}";
        public const ulong IONICEDITPAGE_VIEWCONTROLLER_ID = 0x9fd3b934b70da956;
        public const string IONICEDITPAGE_VIEWCONTROLLER_ID_STRING = "9fd3-b934b70da956";
        public const string IONICEDITPAGE_ViewController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_VIEWCONTROLLER_ID_STRING + "}";
        public const string IONIC_EDIT_PAGE_IMPORTS = IONICEDITPAGE_NavController + ", " + IONICEDITPAGE_LoadingController + ", " + IONICEDITPAGE_NavParams + ", " + IONICEDITPAGE_ViewController + ", " + NGXTRANSLATE_TranslateService + ", " + RXJSOBSERVABLE_Observable + ", " + ANGULARFORMS_Validators + ", " + ANGULARFORMS_FormGroup + ", " + ANGULARFORMS_FormControl + ", " + ANGULARFORMS_AbstractControl + ", " + ANGULARFORMS_ValidatorFn + ", " + ANGULARTEXTMASK_TextMaskModule;
        public const ulong IONICGRIDPAGEBUILTIN_HANDLER_ID = 0xe0772fde4b5d49c4;
        public const string IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING = "e0772fde-4b5d-49c4";
        public const ulong IONICGRIDPAGEBUILTIN_GRIDPAGE_ID = 0xba5ff22fc2f0a8b6;
        public const string IONICGRIDPAGEBUILTIN_GRIDPAGE_ID_STRING = "ba5f-f22fc2f0a8b6";
        public const string IONICGRIDPAGEBUILTIN_GridPage = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_GRIDPAGE_ID_STRING + "}";
        public const ulong IONICGRIDPAGEBUILTIN_PAGENAME_ID = 0x89263ed2ffeef369;
        public const string IONICGRIDPAGEBUILTIN_PAGENAME_ID_STRING = "8926-3ed2ffeef369";
        public const string IONICGRIDPAGEBUILTIN_PageName = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_PAGENAME_ID_STRING + "}";
        public const ulong IONICGRIDPAGEBUILTIN_RECORDEXPRESSION_ID = 0x9ce4406bbdcc3f10;
        public const string IONICGRIDPAGEBUILTIN_RECORDEXPRESSION_ID_STRING = "9ce4-406bbdcc3f10";
        public const string IONICGRIDPAGEBUILTIN_RecordExpression = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_RECORDEXPRESSION_ID_STRING + "}";
        public const ulong IONICGRIDPAGEBUILTIN_EDITDELETEBUTTONS_ID = 0xbd2e9c84f466eb94;
        public const string IONICGRIDPAGEBUILTIN_EDITDELETEBUTTONS_ID_STRING = "bd2e-9c84f466eb94";
        public const string IONICGRIDPAGEBUILTIN_EditDeleteButtons = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_EDITDELETEBUTTONS_ID_STRING + "}";
        public const ulong IONICEDITPAGEBUILTIN_HANDLER_ID = 0x0c33b6139fbb4ff6;
        public const string IONICEDITPAGEBUILTIN_HANDLER_ID_STRING = "0c33b613-9fbb-4ff6";
        public const ulong IONICEDITPAGEBUILTIN_USABLEPOPOVER_ID = 0x87e641bb9dc9dcad;
        public const string IONICEDITPAGEBUILTIN_USABLEPOPOVER_ID_STRING = "87e6-41bb9dc9dcad";
        public const string IONICEDITPAGEBUILTIN_UsablePopover = "{" + IONICEDITPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICEDITPAGEBUILTIN_USABLEPOPOVER_ID_STRING + "}";
        public const ulong IONICPAGEBUILTIN_HANDLER_ID = 0x68ab5b9945ac4db3;
        public const string IONICPAGEBUILTIN_HANDLER_ID_STRING = "68ab5b99-45ac-4db3";
        public const ulong IONICPAGEBUILTIN_AUTHORIZE_ID = 0xadafa305d85ce92e;
        public const string IONICPAGEBUILTIN_AUTHORIZE_ID_STRING = "adaf-a305d85ce92e";
        public const string IONICPAGEBUILTIN_Authorize = "{" + IONICPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICPAGEBUILTIN_AUTHORIZE_ID_STRING + "}";
        public const ulong ANGULARVALIDATIONPAGEBUILTIN_HANDLER_ID = 0x0a355ef882444b8b;
        public const string ANGULARVALIDATIONPAGEBUILTIN_HANDLER_ID_STRING = "0a355ef8-8244-4b8b";
        public const ulong ANGULARVALIDATIONPAGEBUILTIN_VALIDATIONMAP_ID = 0x9759fbad6c2e21ca;
        public const string ANGULARVALIDATIONPAGEBUILTIN_VALIDATIONMAP_ID_STRING = "9759-fbad6c2e21ca";
        public const string ANGULARVALIDATIONPAGEBUILTIN_ValidationMap = "{" + ANGULARVALIDATIONPAGEBUILTIN_HANDLER_ID_STRING + "-" + ANGULARVALIDATIONPAGEBUILTIN_VALIDATIONMAP_ID_STRING + "}";
        public const string IONIC_ANGULAR_BASIC_PAGE_IMPORTS = IONIC_NavController + ", " + IONIC_LoadingController + ", " + IONIC_IonRouterOutlet + ", " + IONIC_ToastController + ", " + IONIC_Config + ", " + NGXTRANSLATE_TranslateService + ", " + IONICPAGEBUILTIN_Authorize + ", " + ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + ANGULAR_OnInit + ", " + ANGULARROUTER_Router;
        public const string IONIC_ANGULAR_SUPERTABS_PAGE_IMPORTS = IONIC_NavController + ", " + IONIC_LoadingController + ", " + IONIC_IonRouterOutlet + ", " + IONIC_ToastController + ", " + IONIC_Config + ", " + NGXTRANSLATE_TranslateService + ", " + IONICPAGEBUILTIN_Authorize + ", " + ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + ANGULAR_OnInit + ", " + ANGULARROUTER_Router + ", " + SUPERTABS_SuperTabsModule;
        public const string IONIC_ANGULAR_GRID_PAGE_IMPORTS = IONICGRIDPAGE_NavController + ", " + IONICGRIDPAGE_LoadingController + ", " + IONICGRIDPAGE_NavParams + ", " + IONICGRIDPAGE_Toast + ", " + IONICGRIDPAGE_AlertController + ", " + IONICGRIDPAGE_PopoverController + ", " + NGXTRANSLATE_TranslateService + ", " + AGGRIDANGULAR_AgGridNg2 + ", " + AGGRID_ICellEditorComp + ", " + RXJSMAP_Default + ", " + IONICPAGEBUILTIN_Authorize + ", " + ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + ANGULAR_OnInit + ", " + ANGULARROUTER_Router + ", " + IONICGRIDPAGEBUILTIN_GridPage + ", " + IONICGRIDPAGEBUILTIN_PageName + ", " + IONICGRIDPAGEBUILTIN_RecordExpression + ", " + IONICGRIDPAGEBUILTIN_EditDeleteButtons;
        public const string IONIC_ANGULAR_EDIT_PAGE_IMPORTS = ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + ANGULAR_OnInit + ", " + ANGULARROUTER_Router + ", " + IONICEDITPAGE_NavController + ", " + IONICEDITPAGE_LoadingController + ", " + IONICEDITPAGE_NavParams + ", " + IONICEDITPAGE_ViewController + ", " + NGXTRANSLATE_TranslateService + ", " + RXJSOBSERVABLE_Observable + ", " + ANGULARFORMS_Validators + ", " + ANGULARFORMS_FormGroup + ", " + ANGULARFORMS_FormControl + ", " + ANGULARFORMS_AbstractControl + ", " + ANGULARFORMS_ValidatorFn + ", " + ANGULARTEXTMASK_TextMaskModule + ", " + IONICPAGEBUILTIN_Authorize + ", " + IONICEDITPAGEBUILTIN_UsablePopover;
        public const string ANGULAR_VALIDATION_PAGE_IMPORTS = ANGULARVALIDATOR_Validators + ", " + ANGULARVALIDATOR_FormGroup + ", " + ANGULARVALIDATOR_FormControl + ", " + ANGULARVALIDATOR_AbstractControl + ", " + ANGULARVALIDATOR_ValidatorFn + ", " + ANGULARPROVIDER_Injectable + ", " + ANGULARPROVIDER_Component + ", " + NGXTRANSLATE_TranslateService + ", " + ANGULARVALIDATIONPAGEBUILTIN_ValidationMap;
    }
}

