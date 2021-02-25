using PoC.Connection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace PoC.Connection.Implementation
{
    public class DataListener : IDataListener
    {
        private readonly string address = "localhost";
        private readonly int port = 12000;
        private readonly TcpClient listener;
        private WebSocket socket;
        private TaskCompletionSource<object> completionSource;

        public DataListener()
        {
            this.listener = new TcpClient(address, port);
        }

        public void SetWebSocket(WebSocket socket, TaskCompletionSource<object> tcs)
        {
            this.socket = socket;
            this.completionSource = tcs;
        }

        public void StartListening()
        {
            if (this.socket == null)
            {
                throw new Exception("Socket not initialized!");
            }
            Thread listeningThread = new Thread(() => ReadData());
            listeningThread.Start();
        }

        private void ReadData()
        {
            using NetworkStream stream = listener.GetStream();
            try
            {
                while (listener.Connected)
                {
                    byte[] buffer = new byte[listener.ReceiveBufferSize];
                    int bytesRead = stream.Read(buffer, 0, listener.ReceiveBufferSize); // Get data from the API

                    _ = socket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), WebSocketMessageType.Text, true, CancellationToken.None); // Send data to the fron end
                }
            }
            catch (Exception)
            {
                this.completionSource.TrySetResult(null);
                throw;
            }

        }
    }
}
