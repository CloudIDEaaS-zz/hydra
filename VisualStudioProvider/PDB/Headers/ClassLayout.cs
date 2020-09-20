using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class ClassLayout
    {
        private unsafe CppSharp.Parser.AST.ClassLayout classLayout;

        public unsafe ClassLayout(CppSharp.Parser.AST.ClassLayout classLayout)
        {
            this.classLayout = classLayout;
            this.classLayout.AssertNotNullAndOfType<CppSharp.Parser.AST.ClassLayout>();
        }

        public string ABI
        {
            get
            {
                return classLayout.ABI.ToString();
            }
        }

        public int Alignment
        {
            get
            {
                return classLayout.Alignment;
            }
        }

        public int DataSize
        {
            get
            {
                return classLayout.DataSize;
            }
        }

        public bool HasOwnVFPtr
        {
            get
            {
                return classLayout.HasOwnVFPtr;
            }
        }

        public VTableLayout Layout
        {
            get
            {
                if (classLayout.Layout == null)
                {
                    return null;
                }
                else
                {
                    return new VTableLayout(classLayout.Layout);
                }
            }
        }
    }
}
