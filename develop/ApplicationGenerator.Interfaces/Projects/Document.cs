// file:	Projects\Document.cs
//
// summary:	Implements the document class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Projects
{
    /// <summary>   A document. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

    public class Document
    {
        /// <summary>   Gets the filename of the file. </summary>
        ///
        /// <value> The name of the file. </value>

        public string FileName { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="fileName"> Filename of the file. </param>

        public Document(string fileName)
        {
            FileName = fileName;
        }
    }
}
