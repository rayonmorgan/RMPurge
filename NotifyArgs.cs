using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMPurge
{
    ///
    public class NotifyArgs : EventArgs
    {
        //TODO: arguments for the NotifyProcessTrace delegate
        public string outValue { get; set; }
    }

    public class HeaderDetailsArgs : EventArgs
    {
        public string outValue { get; set; }
    }
}
