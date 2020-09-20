using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbstraX
{
    public class GeneratorOptions
    {
        public GeneratorPass GeneratorPass { get; set; }
        public PrintMode PrintMode { get; set; }
        public int RecursionStackLimit { get; set; }
        public bool NoFileCreation { get; set; }
        public StreamWriter OutputWriter { get; protected set; }
        public StreamWriter ErrorWriter { get; protected set; }
        public ApplicationFolderHierarchy ApplicationFolderHierarchy { get; set; }
    }

    public class DefaultGeneratorOptions : GeneratorOptions
    {
        public DefaultGeneratorOptions()
        {
            this.PrintMode = PrintMode.PrintUIHierarchyPathOnly;
            this.RecursionStackLimit = 255;
            this.GeneratorPass = GeneratorPass.HierarchyOnly;
        }

        public DefaultGeneratorOptions(PrintMode printMode)
        {
            this.PrintMode = printMode;
            this.RecursionStackLimit = 255;
            this.GeneratorPass = GeneratorPass.All;
        }
    }

    public class RedirectedGeneratorOptions : GeneratorOptions
    {
        public RedirectedGeneratorOptions(StreamWriter outputWriter, StreamWriter errorWriter, GeneratorPass generatorPass, bool noFileCreation)
        {
            this.PrintMode = PrintMode.PrintUIHierarchyPathOnly;
            this.RecursionStackLimit = 255;
            this.OutputWriter = outputWriter;
            this.ErrorWriter = errorWriter;
            this.GeneratorPass = generatorPass;
            this.NoFileCreation = noFileCreation;
        }
    }
}
