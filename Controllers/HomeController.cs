using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PoC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PoC.Connection;
using PoC.Connection.Interfaces;

namespace PoC.Controllers
{
    public class HomeController : Controller
    {
       private readonly ICommandSender commandSender;


        public HomeController(ICommandSender sender)
        {
            this.commandSender = sender;
        }

        public IActionResult Index()
        {
            return View();
        }

        public void Command([FromQuery]string command)
        {
            PLCCommand c = (PLCCommand)Enum.Parse(typeof(PLCCommand), command);
            commandSender.SendCommand(c);
        }

        public void Disconnect()
        {
            commandSender.Disconnect();
        }
    }
}
