using AbstraX.MarketingControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AbstraX
{
    /// <summary>   A control resource management base. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>

    public class ctrlResourceManagementBase : UserControl
    {

        /// <summary>   Gets or sets the state of the document management. </summary>
        ///
        /// <value> The document management state. </value>

        public DocumentManagementState DocumentManagementState { get; set; }

        /// <summary>   Gets the object properties. </summary>
        ///
        /// <value> The object properties. </value>

        [Browsable(false)]
        public ObjectPropertiesDictionary ObjectProperties { get; set; }

        /// <summary>
        /// Event queue for all listeners interested in ObjectPropertiesChanged events.
        /// </summary>

        public event PropertyValueChangedEventHandler ObjectPropertiesChanged;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/4/2020. </remarks>
        
        public ctrlResourceManagementBase()
        {
            this.ObjectProperties = new ObjectPropertiesDictionary();
        }

        /// <summary>   Raises the object properties changed event. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>
        ///
        /// <param name="s">    An object to process. </param>
        /// <param name="e">    Event information to send to registered event handlers. </param>

        protected void RaiseObjectPropertiesChanged(object s, PropertyValueChangedEventArgs e)
        {
            ObjectPropertiesChanged(s, e);
        }

        /// <summary>   Raises the object properties changed event. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/14/2021. </remarks>
        ///
        /// <param name="s">            An object to process. </param>
        /// <param name="changedItem">  The changed item. </param>
        /// <param name="oldValue">     The old value. </param>

        protected void RaiseObjectPropertiesChanged(object s, object changedItem, object oldValue)
        {
            var eventArgs = new PropertyValueChangedEventArgs(new NotAGridItem(changedItem), oldValue);

            ObjectPropertiesChanged(s, eventArgs);
        }

        /// <summary>   Sets a state. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>
        ///
        /// <param name="state">    The state. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        public IDisposable SetState(DocumentManagementState state)
        {
            var disposable = this.CreateDisposable(() => this.DocumentManagementState = DocumentManagementState.UserActive);

            if (this.DocumentManagementState == state)
            {
                DebugUtils.Break();
            }

            this.DocumentManagementState = state;

            return disposable;
        }
    }
}
