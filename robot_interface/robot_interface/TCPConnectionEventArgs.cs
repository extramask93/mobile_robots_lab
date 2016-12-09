using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace robot_interface
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
