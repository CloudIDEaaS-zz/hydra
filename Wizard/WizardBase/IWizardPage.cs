using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WizardBase
{
    public interface IWizardPage
    {
        event EventHandler DisableNext;
        event EventHandler EnableNext;
        void Initialize(WizardSettingsBase wizardSettingsBase);
        void Save(WizardSettingsBase wizardSettingsBase, bool isNext);
    }
}
