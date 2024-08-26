using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Runtime.InteropServices;
using Windows.Management;
using System.Threading.Tasks;

namespace ShutdownTimerWinUI3
{
    public sealed partial class MainWindow : Window
    {
        private int _TimeinSeconds;
        private string _selectedItemOutput;
        public MainWindow()
        {
            this.InitializeComponent();
            //TimerEnd.TimeChanged += TimerEnd_TimeChanged;
            Title = "Shutdown Timer";

            // Get the display area
            var displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary);
            var workArea = displayArea.WorkArea;

            // Set the window size to a percentage of the work area dimensions
            int windowWidth = (int)(workArea.Width * 0.24);
            int windowHeight = (int)(workArea.Height * 0.3);

            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(windowWidth, windowHeight));

            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;

            // Set the window to be resizable, maximizable, and minimizable
            appWindowPresenter.IsResizable = true;
            appWindowPresenter.IsMaximizable = true;
            appWindowPresenter.IsMinimizable = true;
        }
        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        private void SelectActionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectAction.SelectedItem is ComboBoxItem selectedItem)
            {
                Debug.WriteLine("Selection changed: " + selectedItem.Content);
                _selectedItemOutput = selectedItem.Content.ToString();
                Debug.WriteLine("Selected item: " + _selectedItemOutput);
            }
            else
            {
                Debug.WriteLine("Selection changed: null");
                _selectedItemOutput = "null";
            }
        }

        //private void TimerEnd_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        //{
        //    Debug.WriteLine("Time sat: " + TimerEnd);
        //    Debug.WriteLine("TimerEnd event has triggered.");
        //}
        private void TimerEnd_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            // Get the selected time
            TimeSpan _selectedTime = e.NewTime;

            Debug.WriteLine("Time changed: " + _selectedTime);
            TimerText.Text = _selectedItemOutput + " will be performed in hour(s): " + _selectedTime;
            _TimeinSeconds = (int)_selectedTime.TotalSeconds;
            Debug.WriteLine("Time in seconds: " + _TimeinSeconds);
        }

        private async void BeginTimerButton_Click(object sender, RoutedEventArgs e)
        {
            BeginTimer.Content = _selectedItemOutput + "ing...";
            await Task.Delay(_TimeinSeconds * 1000);
            
            if (_selectedItemOutput == "Shutdown")
            {
                // Shutdown the system
                System.Diagnostics.Process.Start("shutdown", "/s /t 0");
            }
            else if (_selectedItemOutput == "Restart")
            {
                // Restart the system
                System.Diagnostics.Process.Start("restart", "/r /t 0");
            }
            else if (_selectedItemOutput == "Sleep")
            {
                // Put the system to sleep
                SetSuspendState(false, true, true);

            }
            else if (_selectedItemOutput == "Hibernate")
            {
                // Hibernate the system
                SetSuspendState(true, true, true);
            }
            else if (_selectedItemOutput == "Beep")
            {
                // Beep at 5000 Hz for 1 second
                Console.Beep(5000, 1000);
            }
            else
            {
                // Do nothing
                Debug.WriteLine("No action selected.");
            }
            BeginTimer.Content = "Begin Timer";
        }

        private void GetHelpButton_Click(object sender, RoutedEventArgs e)
        { 
            GetHelp.Content = "Help is not available :(";
        }
    }
}
