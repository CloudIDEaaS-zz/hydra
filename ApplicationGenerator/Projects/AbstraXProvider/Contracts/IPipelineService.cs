using System;
using System.Xml.Schema;
using AbstraX.Bindings;
using System.Collections.Generic;
using AbstraX.BuildEvents;

namespace AbstraX.Contracts
{
    public delegate void OnOutputHandler(object sender, BuildMessageEventArgs eventArgs);

    public interface IPipelineService
    {
        IEventsService EventsService { get; set; }
        XmlSchema CodeGeneratorInterfaceSchema { get; }
        void GenerateFrom(List<IBindingsTree> bindings);
        void GenerateMiddleLayer(string id);
        event OnOutputHandler OnOutput;
    }
}
