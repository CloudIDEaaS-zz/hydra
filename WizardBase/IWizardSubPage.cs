using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizardBase
{
    public interface IWizardSubPage : IWizardPage
    {
        bool Shown { get; set; }
    }
}
