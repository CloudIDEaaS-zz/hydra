using AbstraX.FolderStructure;
using AbstraX.Models.Interfaces;
using EntityProvider.Web.Entities;

namespace AbstraX
{
    public interface IGeneratorEngine
    {
        GeneratorConfiguration GeneratorConfiguration { get; }

        void Process();
        void WriteError(string format, params object[] args);
        void WriteLine(string format, params object[] args);
        void WriteLine(string format, PrintMode printMode, params object[] args);
        void Reset();
    }
}