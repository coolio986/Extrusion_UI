using ExtrusionUI.Module.Display.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using Microsoft.Practices.Unity;
using Prism.Unity;
using ExtrusionUI.Logic.Navigation;

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

        public void Initialize()
        {
            _container.RegisterTypeForNavigation<DiameterView>();
            _container.RegisterTypeForNavigation<SerialPortSelectionView>();
            _container.RegisterTypeForNavigation<SettingsView>();
            _container.RegisterTypeForNavigation<AutoDetectSerialPort>();

            //_naviService.NavigateTo("DiameterView");

            //_naviService.NavigateTo("SerialPortSelectionView");
            _naviService.NavigateTo("AutoDetectSerialPort");
            
        }
    }
}