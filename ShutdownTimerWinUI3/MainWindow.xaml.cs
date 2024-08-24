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

namespace ShutdownTimerWinUI3
{
    public sealed partial class MainWindow : Window
    {
        private string _selectedItemOutput;
        public MainWindow()
        {
            this.InitializeComponent();
            Title = "Shutdown Timer";

            // Get the display area
            var displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary);
            var workArea = displayArea.WorkArea;

            // Set the window size to a percentage of the work area dimensions
            int windowWidth = (int)(workArea.Width * 0.24); // 50% of work area width
            int windowHeight = (int)(workArea.Height * 0.3); // 50% of work area height

            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(windowWidth, windowHeight));

            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;

            appWindowPresenter.IsResizable = false;
            appWindowPresenter.IsMaximizable = false;
            //appWindowPresenter.IsMinimizable = false;
        }

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

        private void BeginTimerButton_Click(object sender, RoutedEventArgs e)
        {
            BeginTimer.Content = _selectedItemOutput + "ing...";
            //BeginTimer.Content = "Timer has not started :)";
            //System.Diagnostics.Process.Start("shutdown", "/s /t 0");
        }

        private void GetHelpButton_Click(object sender, RoutedEventArgs e)
        {
            GetHelp.Content = "Help is not available :(";
        }
    }
}
