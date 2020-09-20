
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public enum NgxTranslateModules : ulong
    {
        [ModuleImportDeclaration("TranslateService", "@ngx-translate/core")]
        TranslateService = 0xad5c38a0873f8075,
    }

    public enum AgGridAngularModules : ulong
    {
        [ModuleImportDeclaration("AgGridNg2", "ag-grid-angular")]
        AgGridNg2 = 0x8a638d361495625f,
    }

    public enum AgGridModules : ulong
    {
        [ModuleImportDeclaration("ICellEditorComp", "ag-grid")]
        ICellEditorComp = 0xaeac562b00266454,
    }

    public enum AngularTextMaskModules : ulong
    {
        [ModuleImportDeclaration("TextMaskModule", "angular2-text-mask")]
        TextMaskModule = 0xb747daa2994770d7,
    }

    public enum SuperTabsModules : ulong
    {
        [ModuleImportDeclaration("SuperTabsModule", "ionic2-super-tabs")]
        SuperTabsModule = 0x84626f4ef1073b1c,
    }

    public enum LinqJavascriptModules : ulong
    {
        [ModuleImportDeclaration("List", "linq-javascript")]
        List = 0x831d1fee2af6ece2,
    }

    public enum RxJsMapModules : ulong
    {
        [ModuleImportDeclaration("rxjs/add/operator/map")]
        Default = 0x8b8367b298041822,
    }

    public enum RxJsObservableModules : ulong
    {
        [ModuleImportDeclaration("Observable", "rxjs")]
        Observable = 0x82c41117ab13d479,
    }

    public enum AngularProviderModules : ulong
    {
        [ModuleImportDeclaration("Injectable", "@angular/core")]
        Injectable = 0xbb40467c40cfd4db,
        [ModuleImportDeclaration("Component", "@angular/core")]
        Component = 0x87386c02224f14f0,
    }

    public enum AngularModules : ulong
    {
        [ModuleImportDeclaration("Component", "@angular/core")]
        Component = 0x9bb610bea5fe9df0,
        [ModuleImportDeclaration("NgZone", "@angular/core")]
        NgZone = 0x85cd6c0d02cafbeb,
        [ModuleImportDeclaration("ViewChild", "@angular/core")]
        ViewChild = 0xb9370b526fe77cb3,
    }

    public enum AngularValidatorModules : ulong
    {
        [ModuleImportDeclaration("Validators", "@angular/forms")]
        Validators = 0x84acc8656626ddfa,
        [ModuleImportDeclaration("FormGroup", "@angular/forms")]
        FormGroup = 0xb4a04a0077a8cce1,
        [ModuleImportDeclaration("FormControl", "@angular/forms")]
        FormControl = 0xace658d02158d708,
        [ModuleImportDeclaration("AbstractControl", "@angular/forms")]
        AbstractControl = 0x930730896a4046be,
        [ModuleImportDeclaration("ValidatorFn", "@angular/forms")]
        ValidatorFn = 0x96f8efb00e2e3c4f,
    }

    public enum AngularFormsModules : ulong
    {
        [ModuleImportDeclaration("Validators", "@angular/forms")]
        Validators = 0xbd7b2bd4de2987ed,
        [ModuleImportDeclaration("FormGroup", "@angular/forms")]
        FormGroup = 0xb046bef1127818b8,
        [ModuleImportDeclaration("FormControl", "@angular/forms")]
        FormControl = 0x9f3ac5dd594af1b1,
        [ModuleImportDeclaration("AbstractControl", "@angular/forms")]
        AbstractControl = 0x88a2c188147cdcf8,
        [ModuleImportDeclaration("ValidatorFn", "@angular/forms")]
        ValidatorFn = 0x86f8682c8e6e621a,
    }

    public enum IonicModules : ulong
    {
        [ModuleImportDeclaration("IonicPage", "ionic-angular")]
        IonicPage = 0x9e0f1145602850d6,
        [ModuleImportDeclaration("NavController", "ionic-angular")]
        NavController = 0xa500a40479f6d16d,
        [ModuleImportDeclaration("LoadingController", "ionic-angular")]
        LoadingController = 0x8746dfbb08fae8d9,
    }

    public enum IonicGridPageModules : ulong
    {
        [ModuleImportDeclaration("IonicPage", "ionic-angular")]
        IonicPage = 0x897781ad61a40674,
        [ModuleImportDeclaration("NavController", "ionic-angular")]
        NavController = 0xa5211c02659e18e1,
        [ModuleImportDeclaration("LoadingController", "ionic-angular")]
        LoadingController = 0x9df8f432a3441fb0,
        [ModuleImportDeclaration("NavParams", "ionic-angular")]
        NavParams = 0x983a411f7cf80479,
        [ModuleImportDeclaration("ToastController", "ionic-angular")]
        ToastController = 0x8e8216c975cbd6da,
        [ModuleImportDeclaration("Toast", "ionic-angular")]
        Toast = 0x80bedab1d782f81d,
        [ModuleImportDeclaration("AlertController", "ionic-angular")]
        AlertController = 0xb48dee2c0b03817f,
        [ModuleImportDeclaration("PopoverController", "ionic-angular")]
        PopoverController = 0x9fa2ba47197bb397,
    }

    public enum IonicEditPageModules : ulong
    {
        [ModuleImportDeclaration("IonicPage", "ionic-angular")]
        IonicPage = 0xbfd8517278cf0602,
        [ModuleImportDeclaration("NavController", "ionic-angular")]
        NavController = 0xb07813ce47ab09dc,
        [ModuleImportDeclaration("LoadingController", "ionic-angular")]
        LoadingController = 0xb39e99b50d06224c,
        [ModuleImportDeclaration("NavParams", "ionic-angular")]
        NavParams = 0xb7cf3a14e45bfee5,
        [ModuleImportDeclaration("ViewController", "ionic-angular")]
        ViewController = 0x89d90c6cf3860f98,
        [ModuleImportDeclaration("ToastController", "ionic-angular")]
        ToastController = 0x8639e61744561f0a,
    }

    public enum IonicGridPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("GridPage", "BuiltIn")]
        GridPage = 0xbbba26a7aace7e9b,
        [ModuleImportDeclaration("PageName", "BuiltIn")]
        PageName = 0x8f65c8490e85976b,
        [ModuleImportDeclaration("RecordExpression", "BuiltIn")]
        RecordExpression = 0xba63a246f62697de,
        [ModuleImportDeclaration("EditDeleteButtons", "BuiltIn")]
        EditDeleteButtons = 0x8f102e61a6a7fcdd,
    }

    public enum IonicEditPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("UsablePopover", "BuiltIn")]
        UsablePopover = 0x91d64dd86db1f4e5,
    }

    public enum IonicPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("Authorize", "BuiltIn")]
        Authorize = 0x86ea1a545cf691ae,
    }

    public enum AngularValidationPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("ValidationMap", "BuiltIn")]
        ValidationMap = 0x84aab8a5818c8ce0,
    }

    [ModuleImports()]
    public static class ModuleImports
    {
        public const ulong APPLICATION_IMPORT_HANDLER_ID = ulong.MinValue;
        public const ulong NGXTRANSLATE_HANDLER_ID = 0x8b76463b4b7543b8;
        public const string NGXTRANSLATE_HANDLER_ID_STRING = "8b76463b-4b75-43b8";
        public const ulong NGXTRANSLATE_TRANSLATESERVICE_ID = 0xad5c38a0873f8075;
        public const string NGXTRANSLATE_TRANSLATESERVICE_ID_STRING = "ad5c-38a0873f8075";
        public const string NGXTRANSLATE_TranslateService = "{" + NGXTRANSLATE_HANDLER_ID_STRING + "-" + NGXTRANSLATE_TRANSLATESERVICE_ID_STRING + "}";
        public const ulong AGGRIDANGULAR_HANDLER_ID = 0x8a31672aee41436e;
        public const string AGGRIDANGULAR_HANDLER_ID_STRING = "8a31672a-ee41-436e";
        public const ulong AGGRIDANGULAR_AGGRIDNG2_ID = 0x8a638d361495625f;
        public const string AGGRIDANGULAR_AGGRIDNG2_ID_STRING = "8a63-8d361495625f";
        public const string AGGRIDANGULAR_AgGridNg2 = "{" + AGGRIDANGULAR_HANDLER_ID_STRING + "-" + AGGRIDANGULAR_AGGRIDNG2_ID_STRING + "}";
        public const ulong AGGRID_HANDLER_ID = 0xd72ba07ffaf84326;
        public const string AGGRID_HANDLER_ID_STRING = "d72ba07f-faf8-4326";
        public const ulong AGGRID_ICELLEDITORCOMP_ID = 0xaeac562b00266454;
        public const string AGGRID_ICELLEDITORCOMP_ID_STRING = "aeac-562b00266454";
        public const string AGGRID_ICellEditorComp = "{" + AGGRID_HANDLER_ID_STRING + "-" + AGGRID_ICELLEDITORCOMP_ID_STRING + "}";
        public const ulong ANGULARTEXTMASK_HANDLER_ID = 0xadc667a37b824347;
        public const string ANGULARTEXTMASK_HANDLER_ID_STRING = "adc667a3-7b82-4347";
        public const ulong ANGULARTEXTMASK_TEXTMASKMODULE_ID = 0xb747daa2994770d7;
        public const string ANGULARTEXTMASK_TEXTMASKMODULE_ID_STRING = "b747-daa2994770d7";
        public const string ANGULARTEXTMASK_TextMaskModule = "{" + ANGULARTEXTMASK_HANDLER_ID_STRING + "-" + ANGULARTEXTMASK_TEXTMASKMODULE_ID_STRING + "}";
        public const ulong SUPERTABS_HANDLER_ID = 0xc25c9b04a08148c3;
        public const string SUPERTABS_HANDLER_ID_STRING = "c25c9b04-a081-48c3";
        public const ulong SUPERTABS_SUPERTABSMODULE_ID = 0x84626f4ef1073b1c;
        public const string SUPERTABS_SUPERTABSMODULE_ID_STRING = "8462-6f4ef1073b1c";
        public const string SUPERTABS_SuperTabsModule = "{" + SUPERTABS_HANDLER_ID_STRING + "-" + SUPERTABS_SUPERTABSMODULE_ID_STRING + "}";
        public const ulong LINQJAVASCRIPT_HANDLER_ID = 0x6221707c45b34abe;
        public const string LINQJAVASCRIPT_HANDLER_ID_STRING = "6221707c-45b3-4abe";
        public const ulong LINQJAVASCRIPT_LIST_ID = 0x831d1fee2af6ece2;
        public const string LINQJAVASCRIPT_LIST_ID_STRING = "831d-1fee2af6ece2";
        public const string LINQJAVASCRIPT_List = "{" + LINQJAVASCRIPT_HANDLER_ID_STRING + "-" + LINQJAVASCRIPT_LIST_ID_STRING + "}";
        public const ulong RXJSMAP_HANDLER_ID = 0x6372e88a48cc4cae;
        public const string RXJSMAP_HANDLER_ID_STRING = "6372e88a-48cc-4cae";
        public const ulong RXJSMAP_DEFAULT_ID = 0x8b8367b298041822;
        public const string RXJSMAP_DEFAULT_ID_STRING = "8b83-67b298041822";
        public const string RXJSMAP_Default = "{" + RXJSMAP_HANDLER_ID_STRING + "-" + RXJSMAP_DEFAULT_ID_STRING + "}";
        public const ulong RXJSOBSERVABLE_HANDLER_ID = 0x961321828c744d56;
        public const string RXJSOBSERVABLE_HANDLER_ID_STRING = "96132182-8c74-4d56";
        public const ulong RXJSOBSERVABLE_OBSERVABLE_ID = 0x82c41117ab13d479;
        public const string RXJSOBSERVABLE_OBSERVABLE_ID_STRING = "82c4-1117ab13d479";
        public const string RXJSOBSERVABLE_Observable = "{" + RXJSOBSERVABLE_HANDLER_ID_STRING + "-" + RXJSOBSERVABLE_OBSERVABLE_ID_STRING + "}";
        public const ulong ANGULARPROVIDER_HANDLER_ID = 0x5ff5cb32e0e14ed6;
        public const string ANGULARPROVIDER_HANDLER_ID_STRING = "5ff5cb32-e0e1-4ed6";
        public const ulong ANGULARPROVIDER_INJECTABLE_ID = 0xbb40467c40cfd4db;
        public const string ANGULARPROVIDER_INJECTABLE_ID_STRING = "bb40-467c40cfd4db";
        public const string ANGULARPROVIDER_Injectable = "{" + ANGULARPROVIDER_HANDLER_ID_STRING + "-" + ANGULARPROVIDER_INJECTABLE_ID_STRING + "}";
        public const ulong ANGULARPROVIDER_COMPONENT_ID = 0x87386c02224f14f0;
        public const string ANGULARPROVIDER_COMPONENT_ID_STRING = "8738-6c02224f14f0";
        public const string ANGULARPROVIDER_Component = "{" + ANGULARPROVIDER_HANDLER_ID_STRING + "-" + ANGULARPROVIDER_COMPONENT_ID_STRING + "}";
        public const ulong ANGULAR_HANDLER_ID = 0xe9fa5459b39b44d8;
        public const string ANGULAR_HANDLER_ID_STRING = "e9fa5459-b39b-44d8";
        public const ulong ANGULAR_COMPONENT_ID = 0x9bb610bea5fe9df0;
        public const string ANGULAR_COMPONENT_ID_STRING = "9bb6-10bea5fe9df0";
        public const string ANGULAR_Component = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_COMPONENT_ID_STRING + "}";
        public const ulong ANGULAR_NGZONE_ID = 0x85cd6c0d02cafbeb;
        public const string ANGULAR_NGZONE_ID_STRING = "85cd-6c0d02cafbeb";
        public const string ANGULAR_NgZone = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_NGZONE_ID_STRING + "}";
        public const ulong ANGULAR_VIEWCHILD_ID = 0xb9370b526fe77cb3;
        public const string ANGULAR_VIEWCHILD_ID_STRING = "b937-0b526fe77cb3";
        public const string ANGULAR_ViewChild = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_VIEWCHILD_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_HANDLER_ID = 0x6c02d8bdb52a4b67;
        public const string ANGULARVALIDATOR_HANDLER_ID_STRING = "6c02d8bd-b52a-4b67";
        public const ulong ANGULARVALIDATOR_VALIDATORS_ID = 0x84acc8656626ddfa;
        public const string ANGULARVALIDATOR_VALIDATORS_ID_STRING = "84ac-c8656626ddfa";
        public const string ANGULARVALIDATOR_Validators = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_VALIDATORS_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_FORMGROUP_ID = 0xb4a04a0077a8cce1;
        public const string ANGULARVALIDATOR_FORMGROUP_ID_STRING = "b4a0-4a0077a8cce1";
        public const string ANGULARVALIDATOR_FormGroup = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_FORMGROUP_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_FORMCONTROL_ID = 0xace658d02158d708;
        public const string ANGULARVALIDATOR_FORMCONTROL_ID_STRING = "ace6-58d02158d708";
        public const string ANGULARVALIDATOR_FormControl = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_FORMCONTROL_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_ABSTRACTCONTROL_ID = 0x930730896a4046be;
        public const string ANGULARVALIDATOR_ABSTRACTCONTROL_ID_STRING = "9307-30896a4046be";
        public const string ANGULARVALIDATOR_AbstractControl = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_ABSTRACTCONTROL_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_VALIDATORFN_ID = 0x96f8efb00e2e3c4f;
        public const string ANGULARVALIDATOR_VALIDATORFN_ID_STRING = "96f8-efb00e2e3c4f";
        public const string ANGULARVALIDATOR_ValidatorFn = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_VALIDATORFN_ID_STRING + "}";
        public const ulong ANGULARFORMS_HANDLER_ID = 0x969fbc02551c4a4c;
        public const string ANGULARFORMS_HANDLER_ID_STRING = "969fbc02-551c-4a4c";
        public const ulong ANGULARFORMS_VALIDATORS_ID = 0xbd7b2bd4de2987ed;
        public const string ANGULARFORMS_VALIDATORS_ID_STRING = "bd7b-2bd4de2987ed";
        public const string ANGULARFORMS_Validators = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_VALIDATORS_ID_STRING + "}";
        public const ulong ANGULARFORMS_FORMGROUP_ID = 0xb046bef1127818b8;
        public const string ANGULARFORMS_FORMGROUP_ID_STRING = "b046-bef1127818b8";
        public const string ANGULARFORMS_FormGroup = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_FORMGROUP_ID_STRING + "}";
        public const ulong ANGULARFORMS_FORMCONTROL_ID = 0x9f3ac5dd594af1b1;
        public const string ANGULARFORMS_FORMCONTROL_ID_STRING = "9f3a-c5dd594af1b1";
        public const string ANGULARFORMS_FormControl = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_FORMCONTROL_ID_STRING + "}";
        public const ulong ANGULARFORMS_ABSTRACTCONTROL_ID = 0x88a2c188147cdcf8;
        public const string ANGULARFORMS_ABSTRACTCONTROL_ID_STRING = "88a2-c188147cdcf8";
        public const string ANGULARFORMS_AbstractControl = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_ABSTRACTCONTROL_ID_STRING + "}";
        public const ulong ANGULARFORMS_VALIDATORFN_ID = 0x86f8682c8e6e621a;
        public const string ANGULARFORMS_VALIDATORFN_ID_STRING = "86f8-682c8e6e621a";
        public const string ANGULARFORMS_ValidatorFn = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_VALIDATORFN_ID_STRING + "}";
        public const ulong IONIC_HANDLER_ID = 0xe6b79ab8fe1e4b53;
        public const string IONIC_HANDLER_ID_STRING = "e6b79ab8-fe1e-4b53";
        public const ulong IONIC_IONICPAGE_ID = 0x9e0f1145602850d6;
        public const string IONIC_IONICPAGE_ID_STRING = "9e0f-1145602850d6";
        public const string IONIC_IonicPage = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_IONICPAGE_ID_STRING + "}";
        public const ulong IONIC_NAVCONTROLLER_ID = 0xa500a40479f6d16d;
        public const string IONIC_NAVCONTROLLER_ID_STRING = "a500-a40479f6d16d";
        public const string IONIC_NavController = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_NAVCONTROLLER_ID_STRING + "}";
        public const ulong IONIC_LOADINGCONTROLLER_ID = 0x8746dfbb08fae8d9;
        public const string IONIC_LOADINGCONTROLLER_ID_STRING = "8746-dfbb08fae8d9";
        public const string IONIC_LoadingController = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_LOADINGCONTROLLER_ID_STRING + "}";
        public const string IONIC_BASIC_PAGE_IMPORTS = IONIC_IonicPage + ", " + IONIC_NavController + ", " + IONIC_LoadingController + ", " + NGXTRANSLATE_TranslateService;
        public const ulong IONICGRIDPAGE_HANDLER_ID = 0x870693f8c0244c07;
        public const string IONICGRIDPAGE_HANDLER_ID_STRING = "870693f8-c024-4c07";
        public const ulong IONICGRIDPAGE_IONICPAGE_ID = 0x897781ad61a40674;
        public const string IONICGRIDPAGE_IONICPAGE_ID_STRING = "8977-81ad61a40674";
        public const string IONICGRIDPAGE_IonicPage = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_IONICPAGE_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_NAVCONTROLLER_ID = 0xa5211c02659e18e1;
        public const string IONICGRIDPAGE_NAVCONTROLLER_ID_STRING = "a521-1c02659e18e1";
        public const string IONICGRIDPAGE_NavController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_NAVCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_LOADINGCONTROLLER_ID = 0x9df8f432a3441fb0;
        public const string IONICGRIDPAGE_LOADINGCONTROLLER_ID_STRING = "9df8-f432a3441fb0";
        public const string IONICGRIDPAGE_LoadingController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_LOADINGCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_NAVPARAMS_ID = 0x983a411f7cf80479;
        public const string IONICGRIDPAGE_NAVPARAMS_ID_STRING = "983a-411f7cf80479";
        public const string IONICGRIDPAGE_NavParams = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_NAVPARAMS_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_TOASTCONTROLLER_ID = 0x8e8216c975cbd6da;
        public const string IONICGRIDPAGE_TOASTCONTROLLER_ID_STRING = "8e82-16c975cbd6da";
        public const string IONICGRIDPAGE_ToastController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_TOASTCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_TOAST_ID = 0x80bedab1d782f81d;
        public const string IONICGRIDPAGE_TOAST_ID_STRING = "80be-dab1d782f81d";
        public const string IONICGRIDPAGE_Toast = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_TOAST_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_ALERTCONTROLLER_ID = 0xb48dee2c0b03817f;
        public const string IONICGRIDPAGE_ALERTCONTROLLER_ID_STRING = "b48d-ee2c0b03817f";
        public const string IONICGRIDPAGE_AlertController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_ALERTCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_POPOVERCONTROLLER_ID = 0x9fa2ba47197bb397;
        public const string IONICGRIDPAGE_POPOVERCONTROLLER_ID_STRING = "9fa2-ba47197bb397";
        public const string IONICGRIDPAGE_PopoverController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_POPOVERCONTROLLER_ID_STRING + "}";
        public const string IONIC_GRID_PAGE_IMPORTS = IONICGRIDPAGE_IonicPage + ", " + IONICGRIDPAGE_NavController + ", " + IONICGRIDPAGE_LoadingController + ", " + IONICGRIDPAGE_NavParams + ", " + IONICGRIDPAGE_ToastController + ", " + IONICGRIDPAGE_Toast + ", " + IONICGRIDPAGE_AlertController + ", " + IONICGRIDPAGE_PopoverController + ", " + NGXTRANSLATE_TranslateService + ", " + AGGRIDANGULAR_AgGridNg2 + ", " + AGGRID_ICellEditorComp + ", " + LINQJAVASCRIPT_List + ", " + RXJSMAP_Default;
        public const ulong IONICEDITPAGE_HANDLER_ID = 0x703fec64b8664cda;
        public const string IONICEDITPAGE_HANDLER_ID_STRING = "703fec64-b866-4cda";
        public const ulong IONICEDITPAGE_IONICPAGE_ID = 0xbfd8517278cf0602;
        public const string IONICEDITPAGE_IONICPAGE_ID_STRING = "bfd8-517278cf0602";
        public const string IONICEDITPAGE_IonicPage = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_IONICPAGE_ID_STRING + "}";
        public const ulong IONICEDITPAGE_NAVCONTROLLER_ID = 0xb07813ce47ab09dc;
        public const string IONICEDITPAGE_NAVCONTROLLER_ID_STRING = "b078-13ce47ab09dc";
        public const string IONICEDITPAGE_NavController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_NAVCONTROLLER_ID_STRING + "}";
        public const ulong IONICEDITPAGE_LOADINGCONTROLLER_ID = 0xb39e99b50d06224c;
        public const string IONICEDITPAGE_LOADINGCONTROLLER_ID_STRING = "b39e-99b50d06224c";
        public const string IONICEDITPAGE_LoadingController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_LOADINGCONTROLLER_ID_STRING + "}";
        public const ulong IONICEDITPAGE_NAVPARAMS_ID = 0xb7cf3a14e45bfee5;
        public const string IONICEDITPAGE_NAVPARAMS_ID_STRING = "b7cf-3a14e45bfee5";
        public const string IONICEDITPAGE_NavParams = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_NAVPARAMS_ID_STRING + "}";
        public const ulong IONICEDITPAGE_VIEWCONTROLLER_ID = 0x89d90c6cf3860f98;
        public const string IONICEDITPAGE_VIEWCONTROLLER_ID_STRING = "89d9-0c6cf3860f98";
        public const string IONICEDITPAGE_ViewController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_VIEWCONTROLLER_ID_STRING + "}";
        public const ulong IONICEDITPAGE_TOASTCONTROLLER_ID = 0x8639e61744561f0a;
        public const string IONICEDITPAGE_TOASTCONTROLLER_ID_STRING = "8639-e61744561f0a";
        public const string IONICEDITPAGE_ToastController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_TOASTCONTROLLER_ID_STRING + "}";
        public const string IONIC_EDIT_PAGE_IMPORTS = IONICEDITPAGE_IonicPage + ", " + IONICEDITPAGE_NavController + ", " + IONICEDITPAGE_LoadingController + ", " + IONICEDITPAGE_NavParams + ", " + IONICEDITPAGE_ViewController + ", " + IONICEDITPAGE_ToastController + ", " + NGXTRANSLATE_TranslateService + ", " + LINQJAVASCRIPT_List + ", " + RXJSOBSERVABLE_Observable + ", " + ANGULARFORMS_Validators + ", " + ANGULARFORMS_FormGroup + ", " + ANGULARFORMS_FormControl + ", " + ANGULARFORMS_AbstractControl + ", " + ANGULARFORMS_ValidatorFn + ", " + ANGULARTEXTMASK_TextMaskModule;
        public const ulong IONICGRIDPAGEBUILTIN_HANDLER_ID = 0xf9c72d55c8fd447a;
        public const string IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING = "f9c72d55-c8fd-447a";
        public const ulong IONICGRIDPAGEBUILTIN_GRIDPAGE_ID = 0xbbba26a7aace7e9b;
        public const string IONICGRIDPAGEBUILTIN_GRIDPAGE_ID_STRING = "bbba-26a7aace7e9b";
        public const string IONICGRIDPAGEBUILTIN_GridPage = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_GRIDPAGE_ID_STRING + "}";
        public const ulong IONICGRIDPAGEBUILTIN_PAGENAME_ID = 0x8f65c8490e85976b;
        public const string IONICGRIDPAGEBUILTIN_PAGENAME_ID_STRING = "8f65-c8490e85976b";
        public const string IONICGRIDPAGEBUILTIN_PageName = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_PAGENAME_ID_STRING + "}";
        public const ulong IONICGRIDPAGEBUILTIN_RECORDEXPRESSION_ID = 0xba63a246f62697de;
        public const string IONICGRIDPAGEBUILTIN_RECORDEXPRESSION_ID_STRING = "ba63-a246f62697de";
        public const string IONICGRIDPAGEBUILTIN_RecordExpression = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_RECORDEXPRESSION_ID_STRING + "}";
        public const ulong IONICGRIDPAGEBUILTIN_EDITDELETEBUTTONS_ID = 0x8f102e61a6a7fcdd;
        public const string IONICGRIDPAGEBUILTIN_EDITDELETEBUTTONS_ID_STRING = "8f10-2e61a6a7fcdd";
        public const string IONICGRIDPAGEBUILTIN_EditDeleteButtons = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_EDITDELETEBUTTONS_ID_STRING + "}";
        public const ulong IONICEDITPAGEBUILTIN_HANDLER_ID = 0xf5a9cda364e2494a;
        public const string IONICEDITPAGEBUILTIN_HANDLER_ID_STRING = "f5a9cda3-64e2-494a";
        public const ulong IONICEDITPAGEBUILTIN_USABLEPOPOVER_ID = 0x91d64dd86db1f4e5;
        public const string IONICEDITPAGEBUILTIN_USABLEPOPOVER_ID_STRING = "91d6-4dd86db1f4e5";
        public const string IONICEDITPAGEBUILTIN_UsablePopover = "{" + IONICEDITPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICEDITPAGEBUILTIN_USABLEPOPOVER_ID_STRING + "}";
        public const ulong IONICPAGEBUILTIN_HANDLER_ID = 0xee1a907715bb4ab0;
        public const string IONICPAGEBUILTIN_HANDLER_ID_STRING = "ee1a9077-15bb-4ab0";
        public const ulong IONICPAGEBUILTIN_AUTHORIZE_ID = 0x86ea1a545cf691ae;
        public const string IONICPAGEBUILTIN_AUTHORIZE_ID_STRING = "86ea-1a545cf691ae";
        public const string IONICPAGEBUILTIN_Authorize = "{" + IONICPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICPAGEBUILTIN_AUTHORIZE_ID_STRING + "}";
        public const ulong ANGULARVALIDATIONPAGEBUILTIN_HANDLER_ID = 0x06607430b72f479e;
        public const string ANGULARVALIDATIONPAGEBUILTIN_HANDLER_ID_STRING = "06607430-b72f-479e";
        public const ulong ANGULARVALIDATIONPAGEBUILTIN_VALIDATIONMAP_ID = 0x84aab8a5818c8ce0;
        public const string ANGULARVALIDATIONPAGEBUILTIN_VALIDATIONMAP_ID_STRING = "84aa-b8a5818c8ce0";
        public const string ANGULARVALIDATIONPAGEBUILTIN_ValidationMap = "{" + ANGULARVALIDATIONPAGEBUILTIN_HANDLER_ID_STRING + "-" + ANGULARVALIDATIONPAGEBUILTIN_VALIDATIONMAP_ID_STRING + "}";
        public const string IONIC_ANGULAR_BASIC_PAGE_IMPORTS = IONIC_IonicPage + ", " + IONIC_NavController + ", " + IONIC_LoadingController + ", " + NGXTRANSLATE_TranslateService + ", " + IONICPAGEBUILTIN_Authorize + ", " + ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + LINQJAVASCRIPT_List;
        public const string IONIC_ANGULAR_SUPERTABS_PAGE_IMPORTS = IONIC_IonicPage + ", " + IONIC_NavController + ", " + IONIC_LoadingController + ", " + NGXTRANSLATE_TranslateService + ", " + IONICPAGEBUILTIN_Authorize + ", " + ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + SUPERTABS_SuperTabsModule;
        public const string IONIC_ANGULAR_GRID_PAGE_IMPORTS = IONICGRIDPAGE_IonicPage + ", " + IONICGRIDPAGE_NavController + ", " + IONICGRIDPAGE_LoadingController + ", " + IONICGRIDPAGE_NavParams + ", " + IONICGRIDPAGE_ToastController + ", " + IONICGRIDPAGE_Toast + ", " + IONICGRIDPAGE_AlertController + ", " + IONICGRIDPAGE_PopoverController + ", " + NGXTRANSLATE_TranslateService + ", " + AGGRIDANGULAR_AgGridNg2 + ", " + AGGRID_ICellEditorComp + ", " + LINQJAVASCRIPT_List + ", " + RXJSMAP_Default + ", " + IONICPAGEBUILTIN_Authorize + ", " + ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + IONICGRIDPAGEBUILTIN_GridPage + ", " + IONICGRIDPAGEBUILTIN_PageName + ", " + IONICGRIDPAGEBUILTIN_RecordExpression + ", " + IONICGRIDPAGEBUILTIN_EditDeleteButtons;
        public const string IONIC_ANGULAR_EDIT_PAGE_IMPORTS = ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + IONICEDITPAGE_IonicPage + ", " + IONICEDITPAGE_NavController + ", " + IONICEDITPAGE_LoadingController + ", " + IONICEDITPAGE_NavParams + ", " + IONICEDITPAGE_ViewController + ", " + IONICEDITPAGE_ToastController + ", " + NGXTRANSLATE_TranslateService + ", " + LINQJAVASCRIPT_List + ", " + RXJSOBSERVABLE_Observable + ", " + ANGULARFORMS_Validators + ", " + ANGULARFORMS_FormGroup + ", " + ANGULARFORMS_FormControl + ", " + ANGULARFORMS_AbstractControl + ", " + ANGULARFORMS_ValidatorFn + ", " + ANGULARTEXTMASK_TextMaskModule + ", " + IONICPAGEBUILTIN_Authorize + ", " + IONICEDITPAGEBUILTIN_UsablePopover;
        public const string ANGULAR_VALIDATION_PAGE_IMPORTS = ANGULARVALIDATOR_Validators + ", " + ANGULARVALIDATOR_FormGroup + ", " + ANGULARVALIDATOR_FormControl + ", " + ANGULARVALIDATOR_AbstractControl + ", " + ANGULARVALIDATOR_ValidatorFn + ", " + ANGULARPROVIDER_Injectable + ", " + ANGULARPROVIDER_Component + ", " + NGXTRANSLATE_TranslateService + ", " + ANGULARVALIDATIONPAGEBUILTIN_ValidationMap;
    }
}

