using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Object;
using System.Net.Sockets;

namespace robot_interface
{
    class TCPConnection
    {
        Form _Form1;
        string server;
        Int32 port;
        public TCPConnection(Form _Form1)
        {
            this._Form1 = _Form1;

        }
        public void Connect(string server, Int32 port)
        {
            this.server = server;
            this.port = port;
            Thread clientThread = new Thread(()=>Client(this.server,this.port));
            clientThread.Start();
            //_Form1.updateNetworkListBox("Started client Thread");
        }
        static void Client(string server, Int32 _port)
        {
            string ip = server;
            Int32 port = _port;
            TcpClient client = new TcpClient(ip, port);
            NetworkStream nwStream = client.GetStream();
            //_Form1.updateNetworkListBox("Started client Thread");
            //if there is something on the list then
            //byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(/*string z listy*/);
            //nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            // return reading
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            /* add bytes read to the shared list*/
        }
    }
}
