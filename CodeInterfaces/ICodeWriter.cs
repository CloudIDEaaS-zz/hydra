using System;

namespace CodeInterfaces
{
    public interface ICodeWriter : IDisposable
    {
        void Write(string output);
    }
}
