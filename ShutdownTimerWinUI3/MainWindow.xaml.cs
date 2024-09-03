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
using System.Threading;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;

namespace ShutdownTimerWinUI3
{
    public sealed partial class MainWindow : Window
    {
        private string _DaysSelected;
        private CancellationTokenSource _cancellationTokenSource;
        private int _TimeinSeconds;
        private string _selectedItemOutput;
        private DispatcherTimer _timer;
        private TimeSpan _time;
        public MainWindow()
        {
            this.InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
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
            appWindowPresenter.IsResizable = false;
            appWindowPresenter.IsMaximizable = false;
            appWindowPresenter.IsMinimizable = true;
        }

        private void Timer_Tick(object sender, object e)
        {
            if (_time.TotalSeconds > 0)
            {
                _time = _time.Add(TimeSpan.FromSeconds(-1));
                TimerText.Text = _time.ToString(@"d\:hh\:mm\:ss");
            }
            else
            {
                _timer.Stop();
                TimerText.Text = "Time's up!";
            }
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

        private void TimerEnd_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            // Only store the selected time, do not set the timer here
            TimeSpan _selectedTime = e.NewTime;
            Debug.WriteLine("Time changed: " + _selectedTime);
        }

        private async void BeginTimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedItemOutput))
            {
                Debug.WriteLine("No action selected.");
                return;
            }
            else if (TimerText.Text == _selectedItemOutput + " will be performed in hour(s): -00:00:00.0000001")
            {
                Debug.WriteLine("Test");
            }   
            else if (_TimeinSeconds == 0)
            {
                Debug.WriteLine("No time selected.");
                return;
            }
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            _time = TimeSpan.FromSeconds(_TimeinSeconds); // Set the countdown time here
            TimerText.Text = _time.ToString(@"d\:hh\:mm\:ss");

            _timer.Start();

            BeginTimer.Content = "Timer Running...";
            try
            {
                await Task.Delay(_TimeinSeconds * 1000, token);
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Task was canceled.");
                return;
            }

            if (_selectedItemOutput == "Shutdown")
            {
                // Shutdown the system
                BeginTimer.Content = _selectedItemOutput + "ing...";
                System.Diagnostics.Process.Start("shutdown", "/s /t 0");
            }
            else if (_selectedItemOutput == "Restart")
            {
                // Restart the system
                BeginTimer.Content = _selectedItemOutput + "ing...";
                System.Diagnostics.Process.Start("shutdown", "/r /t 0");
            }
            else if (_selectedItemOutput == "Sleep")
            {
                // Put the system to sleep
                BeginTimer.Content = _selectedItemOutput + "ing...";
                SetSuspendState(false, true, true);
            }
            else if (_selectedItemOutput == "Hibernate")
            {
                // Hibernate the system
                BeginTimer.Content = _selectedItemOutput + "ing...";
                SetSuspendState(true, true, true);
            }
            else if (_selectedItemOutput == "Beep")
            {
                // First is frequency, second is duration (1000 per second).
                BeginTimer.Content = _selectedItemOutput + "ing...";
                Console.Beep(5000, 1000);
            }
            else if (_selectedItemOutput == "Logoff")
            {
                // Logoff the current user
                BeginTimer.Content = _selectedItemOutput + "ing...";
                System.Diagnostics.Process.Start("shutdown", "/l /t 0");
            }
            else if (_selectedItemOutput == "Lock")
            {
                // Lock the system
                BeginTimer.Content = _selectedItemOutput + "ing...";
                System.Diagnostics.Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
            }
            else
            {
                // Do nothing
                BeginTimer.Content = _selectedItemOutput + "ing...";
                Debug.WriteLine("No action selected.");
            }
            BeginTimer.Content = "Begin Timer";
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _timer.Stop();
                TimerText.Text = "Timer cancelled.";
                BeginTimer.Content = "Begin Timer";
            } else {
                CancelCount.Content = "Error...";
                Debug.WriteLine("No timer active.");
                await Task.Delay(5000);
                CancelCount.Content = "Cancel";
            }
        }

        private void DayPicker_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            // Only store the selected days, do not set the timer here
            _DaysSelected = args.NewText;
            Debug.WriteLine("Days selected: " + _DaysSelected);
        }
        
        private void SetTimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (TimerEnd == null || TimerEnd.Time == TimeSpan.Zero)
            {
                Debug.WriteLine("Invalid. TimerEnd is null or no time selected.");
                return; // Exit the method if TimerEnd is null or no time is selected
            }

            if (string.IsNullOrEmpty(_DaysSelected) || (_DaysSelected.Length > 6))
            {
                Debug.WriteLine("Invalid. Days selected: " + _DaysSelected + " & Days selected length: " + _DaysSelected.Length);
                return; // Exit the method if the input is invalid
            }
            int DaysInSeconds = int.Parse(_DaysSelected) * 86400;

            if (string.IsNullOrEmpty(_DaysSelected) || (_DaysSelected.Length > 6))
            {
                Debug.WriteLine("Invalid. Days selected: " + _DaysSelected + " & Days selected length: " + _DaysSelected.Length);
            }
            else
            {
                // Get the selected time
                TimeSpan _selectedTime = TimerEnd.Time; // Assuming TimerEnd is a TimePicker
                Debug.WriteLine("Time changed: " + _selectedTime);
                if (_DaysSelected == "0")
                {
                    TimerText.Text = _selectedItemOutput + " will be performed in hour(s): " + _selectedTime;
                }
                else
                {
                    TimerText.Text = _selectedItemOutput + " will be performed in day(s): " + _DaysSelected + ":" + _selectedTime;
                }
                _TimeinSeconds = (int)_selectedTime.TotalSeconds + DaysInSeconds;
                if (TimerText.Text == _selectedItemOutput + " will be performed in hour(s): -00:00:00.0000001")
                {
                    TimerText.Text = _selectedItemOutput + " will be performed instantly.";
                }
                Debug.WriteLine("Time in seconds: " + _TimeinSeconds);
            }
        }

    }
}
