using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PoC.Connection
{
    public class APIConnection // It might be a good idea to split up this class into two others - one for receiving data, one for sending it
    {
        private readonly string address = "localhost";
        private readonly int port1 = 11000;
        private readonly int port2 = 12000;
        private readonly TcpClient commandSender;
        private readonly TcpClient listener;
        private WebSocket socket;
        private TaskCompletionSource<object> taskCompletitionSource;

        public APIConnection()
        {
            // Create TCP Clients for sending and receiveing data
            commandSender = new TcpClient(address, port1);
            listener = new TcpClient(address, port2);


        }

        public void SetWebSocket(WebSocket socket, TaskCompletionSource<object> tcs)
        {
            this.socket = socket;
            this.taskCompletitionSource = tcs;
        }

        /// <summary>
        /// Sends the specified command to the TCP API.
        /// </summary>
        /// <param name="command">The command that will be sent to the API.</param>
        public void SendCommand(string command)
        {
            NetworkStream stream = commandSender.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(command);

            stream.Write(data, 0, data.Length);
        }

        public void StartListening()
        {
            if (this.socket == null)
            {
                throw new Exception("Socket not initialized");
            }

            // Start a thread that can listen to the data from the TCP API.
            Thread listeningThread = new Thread(async () => await ReadDataAsync(listener));
            listeningThread.Start();

        }

        /// <summary>
        /// Continously reads incoming data.
        /// </summary>
        /// <param name="client">The TCP Client that will be used to read data.</param>
        private async Task ReadDataAsync(TcpClient client)
        {


            // Just putting everything into a try-catch is probably not a very good idea.
            try
            {
                using NetworkStream stream = client.GetStream();

                while (true)
                {
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);


                    await socket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), WebSocketMessageType.Text, true, CancellationToken.None);

                    string data = Encoding.ASCII.GetString(buffer);
                    Console.WriteLine(data);
                }
            } catch 
            {
                taskCompletitionSource.TrySetResult(null);
            }
        }

        /// <summary>
        /// Disconnect from the API.
        /// </summary>
        internal void Disconnect()
        {
            this.SendCommand("disconnet");
            if (this.listener != null && this.listener.Connected)
            {
                Console.WriteLine("Closing down the listener");
                this.listener.GetStream().Close();
                this.listener.Close();
            }

            if (this.commandSender != null && this.commandSender.Connected)
            {
                Console.WriteLine("Closing down the command sender");
                this.commandSender.Close();
            }

        }

        ~APIConnection()
        {
            Console.WriteLine("Destructor called");
            if (this.listener != null && this.listener.Connected)
            {
                Console.WriteLine("Closing down the listener");
                this.listener.GetStream().Close();
                this.listener.Close();
            }

            if (this.commandSender != null && this.commandSender.Connected)
            {
                Console.WriteLine("Closing down the comman sender");
                this.SendCommand("disconnect");
                this.commandSender.GetStream().Close();
                this.commandSender.Close();
            }
        }

    }
}
