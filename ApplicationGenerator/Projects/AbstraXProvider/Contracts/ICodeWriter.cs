using System;

namespace AbstraX.Contracts
{
    public interface ICodeWriter : IDisposable
    {
        void Write(string output);
    }
}
