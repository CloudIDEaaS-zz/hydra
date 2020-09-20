using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.Contracts.ComponentModel;

namespace AbstraX.Contracts.Views
{
    public interface IViewInstance : IViewsBase, IComponentInstance
    {
        string InstanceID { get; }
        string ProjectTemplateID { get; }
        string ItemTemplateID { get; }
        IViewsProjectTemplate ProjectTemplate { get; set; }
        IViewsItemTemplate ItemTemplate { get; set; }
    }
}
