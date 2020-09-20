using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public class HierarchyStackItem
    {
        public string Name { get; set; }

        public HierarchyStackItem(string name)
        {
            this.Name = name;
        }

        public static implicit operator string(HierarchyStackItem item)
        {
            return item.Name;
        }

        public static implicit operator HierarchyStackItem(string name)
        {
            return new HierarchyStackItem(name);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
