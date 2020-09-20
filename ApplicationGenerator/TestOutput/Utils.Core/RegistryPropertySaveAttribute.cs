using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Utils;
using System.Diagnostics;
using System.Collections;

namespace Utils
{
    public enum RegistryPropertySaveOptions
    {
        None,
        SaveNullValueAsEmpty
    }

    public class RegistryPropertySaveAttribute : Attribute
    {
        public RegistryPropertySaveOptions SaveOptions { get; }

        public RegistryPropertySaveAttribute(RegistryPropertySaveOptions saveOptions)
        {
            this.SaveOptions = saveOptions;
        }
    }
}
