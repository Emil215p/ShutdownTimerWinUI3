using System;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShutdownTimerWinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Title = "Shutdown Timer";
            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(500, 500, 450, 270));
            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;
        }

        private void BeginTimerButton_Click(object sender, RoutedEventArgs e)
        {
            BeginTimer.Content = "Timer has not started :)";
            //System.Diagnostics.Process.Start("shutdown", "/s /t 0");
        }

        private void GetHelpButton_Click(object sender, RoutedEventArgs e)
        {
            GetHelp.Content = "Help is not available :(";
        }
    }
}