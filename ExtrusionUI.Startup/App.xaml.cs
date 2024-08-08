using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ExtrusionUI.Startup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //Not a great way to solve an issue, but one way to keep the software
            //running until the issue
            //"The data area passed to a system call is too small"
            try
            {
                base.OnStartup(e);

                var bootstrapper = new Bootstrapper(e.Args);
                bootstrapper.Run();
            }
            catch { }
        }
    }
}
