using ExtrusionUI.Logic.Filament;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace ExtrusionUI.Module.Display.Views
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

        public static readonly DependencyProperty UnitProperty =
        DependencyProperty.Register("Unit", typeof(string), typeof(HighSpeedTextBlockBehavior),
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

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
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
            if (FilamentService.FilamentServiceVariables.ContainsKey(VariableName))
            {
                var TextValue = FilamentService.FilamentServiceVariables[VariableName];
                TextValue = TextValue == string.Empty ? "" : TextValue + $" {Unit}";
                AssociatedObject.Text = TextValue;
            }
        }

    }
}
