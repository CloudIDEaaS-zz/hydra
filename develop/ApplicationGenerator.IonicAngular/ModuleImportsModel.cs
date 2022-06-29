using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public enum AngularModules : ulong
    {
        [ModuleImportDeclaration("Component", "@angular/core")]
        Component = 0xABF262A8A52D22DA
    }

    public enum IonicModules : ulong
    {
        Page = 0xA6C005A667D4BD10,
        NavController = 0x893EF453C865CCCC,
    }

    public static class ModuleImports
    {
        public const ulong ANGULAR_HANDLER_ID = 0x073D634A74B84E58;
        public const string ANGULAR_HANDLER_ID_STRING = "073D634A-74B8-4E58";

        public const ulong ANGULAR_CORE_COMPONENT_ID = (ulong) AngularModules.Component;
        public const string ANGULAR_CORE_COMPONENT_ID_STRING = "ABF2-62A8A52D22DA";
        public const string ANGULAR_CORE_Component = "{" + ANGULAR_HANDLER_ID_STRING + "-" + ANGULAR_CORE_COMPONENT_ID_STRING + "}";

        public const ulong IONIC_HANDLER_ID = 0xDB99885028F04FE8;
        public const string IONIC_HANDLER_ID_STRING = "DB998850-28F0-4FE8";
        public const ulong IONIC_PAGE_ID = (ulong) IonicModules.Page;
        public const string IONIC_PAGE_ID_STRING = "A6C0-05A667D4BD10";
        public const ulong IONIC_NAVCONTROLLER_ID = (ulong) IonicModules.NavController;
        public const string IONIC_NAVCONTROLLER_ID_STRING = "893E-F453C865CCCC";

        public const string IONIC_IonicPage = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_PAGE_ID_STRING + "}";
        public const string IONIC_NavController = "{" + IONIC_HANDLER_ID_STRING + "-" + IONIC_NAVCONTROLLER_ID_STRING + "}";
        public const string TranslateService = "{B1143A9B-5F86-4686-8863-6A3B1AAC8901}";

        public const string IONIC_BASIC_PAGE_IMPORTS = ANGULAR_CORE_Component + ", " + TranslateService + ", " + IONIC_IonicPage + ", " + IONIC_NavController;
    }
}
