using ExtrusionUI.Startup.Views;
using System.Windows;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Unity;
using ExtrusionUI.Logic.Navigation;
using Prism.Regions;
using ExtrusionUI.Logic.SerialCommunications;
using ExtrusionUI.Logic.Filament;
using ExtrusionUI.Logic.FileOperations;
using ExtrusionUI.Logic.WebService;

namespace ExtrusionUI.Startup
{
    class Bootstrapper : UnityBootstrapper
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

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            //var moduleCatalog = (ModuleCatalog)ModuleCatalog;

            ModuleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = "Module.Display",
                ModuleType = "ExtrusionUI.Module.Display.DisplayModule, ExtrusionUI.Module.Display",
                InitializationMode = InitializationMode.WhenAvailable,
            });


            //moduleCatalog.AddModule(typeof(YOUR_MODULE));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISerialService, SerialService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IFilamentService, FilamentService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IFileService, FileService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICsvService, CsvService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IXmlService, XmlService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IWebService, Logic.WebService.WebService>(new ContainerControlledLifetimeManager());

            StartFilamentService();
            StartWebService();

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
            IWebService filamentService = Container.Resolve<IWebService>();


        }

        //protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        //{
        //var behaviors = base.ConfigureDefaultRegionBehaviors();
        //behaviors.AddIfMissing(DependentViewRegionBehavior.BehaviorKey, typeof(DependentViewRegionBehavior));
        //return behaviors;
        //}
    }
}
