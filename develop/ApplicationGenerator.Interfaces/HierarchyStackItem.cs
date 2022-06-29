// file:	HierarchyStackItem.cs
//
// summary:	Implements the hierarchy stack item class

using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   A hierarchy stack item. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/24/2020. </remarks>

    public class HierarchyStackItem
    {
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set; }

        /// <summary>   Gets or sets the indentation. </summary>
        ///
        /// <value> The indentation. </value>

        public int Indentation { get; set; }

        /// <summary>   Gets or sets a value indicating whether the popped. </summary>
        ///
        /// <value> True if popped, false if not. </value>

        public bool Popped { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/24/2020. </remarks>
        ///
        /// <param name="name"> The name. </param>

        public HierarchyStackItem(string name)
        {
            this.Name = name;
        }

        /// <summary>   Implicit cast that converts the given HierarchyStackItem to a string. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/24/2020. </remarks>
        ///
        /// <param name="item"> The item. </param>
        ///
        /// <returns>   The result of the operation. </returns>

        public static implicit operator string(HierarchyStackItem item)
        {
            return item.Name;
        }

        /// <summary>   Implicit cast that converts the given string to a HierarchyStackItem. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/24/2020. </remarks>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   The result of the operation. </returns>

        public static implicit operator HierarchyStackItem(string name)
        {
            return new HierarchyStackItem(name);
        }

        /// <summary>   Returns a string that represents the current object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/24/2020. </remarks>
        ///
        /// <returns>   A string that represents the current object. </returns>

        public override string ToString()
        {
            return this.Name;
        }
    }
}
