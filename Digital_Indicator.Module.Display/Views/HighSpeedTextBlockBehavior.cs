using Digital_Indicator.Logic.Filament;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Digital_Indicator.Module.Display.Views
{
    public sealed class HighSpeedTextBlockBehavior : Behavior<TextBlock>
    {
        public HighSpeedTextBlockBehavior()
        {
            
        }

        public static readonly DependencyProperty FilamentServiceProperty =
        DependencyProperty.Register("FilamentService", typeof(IFilamentService), typeof(HighSpeedTextBlockBehavior),
        new PropertyMetadata(null));

        public static readonly DependencyProperty VariableNameProperty =
        DependencyProperty.Register("VariableName", typeof(string), typeof(HighSpeedTextBlockBehavior),
        new PropertyMetadata(null));


        public IFilamentService FilamentService
        {
            get { return (IFilamentService)GetValue(FilamentServiceProperty); }
            set { SetValue(FilamentServiceProperty, value); }
        }

        public string VariableName
        {
            get { return (string)GetValue(VariableNameProperty); }
            set { SetValue(VariableNameProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            CompositionTarget.Rendering += OnRendering;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            CompositionTarget.Rendering -= OnRendering;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            AssociatedObject.Text = FilamentService.FilamentServiceVariables[VariableName];
        }

    }
}
