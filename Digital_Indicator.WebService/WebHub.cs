﻿using Digital_Indicator.Logic.Filament;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Logic.WebService
{
    [HubName("WebServiceHub")]
    public class WebHub : Hub
    {

        IFilamentService _filamentService;

        IHubContext hubContext;

        public WebHub()
        {
            _filamentService = WebService.GetFilamentService();
            _filamentService.DiameterChanged += _filamentService_DiameterChanged;

            hubContext = GlobalHost.ConnectionManager.GetHubContext<WebHub>();
        }

        private void _filamentService_DiameterChanged(object sender, EventArgs e)
        {
            hubContext.Clients.All.ReceiveData(_filamentService.FilamentServiceVariables["ActualDiameter"]);
        }

        public void Send(object obj)
        {
           // _filamentService = WebService.GetFilamentService();

            //Clients.All.ReceiveData(_filamentService.FilamentServiceVariables["ActualDiameter"]);
            //Clients.All.ReceiveData("blah");
            //return message;
        }

        
    }
}
