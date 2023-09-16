namespace io.ecn.MediaImporter
{
    using System;
    using System.Diagnostics;
    using System.Text;

    public class LogWriter
    {
        private static readonly object semaphore = new();
        private static FileStream? fileStream;

        public static void Open()
        {
            fileStream = File.Open("mediaimporter_log.txt", FileMode.Append, FileAccess.Write, FileShare.Read);
        }

        internal static void Write(string className, string level, string message)
        {
            lock (semaphore)
            {
                var line = ($"[{DateTime.Now}][{className}][{Environment.CurrentManagedThreadId}][{level}] {message}").Replace("\r", "\\r").Replace("\n", "\\n");
                Debug.WriteLine(line);        
                fileStream?.Write(new UTF8Encoding().GetBytes(line));
                fileStream?.Write(new UTF8Encoding().GetBytes("\r\n"));
                fileStream?.Flush();
            }
        }
    }

    public class Logger
    {
        public readonly Type type;

        public Logger(Type type)
        {
            this.type = type;
        }

        public void Info(string message)
        {
            LogWriter.Write(this.type.Name, "INFO", message);
        }

        public void Error(string message)
        {
            LogWriter.Write(this.type.Name, "ERROR", message);
        }
    }
}
