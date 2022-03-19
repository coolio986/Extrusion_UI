
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ExtrusionUI.VirtualKeyboard.Control;
using Prism.Commands;

namespace ExtrusionUI.VirtualKeyboard
{
    /// <summary>
    /// Interaction logic for PopupKeyboard.xaml
    /// </summary>
    public partial class NumericKeyboard : Popup
    {
        #region Private data

        //private Popup _parentPopup;
        private Storyboard storyboard;
        private const double AnimationDelay = 150;
        public DelegateCommand<object> numericButtonClick { get; set; }
        public DelegateCommand<object> closeClick { get; set; }
        public DelegateCommand<object> enterClick { get; set; }

        #endregion Private data

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NumericKeyboard()
        {
            InitializeComponent();

            numericButtonClick = new DelegateCommand<object>(numericButton_Click);
            closeClick = new DelegateCommand<object>(close_Click);
            enterClick = new DelegateCommand<object>(enter_Click);

        }
        #endregion Constructor

        #region Properties

        /// <summary>
        /// State
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State",
            typeof(NumericKeyboardState),
            typeof(NumericKeyboard),
            new FrameworkPropertyMetadata(NumericKeyboardState.Normal,
                new PropertyChangedCallback(StateChanged)));

        public NumericKeyboardState State
        {
            get { return (NumericKeyboardState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// PropertyChangedCallback method for State Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void StateChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            NumericKeyboard ctrl = (NumericKeyboard)element;

            if ((NumericKeyboardState)e.NewValue == NumericKeyboardState.Normal)
            {
                ctrl.IsOpen = true;
            }
            else if ((NumericKeyboardState)e.NewValue == NumericKeyboardState.Hidden)
            {
                if (ctrl.IsOpen)
                    ctrl.HideKeyboard();
            }
        }

        /// <summary>
        /// Animation to hide keyboard
        /// </summary>
        public void HideKeyboard()
        {

            //TODO NOT MVVM NEED TO REFACTOR

            // Animation to hide the keyboard
            this.RegisterName("HidePopupKeyboard", this);

            storyboard = new Storyboard();
            storyboard.Completed += new EventHandler(storyboard_Completed);

            DoubleAnimation widthAnimation = new DoubleAnimation();
            widthAnimation.From = this.Width;
            widthAnimation.To = 0.0;
            widthAnimation.Duration = TimeSpan.FromMilliseconds(AnimationDelay);
            widthAnimation.FillBehavior = FillBehavior.Stop;

            DoubleAnimation heightAnimation = new DoubleAnimation();
            heightAnimation.From = this.Height;
            heightAnimation.To = 0.0;
            heightAnimation.Duration = TimeSpan.FromMilliseconds(AnimationDelay);
            heightAnimation.FillBehavior = FillBehavior.Stop;

            Storyboard.SetTargetName(widthAnimation, "HidePopupKeyboard");
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(NumericKeyboard.WidthProperty));
            Storyboard.SetTargetName(heightAnimation, "HidePopupKeyboard");
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(NumericKeyboard.HeightProperty));
            storyboard.Children.Add(widthAnimation);
            storyboard.Children.Add(heightAnimation);

            storyboard.Begin(this);
            
        }

