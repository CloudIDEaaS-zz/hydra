using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class PreprocessedEntity
    {
        private unsafe CppSharp.Parser.AST.PreprocessedEntity preprocessedEntity;
        public unsafe Declaration OwningDeclaration { get; set; }

        public unsafe PreprocessedEntity(Declaration owningDeclaration, CppSharp.Parser.AST.PreprocessedEntity preprocessedEntity)
        {
            this.OwningDeclaration = owningDeclaration;
            this.preprocessedEntity = preprocessedEntity;
            this.preprocessedEntity.AssertNotNull();
        }

        public string Kind
        {
            get
            {
                return preprocessedEntity.Kind.ToString();
            }
        }

        public string MacroLocation
        {
            get
            {
                return preprocessedEntity.MacroLocation.ToString();
            }
        }
    }
}
