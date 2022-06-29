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

    public interface IWizardStepFinal : IWizardSubPage
    {
        /// <summary>   Initializes the control. </summary>
        ///
        /// <param name="desktopForm">  The desktop form. </param>

        void InitializeControl(IDesktopForm desktopForm);
        bool UISubmitButtonEnabled { get; set; }
    }
}
