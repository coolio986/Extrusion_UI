using ExtrusionUI.Startup.Views;
using System.Windows;
using Prism.Modularity;
using Unity;
using Prism.Unity;
using Prism.Ioc;
using ExtrusionUI.Logic.Navigation;
using Prism.Regions;
using ExtrusionUI.Logic.SerialCommunications;
using ExtrusionUI.Logic.Filament;
using ExtrusionUI.Logic.FileOperations;
//using ExtrusionUI.Logic.WebService;
using ExtrusionUI.Logic.UI_Intelligence;
using ExtrusionUI.Logic.ModbusTCP;
using ExtrusionUI.Module.Display;

namespace ExtrusionUI.Startup
{
    class Bootstrapper : PrismBootstrapper
    {
        string[] startArgs;

        public Bootstrapper(string[] args)
        {
            startArgs = args;
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<INavigationService, NavigationService>();
            containerRegistry.Register<ISerialService, SerialService>();
            containerRegistry.Register<IModbusTCPService, ModbusTCPService>();
            containerRegistry.Register<IFilamentService, FilamentService>();
            containerRegistry.Register<IFileService, FileService>();
            containerRegistry.Register<ICsvService, CsvService>();
            containerRegistry.Register<IXmlService, XmlService>();
            //Container.RegisterType<IWebService, Logic.WebService.WebService>(new ContainerControlledLifetimeManager());
            containerRegistry.Register<IUI_IntelligenceService, UI_IntelligenceService>();

            StartFilamentService();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //moduleCatalog.AddModule(new ModuleInfo()
            //{
            //    ModuleName = "Module.Display",
            //    ModuleType = "ExtrusionUI.Module.Display.DisplayModule, ExtrusionUI.Module.Display",
            //    InitializationMode = InitializationMode.WhenAvailable,
            //});
            moduleCatalog.AddModule<DisplayModule>();
        }



        private void StartFilamentService()
        {
            bool simulation = false;

            foreach (string arg in startArgs)
            {
                simulation = arg.Contains("-s");
                if (simulation)
                    break;
            }
            IFilamentService filamentService = Container.Resolve<IFilamentService>();

            filamentService.IsSimulationModeActive = simulation;
        }

        private void StartWebService()
        {
            //IWebService filamentService = Container.Resolve<IWebService>();


        }

        //protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        //{
        //var behaviors = base.ConfigureDefaultRegionBehaviors();
        //behaviors.AddIfMissing(DependentViewRegionBehavior.BehaviorKey, typeof(DependentViewRegionBehavior));
        //return behaviors;
        //}
    }
}
