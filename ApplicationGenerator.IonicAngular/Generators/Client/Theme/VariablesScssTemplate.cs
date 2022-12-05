﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace AbstraX.Generators.Client.Theme
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Utils;
    using AbstraX;
    using AbstraX.Generators;
    using AbstraX.Angular;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Client\Theme\VariablesScssTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class VariablesScssTemplate : AbstraX.Generators.Base.TemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("/*\r\n * Ionic Variables and Theming\r\n * ------------------------------------------" +
                    "----------------------------------\r\n */\r\n\r\n:root {\r\n    /** background **/\r\n    " +
                    "--ion-background-color: ");
            
            #line 20 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Client\Theme\VariablesScssTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.ResourceData.FindColor("BackgroundColor", "#ffffff")));
            
            #line default
            #line hidden
            this.Write(";\r\n    /** primary **/\r\n    --ion-color-primary: ");
            
            #line 22 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Client\Theme\VariablesScssTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.ResourceData.FindColor("PrimaryColor", "#3880ff")));
            
            #line default
            #line hidden
            this.Write(";\r\n    --ion-color-primary-rgb: ");
            
            #line 23 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Client\Theme\VariablesScssTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.ResourceData.FindColorRgb("PrimaryColor", "56, 128, 255")));
            
            #line default
            #line hidden
            this.Write(";\r\n    --ion-color-primary-contrast: #ffffff;\r\n    --ion-color-primary-contrast-r" +
                    "gb: 255, 255, 255;\r\n    --ion-color-primary-shade: #3171e0;\r\n    --ion-color-pri" +
                    "mary-tint: #4c8dff;\r\n    /** secondary **/\r\n    --ion-color-secondary: ");
            
            #line 29 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Client\Theme\VariablesScssTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.ResourceData.FindColor("SecondaryColor", "#3dc2ff")));
            
            #line default
            #line hidden
            this.Write(";\r\n    --ion-color-secondary-rgb: ");
            
            #line 30 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Client\Theme\VariablesScssTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.ResourceData.FindColorRgb("SecondaryColor", "61, 194, 255")));
            
            #line default
            #line hidden
            this.Write(";\r\n    --ion-color-secondary-contrast: #ffffff;\r\n    --ion-color-secondary-contra" +
                    "st-rgb: 255, 255, 255;\r\n    --ion-color-secondary-shade: #36abe0;\r\n    --ion-col" +
                    "or-secondary-tint: #50c8ff;\r\n    /** tertiary **/\r\n    --ion-color-tertiary: ");
            
            #line 36 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Client\Theme\VariablesScssTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.ResourceData.FindColor("TertiaryColor", "#5260ff")));
            
            #line default
            #line hidden
            this.Write(";\r\n    --ion-color-tertiary-rgb: ");
            
            #line 37 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Client\Theme\VariablesScssTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.ResourceData.FindColorRgb("TertiaryColor", "82, 96, 255")));
            
            #line default
            #line hidden
            this.Write(";\r\n    --ion-color-tertiary-contrast: #ffffff;\r\n    --ion-color-tertiary-contrast" +
                    "-rgb: 255, 255, 255;\r\n    --ion-color-tertiary-shade: #4854e0;\r\n    --ion-color-" +
                    "tertiary-tint: #6370ff;\r\n    /** success **/\r\n    --ion-color-success: #2dd36f;\r" +
                    "\n    --ion-color-success-rgb: 45, 211, 111;\r\n    --ion-color-success-contrast: #" +
                    "ffffff;\r\n    --ion-color-success-contrast-rgb: 255, 255, 255;\r\n    --ion-color-s" +
                    "uccess-shade: #28ba62;\r\n    --ion-color-success-tint: #42d77d;\r\n    /** warning " +
                    "**/\r\n    --ion-color-warning: #ffc409;\r\n    --ion-color-warning-rgb: 255, 196, 9" +
                    ";\r\n    --ion-color-warning-contrast: #000000;\r\n    --ion-color-warning-contrast-" +
                    "rgb: 0, 0, 0;\r\n    --ion-color-warning-shade: #e0ac08;\r\n    --ion-color-warning-" +
                    "tint: #ffca22;\r\n    /** danger **/\r\n    --ion-color-danger: #eb445a;\r\n    --ion-" +
                    "color-danger-rgb: 235, 68, 90;\r\n    --ion-color-danger-contrast: #ffffff;\r\n    -" +
                    "-ion-color-danger-contrast-rgb: 255, 255, 255;\r\n    --ion-color-danger-shade: #c" +
                    "f3c4f;\r\n    --ion-color-danger-tint: #ed576b;\r\n    /** dark **/\r\n    --ion-color" +
                    "-dark: #222428;\r\n    --ion-color-dark-rgb: 34, 36, 40;\r\n    --ion-color-dark-con" +
                    "trast: #ffffff;\r\n    --ion-color-dark-contrast-rgb: 255, 255, 255;\r\n    --ion-co" +
                    "lor-dark-shade: #1e2023;\r\n    --ion-color-dark-tint: #383a3e;\r\n    /** medium **" +
                    "/\r\n    --ion-color-medium: #92949c;\r\n    --ion-color-medium-rgb: 146, 148, 156;\r" +
                    "\n    --ion-color-medium-contrast: #ffffff;\r\n    --ion-color-medium-contrast-rgb:" +
                    " 255, 255, 255;\r\n    --ion-color-medium-shade: #808289;\r\n    --ion-color-medium-" +
                    "tint: #9d9fa6;\r\n    /** light **/\r\n    --ion-color-light: #f4f5f8;\r\n    --ion-co" +
                    "lor-light-rgb: 244, 245, 248;\r\n    --ion-color-light-contrast: #000000;\r\n    --i" +
                    "on-color-light-contrast-rgb: 0, 0, 0;\r\n    --ion-color-light-shade: #d7d8da;\r\n  " +
                    "  --ion-color-light-tint: #f5f6f9;\r\n}\r\n\r\n\r\n/*\r\n * Additional Ionic Colors\r\n * --" +
                    "--------------------------------------------------------------------------\r\n * I" +
                    "n order to add colors to be used within Ionic components,\r\n * the color should b" +
                    "e added as a class with the convention `.ion-color-{COLOR}`\r\n * where `{COLOR}` " +
                    "is the color to be used on the Ionic component.\r\n * For more information on addi" +
                    "ng new colors, please see\r\n * https://ionicframework.com/docs/theming/colors#add" +
                    "ing-colors\r\n *\r\n * To generate the code for a new color, check out our new color" +
                    " creator:\r\n * https://ionicframework.com/docs/theming/colors#new-color-creator\r\n" +
                    " */\r\n\r\n :root {\r\n    /** favorite **/\r\n  --ion-color-favorite: #69bb7b;\r\n  --ion" +
                    "-color-favorite-rgb: 105,187,123;\r\n  --ion-color-favorite-contrast: #ffffff;\r\n  " +
                    "--ion-color-favorite-contrast-rgb: 255,255,255;\r\n  --ion-color-favorite-shade: #" +
                    "5ca56c;\r\n  --ion-color-favorite-tint: #78c288;\r\n    /** twitter **/\r\n  --ion-col" +
                    "or-twitter: #1da1f4;\r\n  --ion-color-twitter-rgb: 29,161,244;\r\n  --ion-color-twit" +
                    "ter-contrast: #ffffff;\r\n  --ion-color-twitter-contrast-rgb: 255,255,255;\r\n  --io" +
                    "n-color-twitter-shade: #1a8ed7;\r\n  --ion-color-twitter-tint: #34aaf5;\r\n    /** i" +
                    "nstagram **/\r\n  --ion-color-instagram: #5956d8;\r\n  --ion-color-instagram-rgb: 89" +
                    ",86,216;\r\n  --ion-color-instagram-contrast: #ffffff;\r\n  --ion-color-instagram-co" +
                    "ntrast-rgb: 255,255,255;\r\n  --ion-color-instagram-shade: #4e4cbe;\r\n  --ion-color" +
                    "-instagram-tint: #6a67dc;\r\n    /** vimeo **/\r\n  --ion-color-vimeo: #23b6ea;\r\n  -" +
                    "-ion-color-vimeo-rgb: 35,182,234;\r\n  --ion-color-vimeo-contrast: #ffffff;\r\n  --i" +
                    "on-color-vimeo-contrast-rgb: 255,255,255;\r\n  --ion-color-vimeo-shade: #1fa0ce;\r\n" +
                    "  --ion-color-vimeo-tint: #39bdec;\r\n    /** facebook **/\r\n  --ion-color-facebook" +
                    ": #3b5998;\r\n  --ion-color-facebook-rgb: 59,89,152;\r\n  --ion-color-facebook-contr" +
                    "ast: #ffffff;\r\n  --ion-color-facebook-contrast-rgb: 255,255,255;\r\n  --ion-color-" +
                    "facebook-shade: #344e86;\r\n  --ion-color-facebook-tint: #4f6aa2;\r\n}\r\n\r\n.ion-color" +
                    "-favorite {\r\n    /** favorite **/\r\n    --ion-color-base: var(--ion-color-favorit" +
                    "e);\r\n    --ion-color-base-rgb: var(--ion-color-favorite-rgb);\r\n    --ion-color-c" +
                    "ontrast: var(--ion-color-favorite-contrast);\r\n    --ion-color-contrast-rgb: var(" +
                    "--ion-color-favorite-contrast-rgb);\r\n    --ion-color-shade: var(--ion-color-favo" +
                    "rite-shade);\r\n    --ion-color-tint: var(--ion-color-favorite-tint);\r\n}\r\n\r\n.ion-c" +
                    "olor-twitter {\r\n    /** twitter **/\r\n    --ion-color-base: var(--ion-color-twitt" +
                    "er);\r\n    --ion-color-base-rgb: var(--ion-color-twitter-rgb);\r\n    --ion-color-c" +
                    "ontrast: var(--ion-color-twitter-contrast);\r\n    --ion-color-contrast-rgb: var(-" +
                    "-ion-color-twitter-contrast-rgb);\r\n    --ion-color-shade: var(--ion-color-twitte" +
                    "r-shade);\r\n    --ion-color-tint: var(--ion-color-twitter-tint);\r\n}\r\n\r\n.ion-color" +
                    "-instagram {\r\n    /** instagram **/\r\n    --ion-color-base: var(--ion-color-insta" +
                    "gram);\r\n    --ion-color-base-rgb: var(--ion-color-instagram-rgb);\r\n    --ion-col" +
                    "or-contrast: var(--ion-color-instagram-contrast);\r\n    --ion-color-contrast-rgb:" +
                    " var(--ion-color-instagram-contrast-rgb);\r\n    --ion-color-shade: var(--ion-colo" +
                    "r-instagram-shade);\r\n    --ion-color-tint: var(--ion-color-instagram-tint);\r\n}\r\n" +
                    "\r\n.ion-color-vimeo {\r\n    /** vimeo **/\r\n    --ion-color-base: var(--ion-color-v" +
                    "imeo);\r\n    --ion-color-base-rgb: var(--ion-color-vimeo-rgb);\r\n    --ion-color-c" +
                    "ontrast: var(--ion-color-vimeo-contrast);\r\n    --ion-color-contrast-rgb: var(--i" +
                    "on-color-vimeo-contrast-rgb);\r\n    --ion-color-shade: var(--ion-color-vimeo-shad" +
                    "e);\r\n    --ion-color-tint: var(--ion-color-vimeo-tint);\r\n}\r\n\r\n.ion-color-faceboo" +
                    "k {\r\n    /** facebook **/\r\n    --ion-color-base: var(--ion-color-facebook);\r\n   " +
                    " --ion-color-base-rgb: var(--ion-color-facebook-rgb);\r\n    --ion-color-contrast:" +
                    " var(--ion-color-facebook-contrast);\r\n    --ion-color-contrast-rgb: var(--ion-co" +
                    "lor-facebook-contrast-rgb);\r\n    --ion-color-shade: var(--ion-color-facebook-sha" +
                    "de);\r\n    --ion-color-tint: var(--ion-color-facebook-tint);\r\n}\r\n\r\n/*\r\n * Shared " +
                    "Variables\r\n * ------------------------------------------------------------------" +
                    "----------\r\n * To customize the look and feel of this app, you can override\r\n * " +
                    "the CSS variables found in Ionic\'s source files.\r\n * To view all of the possible" +
                    " Ionic variables, see:\r\n * https://ionicframework.com/docs/theming/css-variables" +
                    "#ionic-variables\r\n */\r\n\r\n:root {\r\n  --ion-headings-font-weight: 300;\r\n\r\n  --ion-" +
                    "color-angular: #ac282b;\r\n  --ion-color-communication: #8e8d93;\r\n  --ion-color-to" +
                    "oling: #fe4c52;\r\n  --ion-color-services: #fd8b2d;\r\n  --ion-color-design: #fed035" +
                    ";\r\n  --ion-color-workshop: #69bb7b;\r\n  --ion-color-food: #3bc7c4;\r\n  --ion-color" +
                    "-documentation: #b16be3;\r\n  --ion-color-navigation: #6600cc;\r\n}\r\n\r\n/*\r\n * App iO" +
                    "S Variables\r\n * ----------------------------------------------------------------" +
                    "------------\r\n * iOS only CSS variables can go here\r\n */\r\n\r\n.ios {\r\n\r\n}\r\n\r\n/*\r\n " +
                    "* App Material Design Variables\r\n * --------------------------------------------" +
                    "--------------------------------\r\n * Material Design only CSS variables can go h" +
                    "ere\r\n */\r\n\r\n.md {\r\n\r\n}\r\n\r\n/*\r\n * App Theme\r\n * ---------------------------------" +
                    "-------------------------------------------\r\n * Ionic apps can have different th" +
                    "emes applied, which can\r\n * then be further customized. These variables come las" +
                    "t\r\n * so that the above variables are used by default.\r\n */\r\n\r\n/*\r\n * Dark Theme" +
                    "\r\n * ---------------------------------------------------------------------------" +
                    "-\r\n */\r\n\r\n.dark-theme {\r\n    /** primary **/\r\n  --ion-color-primary: #428cff;\r\n " +
                    " --ion-color-primary-rgb: 66,140,255;\r\n  --ion-color-primary-contrast: #ffffff;\r" +
                    "\n  --ion-color-primary-contrast-rgb: 255,255,255;\r\n  --ion-color-primary-shade: " +
                    "#3a7be0;\r\n  --ion-color-primary-tint: #5598ff;\r\n    /** secondary **/\r\n  --ion-c" +
                    "olor-secondary: #50c8ff;\r\n  --ion-color-secondary-rgb: 80,200,255;\r\n  --ion-colo" +
                    "r-secondary-contrast: #ffffff;\r\n  --ion-color-secondary-contrast-rgb: 255,255,25" +
                    "5;\r\n  --ion-color-secondary-shade: #46b0e0;\r\n  --ion-color-secondary-tint: #62ce" +
                    "ff;\r\n    /** tertiary **/\r\n  --ion-color-tertiary: #6a64ff;\r\n  --ion-color-terti" +
                    "ary-rgb: 106,100,255;\r\n  --ion-color-tertiary-contrast: #ffffff;\r\n  --ion-color-" +
                    "tertiary-contrast-rgb: 255,255,255;\r\n  --ion-color-tertiary-shade: #5d58e0;\r\n  -" +
                    "-ion-color-tertiary-tint: #7974ff;\r\n    /** success **/\r\n  --ion-color-success: " +
                    "#2fdf75;\r\n  --ion-color-success-rgb: 47,223,117;\r\n  --ion-color-success-contrast" +
                    ": #000000;\r\n  --ion-color-success-contrast-rgb: 0,0,0;\r\n  --ion-color-success-sh" +
                    "ade: #29c467;\r\n  --ion-color-success-tint: #44e283;\r\n    /** warning **/\r\n  --io" +
                    "n-color-warning: #ffd534;\r\n  --ion-color-warning-rgb: 255,213,52;\r\n  --ion-color" +
                    "-warning-contrast: #000000;\r\n  --ion-color-warning-contrast-rgb: 0,0,0;\r\n  --ion" +
                    "-color-warning-shade: #e0bb2e;\r\n  --ion-color-warning-tint: #ffd948;\r\n    /** da" +
                    "nger **/\r\n  --ion-color-danger: #ff4961;\r\n  --ion-color-danger-rgb: 255,73,97;\r\n" +
                    "  --ion-color-danger-contrast: #ffffff;\r\n  --ion-color-danger-contrast-rgb: 255," +
                    "255,255;\r\n  --ion-color-danger-shade: #e04055;\r\n  --ion-color-danger-tint: #ff5b" +
                    "71;\r\n    /** dark **/\r\n  --ion-color-dark: #f4f5f8;\r\n  --ion-color-dark-rgb: 244" +
                    ",245,248;\r\n  --ion-color-dark-contrast: #000000;\r\n  --ion-color-dark-contrast-rg" +
                    "b: 0,0,0;\r\n  --ion-color-dark-shade: #d7d8da;\r\n  --ion-color-dark-tint: #f5f6f9;" +
                    "\r\n    /** medium **/\r\n  --ion-color-medium: #989aa2;\r\n  --ion-color-medium-rgb: " +
                    "152,154,162;\r\n  --ion-color-medium-contrast: #000000;\r\n  --ion-color-medium-cont" +
                    "rast-rgb: 0,0,0;\r\n  --ion-color-medium-shade: #86888f;\r\n  --ion-color-medium-tin" +
                    "t: #a2a4ab;\r\n    /** light **/\r\n  --ion-color-light: #222428;\r\n  --ion-color-lig" +
                    "ht-rgb: 34,36,40;\r\n  --ion-color-light-contrast: #ffffff;\r\n  --ion-color-light-c" +
                    "ontrast-rgb: 255,255,255;\r\n  --ion-color-light-shade: #1e2023;\r\n  --ion-color-li" +
                    "ght-tint: #383a3e;\r\n}\r\n\r\n/*\r\n * iOS Dark Theme\r\n * -----------------------------" +
                    "-----------------------------------------------\r\n */\r\n\r\n.dark-theme.ios {\r\n    /" +
                    "** background **/\r\n  --ion-background-color: #000000;\r\n  --ion-background-color-" +
                    "rgb: 0,0,0;\r\n    /** text **/\r\n  --ion-text-color: #ffffff;\r\n  --ion-text-color-" +
                    "rgb: 255,255,255;\r\n    /** step **/\r\n  --ion-color-step-50: #0d0d0d;\r\n  --ion-co" +
                    "lor-step-100: #1a1a1a;\r\n  --ion-color-step-150: #262626;\r\n  --ion-color-step-200" +
                    ": #333333;\r\n  --ion-color-step-250: #404040;\r\n  --ion-color-step-300: #4d4d4d;\r\n" +
                    "  --ion-color-step-350: #595959;\r\n  --ion-color-step-400: #666666;\r\n  --ion-colo" +
                    "r-step-450: #737373;\r\n  --ion-color-step-500: #808080;\r\n  --ion-color-step-550: " +
                    "#8c8c8c;\r\n  --ion-color-step-600: #999999;\r\n  --ion-color-step-650: #a6a6a6;\r\n  " +
                    "--ion-color-step-700: #b3b3b3;\r\n  --ion-color-step-750: #bfbfbf;\r\n  --ion-color-" +
                    "step-800: #cccccc;\r\n  --ion-color-step-850: #d9d9d9;\r\n  --ion-color-step-900: #e" +
                    "6e6e6;\r\n  --ion-color-step-950: #f2f2f2;\r\n    /** toobar background **/\r\n  --ion" +
                    "-toolbar-background: #0d0d0d;\r\n    /** item background **/\r\n  --ion-item-backgro" +
                    "und: #000000;\r\n}\r\n\r\n\r\n/*\r\n * Material Design Dark Theme\r\n * --------------------" +
                    "--------------------------------------------------------\r\n */\r\n\r\n.dark-theme.md " +
                    "{\r\n    /** background **/\r\n    --ion-background-color: #121212;\r\n    --ion-backg" +
                    "round-color-rgb: 18,18,18;\r\n    /** text **/\r\n    --ion-text-color: #ffffff;\r\n  " +
                    "  --ion-text-color-rgb: 255,255,255;\r\n    /** border **/\r\n    --ion-border-color" +
                    ": #222222;\r\n    /** step **/\r\n    --ion-color-step-50: #1e1e1e;\r\n    --ion-color" +
                    "-step-100: #2a2a2a;\r\n    --ion-color-step-150: #363636;\r\n    --ion-color-step-20" +
                    "0: #414141;\r\n    --ion-color-step-250: #4d4d4d;\r\n    --ion-color-step-300: #5959" +
                    "59;\r\n    --ion-color-step-350: #656565;\r\n    --ion-color-step-400: #717171;\r\n   " +
                    " --ion-color-step-450: #7d7d7d;\r\n    --ion-color-step-500: #898989;\r\n    --ion-c" +
                    "olor-step-550: #949494;\r\n    --ion-color-step-600: #a0a0a0;\r\n    --ion-color-ste" +
                    "p-650: #acacac;\r\n    --ion-color-step-700: #b8b8b8;\r\n    --ion-color-step-750: #" +
                    "c4c4c4;\r\n    --ion-color-step-800: #d0d0d0;\r\n    --ion-color-step-850: #dbdbdb;\r" +
                    "\n    --ion-color-step-900: #e7e7e7;\r\n    --ion-color-step-950: #f3f3f3;\r\n    /**" +
                    " item background **/\r\n    --ion-item-background: #1e1e1e;\r\n    /** toolbar backg" +
                    "round **/\r\n    --ion-toolbar-background: #1f1f1f;\r\n    /** tabbar background **/" +
                    "\r\n    --ion-tab-bar-background: #1f1f1f;\r\n}\r\n\r\ndefault {\r\n    /** default **/\r\n " +
                    "   background: var(--ion-background-color);\r\n    --ion-default-font: \"Roboto\", \"" +
                    "Helvetica Neue\", sans-serif;\r\n}");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 1 "D:\MC\CloudIDEaaS\root\ApplicationGenerator.IonicAngular\Generators\Client\Theme\VariablesScssTemplate.tt"

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

private global::AbstraX.IResourceData _ResourceDataField;

/// <summary>
/// Access the ResourceData parameter of the template.
/// </summary>
private global::AbstraX.IResourceData ResourceData
{
    get
    {
        return this._ResourceDataField;
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
bool ResourceDataValueAcquired = false;
if (this.Session.ContainsKey("ResourceData"))
{
    this._ResourceDataField = ((global::AbstraX.IResourceData)(this.Session["ResourceData"]));
    ResourceDataValueAcquired = true;
}
if ((ResourceDataValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("ResourceData");
    if ((data != null))
    {
        this._ResourceDataField = ((global::AbstraX.IResourceData)(data));
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
