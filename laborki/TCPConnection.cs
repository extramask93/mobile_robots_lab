using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace laborki
{
    class TCPConnection
    {
        string server;
        Int32 port;
        bool exitFlag = false;
        ConcurrentQueue<string> outQueue;
        public ConcurrentQueue<string> inQueue;
        TcpClient client;
        NetworkStream nwStream;
        Thread clientThread;
        EventWaitHandle messageHandle;
        EventWaitHandle exitHandle;
        WaitHandle[] handles;
        //event1
        public event EventHandler UIMessage;
        public void OnUIMessage(TCPConnectionEventArgs e)
        {
            UIMessage?.Invoke(this, e);
        }
        //event2
        public event EventHandler MessageReceived;
        public void OnMessageReceived(TCPConnectionEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
        //
        public TCPConnection()
        {
            messageHandle = new AutoResetEvent(false);
            exitHandle = new AutoResetEvent(false);
            handles = new WaitHandle[] {messageHandle,exitHandle }; 
            inQueue = new ConcurrentQueue<string>(30,new AutoResetEvent(false));
            outQueue = new ConcurrentQueue<string>(30,messageHandle);;

        }
        public void Send(string toSend)
        {
            outQueue.Add(toSend);
        }
        public void Close()
        {
            exitHandle.Set();
            inQueue.Clear();
            outQueue.Clear();
            exitFlag = true;
            if(clientThread!=null)
               clientThread.Join();
            if(client!=null)
                client.Close();
        }
        public void Connect(string server, Int32 port)
        {
            try
            {
                this.server = server;
                this.port = port;
                outQueue.Clear();
                inQueue.Clear();
                exitFlag = false;
                this.OnUIMessage(new TCPConnectionEventArgs("Connecting"));
                client = new TcpClient(server, port);
                nwStream = client.GetStream();
                this.OnUIMessage(new TCPConnectionEventArgs("Połączono z" + server.ToString() + "port: " + port.ToString() + "    "));
                exitHandle = new AutoResetEvent(false);
                clientThread = new Thread(() => Client());
                clientThread.Start();

            }
            catch(Exception e)
            {
                this.OnUIMessage(new TCPConnectionEventArgs(e.Message));
            }
  
        }
        public void Client()
        {
    
            try
            {
                while (!exitFlag)
                {
                    WaitHandle.WaitAny(handles,2000);
                    //if (exitHandle.WaitOne(0))
                       // throw new Exception("User requested");
                    while(outQueue.Count() > 0)
                    {
                        string toSend = outQueue.Remove();
                        byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(toSend);
                        nwStream.WriteTimeout = 1000;
                        nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                        nwStream.ReadTimeout = 1000;
                        byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                        int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                        inQueue.Add(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                        if (inQueue.Count() > 0)
                            this.OnMessageReceived(new TCPConnectionEventArgs("inqueue"));
                    }
                }
            }
            catch (Exception e)
            {
                this.OnUIMessage(new TCPConnectionEventArgs("Disconnected by: "+e.Message));
            }
            finally
            {
                exitFlag = false;
                this.OnUIMessage(new TCPConnectionEventArgs("Disconnected "));
            }
        }
    }
}
