using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Microsoft.VisualStudio.TextTemplating;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace Utils
{
    public interface ITemplateEngineBasePartialClass
    {
        void Initialize();
    }
}
