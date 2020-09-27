
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public enum NgxTranslateModules : ulong
    {
        [ModuleImportDeclaration("TranslateService", "@ngx-translate/core")]
        TranslateService = 0xa726a75f9ffb5cd6,
    }

    public enum AgGridAngularModules : ulong
    {
        [ModuleImportDeclaration("AgGridNg2", "ag-grid-angular")]
        AgGridNg2 = 0x8ddf9d55e19d8d46,
    }

    public enum AgGridModules : ulong
    {
        [ModuleImportDeclaration("ICellEditorComp", "ag-grid")]
        ICellEditorComp = 0x8b52414785a423b5,
    }

    public enum AngularTextMaskModules : ulong
    {
        [ModuleImportDeclaration("TextMaskModule", "angular2-text-mask")]
        TextMaskModule = 0xbd338e5bea369c7f,
    }

    public enum SuperTabsModules : ulong
    {
        [ModuleImportDeclaration("SuperTabsModule", "ionic2-super-tabs")]
        SuperTabsModule = 0x8267197ba9d13fdb,
    }

    public enum LinqJavascriptModules : ulong
    {
        [ModuleImportDeclaration("List", "linq-javascript")]
        List = 0x87acac24ed9b8af8,
    }

    public enum RxJsMapModules : ulong
    {
        [ModuleImportDeclaration("rxjs/add/operator/map")]
        Default = 0x90819c16b902e08e,
    }

    public enum RxJsObservableModules : ulong
    {
        [ModuleImportDeclaration("Observable", "rxjs")]
        Observable = 0xb853df53f32bbaa5,
    }

    public enum AngularProviderModules : ulong
    {
        [ModuleImportDeclaration("Injectable", "@angular/core")]
        Injectable = 0xa45e1ee8d5ed73f4,
        [ModuleImportDeclaration("Component", "@angular/core")]
        Component = 0x8e5b372a42d7f0ba,
    }

    public enum AngularModules : ulong
    {
        [ModuleImportDeclaration("Component", "@angular/core")]
        Component = 0xb21a74672c25a9f3,
        [ModuleImportDeclaration("NgZone", "@angular/core")]
        NgZone = 0xa339ffdd95a41c97,
        [ModuleImportDeclaration("ViewChild", "@angular/core")]
        ViewChild = 0xaca1a1359d33835a,
    }

    public enum AngularValidatorModules : ulong
    {
        [ModuleImportDeclaration("Validators", "@angular/forms")]
        Validators = 0x913267cf551a482a,
        [ModuleImportDeclaration("FormGroup", "@angular/forms")]
        FormGroup = 0x96e06116bd67a3f7,
        [ModuleImportDeclaration("FormControl", "@angular/forms")]
        FormControl = 0x84784f4e856d6e3c,
        [ModuleImportDeclaration("AbstractControl", "@angular/forms")]
        AbstractControl = 0xb6c659af36e4be10,
        [ModuleImportDeclaration("ValidatorFn", "@angular/forms")]
        ValidatorFn = 0x8a835a25dbf2c334,
    }

    public enum AngularFormsModules : ulong
    {
        [ModuleImportDeclaration("Validators", "@angular/forms")]
        Validators = 0xb75b2b469ef76504,
        [ModuleImportDeclaration("FormGroup", "@angular/forms")]
        FormGroup = 0xa77094d6dc1c3fee,
        [ModuleImportDeclaration("FormControl", "@angular/forms")]
        FormControl = 0xa0478eb23ead2b75,
        [ModuleImportDeclaration("AbstractControl", "@angular/forms")]
        AbstractControl = 0x92deabf70ca81c43,
        [ModuleImportDeclaration("ValidatorFn", "@angular/forms")]
        ValidatorFn = 0x9d0130d498c85bc2,
    }

    public enum IonicModules : ulong
    {
        [ModuleImportDeclaration("IonicPage", "ionic-angular")]
        IonicPage = 0x95b52a5e26094607,
        [ModuleImportDeclaration("NavController", "ionic-angular")]
        NavController = 0xb85bdd2c26bef6cf,
        [ModuleImportDeclaration("LoadingController", "ionic-angular")]
        LoadingController = 0x92c720b9845cdcc2,
    }

    public enum IonicGridPageModules : ulong
    {
        [ModuleImportDeclaration("IonicPage", "ionic-angular")]
        IonicPage = 0x8e71d2c710ec10c9,
        [ModuleImportDeclaration("NavController", "ionic-angular")]
        NavController = 0xbfc89ed02efc73e0,
        [ModuleImportDeclaration("LoadingController", "ionic-angular")]
        LoadingController = 0xbbdc2af2415930c0,
        [ModuleImportDeclaration("NavParams", "ionic-angular")]
        NavParams = 0x97e0fa33071b2e7c,
        [ModuleImportDeclaration("ToastController", "ionic-angular")]
        ToastController = 0x888625ca0a2e2bca,
        [ModuleImportDeclaration("Toast", "ionic-angular")]
        Toast = 0x936d44c26b08f3a3,
        [ModuleImportDeclaration("AlertController", "ionic-angular")]
        AlertController = 0x81b830047492d606,
        [ModuleImportDeclaration("PopoverController", "ionic-angular")]
        PopoverController = 0xabb58d41871d1091,
    }

    public enum IonicEditPageModules : ulong
    {
        [ModuleImportDeclaration("IonicPage", "ionic-angular")]
        IonicPage = 0x89c86718d6ac48c5,
        [ModuleImportDeclaration("NavController", "ionic-angular")]
        NavController = 0x966bc7f9f6bb2d21,
        [ModuleImportDeclaration("LoadingController", "ionic-angular")]
        LoadingController = 0x9363bd3ed5aac19a,
        [ModuleImportDeclaration("NavParams", "ionic-angular")]
        NavParams = 0x8828a6513b32df04,
        [ModuleImportDeclaration("ViewController", "ionic-angular")]
        ViewController = 0xabe07b1471691b20,
        [ModuleImportDeclaration("ToastController", "ionic-angular")]
        ToastController = 0x8332c75329ef39a4,
    }

    public enum IonicGridPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("GridPage", "BuiltIn")]
        GridPage = 0x8dfa9002fdb72db8,
        [ModuleImportDeclaration("PageName", "BuiltIn")]
        PageName = 0xa9b1872120e31098,
        [ModuleImportDeclaration("RecordExpression", "BuiltIn")]
        RecordExpression = 0xa87f51c14a11bd4a,
        [ModuleImportDeclaration("EditDeleteButtons", "BuiltIn")]
        EditDeleteButtons = 0x855d71dbc8992e8a,
    }

    public enum IonicEditPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("UsablePopover", "BuiltIn")]
        UsablePopover = 0x8515c6e80bab8fe7,
    }

    public enum IonicPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("Authorize", "BuiltIn")]
        Authorize = 0xb85f3dfe43c43b84,
    }

    public enum AngularValidationPageBuiltInModules : ulong
    {
        [ModuleImportDeclaration("ValidationMap", "BuiltIn")]
        ValidationMap = 0x8fb3d8f204ebcf2b,
    }

    [ModuleImports()]
    public static class ModuleImports
    {
        public const ulong APPLICATION_IMPORT_HANDLER_ID = ulong.MinValue;
        public const ulong NGXTRANSLATE_HANDLER_ID = 0xbd5ab40468d04cc3;
        public const string NGXTRANSLATE_HANDLER_ID_STRING = "bd5ab404-68d0-4cc3";
        public const ulong NGXTRANSLATE_TRANSLATESERVICE_ID = 0xa726a75f9ffb5cd6;
        public const string NGXTRANSLATE_TRANSLATESERVICE_ID_STRING = "a726-a75f9ffb5cd6";
        public const string NGXTRANSLATE_TranslateService = "{" + NGXTRANSLATE_HANDLER_ID_STRING + "-" + NGXTRANSLATE_TRANSLATESERVICE_ID_STRING + "}";
        public const ulong AGGRIDANGULAR_HANDLER_ID = 0x36098c62666b4c87;
        public const string AGGRIDANGULAR_HANDLER_ID_STRING = "36098c62-666b-4c87";
        public const ulong AGGRIDANGULAR_AGGRIDNG2_ID = 0x8ddf9d55e19d8d46;
        public const string AGGRIDANGULAR_AGGRIDNG2_ID_STRING = "8ddf-9d55e19d8d46";
        public const string AGGRIDANGULAR_AgGridNg2 = "{" + AGGRIDANGULAR_HANDLER_ID_STRING + "-" + AGGRIDANGULAR_AGGRIDNG2_ID_STRING + "}";
        public const ulong AGGRID_HANDLER_ID = 0xb2a82cf15a0d4b5a;
        public const string AGGRID_HANDLER_ID_STRING = "b2a82cf1-5a0d-4b5a";
        public const ulong AGGRID_ICELLEDITORCOMP_ID = 0x8b52414785a423b5;
        public const string AGGRID_ICELLEDITORCOMP_ID_STRING = "8b52-414785a423b5";
        public const string AGGRID_ICellEditorComp = "{" + AGGRID_HANDLER_ID_STRING + "-" + AGGRID_ICELLEDITORCOMP_ID_STRING + "}";
        public const ulong ANGULARTEXTMASK_HANDLER_ID = 0x3eaaf666a8994e2b;
        public const string ANGULARTEXTMASK_HANDLER_ID_STRING = "3eaaf666-a899-4e2b";
        public const ulong ANGULARTEXTMASK_TEXTMASKMODULE_ID = 0xbd338e5bea369c7f;
        public const string ANGULARTEXTMASK_TEXTMASKMODULE_ID_STRING = "bd33-8e5bea369c7f";
        public const string ANGULARTEXTMASK_TextMaskModule = "{" + ANGULARTEXTMASK_HANDLER_ID_STRING + "-" + ANGULARTEXTMASK_TEXTMASKMODULE_ID_STRING + "}";
        public const ulong SUPERTABS_HANDLER_ID = 0x8e7c332a25144a26;
        public const string SUPERTABS_HANDLER_ID_STRING = "8e7c332a-2514-4a26";
        public const ulong SUPERTABS_SUPERTABSMODULE_ID = 0x8267197ba9d13fdb;
        public const string SUPERTABS_SUPERTABSMODULE_ID_STRING = "8267-197ba9d13fdb";
        public const string SUPERTABS_SuperTabsModule = "{" + SUPERTABS_HANDLER_ID_STRING + "-" + SUPERTABS_SUPERTABSMODULE_ID_STRING + "}";
        public const ulong LINQJAVASCRIPT_HANDLER_ID = 0x89a19aa5b7b04011;
        public const string LINQJAVASCRIPT_HANDLER_ID_STRING = "89a19aa5-b7b0-4011";
        public const ulong LINQJAVASCRIPT_LIST_ID = 0x87acac24ed9b8af8;
        public const string LINQJAVASCRIPT_LIST_ID_STRING = "87ac-ac24ed9b8af8";
        public const string LINQJAVASCRIPT_List = "{" + LINQJAVASCRIPT_HANDLER_ID_STRING + "-" + LINQJAVASCRIPT_LIST_ID_STRING + "}";
        public const ulong RXJSMAP_HANDLER_ID = 0xb134156360534bf2;
        public const string RXJSMAP_HANDLER_ID_STRING = "b1341563-6053-4bf2";
        public const ulong RXJSMAP_DEFAULT_ID = 0x90819c16b902e08e;
        public const string RXJSMAP_DEFAULT_ID_STRING = "9081-9c16b902e08e";
        public const string RXJSMAP_Default = "{" + RXJSMAP_HANDLER_ID_STRING + "-" + RXJSMAP_DEFAULT_ID_STRING + "}";
        public const ulong RXJSOBSERVABLE_HANDLER_ID = 0x61b2ad840fe04123;
        public const string RXJSOBSERVABLE_HANDLER_ID_STRING = "61b2ad84-0fe0-4123";
        public const ulong RXJSOBSERVABLE_OBSERVABLE_ID = 0xb853df53f32bbaa5;
        public const string RXJSOBSERVABLE_OBSERVABLE_ID_STRING = "b853-df53f32bbaa5";
        public const string RXJSOBSERVABLE_Observable = "{" + RXJSOBSERVABLE_HANDLER_ID_STRING + "-" + RXJSOBSERVABLE_OBSERVABLE_ID_STRING + "}";
        public const ulong ANGULARPROVIDER_HANDLER_ID = 0x69daea3be7b14fce;
        public const string ANGULARPROVIDER_HANDLER_ID_STRING = "69daea3b-e7b1-4fce";
        public const ulong ANGULARPROVIDER_INJECTABLE_ID = 0xa45e1ee8d5ed73f4;
        public const string ANGULARPROVIDER_INJECTABLE_ID_STRING = "a45e-1ee8d5ed73f4";
        public const string ANGULARPROVIDER_Injectable = "{" + ANGULARPROVIDER_HANDLER_ID_STRING + "-" + ANGULARPROVIDER_INJECTABLE_ID_STRING + "}";
        public const ulong ANGULARPROVIDER_COMPONENT_ID = 0x8e5b372a42d7f0ba;
        public const string ANGULARPROVIDER_COMPONENT_ID_STRING = "8e5b-372a42d7f0ba";
        public const string ANGULARPROVIDER_Component = "{" + ANGULARPROVIDER_HANDLER_ID_STRING + "-" + ANGULARPROVIDER_COMPONENT_ID_STRING + "}";
        public const ulong ANGULAR_HANDLER_ID = 0x6b8cbb05aada4892;
        public const string ANGULAR_HANDLER_ID_STRING = "6b8cbb05-aada-4892";
        public const ulong ANGULAR_COMPONENT_ID = 0xb21a74672c25a9f3;
        public const string ANGULAR_COMPONENT_ID_STRING = "b21a-74672c25a9f3";
        public const string ANGULAR_Component = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_COMPONENT_ID_STRING + "}";
        public const ulong ANGULAR_NGZONE_ID = 0xa339ffdd95a41c97;
        public const string ANGULAR_NGZONE_ID_STRING = "a339-ffdd95a41c97";
        public const string ANGULAR_NgZone = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_NGZONE_ID_STRING + "}";
        public const ulong ANGULAR_VIEWCHILD_ID = 0xaca1a1359d33835a;
        public const string ANGULAR_VIEWCHILD_ID_STRING = "aca1-a1359d33835a";
        public const string ANGULAR_ViewChild = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_VIEWCHILD_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_HANDLER_ID = 0x4407a089e7b544db;
        public const string ANGULARVALIDATOR_HANDLER_ID_STRING = "4407a089-e7b5-44db";
        public const ulong ANGULARVALIDATOR_VALIDATORS_ID = 0x913267cf551a482a;
        public const string ANGULARVALIDATOR_VALIDATORS_ID_STRING = "9132-67cf551a482a";
        public const string ANGULARVALIDATOR_Validators = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_VALIDATORS_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_FORMGROUP_ID = 0x96e06116bd67a3f7;
        public const string ANGULARVALIDATOR_FORMGROUP_ID_STRING = "96e0-6116bd67a3f7";
        public const string ANGULARVALIDATOR_FormGroup = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_FORMGROUP_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_FORMCONTROL_ID = 0x84784f4e856d6e3c;
        public const string ANGULARVALIDATOR_FORMCONTROL_ID_STRING = "8478-4f4e856d6e3c";
        public const string ANGULARVALIDATOR_FormControl = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_FORMCONTROL_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_ABSTRACTCONTROL_ID = 0xb6c659af36e4be10;
        public const string ANGULARVALIDATOR_ABSTRACTCONTROL_ID_STRING = "b6c6-59af36e4be10";
        public const string ANGULARVALIDATOR_AbstractControl = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_ABSTRACTCONTROL_ID_STRING + "}";
        public const ulong ANGULARVALIDATOR_VALIDATORFN_ID = 0x8a835a25dbf2c334;
        public const string ANGULARVALIDATOR_VALIDATORFN_ID_STRING = "8a83-5a25dbf2c334";
        public const string ANGULARVALIDATOR_ValidatorFn = "{" + ANGULARVALIDATOR_HANDLER_ID_STRING + "-" + ANGULARVALIDATOR_VALIDATORFN_ID_STRING + "}";
        public const ulong ANGULARFORMS_HANDLER_ID = 0xaa0266a578d1486d;
        public const string ANGULARFORMS_HANDLER_ID_STRING = "aa0266a5-78d1-486d";
        public const ulong ANGULARFORMS_VALIDATORS_ID = 0xb75b2b469ef76504;
        public const string ANGULARFORMS_VALIDATORS_ID_STRING = "b75b-2b469ef76504";
        public const string ANGULARFORMS_Validators = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_VALIDATORS_ID_STRING + "}";
        public const ulong ANGULARFORMS_FORMGROUP_ID = 0xa77094d6dc1c3fee;
        public const string ANGULARFORMS_FORMGROUP_ID_STRING = "a770-94d6dc1c3fee";
        public const string ANGULARFORMS_FormGroup = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_FORMGROUP_ID_STRING + "}";
        public const ulong ANGULARFORMS_FORMCONTROL_ID = 0xa0478eb23ead2b75;
        public const string ANGULARFORMS_FORMCONTROL_ID_STRING = "a047-8eb23ead2b75";
        public const string ANGULARFORMS_FormControl = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_FORMCONTROL_ID_STRING + "}";
        public const ulong ANGULARFORMS_ABSTRACTCONTROL_ID = 0x92deabf70ca81c43;
        public const string ANGULARFORMS_ABSTRACTCONTROL_ID_STRING = "92de-abf70ca81c43";
        public const string ANGULARFORMS_AbstractControl = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_ABSTRACTCONTROL_ID_STRING + "}";
        public const ulong ANGULARFORMS_VALIDATORFN_ID = 0x9d0130d498c85bc2;
        public const string ANGULARFORMS_VALIDATORFN_ID_STRING = "9d01-30d498c85bc2";
        public const string ANGULARFORMS_ValidatorFn = "{" + ANGULARFORMS_HANDLER_ID_STRING + "-" + ANGULARFORMS_VALIDATORFN_ID_STRING + "}";
        public const ulong IONIC_HANDLER_ID = 0x7ffa43b398214720;
        public const string IONIC_HANDLER_ID_STRING = "7ffa43b3-9821-4720";
        public const ulong IONIC_IONICPAGE_ID = 0x95b52a5e26094607;
        public const string IONIC_IONICPAGE_ID_STRING = "95b5-2a5e26094607";
        public const string IONIC_IonicPage = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_IONICPAGE_ID_STRING + "}";
        public const ulong IONIC_NAVCONTROLLER_ID = 0xb85bdd2c26bef6cf;
        public const string IONIC_NAVCONTROLLER_ID_STRING = "b85b-dd2c26bef6cf";
        public const string IONIC_NavController = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_NAVCONTROLLER_ID_STRING + "}";
        public const ulong IONIC_LOADINGCONTROLLER_ID = 0x92c720b9845cdcc2;
        public const string IONIC_LOADINGCONTROLLER_ID_STRING = "92c7-20b9845cdcc2";
        public const string IONIC_LoadingController = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_LOADINGCONTROLLER_ID_STRING + "}";
        public const string IONIC_BASIC_PAGE_IMPORTS = IONIC_IonicPage + ", " + IONIC_NavController + ", " + IONIC_LoadingController + ", " + NGXTRANSLATE_TranslateService;
        public const ulong IONICGRIDPAGE_HANDLER_ID = 0x6a6528c4e1f541f3;
        public const string IONICGRIDPAGE_HANDLER_ID_STRING = "6a6528c4-e1f5-41f3";
        public const ulong IONICGRIDPAGE_IONICPAGE_ID = 0x8e71d2c710ec10c9;
        public const string IONICGRIDPAGE_IONICPAGE_ID_STRING = "8e71-d2c710ec10c9";
        public const string IONICGRIDPAGE_IonicPage = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_IONICPAGE_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_NAVCONTROLLER_ID = 0xbfc89ed02efc73e0;
        public const string IONICGRIDPAGE_NAVCONTROLLER_ID_STRING = "bfc8-9ed02efc73e0";
        public const string IONICGRIDPAGE_NavController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_NAVCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_LOADINGCONTROLLER_ID = 0xbbdc2af2415930c0;
        public const string IONICGRIDPAGE_LOADINGCONTROLLER_ID_STRING = "bbdc-2af2415930c0";
        public const string IONICGRIDPAGE_LoadingController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_LOADINGCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_NAVPARAMS_ID = 0x97e0fa33071b2e7c;
        public const string IONICGRIDPAGE_NAVPARAMS_ID_STRING = "97e0-fa33071b2e7c";
        public const string IONICGRIDPAGE_NavParams = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_NAVPARAMS_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_TOASTCONTROLLER_ID = 0x888625ca0a2e2bca;
        public const string IONICGRIDPAGE_TOASTCONTROLLER_ID_STRING = "8886-25ca0a2e2bca";
        public const string IONICGRIDPAGE_ToastController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_TOASTCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_TOAST_ID = 0x936d44c26b08f3a3;
        public const string IONICGRIDPAGE_TOAST_ID_STRING = "936d-44c26b08f3a3";
        public const string IONICGRIDPAGE_Toast = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_TOAST_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_ALERTCONTROLLER_ID = 0x81b830047492d606;
        public const string IONICGRIDPAGE_ALERTCONTROLLER_ID_STRING = "81b8-30047492d606";
        public const string IONICGRIDPAGE_AlertController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_ALERTCONTROLLER_ID_STRING + "}";
        public const ulong IONICGRIDPAGE_POPOVERCONTROLLER_ID = 0xabb58d41871d1091;
        public const string IONICGRIDPAGE_POPOVERCONTROLLER_ID_STRING = "abb5-8d41871d1091";
        public const string IONICGRIDPAGE_PopoverController = "{" + IONICGRIDPAGE_HANDLER_ID_STRING + "-" + IONICGRIDPAGE_POPOVERCONTROLLER_ID_STRING + "}";
        public const string IONIC_GRID_PAGE_IMPORTS = IONICGRIDPAGE_IonicPage + ", " + IONICGRIDPAGE_NavController + ", " + IONICGRIDPAGE_LoadingController + ", " + IONICGRIDPAGE_NavParams + ", " + IONICGRIDPAGE_ToastController + ", " + IONICGRIDPAGE_Toast + ", " + IONICGRIDPAGE_AlertController + ", " + IONICGRIDPAGE_PopoverController + ", " + NGXTRANSLATE_TranslateService + ", " + AGGRIDANGULAR_AgGridNg2 + ", " + AGGRID_ICellEditorComp + ", " + LINQJAVASCRIPT_List + ", " + RXJSMAP_Default;
        public const ulong IONICEDITPAGE_HANDLER_ID = 0x5094d1f9fee94817;
        public const string IONICEDITPAGE_HANDLER_ID_STRING = "5094d1f9-fee9-4817";
        public const ulong IONICEDITPAGE_IONICPAGE_ID = 0x89c86718d6ac48c5;
        public const string IONICEDITPAGE_IONICPAGE_ID_STRING = "89c8-6718d6ac48c5";
        public const string IONICEDITPAGE_IonicPage = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_IONICPAGE_ID_STRING + "}";
        public const ulong IONICEDITPAGE_NAVCONTROLLER_ID = 0x966bc7f9f6bb2d21;
        public const string IONICEDITPAGE_NAVCONTROLLER_ID_STRING = "966b-c7f9f6bb2d21";
        public const string IONICEDITPAGE_NavController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_NAVCONTROLLER_ID_STRING + "}";
        public const ulong IONICEDITPAGE_LOADINGCONTROLLER_ID = 0x9363bd3ed5aac19a;
        public const string IONICEDITPAGE_LOADINGCONTROLLER_ID_STRING = "9363-bd3ed5aac19a";
        public const string IONICEDITPAGE_LoadingController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_LOADINGCONTROLLER_ID_STRING + "}";
        public const ulong IONICEDITPAGE_NAVPARAMS_ID = 0x8828a6513b32df04;
        public const string IONICEDITPAGE_NAVPARAMS_ID_STRING = "8828-a6513b32df04";
        public const string IONICEDITPAGE_NavParams = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_NAVPARAMS_ID_STRING + "}";
        public const ulong IONICEDITPAGE_VIEWCONTROLLER_ID = 0xabe07b1471691b20;
        public const string IONICEDITPAGE_VIEWCONTROLLER_ID_STRING = "abe0-7b1471691b20";
        public const string IONICEDITPAGE_ViewController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_VIEWCONTROLLER_ID_STRING + "}";
        public const ulong IONICEDITPAGE_TOASTCONTROLLER_ID = 0x8332c75329ef39a4;
        public const string IONICEDITPAGE_TOASTCONTROLLER_ID_STRING = "8332-c75329ef39a4";
        public const string IONICEDITPAGE_ToastController = "{" + IONICEDITPAGE_HANDLER_ID_STRING + "-" + IONICEDITPAGE_TOASTCONTROLLER_ID_STRING + "}";
        public const string IONIC_EDIT_PAGE_IMPORTS = IONICEDITPAGE_IonicPage + ", " + IONICEDITPAGE_NavController + ", " + IONICEDITPAGE_LoadingController + ", " + IONICEDITPAGE_NavParams + ", " + IONICEDITPAGE_ViewController + ", " + IONICEDITPAGE_ToastController + ", " + NGXTRANSLATE_TranslateService + ", " + LINQJAVASCRIPT_List + ", " + RXJSOBSERVABLE_Observable + ", " + ANGULARFORMS_Validators + ", " + ANGULARFORMS_FormGroup + ", " + ANGULARFORMS_FormControl + ", " + ANGULARFORMS_AbstractControl + ", " + ANGULARFORMS_ValidatorFn + ", " + ANGULARTEXTMASK_TextMaskModule;
        public const ulong IONICGRIDPAGEBUILTIN_HANDLER_ID = 0x8dd225dfdd5340c1;
        public const string IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING = "8dd225df-dd53-40c1";
        public const ulong IONICGRIDPAGEBUILTIN_GRIDPAGE_ID = 0x8dfa9002fdb72db8;
        public const string IONICGRIDPAGEBUILTIN_GRIDPAGE_ID_STRING = "8dfa-9002fdb72db8";
        public const string IONICGRIDPAGEBUILTIN_GridPage = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_GRIDPAGE_ID_STRING + "}";
        public const ulong IONICGRIDPAGEBUILTIN_PAGENAME_ID = 0xa9b1872120e31098;
        public const string IONICGRIDPAGEBUILTIN_PAGENAME_ID_STRING = "a9b1-872120e31098";
        public const string IONICGRIDPAGEBUILTIN_PageName = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_PAGENAME_ID_STRING + "}";
        public const ulong IONICGRIDPAGEBUILTIN_RECORDEXPRESSION_ID = 0xa87f51c14a11bd4a;
        public const string IONICGRIDPAGEBUILTIN_RECORDEXPRESSION_ID_STRING = "a87f-51c14a11bd4a";
        public const string IONICGRIDPAGEBUILTIN_RecordExpression = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_RECORDEXPRESSION_ID_STRING + "}";
        public const ulong IONICGRIDPAGEBUILTIN_EDITDELETEBUTTONS_ID = 0x855d71dbc8992e8a;
        public const string IONICGRIDPAGEBUILTIN_EDITDELETEBUTTONS_ID_STRING = "855d-71dbc8992e8a";
        public const string IONICGRIDPAGEBUILTIN_EditDeleteButtons = "{" + IONICGRIDPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICGRIDPAGEBUILTIN_EDITDELETEBUTTONS_ID_STRING + "}";
        public const ulong IONICEDITPAGEBUILTIN_HANDLER_ID = 0xde315c8c000648f7;
        public const string IONICEDITPAGEBUILTIN_HANDLER_ID_STRING = "de315c8c-0006-48f7";
        public const ulong IONICEDITPAGEBUILTIN_USABLEPOPOVER_ID = 0x8515c6e80bab8fe7;
        public const string IONICEDITPAGEBUILTIN_USABLEPOPOVER_ID_STRING = "8515-c6e80bab8fe7";
        public const string IONICEDITPAGEBUILTIN_UsablePopover = "{" + IONICEDITPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICEDITPAGEBUILTIN_USABLEPOPOVER_ID_STRING + "}";
        public const ulong IONICPAGEBUILTIN_HANDLER_ID = 0xfb3cc3e12d5240cf;
        public const string IONICPAGEBUILTIN_HANDLER_ID_STRING = "fb3cc3e1-2d52-40cf";
        public const ulong IONICPAGEBUILTIN_AUTHORIZE_ID = 0xb85f3dfe43c43b84;
        public const string IONICPAGEBUILTIN_AUTHORIZE_ID_STRING = "b85f-3dfe43c43b84";
        public const string IONICPAGEBUILTIN_Authorize = "{" + IONICPAGEBUILTIN_HANDLER_ID_STRING + "-" + IONICPAGEBUILTIN_AUTHORIZE_ID_STRING + "}";
        public const ulong ANGULARVALIDATIONPAGEBUILTIN_HANDLER_ID = 0xb9ecfb9923154865;
        public const string ANGULARVALIDATIONPAGEBUILTIN_HANDLER_ID_STRING = "b9ecfb99-2315-4865";
        public const ulong ANGULARVALIDATIONPAGEBUILTIN_VALIDATIONMAP_ID = 0x8fb3d8f204ebcf2b;
        public const string ANGULARVALIDATIONPAGEBUILTIN_VALIDATIONMAP_ID_STRING = "8fb3-d8f204ebcf2b";
        public const string ANGULARVALIDATIONPAGEBUILTIN_ValidationMap = "{" + ANGULARVALIDATIONPAGEBUILTIN_HANDLER_ID_STRING + "-" + ANGULARVALIDATIONPAGEBUILTIN_VALIDATIONMAP_ID_STRING + "}";
        public const string IONIC_ANGULAR_BASIC_PAGE_IMPORTS = IONIC_IonicPage + ", " + IONIC_NavController + ", " + IONIC_LoadingController + ", " + NGXTRANSLATE_TranslateService + ", " + IONICPAGEBUILTIN_Authorize + ", " + ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + LINQJAVASCRIPT_List;
        public const string IONIC_ANGULAR_SUPERTABS_PAGE_IMPORTS = IONIC_IonicPage + ", " + IONIC_NavController + ", " + IONIC_LoadingController + ", " + NGXTRANSLATE_TranslateService + ", " + IONICPAGEBUILTIN_Authorize + ", " + ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + SUPERTABS_SuperTabsModule;
        public const string IONIC_ANGULAR_GRID_PAGE_IMPORTS = IONICGRIDPAGE_IonicPage + ", " + IONICGRIDPAGE_NavController + ", " + IONICGRIDPAGE_LoadingController + ", " + IONICGRIDPAGE_NavParams + ", " + IONICGRIDPAGE_ToastController + ", " + IONICGRIDPAGE_Toast + ", " + IONICGRIDPAGE_AlertController + ", " + IONICGRIDPAGE_PopoverController + ", " + NGXTRANSLATE_TranslateService + ", " + AGGRIDANGULAR_AgGridNg2 + ", " + AGGRID_ICellEditorComp + ", " + LINQJAVASCRIPT_List + ", " + RXJSMAP_Default + ", " + IONICPAGEBUILTIN_Authorize + ", " + ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + IONICGRIDPAGEBUILTIN_GridPage + ", " + IONICGRIDPAGEBUILTIN_PageName + ", " + IONICGRIDPAGEBUILTIN_RecordExpression + ", " + IONICGRIDPAGEBUILTIN_EditDeleteButtons;
        public const string IONIC_ANGULAR_EDIT_PAGE_IMPORTS = ANGULAR_Component + ", " + ANGULAR_NgZone + ", " + ANGULAR_ViewChild + ", " + IONICEDITPAGE_IonicPage + ", " + IONICEDITPAGE_NavController + ", " + IONICEDITPAGE_LoadingController + ", " + IONICEDITPAGE_NavParams + ", " + IONICEDITPAGE_ViewController + ", " + IONICEDITPAGE_ToastController + ", " + NGXTRANSLATE_TranslateService + ", " + LINQJAVASCRIPT_List + ", " + RXJSOBSERVABLE_Observable + ", " + ANGULARFORMS_Validators + ", " + ANGULARFORMS_FormGroup + ", " + ANGULARFORMS_FormControl + ", " + ANGULARFORMS_AbstractControl + ", " + ANGULARFORMS_ValidatorFn + ", " + ANGULARTEXTMASK_TextMaskModule + ", " + IONICPAGEBUILTIN_Authorize + ", " + IONICEDITPAGEBUILTIN_UsablePopover;
        public const string ANGULAR_VALIDATION_PAGE_IMPORTS = ANGULARVALIDATOR_Validators + ", " + ANGULARVALIDATOR_FormGroup + ", " + ANGULARVALIDATOR_FormControl + ", " + ANGULARVALIDATOR_AbstractControl + ", " + ANGULARVALIDATOR_ValidatorFn + ", " + ANGULARPROVIDER_Injectable + ", " + ANGULARPROVIDER_Component + ", " + NGXTRANSLATE_TranslateService + ", " + ANGULARVALIDATIONPAGEBUILTIN_ValidationMap;
    }
}

