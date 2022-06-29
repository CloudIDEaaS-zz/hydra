// file:	Resources\IDocumentComparisonResolver.cs
//
// summary:	Declares the IDocumentComparisonResolver interface

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for document comparison resolver. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>

    public interface IDocumentComparisonResolver
    {
        /// <summary>   Reports an unsaved. </summary>
        ///
        /// <param name="outputFile">               The output file. </param>
        /// <param name="zipFile">                  The zip file. </param>
        /// <param name="zipDocumentLength">        Length of the zip document. </param>
        /// <param name="existingDocumentLength">   Length of the existing document. </param>
        /// <param name="continueWriteFromZip">     The continue write from zip. </param>

        void ReportUnsaved(FileInfo outputFile, FileInfo zipFile, byte[] content, int zipDocumentLength, int existingDocumentLength, Action continueWriteFromZip);
    }
}
