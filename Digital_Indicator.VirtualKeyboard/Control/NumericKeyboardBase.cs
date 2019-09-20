using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Digital_Indicator.VirtualKeyboard.Control
{
    public static class NumericKeyboardBase
    {
        #region Private data

        private static NumericKeyboard _numericKeyboard;
        private static readonly double MinimumWidth = 180.0;
        private static readonly double MinimumHeight = 200.0;

        #endregion Private data

        #region Public Attached Properties

        /// <summary>
        /// Placement
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.RegisterAttached("Placement",
            typeof(PlacementMode),
            typeof(NumericKeyboardBase),
            new FrameworkPropertyMetadata(PlacementMode.Bottom,
                new PropertyChangedCallback(NumericKeyboardBase.OnPlacementChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static PlacementMode GetPlacement(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (PlacementMode)element.GetValue(PlacementProperty);
        }

        public static void SetPlacement(DependencyObject element, PlacementMode value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(PlacementProperty, value);
        }

        /// <summary>
        /// PlacementTarget
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
           DependencyProperty.RegisterAttached("PlacementTarget",
           typeof(UIElement),
           typeof(NumericKeyboardBase),
           new FrameworkPropertyMetadata(null,
               new PropertyChangedCallback(NumericKeyboardBase.OnPlacementTargetChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static UIElement GetPlacementTarget(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (UIElement)element.GetValue(PlacementTargetProperty);
        }

        public static void SetPlacementTarget(DependencyObject element, UIElement value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(PlacementTargetProperty, value);
        }

        /// <summary>
        /// PlacementRectangle
        /// </summary>
        public static readonly DependencyProperty PlacementRectangleProperty =
            DependencyProperty.RegisterAttached("PlacementRectangle",
            typeof(Rect),
            typeof(NumericKeyboardBase),
            new FrameworkPropertyMetadata(Rect.Empty,
                new PropertyChangedCallback(NumericKeyboardBase.OnPlacementRectangleChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static Rect GetPlacementRectangle(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (Rect)element.GetValue(PlacementRectangleProperty);
        }

        public static void SetPlacementRectangle(DependencyObject element, Rect value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(PlacementRectangleProperty, value);
        }

        /// <summary>
        /// HorizontalOffset
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached("HorizontalOffset",
            typeof(double),
            typeof(NumericKeyboardBase),
            new FrameworkPropertyMetadata(0.0,
                new PropertyChangedCallback(NumericKeyboardBase.OnHorizontalOffsetChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetHorizontalOffset(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (double)element.GetValue(HorizontalOffsetProperty);
        }

        public static void SetHorizontalOffset(DependencyObject element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(HorizontalOffsetProperty, value);
        }

        /// <summary>
        /// VerticalOffset
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset",
            typeof(double),
            typeof(NumericKeyboardBase),
            new FrameworkPropertyMetadata(0.0,
                new PropertyChangedCallback(NumericKeyboardBase.OnVerticalOffsetChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetVerticalOffset(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (double)element.GetValue(VerticalOffsetProperty);
        }

        public static void SetVerticalOffset(DependencyObject element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(VerticalOffsetProperty, value);
        }

        /// <summary>
        /// CustomPopupPlacementCallback
        /// </summary>
        public static readonly DependencyProperty CustomPopupPlacementCallbackProperty =
            DependencyProperty.RegisterAttached("CustomPopupPlacementCallback",
            typeof(CustomPopupPlacementCallback),
            typeof(NumericKeyboardBase),
            new FrameworkPropertyMetadata(null,
                new PropertyChangedCallback(NumericKeyboardBase.OnCustomPopupPlacementCallbackChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static CustomPopupPlacementCallback GetCustomPopupPlacementCallback(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (CustomPopupPlacementCallback)element.GetValue(CustomPopupPlacementCallbackProperty);
        }

        public static void SetCustomPopupPlacementCallback(DependencyObject element, CustomPopupPlacementCallback value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(CustomPopupPlacementCallbackProperty, value);
        }

        /// <summary>
        /// State
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached("State",
            typeof(NumericKeyboardState),
            typeof(NumericKeyboardBase),
            new FrameworkPropertyMetadata(NumericKeyboardState.Normal,
                new PropertyChangedCallback(NumericKeyboardBase.OnStateChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static NumericKeyboardState GetState(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (NumericKeyboardState)element.GetValue(StateProperty);
        }

        public static void SetState(DependencyObject element, NumericKeyboardState value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(StateProperty, value);
        }

        /// <summary>
        /// Height
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.RegisterAttached("Height",
            typeof(double),
            typeof(NumericKeyboardBase),
            new FrameworkPropertyMetadata(MinimumHeight,
                new PropertyChangedCallback(NumericKeyboardBase.OnHeightChanged),
                new CoerceValueCallback(NumericKeyboardBase.CoerceHeight)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetHeight(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (double)element.GetValue(HeightProperty);
        }

        public static void SetHeight(DependencyObject element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(HeightProperty, value);
        }

        /// <summary>
        /// Width
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width",
            typeof(double),
            typeof(NumericKeyboardBase),
            new FrameworkPropertyMetadata(MinimumWidth,
                new PropertyChangedCallback(NumericKeyboardBase.OnWidthChanged),
                new CoerceValueCallback(NumericKeyboardBase.CoerceWidth)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetWidth(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (double)element.GetValue(WidthProperty);
        }

        public static void SetWidth(DependencyObject element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(WidthProperty, value);
        }

        /// <summary>
        /// IsEnabled
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled",
            typeof(bool),
            typeof(NumericKeyboardBase),
            new FrameworkPropertyMetadata(false,
                new PropertyChangedCallback(NumericKeyboardBase.OnIsEnabledChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetIsEnabled(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (bool)element.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(IsEnabledProperty, value);
        }

        #endregion Public Attached Properties

        #region Private Methods

        /// <summary>
        /// PropertyChangedCallback method for Placement Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnPlacementChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (NumericKeyboardBase._numericKeyboard != null))
            {
                _numericKeyboard.Placement = NumericKeyboardBase.GetPlacement(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for PlacementTarget Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnPlacementTargetChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (NumericKeyboardBase._numericKeyboard != null))
            {
                _numericKeyboard.PlacementTarget = NumericKeyboardBase.GetPlacementTarget(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for PlacementRectangle Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnPlacementRectangleChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (NumericKeyboardBase._numericKeyboard != null))
            {
                _numericKeyboard.PlacementRectangle = NumericKeyboardBase.GetPlacementRectangle(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for HorizontalOffset Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnHorizontalOffsetChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (NumericKeyboardBase._numericKeyboard != null))
            {
                _numericKeyboard.HorizontalOffset = NumericKeyboardBase.GetHorizontalOffset(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for VerticalOffset Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnVerticalOffsetChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (NumericKeyboardBase._numericKeyboard != null))
            {
                _numericKeyboard.VerticalOffset = NumericKeyboardBase.GetVerticalOffset(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for CustomPopupPlacementCallback Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnCustomPopupPlacementCallbackChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (NumericKeyboardBase._numericKeyboard != null))
            {
                _numericKeyboard.CustomPopupPlacementCallback = NumericKeyboardBase.GetCustomPopupPlacementCallback(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for State Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnStateChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (NumericKeyboardBase._numericKeyboard != null))
            {
                _numericKeyboard.State = NumericKeyboardBase.GetState(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for Height Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnHeightChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (NumericKeyboardBase._numericKeyboard != null))
            {
                _numericKeyboard.Height = NumericKeyboardBase.GetHeight(frameworkElement);
            }
        }

        /// <summary>
        /// CoerceValueCallback method for Height Attached Property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceHeight(DependencyObject d, object value)
        {
            if ((double)value < NumericKeyboardBase.MinimumHeight)
                return NumericKeyboardBase.MinimumHeight;

            return value;
        }

        /// <summary>
        /// PropertyChangedCallback method for Width Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnWidthChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (NumericKeyboardBase._numericKeyboard != null))
            {
                _numericKeyboard.Width = NumericKeyboardBase.GetWidth(frameworkElement);
            }
        }

        /// <summary>
        /// CoerceValueCallback method for Width Attached Property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceWidth(DependencyObject d, object value)
        {
            if ((double)value < NumericKeyboardBase.MinimumWidth)
                return NumericKeyboardBase.MinimumWidth;

            return value;
        }

        /// <summary>
        /// PropertyChangedCallback method for IsEnabled Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnIsEnabledChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            // Attach & detach handlers for events GotKeyboardFocus, LostKeyboardFocus, MouseDown, and SizeChanged
            if (frameworkElement != null)
            {
                if (((bool)e.NewValue == true) && ((bool)e.OldValue == false))
                {
                    frameworkElement.AddHandler(FrameworkElement.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(frameworkElement_GotKeyboardFocus), true);
                    frameworkElement.AddHandler(FrameworkElement.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(frameworkElement_LostKeyboardFocus), true);
                    frameworkElement.AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(frameworkElement_MouseDown), true);
                    frameworkElement.AddHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(frameworkElement_MouseUp), true);
                    frameworkElement.AddHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(frameworkElement_SizeChanged), true);
                    frameworkElement.AddHandler(FrameworkElement.KeyDownEvent, new KeyEventHandler(frameworkElement_KeyboardDown), true);
                }
                else if (((bool)e.NewValue == false) && ((bool)e.OldValue == true))
                {
                    frameworkElement.RemoveHandler(FrameworkElement.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(frameworkElement_GotKeyboardFocus));
                    frameworkElement.RemoveHandler(FrameworkElement.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(frameworkElement_LostKeyboardFocus));
                    frameworkElement.RemoveHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(frameworkElement_MouseDown));
                    frameworkElement.RemoveHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(frameworkElement_MouseUp));
                    frameworkElement.RemoveHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(frameworkElement_SizeChanged));
                    frameworkElement.RemoveHandler(FrameworkElement.KeyDownEvent, new KeyEventHandler(frameworkElement_KeyboardDown));
                }
            }

            Window currentWindow = Window.GetWindow(element);

            // Attach or detach handler for event LocationChanged
            if (currentWindow != null)
            {
                if (((bool)e.NewValue == true) && ((bool)e.OldValue == false))
                {
                    currentWindow.LocationChanged += currentWindow_LocationChanged;
                }
                else if (((bool)e.NewValue == false) && ((bool)e.OldValue == true))
                {
                    currentWindow.LocationChanged -= currentWindow_LocationChanged;
                }
            }
        }

        /// <summary>
        /// Event handler for Keyboard key down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void frameworkElement_KeyboardDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FrameworkElement frameworkElement = sender as FrameworkElement;

                if (frameworkElement != null)
                {
                    if (NumericKeyboardBase._numericKeyboard != null)
                    {
                        // Retrieves the setting for the State property
                        NumericKeyboardBase.SetState(frameworkElement, _numericKeyboard.State);
                        _numericKeyboard.HideKeyboard();
                    }
                }
            }
        }


        /// <summary>
        /// Event handler for GotKeyboardFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void frameworkElement_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (NumericKeyboardBase._numericKeyboard == null)
                {
                    _numericKeyboard = new NumericKeyboard();

                    // Set all the necessary properties
                    _numericKeyboard.Placement = NumericKeyboardBase.GetPlacement(frameworkElement);
                    _numericKeyboard.PlacementTarget = NumericKeyboardBase.GetPlacementTarget(frameworkElement);
                    _numericKeyboard.PlacementRectangle = NumericKeyboardBase.GetPlacementRectangle(frameworkElement);
                    _numericKeyboard.HorizontalOffset = NumericKeyboardBase.GetHorizontalOffset(frameworkElement);
                    _numericKeyboard.VerticalOffset = NumericKeyboardBase.GetVerticalOffset(frameworkElement);
                    _numericKeyboard.StaysOpen = true;
                    _numericKeyboard.CustomPopupPlacementCallback = NumericKeyboardBase.GetCustomPopupPlacementCallback(frameworkElement);
                    _numericKeyboard.State = NumericKeyboardBase.GetState(frameworkElement);
                    _numericKeyboard.Height = NumericKeyboardBase.GetHeight(frameworkElement);
                    _numericKeyboard.Width = NumericKeyboardBase.GetWidth(frameworkElement);

                    if (NumericKeyboardBase.GetState(frameworkElement) == NumericKeyboardState.Normal)
                        NumericKeyboardBase._numericKeyboard.IsOpen = true;
                }

            }
        }

        /// <summary>
        /// Event handler for LostKeyboardFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void frameworkElement_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {

            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (NumericKeyboardBase._numericKeyboard != null)
                {
                    // Retrieves the setting for the State property
                    NumericKeyboardBase.SetState(frameworkElement, _numericKeyboard.State);

                    NumericKeyboardBase._numericKeyboard.IsOpen = false;
                    NumericKeyboardBase._numericKeyboard.State = NumericKeyboardState.Hidden;
                    NumericKeyboardBase._numericKeyboard = null;
                }


            }
        }

        /// <summary>
        /// Event handler for MouseDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void frameworkElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (NumericKeyboardBase._numericKeyboard != null)
                {
                    NumericKeyboardBase._numericKeyboard.State = NumericKeyboardState.Normal;
                    NumericKeyboardBase._numericKeyboard.IsOpen = true;

                    if (frameworkElement is System.Windows.Controls.TextBox)
                    {
                        ((System.Windows.Controls.TextBox)frameworkElement).SelectAll();
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for MouseDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void frameworkElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (frameworkElement is System.Windows.Controls.TextBox)
                {
                    ((System.Windows.Controls.TextBox)frameworkElement).SelectAll();
                }
            }
        }

        /// <summary>
        /// Event handler for SizeChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void frameworkElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (NumericKeyboardBase._numericKeyboard != null &&
                    NumericKeyboardBase._numericKeyboard.State == NumericKeyboardState.Normal)
                {
                    NumericKeyboardBase._numericKeyboard.LocationChange();
                }
            }
        }

        /// <summary>
        /// Event handler for LocationChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void currentWindow_LocationChanged(object sender, EventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (NumericKeyboardBase._numericKeyboard != null &&
                    NumericKeyboardBase._numericKeyboard.State == NumericKeyboardState.Normal)
                {
                    NumericKeyboardBase._numericKeyboard.LocationChange();
                }
            }
        }

        #endregion Private Methods
    }


}

