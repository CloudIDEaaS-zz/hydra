#if INCLUDE_RTFLIBRARY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itenso.Rtf;
using Itenso.Rtf.Parser;
using Itenso.Rtf.Support;
using System.IO;
using Itenso.Rtf.Converter.Image;
using Itenso.Rtf.Interpreter;
using Itenso.Rtf.Converter.Html;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Utils
{
    public static class RtfStringExtensions
    {
        public enum ProgramExitCode
        {
            Successfully = 0,
            InvalidSettings = -1,
            ParseRtf = -2,
            DestinationDirectory = -3,
            InterpretRtf = -4,
            ConvertHtml = -5,
            SaveHtml = -6,
        }

        public static string RtfToHtml(this string str, RtfHtmlConvertSettings htmlConvertSettings, bool throwOnError = false, IRtfParserListener listener = null, string destinationDirectory = null, RtfVisualImageAdapter imageAdapter = null, string imageAdapterLogFile = null, RtfImageConvertSettings imageConvertSettings = null)
        {
            IRtfGroup rtfStructure;
            IRtfDocument rtfDocument = null;

            try
            {
                using (var stream = str.ToStream())
                {
                    // parse the rtf structure
                    var structureBuilder = new RtfParserListenerStructureBuilder();
                    var parser = new RtfParser(structureBuilder);
                    DirectoryInfo destination;

                    if (destinationDirectory != null)
                    {
                        destination = new DirectoryInfo(destinationDirectory);
                    }

                    parser.IgnoreContentAfterRootGroup = true; // support WordPad documents

                    if (listener != null)
                    {
                        parser.AddParserListener(listener);
                    }

                    parser.Parse(new RtfSource(stream));
                    rtfStructure = structureBuilder.StructureRoot;

                    ThrowOnUnexpectedExitCode();

                    rtfDocument = InterpretRtf(rtfStructure, imageAdapter, imageAdapterLogFile, imageConvertSettings, throwOnError);

                    if (throwOnError)
                    {
                        ThrowOnUnexpectedExitCode();
                    }

                    // convert to hmtl

                    string html = ConvertHmtl(rtfDocument, imageAdapter, htmlConvertSettings, throwOnError);

                    if (throwOnError)
                    {
                        ThrowOnUnexpectedExitCode();
                    }

                    return html;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("RtfToHtml parser failed with error: {0}", e.Message);
            }

            return null;
        }

        private static string ConvertHmtl(IRtfDocument rtfDocument, IRtfVisualImageAdapter imageAdapter, RtfHtmlConvertSettings htmlConvertSettings, bool throwOnError = false)
        {
            string html = null;

            try
            {
                var htmlConverter = new RtfHtmlConverter(rtfDocument, htmlConvertSettings);
                html = htmlConverter.Convert();
            }
            catch (Exception e)
            {
                if (throwOnError)
                {
                    ThrowOnUnexpectedExitCode(e);
                }
            }

            return html;
        }

        private static IRtfDocument InterpretRtf(IRtfGroup rtfStructure, IRtfVisualImageAdapter imageAdapter, string imageAdapterLogFile = null, RtfImageConvertSettings imageConvertSettings = null, bool throwOnError = false)
        {
            IRtfDocument rtfDocument;
            RtfInterpreterListenerFileLogger interpreterLogger = null;

            try
            {
                // logger
                if (imageAdapterLogFile != null)
                {
                    interpreterLogger = new RtfInterpreterListenerFileLogger(imageAdapterLogFile);
                }

                // image converter
                RtfImageConverter imageConverter = null;

                if (imageConvertSettings != null)
                {
                    imageConverter = new RtfImageConverter(imageConvertSettings);
                }

                // rtf interpreter
                RtfInterpreterSettings interpreterSettings = new RtfInterpreterSettings();
                interpreterSettings.IgnoreDuplicatedFonts = true;
                interpreterSettings.IgnoreUnknownFonts = true;

                // interpret the rtf structure using the extractors
                rtfDocument = RtfInterpreterTool.BuildDoc(rtfStructure, interpreterSettings, interpreterLogger, imageConverter);

            }
            catch (Exception e)
            {
                if (interpreterLogger != null)
                {
                    interpreterLogger.Dispose();
                }

                if (throwOnError)
                {
                    ThrowOnUnexpectedExitCode(e);
                }

                return null;
            }

            return rtfDocument;
        }

        private static void ThrowOnUnexpectedExitCode(Exception ex = null)
        {
            string error = null;

            if (ex != null)
            {
                error = ex.Message;
            }

            if (Environment.ExitCode != (int)ProgramExitCode.Successfully)
            {
                if (error != null)
                {
                    throw new Exception(string.Format("RtfToHtml parser failed with exit code: {0}, Error: {1}", ((ProgramExitCode)Environment.ExitCode).ToString(), error));
                }
                else
                {
                    throw new Exception(string.Format("RtfToHtml parser failed with exit code: {0}", ((ProgramExitCode)Environment.ExitCode).ToString()));
                }
            }
            else if (error != null)
            {
                throw ex;
            }
        }
    }
}

#endif