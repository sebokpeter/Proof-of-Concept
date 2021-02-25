using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoC.Connection.Interfaces
{
    /// <summary>
    /// Send commands to the PLC API.
    /// </summary>
    public interface ICommandSender
    {
        /// <summary>
        /// Send a given command to the PLC API.
        /// </summary>
        /// <param name="command">The command that will be sent.</param>
        public void SendCommand(PLCCommand command);

        /// <summary>
        /// Disconnect from the PLC API.
        /// </summary>
        public void Disconnect();
    }
}
