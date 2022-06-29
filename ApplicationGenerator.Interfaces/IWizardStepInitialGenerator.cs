// file:	IWizardStepStatus.cs
//
// summary:	Declares the IWizardStepStatus interface

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBase;

namespace AbstraX
{
    /// <summary>   Interface for wizard step initial generator. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/3/2021. </remarks>

    public interface IWizardStepInitialGenerator : IWizardStepStatus
    {
        /// <summary>   Starts the given action. </summary>
        ///
        /// <param name="action">   The action. </param>

        void Start(Action action);
    }
}
