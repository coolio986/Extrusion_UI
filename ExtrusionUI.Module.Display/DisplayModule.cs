using ExtrusionUI.Module.Display.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using Prism.Unity;
using ExtrusionUI.Logic.Navigation;
using Unity;
using Prism.Ioc;
using System.Windows.Forms;

namespace ExtrusionUI.Module.Display
{
    public class DisplayModule : IModule
    {
        
        private IUnityContainer _container;
        private INavigationService _naviService;

        public DisplayModule(IUnityContainer container, INavigationService naviService)
        {
            _container = container;
            _naviService = naviService;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _naviService.NavigateTo("AutoDetectSerialPort");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DiameterView>();
            containerRegistry.RegisterForNavigation<SerialPortSelectionView>();
            containerRegistry.RegisterForNavigation<SettingsView>();
            containerRegistry.RegisterForNavigation<AutoDetectSerialPort>();
        }
    }
}