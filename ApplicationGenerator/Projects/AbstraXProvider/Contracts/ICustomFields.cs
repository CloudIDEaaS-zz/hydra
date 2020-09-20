using System;

namespace AbstraX.Contracts
{
    public interface ICustomFields
    {
        string this[string name] { get; set; }
    }
}
