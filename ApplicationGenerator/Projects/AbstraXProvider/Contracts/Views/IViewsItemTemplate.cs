using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts.Views
{
    public interface IViewsItemTemplate : IViewsTemplateBase
    {
        ICodeTemplateProjectItem ItemTemplate { set; }
        List<WellKnownLanguage> SupportedLanguages { get; }
    }
}
