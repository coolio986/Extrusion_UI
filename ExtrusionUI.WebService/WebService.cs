using ExtrusionUI.Logic.Filament;
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
using System.Threading;

namespace ExtrusionUI.Logic.WebService
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

            StartOptions options = new StartOptions();
            options.Urls.Add("http://localhost:8080");
            //options.Urls.Add("http://192.168.2.53:8080");
            options.Urls.Add("http://+:8080");

            var server = WebApp.Start<Startup>(options);

            //keep webserver alive.
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    //Console.WriteLine(string.Format("Server running at {0}", url));
                    //   Console.ReadLine();
                    Thread.Sleep(1);
                }
            });
        }

        public static IFilamentService GetFilamentService()
        {
            return _filamentService;
        }
    }
}
