using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.Contracts.CodeParsers;

namespace AbstraX.Contracts
{
    public enum WellKnownLanguage
    {
        CSharp,
        VisualBasicNet,
        Java,
        NotWellKnown
    }

    public interface IProgrammingLanguage
    {
        WellKnownLanguage WellKnownLanguage { get; }
        ICodeParser CodeParser { get; }
    }
}
