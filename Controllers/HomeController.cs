using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PoC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PoC.Connection;

namespace PoC.Controllers
{
    public class HomeController : Controller
    {
       private readonly APIConnection connection;


        public HomeController(APIConnection connection)
        {
            this.connection = connection;
        }

        public IActionResult Index()
        {
            return View();
        }

        public void Command([FromQuery]string command)
        {
            connection.SendCommand(command);
        }

        public void Disconnect()
        {
            connection.Disconnect();
        }
    }
}
