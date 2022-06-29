// file:	ctrlLayoutDesigner.cs
//
// summary:	Implements the control layout designer class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using WizardBase;

namespace AbstraX
{
    /// <summary>   Designer for Control layout. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

    public partial class ctrlLayoutDesigner : UserControl, IWizardStepDesign
    {
        /// <summary>   Event queue for all listeners interested in DisableNext events. </summary>
        public event EventHandler DisableNext;
        /// <summary>   Event queue for all listeners interested in EnableNext events. </summary>
        public event EventHandler EnableNext;
        /// <summary>   Event queue for all listeners interested in OnPageValidated events. </summary>
        public event EventHandler OnPageValid;
        /// <summary>   Event queue for all listeners interested in OnPageInvalid events. </summary>
        public event EventHandlerT<Exception> OnPageInvalid;

        /// <summary>   Gets or sets information describing the resource. </summary>
        ///
        /// <value> Information describing the resource. </value>

        public IResourceData ResourceData { get; set; }

        /// <summary>   Gets or sets the pathname of the local theme folder. </summary>
        ///
        /// <value> The pathname of the local theme folder. </value>

        public string LocalThemeFolder { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is shown. </summary>
        ///
        /// <value> True if shown, false if not. </value>

        public bool Shown { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

        public ctrlLayoutDesigner()
        {
            InitializeComponent();
        }

        /// <summary>   Initializes this. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="wizardSettingsBase">   The wizard settings base. </param>

        public void Initialize(WizardSettingsBase wizardSettingsBase)
        {
        }

        /// <summary>   Saves. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="wizardSettingsBase">   The wizard settings base. </param>
        /// <param name="isNext">               True if is next, false if not. </param>

        public void Save(WizardSettingsBase wizardSettingsBase, bool isNext)
        {
        }

        /// <summary>   Initializes the control. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="workingDirectory"> Pathname of the working directory. </param>

        public void InitializeControl(string workingDirectory)
        {
        }

        /// <summary>   Validates this. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void ValidatePage()
        {
        }
    }
}
