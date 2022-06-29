// file:	IDesignWizardPage.cs
//
// summary:	Declares the IDesignWizardPage interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using WizardBase;

namespace AbstraX
{
    /// <summary>   Interface for design wizard page. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

    public interface IWizardStepDesign : IWizardSubPage
    {
        /// <summary>   Event queue for all listeners interested in OnPageValidated events. </summary>
        event EventHandler OnPageValid;
        /// <summary>   Event queue for all listeners interested in OnPageInvalid events. </summary>
        event EventHandlerT<Exception> OnPageInvalid;

        /// <summary>   Validates this.  </summary>
        void ValidatePage();

        /// <summary>   Gets or sets information describing the resource. </summary>
        ///
        /// <value> Information describing the resource. </value>

        IResourceData ResourceData { get; set; }

        /// <summary>   Gets or sets the pathname of the local theme folder. </summary>
        ///
        /// <value> The pathname of the local theme folder. </value>

        string LocalThemeFolder { get; set; }

        /// <summary>   Initializes the control. </summary>
        ///
        /// <param name="workingDirectory"> Pathname of the working directory. </param>

        void InitializeControl(string workingDirectory);
    }
}
