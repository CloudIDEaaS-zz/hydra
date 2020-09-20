using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public interface IAppLoaderPlugin
    {
        void Load(params object[] parms);
    }
}
