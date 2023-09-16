namespace io.ecn.MediaImporter
{
    internal static class Program
    {
        private readonly static Logger logger = new Logger(typeof(Program));

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LogWriter.Open();
            logger.Info("MediaImporter starting...");

            Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

            Preferences.Load();
            Preferences.Save();

            ApplicationConfiguration.Initialize();
            Application.Run(new MainView());
        }

        static void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            logger.Error($"Unhandled exception: {e.Exception.Message} {e.Exception.StackTrace}");
        }


        static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            logger.Error($"Unhandled exception: {e.Message} {e.StackTrace}");
        }
    }
}