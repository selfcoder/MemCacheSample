using System;
using System.Threading;
using System.Windows;
using LightInject;

namespace MemCache.WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var container = new ServiceContainer();
            container.RegisterAssembly("MemCache.Client.dll");
            container.Register<MainViewModel>(new PerContainerLifetime());

            var viewModel = container.GetInstance<MainViewModel>();
            var window = new MainWindow();
            window.DataContext = viewModel;
            window.Show();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is ThreadAbortException)
                return;

            // todo
            if (e.IsTerminating)
            {
                //Logger.Fatal("Unhandled", (Exception)e.ExceptionObject);
            }
            else
            {
                //Logger.Error("Unhandled", (Exception)e.ExceptionObject);
            }

            ShowErrorMessage(e.IsTerminating);
        }

        private static void ShowErrorMessage(bool terminating)
        {
            string message = terminating
                ? "An unknown error occurred. The program will be terminated."
                : "An unknown error occurred.";
            MessageBox.Show(message, "MemCache");
        }
    }
}
