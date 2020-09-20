using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    public enum UIKind
    {
        [KindGuid(UIKindGuids.None)]
        None,
        [KindGuid(UIKindGuids.BlankPage, UIFeatureKind.Standard)]
        BlankPage,
        [KindGuid(UIKindGuids.WelcomePage)]
        WelcomePage,
        [KindGuid(UIKindGuids.LoginPage, UIFeatureKind.Edit)]
        LoginPage,
        [KindGuid(UIKindGuids.RegisterPage, UIFeatureKind.Edit)]
        RegisterPage,
        [KindGuid(UIKindGuids.StandalonePage)]
        StandalonePage,
        [KindGuid(UIKindGuids.SlidingTabsPage)]
        SlidingTabsPage,
        [KindGuid(UIKindGuids.EditPage, UIFeatureKind.Edit)]
        EditPage,
        [KindGuid(UIKindGuids.EditPopup, UIFeatureKind.EditPopup)]
        EditPopup,
        [KindGuid(UIKindGuids.EditPageWithNavigationGrids)]
        EditPageWithNavigationGrids,
        [KindGuid(UIKindGuids.TabsPage, UIFeatureKind.Standard)]
        TabsPage,
        [KindGuid(UIKindGuids.NavigationGridPage)]
        NavigationGridPage,
        [KindGuid(UIKindGuids.NavigationGridPageWithEdits)]
        NavigationGridPageWithEdits,
        [KindGuid(UIKindGuids.ModalDialog)]
        ModalDialog,
        [KindGuid(UIKindGuids.CustomPage)]
        CustomPage,
        [KindGuid(UIKindGuids.SlidingTabsComponent, UIFeatureKind.CustomComponent)]
        SlidingTabsComponent,
        [KindGuid(UIKindGuids.TabsComponent, UIFeatureKind.StandardComponent)]
        TabsComponent,
        [KindGuid(UIKindGuids.NavigationGridComponentWithEdits, UIFeatureKind.CustomComponent)]
        NavigationGridComponentWithEdits,
    }

    public enum UIFeatureKind
    {
        None,
        Custom,
        Standard,
        Edit,
        EditPopup,
        CustomComponent,
        StandardComponent,
    }

    public static class UIKindGuids
    {
        public const string Any = "{4482B57C-25F3-4FD2-BE06-0990AFA7FD76}";
        public const string Element = "{3464CC4D-179D-4C91-849F-DCC596B32A82}";
        public const string StaticContainer = "{C302B688-53E9-459B-BBF3-A7C73D8F208F}";
        public const string None = "{9C8C4A33-80F6-4A8A-8ADA-D9866E0617AD}";
        public const string BlankPage = "{6657B2CC-29A5-455D-A00A-EE2297213F99}";
        public const string WelcomePage = "{C9EEEAA6-2CA9-448F-B244-9DEFDDB61304}";
        public const string LoginPage = "{80DB3E9B-ADB3-4B4C-B7CA-23802A8F6432}";
        public const string RegisterPage = "{2829A23C-950C-4866-A4D5-F82A84C5A91E}";
        public const string StandalonePage = "{0C30006F-7B50-4687-BAB2-76911908818A}";
        public const string SlidingTabsPage = "{BB39AB56-B255-4B01-899D-39C6586C909F}";
        public const string EditPage = "{A1F514E8-7D7C-47D9-BB51-F2976C846783}";
        public const string EditPopup = "{FFDB6703-EB53-4408-A3D9-785AC8E117D2}";
        public const string EditPageWithNavigationGrids = "{070E0904-6D95-4F28-BEF5-AFA0D3F82BCD}";
        public const string TabsPage = "{653E2080-CA2E-432B-BF62-DBAF2F1616FC}";
        public const string NavigationGridPage = "{CA1528CE-38F7-4F30-A301-CB370DFF6E54}";
        public const string NavigationGridPageWithEdits = "{4473E585-3D61-4886-80D5-30E6FD00AEA4}";
        public const string ModalDialog = "{0DFA3985-B251-44CC-AF04-00BE35586861}";
        public const string CustomPage = "{F1F38993-87A8-42CB-B1A6-38F42C53A263}";
        public const string SlidingTabsComponent = "{BB39AB56-B255-4B01-899D-39C6586C909F}";
        public const string TabsComponent = "{653E2080-CA2E-432B-BF62-DBAF2F1616FC}";
        public const string NavigationGridComponentWithEdits = "{4473E585-3D61-4886-80D5-30E6FD00AEA4}";
    }
}