        /// <summary>
        /// Event handler for storyboard Completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void storyboard_Completed(object sender, EventArgs e)
        {
            this.IsOpen = false;
            storyboard.Completed -= new EventHandler(storyboard_Completed);
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Method to update Popup window location
        /// </summary>
        public void LocationChange()
        {
            //if (this._parentPopup != null)
            //{
            //    this.IsOpen = false;
            //    _parentPopup.PopupAnimation = PopupAnimation.None;
            //    this.IsOpen = true;
            //    _parentPopup.PopupAnimation = PopupAnimation.Scroll;
            //}
        }

        #endregion Public Methods

        #region Keyboard Constants

        private const uint KEYEVENTF_KEYUP = 0x2;  // Release key
        private const byte VK_BACK = 0x8;          // back space
        private const byte VK_LEFT = 0x25;
        private const byte VK_RIGHT = 0x27;
        private const byte VK_0 = 0x30;
        private const byte VK_1 = 0x31;
        private const byte VK_2 = 0x32;
        private const byte VK_3 = 0x33;
        private const byte VK_4 = 0x34;
        private const byte VK_5 = 0x35;
        private const byte VK_6 = 0x36;
        private const byte VK_7 = 0x37;
        private const byte VK_8 = 0x38;
        private const byte VK_9 = 0x39;
        private const byte VK_RETURN = 0x0D;
        private const byte VK_TAB = 0x09;

        #endregion Keyboard Constants

        #region Keyboard Private Methods

        /// <summary>
        /// Event handler for all numeric keyboard events
        /// </summary>
        private void numericButton_Click(object arg1)
        {
            try
            {
                Button key = (Button)arg1;

                switch (key.Name)
                {
                    // Number 1
                    case "btn010300":
                        keybd_event(VK_1, 0, 0, (UIntPtr)0);
                        keybd_event(VK_1, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Number 2
                    case "btn010301":
                        keybd_event(VK_2, 0, 0, (UIntPtr)0);
                        keybd_event(VK_2, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Number 3
                    case "btn010302":
                        keybd_event(VK_3, 0, 0, (UIntPtr)0);
                        keybd_event(VK_3, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Number 4
                    case "btn010200":
                        keybd_event(VK_4, 0, 0, (UIntPtr)0);
                        keybd_event(VK_4, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Number 5
                    case "btn010201":
                        keybd_event(VK_5, 0, 0, (UIntPtr)0);
                        keybd_event(VK_5, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Number 6
                    case "btn010202":
                        keybd_event(VK_6, 0, 0, (UIntPtr)0);
                        keybd_event(VK_6, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Number 7
                    case "btn010100":
                        keybd_event(VK_7, 0, 0, (UIntPtr)0);
                        keybd_event(VK_7, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                       // e.Handled = true;
                        break;
                    // Number 8
                    case "btn010101":
                        keybd_event(VK_8, 0, 0, (UIntPtr)0);
                        keybd_event(VK_8, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Number 9
                    case "btn010102":
                        keybd_event(VK_9, 0, 0, (UIntPtr)0);
                        keybd_event(VK_9, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Number 0
                    case "btn010400":
                        keybd_event(VK_0, 0, 0, (UIntPtr)0);
                        keybd_event(VK_0, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Symbol minus sign
                    case "btn010103":
                        keybd_event(0xbd, 0, 0, (UIntPtr)0);
                        keybd_event(0xbd, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Back Space
                    case "btn010402":
                        keybd_event(VK_BACK, 0, 0, (UIntPtr)0);
                        keybd_event(VK_BACK, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Left arrow Key
                    case "btn010203":
                        keybd_event(VK_LEFT, 0, 0, (UIntPtr)0);
                        keybd_event(VK_LEFT, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Right arrow Key
                    case "btn010303":
                        keybd_event(VK_RIGHT, 0, 0, (UIntPtr)0);
                        keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                    // Symbol full stop
                    case "btn010401":
                        keybd_event(0xbe, 0, 0, (UIntPtr)0);
                        keybd_event(0xbe, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        //e.Handled = true;
                        break;
                }
            }
            catch
            {
                // Any exception handling here.  Otherwise, swallow the exception.
            }

        }

        /// <summary>
        /// Event handler to close the UserControl when close button is clicked
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void close_Click(object arg1)
        {
            this.State = NumericKeyboardState.Hidden;
        }

        private void enter_Click(object arg1)
        {

            keybd_event(VK_RETURN, 0, 0, (UIntPtr)0);
            keybd_event(VK_RETURN, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
            this.State = NumericKeyboardState.Hidden;

        }

        #endregion Keyboard Private Methods

        #region Windows API Functions

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        #endregion Windows API Functions
    }



}
