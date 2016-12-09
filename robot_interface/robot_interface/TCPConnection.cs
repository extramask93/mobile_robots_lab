using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace robot_interface
{
    class TCPConnection
    {
        string server;
        Int32 port;
        ConcurrentQueue<string> outQueue;
        public ConcurrentQueue<string> inQueue;
        TcpClient client;
        NetworkStream nwStream;
        Thread clientThread;
        bool _quit;
        //event1
        public event EventHandler UIMessage;
        public void OnUIMessage(TCPConnectionEventArgs e)
        {
            EventHandler uiMessage = UIMessage;
            if (uiMessage != null)
            {
                uiMessage(this, e);
            }
        }
        //event2
        public event EventHandler MessageReceived;
        public void OnMessageReceived(TCPConnectionEventArgs e)
        {
            EventHandler messageReceived = MessageReceived;
            if(messageReceived != null)
            {
                messageReceived(this, e);
            }
        }
        //
        public TCPConnection()
        {
            inQueue = new ConcurrentQueue<string>(30);
            outQueue = new ConcurrentQueue<string>(30);
            _quit = false;

        }
        public void Send(string toSend)
        {
            outQueue.Add(toSend);
        }
        public void Close()
        {
            _quit = true;
            
        }
        
        public void Connect(string server, Int32 port)
        {
            this.server = server;
            this.port = port;
            clientThread = new Thread(()=>Client());
            clientThread.Start();
  
        }
        public void Client()
        {
    
            try
            {
                client = new TcpClient(server, port);
                nwStream = client.GetStream();
                this.OnUIMessage(new TCPConnectionEventArgs("Połączono z" + server + "port: " + port.ToString() + "    "));
                while (true && _quit!=true)
                {
                    if (outQueue._queue.Count() > 0)
                    {
                        string toSend = outQueue.Remove();
                        byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(toSend);
                        nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                        nwStream.ReadTimeout = 1000;
                        byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                        int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                        inQueue.Add(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                        if (inQueue._queue.Count > 0)
                            this.OnMessageReceived(new TCPConnectionEventArgs("inqueue"));
                    }
                    Thread.Sleep(100);
                }
                
            }
            catch (Exception e)
            {
                this.OnUIMessage(new TCPConnectionEventArgs("Błąd w wątku TCP"+e.ToString()));
            }
            finally
            {
                this.Close();
                this.OnUIMessage(new TCPConnectionEventArgs("Connection Closed"));
                if(client!=null) client.Close();
            }
        }
    }
}
