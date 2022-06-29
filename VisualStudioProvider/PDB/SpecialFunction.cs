using Pdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Parsing;

namespace VisualStudioProvider.PDB
{
    public class SpecialFunction
    {
        public DebugSymbol DebugSymbol { get; }
        public bool IdentifierOnly { get; set; }
        public bool DynamicInitializer { get; set; }
        public bool DynamicAtExitDestructor { get; set; }
        public bool AnonymousNamespace { get; set; }
        public string AnonymousNamespaceName { get; set; }
        public string DynamicAtExitDestructorTypeOrNamespace { get; set; }
        public string DynamicInitializerTypeOrNamespace { get; set; }
        public string Type { get; set; }
        public bool ConstVTable { get; private set; }
        public string ConstVTableTypeOrNamespace { get; private set; }
        public string ConstVTableInterface { get; private set; }

        public SpecialFunction(DebugSymbol debugSymbol)
        {
            DebugSymbol = debugSymbol;

            Evaluate(debugSymbol, this);
        }

        public static bool Evaluate(DebugSymbol debugSymbol, SpecialFunction specialFunction = null)
        {
            var name = debugSymbol.Name;

            if (name.Contains("vftable") || (debugSymbol.UndecoratedName != null && debugSymbol.UndecoratedName.Contains("vftable")))
            {

            }

            if (name.RegexIsMatch(StringExtensions.REGEX_IDENTIFIER))
            {
                if (specialFunction != null)
                {
                    specialFunction.IdentifierOnly = true;
                }

                return true;
            }
            else if (name.RegexIsMatch("`dynamic initializer for '(?<typeornamespace>.*?)'"))
            {
                var typeOrNamespace = name.RegexGet("`dynamic initializer for '(?<typeornamespace>.*?)'", "typeornamespace");

                if (typeOrNamespace.IsNullOrEmpty())
                {
                    DebugUtils.Break();
                }

                if (specialFunction != null)
                {
                    specialFunction.DynamicInitializer = true;
                    specialFunction.DynamicInitializerTypeOrNamespace = typeOrNamespace;
                }

                return TypeSignatureParser.CanParse(typeOrNamespace);
            }
            else if (name.RegexIsMatch("`dynamic initializer for '(?<typeornamespace>.*)$"))
            {
                var typeOrNamespace = name.RegexGet("`dynamic initializer for '(?<typeornamespace>.*)$", "typeornamespace");

                if (typeOrNamespace.IsNullOrEmpty())
                {
                    DebugUtils.Break();
                }

                if (specialFunction != null)
                {
                    specialFunction.DynamicInitializer = true;
                    specialFunction.DynamicInitializerTypeOrNamespace = typeOrNamespace;
                }

                return TypeSignatureParser.CanParse(typeOrNamespace);
            }
            else if (name.RegexIsMatch("`dynamic atexit destructor for '(?<typeornamespace>.*?)'"))
            {
                var typeOrNamespace = name.RegexGet("`dynamic atexit destructor for '(?<typeornamespace>.*?)'", "typeornamespace");

                if (typeOrNamespace.IsNullOrEmpty())
                {
                    DebugUtils.Break();
                }

                if (specialFunction != null)
                {
                    specialFunction.DynamicAtExitDestructor = true;
                    specialFunction.DynamicAtExitDestructorTypeOrNamespace = typeOrNamespace;
                }

                return TypeSignatureParser.CanParse(typeOrNamespace);
            }
            else if (name.RegexIsMatch("`dynamic atexit destructor for '(?<typeornamespace>.*)$"))
            {
                var typeOrNamespace = name.RegexGet("`dynamic atexit destructor for '(?<typeornamespace>.*)$", "typeornamespace");

                if (typeOrNamespace.IsNullOrEmpty())
                {
                    DebugUtils.Break();
                }

                if (specialFunction != null)
                {
                    specialFunction.DynamicAtExitDestructor = true;
                    specialFunction.DynamicAtExitDestructorTypeOrNamespace = typeOrNamespace;
                }

                return TypeSignatureParser.CanParse(typeOrNamespace);
            }
            else if (name.RegexIsMatch("`anonymous namespace'(::)*(?<namespace>.*?)'"))
            {
                var typeOrNamespace = name.RegexGet("`anonymous namespace'(::)*(?<namespace>.*?)'", "namespace");

                if (typeOrNamespace.IsNullOrEmpty())
                {
                    DebugUtils.Break();
                }

                if (specialFunction != null)
                {
                    specialFunction.AnonymousNamespace = true;
                    specialFunction.AnonymousNamespaceName = typeOrNamespace;
                }

                return TypeSignatureParser.CanParse(typeOrNamespace);
            }
            else if (name.RegexIsMatch("`anonymous namespace'(::)*(?<namespace>.*)$"))
            {
                var typeOrNamespace = name.RegexGet("`anonymous namespace'(::)*(?<namespace>.*)$", "namespace");

                if (typeOrNamespace.IsNullOrEmpty())
                {
                    DebugUtils.Break();
                }

                if (specialFunction != null)
                {
                    specialFunction.AnonymousNamespace = true;
                    specialFunction.AnonymousNamespaceName = typeOrNamespace;
                }

                return TypeSignatureParser.CanParse(typeOrNamespace);
            }
            else if (TypeSignatureParser.CanParse(name))
            {
                if (specialFunction != null)
                {
                    specialFunction.Type = name;
                }

                return true;
            }

            return false;
        }
    }
}
