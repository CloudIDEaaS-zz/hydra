using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class SortByAttribute : Attribute
    {
        public string PropertyName { get; }
        public SortDirection SortDirection { get; }

        public SortByAttribute(string propertyName, SortDirection sortDirection = SortDirection.Ascending)
        {
            this.PropertyName = propertyName;
            this.SortDirection = sortDirection;
        }
    }
}
