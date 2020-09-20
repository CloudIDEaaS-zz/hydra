# Hydra

[![Twitter URL](https://img.shields.io/twitter/url?style=social&url=https://twitter.com/cloudideaas)](https://twitter.com/cloudideaas)
[![Twitter Follow](https://img.shields.io/twitter/follow/cloudideaas?label=Followers&style=social&url=https://twitter.com/cloudideaas)](https://twitter.com/cloudideaas)
[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/cloudideaas-hydra)
![npm](https://img.shields.io/npm/dt/@cloudideaas/hydra)
![GitHub commit activity](https://img.shields.io/github/commit-activity/w/cloudideaas/hydra)


Hydra is a app generation tool with resulting source code. Generated front-end support for Ionic/Angular. Restful service layer support for .NET Core. Back-end support for SQL Server. Other supported technologies coming soon as Hydra is highly extensible. Also includes support for mobile capability, Docker, Helm, Azure Devops, and Kubernetes.

![Hydra Splash](https://www.cloudideaas.com/images/HydraSplashNarrow.png)

## Current Features

- Generates a Visual Studio boiler plate solution:
 ```
hydra generate workspace
 ```
- Generates a Business Model and Entity Domain Model, respectively:
 ```
hydra generate businessmodel
hydra generate entities
 ```
- Generates front-end framework (i.e. Ionic) and resulting application:
 ```
hydra generate start
hydra generate app
 ```

## How it Works

Most everything in life is naturally hierarchical, your family, your work organization, data relationships, 
user interfaces, security models, etc. Hydra starts with what is called a "business model".  Business models are broken 
into the following levels: stakeholders, organizational units, roles (people and systems), responsibilities (or features - for systems),
tasks, and data items.  Once you get to the data item level, then you can define the detail that results in properties of entity models, 
columns in database tables, and elements on a screen.

Most rapid application development generators are designed for end users that have no concept of development best practices.  Hydra is 
different in that it is designed with the developer in mind.  It results in source code utilizing preferred coding styles, readability, 
design patterns, and code quality.  It doesn't end there.  Hydra is highly extensible, allowing for generator participation, interface driven
extensions, and even full control UI, business logic, and servicing.  It aims to remove the 80% of application development that is repetitive,
dull, and drone.  It does this while at the same time, allowing for creativity and full autonomy.

## Help

- [Installation Instructions](http://cloudideaas.com/hydra/installation.htm)
- [Usage](http://cloudideaas.com/hydra/usage.htm)
    - [Hydra Command Line Interface](http://cloudideaas.com/hydra/hydracli.htm)
    - [Generating a Workspace](http://cloudideaas.com/hydra/generateworkspace.htm)
    - [Generating a Business Model](http://cloudideaas.com/hydra/generatebusinessmodel.htm)
    - [Business Model Input Files](http://cloudideaas.com/hydra/businessmodeltemplate.htm)
    - [Entity Domain Model Input Files](http://cloudideaas.com/hydra/generateentities.htm)
    - [Entity Domain Model Input Files](http://cloudideaas.com/hydra/entitydomainmodeltemplate.htm)
    - [Entity Domain Model Attributes](http://cloudideaas.com/hydra/entitydomainmodel.htm)
    - [Generate App](http://cloudideaas.com/hydra/generateapp.htm)
    - [package.json Configuration](http://cloudideaas.com/hydra/packagejson.htm)
    - [Writing custom UI's with Razor syntax](http://cloudideaas.com/hydra/razorUI.htm)
- [Extensibility](http://cloudideaas.com/hydra/extensibility.htm)
    - [Extension model](http://cloudideaas.com/hydra/extensionmodel.htm)
    - [ApplicationGenerator.Interfaces](http://cloudideaas.com/hydra/applicationgenerator.htm)
    - [Generator Engine](http://cloudideaas.com/hydra/generatorengine.htm)
    - [ApplicationFolderHierarchy](http://cloudideaas.com/hydra/applicationfolderhierarchy.htm)
    - [Generator Configuration](http://cloudideaas.com/hydra/facethandlers.htm)
    - [AbstraX Model](http://cloudideaas.com/hydra/abstraxmodel.htm)
    - [AbstraX Providers](http://cloudideaas.com/hydra/abstraxproviders.htm)
      - [Facets](http://cloudideaas.com/hydra/abstraxproviders.htm)
      - [Facet Handlers](http://cloudideaas.com/hydra/facethandlers.htm)
- [Visual Studio Integration](http://cloudideaas.com/hydra/visualstudiointegration.htm)
- [Contribution](http://cloudideaas.com/hydra/contribution.htm)

## Requirements

- Windows 10 or higher
- NodeJs (for npm)
- Windows Build Tools (dotnet, MSBuild, etc) for development
- Hydra App Generator Windows Cli (downloadable through the npm Hydra installation package)
- Preferred though not required: Visual Studio or VS Code

### Razor Syntax

 ```cs
@using Ripley.Entities
@using AbstraX

@model EntityModel<Post>
@{
    Layout = "_Repeater.cshtml";

    ViewBag.VirtualScroll = true;
    ViewData["Title"] = "Posts";

    var media = Model.CreateScriptObject<PostMedia>();
    var user = Model.GetLoggedInUser<User>();
}

@section ItemTemplate
{
    <div>
        @* relies on the facet handler on the entity or entity property *@
        <@Model.Entity.Predicate />

        @* creates a binding to an element in the view to code *@
        <ion-input formControlName="@Model.Entity.TimeStamp" type="text"></ion-input>

        @* create an element with script *@
        <input type="file" id="files" name="files[]" multiple onchange="handleFileSelect($event)" />

        @Html.Partial("PostFeedback", Model.Entity.PostFeedbacks, new ViewDataDictionary(this.ViewData) { { "counter", 1 } });
    </div>
}

@section CustomScripts
{
    <script>

        handleFileSelect(evt : Event) {

            let files = evt.target.files; // FileList object

            // files is a FileList of File objects. List some properties.

            let output = [];
            let @media;
            let @user;

            for (let i = 0, f; f = files[i]; i++) {

                @media.FileContents = f.readAsBinaryString();
                @media.FileName = f.name;
                @media.User = user;

                @Model.Create(media);
                                
                output.push('<li><strong>', escape(f.name), '</strong> (', f.type || 'n/a', ') - ',
                          f.size, ' bytes, last modified: ',
                          f.lastModifiedDate ? f.lastModifiedDate.toLocaleDateString() : 'n/a',
                          '</li>');
            }
          }

    </script>
}    
 ```

## Extension Settings

Include if your extension adds any VS Code settings through the `contributes.configuration` extension point.

For example:

This extension contributes the following settings:

* `myExtension.enable`: enable/disable this extension
* `myExtension.thing`: set to `blah` to do something

## Known Issues

Hydra is a "work in progress".  Please be patient as stability evolves.

## Release Notes


### For more information

Visit: http://www.cloudideaas.com/hydra

**Enjoy!**
