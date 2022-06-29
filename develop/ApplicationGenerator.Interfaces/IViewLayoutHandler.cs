using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;

namespace AbstraX
{
    public interface IViewLayoutHandler : IModuleHandler
    {
        event ChildViewHandler OnChildView;
        Dictionary<string, object> ViewData { get; }
        string PartialComponentName { get; }
        List<IBase> ModelObjectGraph { get; set; }

        bool Process(IBase baseObject, Facet facet, IView view, IGeneratorConfiguration generatorConfiguration);
        bool PreProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IViewLayoutHandler currentHandler);
        bool PostProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IViewLayoutHandler currentHandler);
    }
}
