using Digital_Indicator.Startup.Views;
using System.Windows;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Digital_Indicator.Logic.Navigation;
using Prism.Regions;
using Digital_Indicator.Logic.SerialCommunications;

namespace Digital_Indicator.Startup
{
    class Bootstrapper : UnityBootstrapper
    {
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
                ModuleType = "Digital_Indicator.Module.Display.DisplayModule, Digital_Indicator.Module.Display",
                InitializationMode = InitializationMode.WhenAvailable,
            });
            //moduleCatalog.AddModule(typeof(YOUR_MODULE));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISerialService, SerialService>(new ContainerControlledLifetimeManager());
            
        }

        //protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        //{
            //var behaviors = base.ConfigureDefaultRegionBehaviors();
            //behaviors.AddIfMissing(DependentViewRegionBehavior.BehaviorKey, typeof(DependentViewRegionBehavior));
            //return behaviors;
        //}
    }
}
