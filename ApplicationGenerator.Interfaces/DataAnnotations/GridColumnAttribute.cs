using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridColumnAttribute : NavigableAttribute
    {
        public GridColumnKind GridColumnKind { get; }
        public bool IsTextIdentifier { get; }

        public GridColumnAttribute(string uiHierarchyPath, GridColumnKind kind = GridColumnKind.Text) : base(uiHierarchyPath)
        {
            this.GridColumnKind = kind;
        }

        public GridColumnAttribute(string uiHierarchyPath, bool isTextIdentifier, GridColumnKind kind = GridColumnKind.Text) : base(uiHierarchyPath)
        {
            this.GridColumnKind = kind;
            this.IsTextIdentifier = isTextIdentifier;
        }
    }
}
