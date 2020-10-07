// file:	DataAnnotations\UIKind.cs
//
// summary:	Implements the kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Values that represent kinds. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public enum UIKind
    {
        /// <summary>   An enum constant representing the not specified option. </summary>
        NotSpecified,
        /// <summary>   An enum constant representing the none option. </summary>
        [KindGuid(UIKindGuids.None)]
        None,
        /// <summary>   An enum constant representing the blank page option. </summary>
        [KindGuid(UIKindGuids.BlankPage, UIFeatureKind.Standard)]
        BlankPage,
        /// <summary>   An enum constant representing the welcome page option. </summary>
        [KindGuid(UIKindGuids.WelcomePage)]
        WelcomePage,
        /// <summary>   An enum constant representing the login page option. </summary>
        [KindGuid(UIKindGuids.LoginPage, UIFeatureKind.Edit)]
        LoginPage,
        /// <summary>   An enum constant representing the register page option. </summary>
        [KindGuid(UIKindGuids.RegisterPage, UIFeatureKind.Edit)]
        RegisterPage,
        /// <summary>   An enum constant representing the standalone page option. </summary>
        [KindGuid(UIKindGuids.StandalonePage)]
        StandalonePage,
        /// <summary>   An enum constant representing the sliding tabs page option. </summary>
        [KindGuid(UIKindGuids.SlidingTabsPage)]
        SlidingTabsPage,
        /// <summary>   An enum constant representing the edit page option. </summary>
        [KindGuid(UIKindGuids.EditPage, UIFeatureKind.Edit)]
        EditPage,
        /// <summary>   An enum constant representing the edit popup option. </summary>
        [KindGuid(UIKindGuids.EditPopup, UIFeatureKind.EditPopup)]
        EditPopup,
        /// <summary>   An enum constant representing the edit page with navigation grids option. </summary>
        [KindGuid(UIKindGuids.EditPageWithNavigationGrids)]
        EditPageWithNavigationGrids,
        /// <summary>   An enum constant representing the tabs page option. </summary>
        [KindGuid(UIKindGuids.TabsPage, UIFeatureKind.Standard)]
        TabsPage,
        /// <summary>   An enum constant representing the navigation grid page option. </summary>
        [KindGuid(UIKindGuids.NavigationGridPage)]
        NavigationGridPage,
        /// <summary>   An enum constant representing the navigation grid page with edits option. </summary>
        [KindGuid(UIKindGuids.NavigationGridPageWithEdits)]
        NavigationGridPageWithEdits,
        /// <summary>   An enum constant representing the modal dialog option. </summary>
        [KindGuid(UIKindGuids.ModalDialog)]
        ModalDialog,
        /// <summary>   An enum constant representing the custom page option. </summary>
        [KindGuid(UIKindGuids.CustomPage)]
        CustomPage,
        /// <summary>   An enum constant representing the sliding tabs component option. </summary>
        [KindGuid(UIKindGuids.SlidingTabsComponent, UIFeatureKind.CustomComponent)]
        SlidingTabsComponent,
        /// <summary>   An enum constant representing the tabs component option. </summary>
        [KindGuid(UIKindGuids.TabsComponent, UIFeatureKind.StandardComponent)]
        TabsComponent,
        /// <summary>
        /// An enum constant representing the navigation grid component with edits option.
        /// </summary>
        [KindGuid(UIKindGuids.NavigationGridComponentWithEdits, UIFeatureKind.CustomComponent)]
        NavigationGridComponentWithEdits,
    }

    /// <summary>   Values that represent feature kinds. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public enum UIFeatureKind
    {
        /// <summary>   An enum constant representing the none option. </summary>
        None,
        /// <summary>   An enum constant representing the custom option. </summary>
        Custom,
        /// <summary>   An enum constant representing the standard option. </summary>
        Standard,
        /// <summary>   An enum constant representing the edit option. </summary>
        Edit,
        /// <summary>   An enum constant representing the edit popup option. </summary>
        EditPopup,
        /// <summary>   An enum constant representing the custom component option. </summary>
        CustomComponent,
        /// <summary>   An enum constant representing the standard component option. </summary>
        StandardComponent,
    }

    /// <summary>   A kind guids. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public static class UIKindGuids
    {
        /// <summary>   any. </summary>
        public const string Any = "{4482B57C-25F3-4FD2-BE06-0990AFA7FD76}";
        /// <summary>   The element. </summary>
        public const string Element = "{3464CC4D-179D-4C91-849F-DCC596B32A82}";
        /// <summary>   The static container. </summary>
        public const string StaticContainer = "{C302B688-53E9-459B-BBF3-A7C73D8F208F}";
        /// <summary>   The none. </summary>
        public const string None = "{9C8C4A33-80F6-4A8A-8ADA-D9866E0617AD}";
        /// <summary>   The blank page. </summary>
        public const string BlankPage = "{6657B2CC-29A5-455D-A00A-EE2297213F99}";
        /// <summary>   The welcome page. </summary>
        public const string WelcomePage = "{C9EEEAA6-2CA9-448F-B244-9DEFDDB61304}";
        /// <summary>   The login page. </summary>
        public const string LoginPage = "{80DB3E9B-ADB3-4B4C-B7CA-23802A8F6432}";
        /// <summary>   The register page. </summary>
        public const string RegisterPage = "{2829A23C-950C-4866-A4D5-F82A84C5A91E}";
        /// <summary>   The standalone page. </summary>
        public const string StandalonePage = "{0C30006F-7B50-4687-BAB2-76911908818A}";
        /// <summary>   The sliding tabs page. </summary>
        public const string SlidingTabsPage = "{BB39AB56-B255-4B01-899D-39C6586C909F}";
        /// <summary>   The edit page. </summary>
        public const string EditPage = "{A1F514E8-7D7C-47D9-BB51-F2976C846783}";
        /// <summary>   The edit popup. </summary>
        public const string EditPopup = "{FFDB6703-EB53-4408-A3D9-785AC8E117D2}";
        /// <summary>   The edit page with navigation grids. </summary>
        public const string EditPageWithNavigationGrids = "{070E0904-6D95-4F28-BEF5-AFA0D3F82BCD}";
        /// <summary>   The tabs page. </summary>
        public const string TabsPage = "{653E2080-CA2E-432B-BF62-DBAF2F1616FC}";
        /// <summary>   The navigation grid page. </summary>
        public const string NavigationGridPage = "{CA1528CE-38F7-4F30-A301-CB370DFF6E54}";
        /// <summary>   The navigation grid page with edits. </summary>
        public const string NavigationGridPageWithEdits = "{4473E585-3D61-4886-80D5-30E6FD00AEA4}";
        /// <summary>   The modal dialog. </summary>
        public const string ModalDialog = "{0DFA3985-B251-44CC-AF04-00BE35586861}";
        /// <summary>   The custom page. </summary>
        public const string CustomPage = "{F1F38993-87A8-42CB-B1A6-38F42C53A263}";
        /// <summary>   The sliding tabs component. </summary>
        public const string SlidingTabsComponent = "{BB39AB56-B255-4B01-899D-39C6586C909F}";
        /// <summary>   The tabs component. </summary>
        public const string TabsComponent = "{653E2080-CA2E-432B-BF62-DBAF2F1616FC}";
        /// <summary>   The navigation grid component with edits. </summary>
        public const string NavigationGridComponentWithEdits = "{4473E585-3D61-4886-80D5-30E6FD00AEA4}";
    }
}