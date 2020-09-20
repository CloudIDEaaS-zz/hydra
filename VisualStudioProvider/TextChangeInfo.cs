using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;

namespace VisualStudioProvider
{
    public class TextChangeInfo
    {
        public int FirstLine { get; private set; }
        public int LastLine { get; private set; }
        public TextLineChange[] TextLineChanges { get; private set; }

        public TextChangeInfo(int firstLine, int lastLine)
        {
            this.FirstLine = firstLine;
            this.LastLine = lastLine;
        }

        public TextChangeInfo(TextLineChange[] pTextLineChange)
        {
            this.TextLineChanges = pTextLineChange;
        }
    }
}
