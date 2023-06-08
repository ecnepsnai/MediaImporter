namespace MediaImporter
{
    using Microsoft.UI.Xaml;
    using System.Threading.Tasks;
    using System;
    using System.IO;
    using ImageMagick;

    public partial class App : Application
    {
        public App()
        {
            UnhandledException += AppUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += DomainUnhandledException;
            TaskScheduler.UnobservedTaskException += UnobservedTaskException;

            Preferences.Load();
            MagickNET.Initialize();
            this.InitializeComponent();
        }

        private void AppUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            File.WriteAllText("appunhandledex_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", (e.Exception as Exception).Message + "\r\n\r\n" + (e.Exception as Exception).StackTrace);
        }

        private void UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            File.WriteAllText("taskunhandledex_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", (e.Exception as Exception).Message + "\r\n\r\n" + (e.Exception as Exception).StackTrace);
        }

        private void DomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            File.WriteAllText("domainunhandledex_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", (e.ExceptionObject as Exception).Message + "\r\n\r\n" + (e.ExceptionObject as Exception).StackTrace);
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Window = new MainWindow();
            Window.Activate();
        }

        public Window Window { get; private set; }
    }
}
