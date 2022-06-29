// file:	MarketingControls\NotAGridItem.cs
//
// summary:	Implements the not a grid item class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstraX.MarketingControls
{
    /// <summary>   A not a grid item. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/14/2021. </remarks>

    public class NotAGridItem : GridItem
    {
        /// <summary>
        /// When overridden in a derived class, gets the collection of
        /// <see cref="T:System.Windows.Forms.GridItem" /> objects, if any, associated as a child of this
        /// <see cref="T:System.Windows.Forms.GridItem" />.
        /// </summary>
        ///
        /// <value> The collection of <see cref="T:System.Windows.Forms.GridItem" /> objects. </value>

        public override GridItemCollection GridItems => throw new NotImplementedException();

        /// <summary>
        /// When overridden in a derived class, gets the type of this
        /// <see cref="T:System.Windows.Forms.GridItem" />.
        /// </summary>
        ///
        /// <value> One of the <see cref="T:System.Windows.Forms.GridItemType" /> values. </value>

        public override GridItemType GridItemType => throw new NotImplementedException();

        /// <summary>
        /// When overridden in a derived class, gets the text of this
        /// <see cref="T:System.Windows.Forms.GridItem" />.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="T:System.String" /> representing the text associated with this
        /// <see cref="T:System.Windows.Forms.GridItem" />.
        /// </value>

        public override string Label => throw new NotImplementedException();

        /// <summary>
        /// When overridden in a derived class, gets the parent
        /// <see cref="T:System.Windows.Forms.GridItem" /> of this
        /// <see cref="T:System.Windows.Forms.GridItem" />, if any.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="T:System.Windows.Forms.GridItem" /> representing the parent of the
        /// <see cref="T:System.Windows.Forms.GridItem" />.
        /// </value>

        public override GridItem Parent => throw new NotImplementedException();

        /// <summary>
        /// When overridden in a derived class, gets the
        /// <see cref="T:System.ComponentModel.PropertyDescriptor" /> that is associated with this
        /// <see cref="T:System.Windows.Forms.GridItem" />.
        /// </summary>
        ///
        /// <value>
        /// The <see cref="T:System.ComponentModel.PropertyDescriptor" /> associated with this
        /// <see cref="T:System.Windows.Forms.GridItem" />.
        /// </value>

        public override PropertyDescriptor PropertyDescriptor => throw new NotImplementedException();

        /// <summary>
        /// When overridden in a derived class, gets the current value of this
        /// <see cref="T:System.Windows.Forms.GridItem" />.
        /// </summary>
        ///
        /// <value>
        /// The current value of this <see cref="T:System.Windows.Forms.GridItem" />. This can be null.
        /// </value>

        public override object Value { get; }

        /// <summary>
        /// When overridden in a derived class, selects this
        /// <see cref="T:System.Windows.Forms.GridItem" /> in the
        /// <see cref="T:System.Windows.Forms.PropertyGrid" />.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/14/2021. </remarks>
        ///
        /// <returns>   true if the selection is successful; otherwise, false. </returns>

        public override bool Select()
        {
            throw new NotImplementedException();
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/14/2021. </remarks>
        ///
        /// <param name="value">    The value. </param>

        public NotAGridItem(object value)
        {
            this.Value = value;
        }
    }
}
