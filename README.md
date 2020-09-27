# Hydra

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

<div>
 <pre style="font-family:Consolas;font-size:13px;color:black;background:white;"><span style="background:yellow;">@</span><span style="color:blue;">using</span>&nbsp;Ripley.Entities
<span style="background:yellow;">@</span><span style="color:blue;">using</span>&nbsp;AbstraX
 
<span style="background:yellow;">@model</span>&nbsp;<span style="color:#2b91af;">EntityModel</span>&lt;<span style="color:#2b91af;">Post</span>&gt;
<span style="background:yellow;">@{</span>
&nbsp;&nbsp;&nbsp;&nbsp;Layout&nbsp;=&nbsp;<span style="color:#a31515;">&quot;_Repeater.cshtml&quot;</span>;
 
&nbsp;&nbsp;&nbsp;&nbsp;ViewBag.VirtualScroll&nbsp;=&nbsp;<span style="color:blue;">true</span>;
&nbsp;&nbsp;&nbsp;&nbsp;ViewData[<span style="color:#a31515;">&quot;Title&quot;</span>]&nbsp;=&nbsp;<span style="color:#a31515;">&quot;Posts&quot;</span>;
 
&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">var</span>&nbsp;media&nbsp;=&nbsp;Model.CreateScriptObject&lt;<span style="color:#2b91af;">PostMedia</span>&gt;();
&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">var</span>&nbsp;user&nbsp;=&nbsp;Model.GetLoggedInUser&lt;<span style="color:#2b91af;">User</span>&gt;();
<span style="background:yellow;">}</span>
 
<span style="background:yellow;">@section</span>&nbsp;ItemTemplate
<span style="background:yellow;">{</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">&lt;</span><span style="color:maroon;">div</span><span style="color:blue;">&gt;</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="background:yellow;">@*</span><span style="color:darkgreen;">&nbsp;relies&nbsp;on&nbsp;the&nbsp;facet&nbsp;handler&nbsp;on&nbsp;the&nbsp;entity&nbsp;or&nbsp;entity&nbsp;property&nbsp;</span><span style="background:yellow;">*@</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">&lt;</span><span style="background:yellow;">@</span><span style="color:maroon;">Model</span>.<span style="color:maroon;">Entity</span>.<span style="color:maroon;">Predicate</span>&nbsp;<span style="color:blue;">/&gt;</span>
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="background:yellow;">@*</span><span style="color:darkgreen;">&nbsp;creates&nbsp;a&nbsp;binding&nbsp;to&nbsp;an&nbsp;element&nbsp;in&nbsp;the&nbsp;view&nbsp;to&nbsp;code&nbsp;</span><span style="background:yellow;">*@</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">&lt;</span><span style="color:maroon;">ion-input</span>&nbsp;<span style="color:red;">formControlName</span><span style="color:blue;">=</span><span style="color:blue;">&quot;</span><span style="background:yellow;">@</span>Model.Entity.TimeStamp<span style="color:blue;">&quot;</span>&nbsp;<span style="color:red;">type</span><span style="color:blue;">=</span><span style="color:blue;">&quot;text&quot;</span><span style="color:blue;">&gt;&lt;/</span><span style="color:maroon;">ion-input</span><span style="color:blue;">&gt;</span>
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="background:yellow;">@*</span><span style="color:darkgreen;">&nbsp;create&nbsp;an&nbsp;element&nbsp;with&nbsp;script&nbsp;</span><span style="background:yellow;">*@</span>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">&lt;</span><span style="color:maroon;">input</span>&nbsp;<span style="color:red;">type</span><span style="color:blue;">=</span><span style="color:blue;">&quot;file&quot;</span>&nbsp;<span style="color:red;">id</span><span style="color:blue;">=</span><span style="color:blue;">&quot;files&quot;</span>&nbsp;<span style="color:red;">name</span><span style="color:blue;">=</span><span style="color:blue;">&quot;files[]&quot;</span>&nbsp;<span style="color:red;">multiple</span>&nbsp;<span style="color:red;">onchange</span><span style="color:blue;">=</span><span style="color:blue;">&quot;</span>handleFileSelect($event)<span style="color:blue;">&quot;</span>&nbsp;<span style="color:blue;">/&gt;</span>
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="background:yellow;">@</span>Html.Partial(<span style="color:#a31515;">&quot;PostFeedback&quot;</span>,&nbsp;Model.Entity.PostFeedbacks,&nbsp;<span style="color:blue;">new</span>&nbsp;<span style="color:#2b91af;">ViewDataDictionary</span>(<span style="color:blue;">this</span>.ViewData)&nbsp;{&nbsp;{&nbsp;<span style="color:#a31515;">&quot;counter&quot;</span>,&nbsp;1&nbsp;}&nbsp;});
&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">&lt;/</span><span style="color:maroon;">div</span><span style="color:blue;">&gt;</span>
<span style="background:yellow;">}</span>
 
