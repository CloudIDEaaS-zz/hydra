using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.Contracts;

namespace AbstraX
{
    public class BuildUIQueueEntry
    {
        public string URL;
        public IBuildUIDaemon BuildUIDaemon;
    }
}
