using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts.Views
{
    public interface IViewsTemplateBase : IViewsBase
    {
        string TemplateID { get; set; }
        byte[] IconImageSmall { get; }
        byte[] IconImageLarge { get; }
    }
}