<span style="background:yellow;">@section</span>&nbsp;CustomScripts
<span style="background:yellow;">{</span>
&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">&lt;</span><span style="color:maroon;">script</span><span style="color:blue;">&gt;</span>
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;handleFileSelect(evt&nbsp;:&nbsp;Event)&nbsp;{
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">let</span>&nbsp;files&nbsp;=&nbsp;evt.target.files;&nbsp;<span style="color:green;">//&nbsp;FileList&nbsp;object</span>
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:green;">//&nbsp;files&nbsp;is&nbsp;a&nbsp;FileList&nbsp;of&nbsp;File&nbsp;objects.&nbsp;List&nbsp;some&nbsp;properties.</span>
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">let</span>&nbsp;output&nbsp;=&nbsp;[];
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;let&nbsp;<span style="background:yellow;">@</span>media;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;let&nbsp;<span style="background:yellow;">@</span>user;
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">for</span>&nbsp;(<span style="color:blue;">let</span>&nbsp;i&nbsp;=&nbsp;0,&nbsp;f;&nbsp;f&nbsp;=&nbsp;files[i];&nbsp;i++)&nbsp;{
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="background:yellow;">@</span>media.FileContents&nbsp;=&nbsp;f.readAsBinaryString();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="background:yellow;">@</span>media.FileName&nbsp;=&nbsp;f.name;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="background:yellow;">@</span>media.User&nbsp;=&nbsp;user;
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="background:yellow;">@</span>Model.Create(media);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;output.push(<span style="color:#a31515;">&#39;&lt;li&gt;&lt;strong&gt;&#39;</span>,&nbsp;escape(f.name),&nbsp;<span style="color:#a31515;">&#39;&lt;/strong&gt;&nbsp;(&#39;</span>,&nbsp;f.type&nbsp;||&nbsp;<span style="color:#a31515;">&#39;n/a&#39;</span>,&nbsp;<span style="color:#a31515;">&#39;)&nbsp;-&nbsp;&#39;</span>,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;f.size,&nbsp;<span style="color:#a31515;">&#39;&nbsp;bytes,&nbsp;last&nbsp;modified:&nbsp;&#39;</span>,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;f.lastModifiedDate&nbsp;?&nbsp;f.lastModifiedDate.toLocaleDateString()&nbsp;:&nbsp;<span style="color:#a31515;">&#39;n/a&#39;</span>,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:#a31515;">&#39;&lt;/li&gt;&#39;</span>);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
 
&nbsp;&nbsp;&nbsp;&nbsp;<span style="color:blue;">&lt;/</span><span style="color:maroon;">script</span><span style="color:blue;">&gt;</span>
<span style="background:yellow;">}</span></pre>
</div>

## Known Issues

Hydra is a "work in progress".  Please be patient as stability evolves.

### For more information

Visit: http://www.cloudideaas.com/hydra

**Enjoy!**
