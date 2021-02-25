using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PoC.Connection.Interfaces;

namespace PoC.Connection.Implementation
{
    public class CommandSender : ICommandSender
    {
        private readonly string address = "localhost";
        private readonly int port = 11000;
        private readonly TcpClient client;

        public CommandSender()
        {
            this.client = new TcpClient(address, port);
        }


        public void Disconnect()
        {
            this.SendCommand(PLCCommand.DISCONNECT); // Disconnect from the API
            // Clean up resources
            if (this.client != null)
            {
                this.client.Dispose();
            }

        }

        public void SendCommand(PLCCommand command)
        {
            NetworkStream stream = client.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(command.ToString().ToLower());

            stream.Write(data, 0, data.Length);
        }
    }
}
