using PrismTest.ViewModels;
using PrismTest.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PrismTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Trace.WriteLine(DateTime.Now + ": Fatal Unhandled Exception:  " + args.Exception);
            args.Handled = true;
            Environment.Exit(0);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            LoginRegisterViewModel loginRegister = new LoginRegisterViewModel();
            mainWindow.DataContext = loginRegister;
            mainWindow.AddGameButton.Visibility = Visibility.Hidden;
            mainWindow.Menu.Visibility = Visibility.Collapsed;
            mainWindow.ContentControl.Margin = new Thickness(0);
            mainWindow.Show();
        }
    }
}
