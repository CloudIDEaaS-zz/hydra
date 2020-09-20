using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts.Views
{
    public interface IViewsProjectTemplate : IViewsTemplateBase
    {
        string TemplateID { get; set; }
        ICodeTemplateProject ProjectTemplate { set; }
        List<IViewsNavigationTemplate> AvailableNavigations { get; }
        List<IViewsItemTemplate> AvailableItems { get; }
        List<WellKnownLanguage> SupportedLanguages { get; }
    }
}
