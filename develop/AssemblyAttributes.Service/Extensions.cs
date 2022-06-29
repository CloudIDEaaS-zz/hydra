using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Utils;

namespace AssemblyAttributesService
{
    public static class Extensions
    {
        public static string GetSnippet(this CommandPacket commandPacket, int length = 255)
        {
            return JsonExtensions.ToJsonText(commandPacket).Flatten().Crop(length, true);
        }
    }
}
