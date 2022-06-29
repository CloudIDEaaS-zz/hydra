// file:	GeneratorOptions.cs
//
// summary:	Implements the generator options class

using MailSlot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace AbstraX
{
    /// <summary>   A generator options. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>

    public class GeneratorOptions
    {
        /// <summary>   Name of the thread slot redirected output. </summary>
        public const string THREAD_SLOT_REDIRECTED_OUTPUT_NAME = "REDIRECTED_OUTPUT";

        /// <summary>   Gets or sets the generator pass. </summary>
        ///
        /// <value> The generator pass. </value>
        public GeneratorPass GeneratorPass { get; set; }

        /// <summary>   Gets or sets the print mode. </summary>
        ///
        /// <value> The print mode. </value>

        public PrintMode PrintMode { get; set; }

        /// <summary>   Gets or sets the debug package installs. </summary>
        ///
        /// <value> The debug package installs. </value>

        public List<string> DebugPackageInstalls { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  use overrides. </summary>
        ///
        /// <value> True if use overrides, false if not. </value>

        public bool UseOverrides { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  use dynamic templates. </summary>
        ///
        /// <value> True if use dynamic templates, false if not. </value>

        public bool UseDynamicTemplates { get; set; }

        /// <summary>   Gets or sets the application layout surveyor. </summary>
        ///
        /// <value> The application layout surveyor. </value>

        public IAppFolderStructureSurveyor AppFolderStructureSurveyor { get; set; }

        /// <summary>   Gets or sets a value indicating whether the log package listing. </summary>
        ///
        /// <value> True if log package listing, false if not. </value>

        public bool LogPackageListing { get; set; }

        /// <summary>   Gets the test mode. </summary>
        ///
        /// <value> The test mode. </value>

        public TestMode TestMode { get; protected set; }

        /// <summary>   Gets or sets a value indicating whether the debug shim service. </summary>
        ///
        /// <value> True if debug shim service, false if not. </value>

        public bool DebugShimService { get; set; }

        /// <summary>   Gets or sets the recursion stack limit. </summary>
        ///
        /// <value> The recursion stack limit. </value>

        public int RecursionStackLimit { get; set; }

        /// <summary>   Gets or sets a value indicating whether the no file creation. </summary>
        ///
        /// <value> True if no file creation, false if not. </value>

        public bool NoFileCreation { get; set; }

        /// <summary>   Gets or sets the output writer. </summary>
        ///
        /// <value> The output writer. </value>

        public StreamWriter OutputWriter { get; protected set; }

        /// <summary>   Gets or sets the redirected writer. </summary>
        ///
        /// <value> The redirected writer. </value>

        public TextWriter RedirectedWriter { get; protected set; }

        /// <summary>   Gets or sets the error writer. </summary>
        ///
        /// <value> The error writer. </value>

        public StreamWriter ErrorWriter { get; protected set; }

        /// <summary>   Gets or sets the application folder hierarchy. </summary>
        ///
        /// <value> The application folder hierarchy. </value>

        public ApplicationFolderHierarchy ApplicationFolderHierarchy { get; set; }

        /// <summary>   Gets or sets the debug assistant address. </summary>
        ///
        /// <value> The debug assistant address. </value>

        public string DebugAssistantAddress { get; set; }

        /// <summary>   Gets or sets the mailslot client. </summary>
        ///
        /// <value> The mailslot client. </value>

        public MailslotClient MailslotClient { get; set; }

        /// <summary>   Gets or sets a value indicating whether the debug. </summary>
        ///
        /// <value> True if debug, false if not. </value>

        public bool Debug { get; set; }

        /// <summary>   Gets or sets a value indicating whether the run as automated. </summary>
        ///
        /// <value> True if run as automated, false if not. </value>

        public bool RunAsAutomated { get; set; }

        /// <summary>   Gets or sets the parent process. </summary>
        ///
        /// <value> The parent process. </value>

        public int ParentProcessId { get; set; }
    }

    /// <summary>   A default generator options. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>

    public class DefaultGeneratorOptions : GeneratorOptions
    {
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>

        public DefaultGeneratorOptions()
        {
            this.PrintMode = PrintMode.PrintUIHierarchyPath;
            this.RecursionStackLimit = 255;
            this.GeneratorPass = GeneratorPass.StructureOnly;
            this.OutputWriter = new StreamWriter(Console.OpenStandardOutput());
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>
        ///
        /// <param name="printMode">    The print mode. </param>
        /// <param name="testMode">     (Optional) The test mode. </param>

        public DefaultGeneratorOptions(PrintMode printMode, TestMode testMode = TestMode.NotSet)
        {
            this.PrintMode = printMode;
            this.RecursionStackLimit = 255;
            this.GeneratorPass = GeneratorPass.All;
            this.TestMode = testMode;
            this.OutputWriter = new StreamWriter(Console.OpenStandardOutput());
        }
    }

    /// <summary>   A redirected generator options. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>

    public class RedirectedGeneratorOptions : GeneratorOptions
    {
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>
        ///
        /// <param name="outputWriter">         The output writer. </param>
        /// <param name="errorWriter">          The error writer. </param>
        /// <param name="generatorPass">        The generator pass. </param>
        /// <param name="noFileCreation">       True to no file creation. </param>
        /// <param name="debugShimService">     True to debug shim service. </param>
        /// <param name="useDynamicTemplates">  True to use dynamic templates. </param>
        /// <param name="debugAssistantAddress"></param>

        public RedirectedGeneratorOptions(StreamWriter outputWriter, StreamWriter errorWriter, GeneratorPass generatorPass, bool noFileCreation, bool debugShimService, bool useDynamicTemplates, string debugAssistantAddress)
        {
            LocalDataStoreSlot localDataStoreSlot;
            var outputBuilder = new StringBuilder();

            this.PrintMode = PrintMode.PrintUIHierarchyPath;
            this.RecursionStackLimit = 255;
            this.OutputWriter = outputWriter;
            this.ErrorWriter = errorWriter;
            this.GeneratorPass = generatorPass;
            this.NoFileCreation = noFileCreation;
            this.DebugShimService = debugShimService;
            this.UseDynamicTemplates = useDynamicTemplates;
            this.DebugAssistantAddress = debugAssistantAddress;
            this.RedirectedWriter = new StringWriter(outputBuilder);

            localDataStoreSlot = Thread.AllocateNamedDataSlot(THREAD_SLOT_REDIRECTED_OUTPUT_NAME);

            Thread.SetData(localDataStoreSlot, outputBuilder);
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 5/30/2022. </remarks>
        ///
        /// <param name="outputWriter">             The output writer. </param>
        /// <param name="errorWriter">              The error writer. </param>
        /// <param name="redirectedWriter">         The redirected writer. </param>
        /// <param name="generatorPass">            The generator pass. </param>
        /// <param name="noFileCreation">           True to no file creation. </param>
        /// <param name="debugShimService">         True to debug shim service. </param>
        /// <param name="useDynamicTemplates">      True to use dynamic templates. </param>
        /// <param name="debugAssistantAddress">    . </param>

        public RedirectedGeneratorOptions(StreamWriter outputWriter, StreamWriter errorWriter, TextWriter redirectedWriter, GeneratorPass generatorPass, bool noFileCreation, bool debugShimService, bool useDynamicTemplates, string debugAssistantAddress)
        {
            this.PrintMode = PrintMode.PrintUIHierarchyPath;
            this.RecursionStackLimit = 255;
            this.OutputWriter = outputWriter;
            this.ErrorWriter = errorWriter;
            this.GeneratorPass = generatorPass;
            this.NoFileCreation = noFileCreation;
            this.DebugShimService = debugShimService;
            this.UseDynamicTemplates = useDynamicTemplates;
            this.DebugAssistantAddress = debugAssistantAddress;
            this.RedirectedWriter = redirectedWriter;
        }
    }
}
