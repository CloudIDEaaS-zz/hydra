using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Declaration
    {
        private unsafe CppSharp.Parser.AST.Declaration declaration;
        private string name;
        public Declaration OwningDeclaration { get; set; }

        public Declaration(CppSharp.Parser.AST.Declaration declaration)
        {
            this.declaration = declaration;
            this.declaration.AssertNotNull();
        }

        public Declaration(string name)
        {
            this.name = name;
        }

        public Declaration(Declaration owningDeclaration, CppSharp.Parser.AST.Declaration declaration)
        {
            this.OwningDeclaration = owningDeclaration;
            this.declaration = declaration;

            this.OwningDeclaration.AssertNotNull();
            this.declaration.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return declaration.Location.ID;
            }
        }

        public int DefinitionOrder
        {
            get
            {
                if (declaration == null)
                {
                    return -1;
                }
                else
                {
                    return (int)declaration.DefinitionOrder;
                }
            }
        }

        public int LineNumberStart
        {
            get
            {
                if (declaration == null)
                {
                    return -1;
                }
                else
                {
                    return declaration.LineNumberStart;
                }
            }
        }

        public int LineNumberEnd
        {
            get
            {
                if (declaration == null)
                {
                    return -1;
                }
                else
                {
                    return declaration.LineNumberEnd;
                }
            }
        }

        public bool IsDependent
        {
            get
            {
                if (declaration == null)
                {
                    return false;
                }
                else
                {
                    return declaration.IsDependent;
                }
            }
        }

        public bool IsImplicit
        {
            get
            {
                if (declaration == null)
                {
                    return false;
                }
                else
                {
                    return declaration.IsImplicit;
                }
            }
        }

        public bool IsIncomplete
        {
            get
            {
                if (declaration == null)
                {
                    return false;
                }
                else
                {
                    return declaration.IsIncomplete;
                }
            }
        }

        public string Kind
        {
            get
            {
                if (declaration == null)
                {
                    return string.Empty;
                }
                else
                {
                    return declaration.Kind.ToString();
                }
            }
        }

        public string USR
        {
            get
            {
                if (declaration == null)
                {
                    return string.Empty;
                }
                else
                {
                    return declaration.USR;
                }
            }
        }

        public virtual string Name
        {
            get
            {
                if (declaration == null)
                {
                    return name;
                }
                else
                {
                    return declaration.Name;
                }
            }
        }

        public Declaration CompleteDeclaration
        {
            get
            {
                if (declaration.CompleteDeclaration == null)
                {
                    return null;
                }
                else
                {
                    return declaration.CompleteDeclaration.GetRealDeclarationInternal();
                }
            }
        }

        public IEnumerable<PreprocessedEntity> PreprocessedEntities
        {
            get
            {
                if (declaration == null)
                {
                    yield break;
                }
                else
                {
                    foreach (var preprocessedEntity in declaration.GetPreprocessedEntities())
                    {
                        yield return preprocessedEntity.GetRealPreprocessedEntityInternal(this);
                    }
                }
            }
        }
    }
}
