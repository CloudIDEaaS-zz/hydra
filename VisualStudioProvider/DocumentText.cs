using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Utils;
using Microsoft.VisualStudio.OLE.Interop;

namespace VisualStudioProvider
{
    public class DocumentText : IVsTextLinesEvents
    {
        private IVsTextLines textLines;
        private uint textLineEventsCookie;
        private DocumentInfo documentInfo;
        private IConnectionPoint connectionPoint;
        public event EventHandler<EventArgs<TextChangeInfo>> OnChange;

        public DocumentText(DocumentInfo documentInfo, IVsTextLines textLines)
        {
            this.textLines = textLines;
            this.documentInfo = documentInfo;
            var container = textLines as IConnectionPointContainer;
            
            if (container != null)
            {
                var eventsGuid = typeof(IVsTextLinesEvents).GUID;

                container.FindConnectionPoint(ref eventsGuid, out connectionPoint);
                connectionPoint.Advise(this as IVsTextLinesEvents, out textLineEventsCookie);
            }
        }

        ~DocumentText()
        {
            if (connectionPoint != null && textLineEventsCookie != 0) 
            {
                connectionPoint.Unadvise(textLineEventsCookie);
            }

            textLineEventsCookie = 0;
            connectionPoint = null;
        }

        public void OnChangeLineAttributes(int iFirstLine, int iLastLine)
        {
            if (OnChange != null)
            {
                OnChange(this, new EventArgs<TextChangeInfo>(new TextChangeInfo(iFirstLine, iLastLine)));
            }
        }

        public void OnChangeLineText(TextLineChange[] pTextLineChange, int fLast)
        {
            if (OnChange != null)
            {
                OnChange(this, new EventArgs<TextChangeInfo>(new TextChangeInfo(pTextLineChange)));
            }
        }
    }
}
