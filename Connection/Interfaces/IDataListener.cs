using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace PoC.Connection.Interfaces
{
    /// <summary>
    /// Listens to data from the PLC API.
    /// </summary>
    public interface IDataListener
    {
        /// <summary>
        /// Start listening to the PLC API.
        /// </summary>
        public void StartListening();

        /// <summary>
        /// Sets the web socket that will be used to transmit updates to the front end.
        /// </summary>
        /// <param name="socket">The websocket that will transmit updates.</param>
        /// <param name="tsc"></param>
        public void SetWebSocket(WebSocket socket, TaskCompletionSource<object> tcs);
    }
}
