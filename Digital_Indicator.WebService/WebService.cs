using Digital_Indicator.Logic.Filament;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.WebService
{
    [HubName("WebServiceHub")]
    public class WebService : IWebService
    {
        static IFilamentService _filamentService;

        public WebService(IFilamentService filamentService)
        {
            _filamentService = filamentService;

            StartWeb();
        }

        private void StartWeb()
        {
            string url = @"http://localhost:8080/";
            var server = WebApp.Start<Startup>(url);

            //keep webserver alive.
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    //Console.WriteLine(string.Format("Server running at {0}", url));
                    //   Console.ReadLine();
                }
            });
        }

        public static IFilamentService GetFilamentService()
        {
            return _filamentService;
        }
    }
}
