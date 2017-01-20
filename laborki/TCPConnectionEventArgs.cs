using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laborki
{
    class TCPConnectionEventArgs: EventArgs
    {
        public string info { get; set; }
        public TCPConnectionEventArgs(string info)
        {
            this.info = info;
        }
    }
}
