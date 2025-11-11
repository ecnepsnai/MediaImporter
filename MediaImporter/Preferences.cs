namespace io.ecn.MediaImporter
{
    using System;
    using System.IO;

    internal class Preferences
    {
        private readonly static Logger logger = new Logger(typeof(Preferences));
        private static string ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MediaImporter", "config.txt");

        public static void Load()
        {
            var dirName = Path.GetDirectoryName(ConfigFilePath);
            if (dirName == null)
            {
                throw new DirectoryNotFoundException("Unable to find suitable configuration path");
            }

            if (!Directory.Exists(dirName))
            {
                logger.Info($"Creating preferences directory {dirName}");
                Directory.CreateDirectory(dirName);
            }

            if (!File.Exists(ConfigFilePath))
            {
                logger.Info("Saving default preferences");
                Save();
            }

            var lines = File.ReadAllLines(ConfigFilePath);
            foreach (var line in lines)
            {
                if (line[0] == '#')
                {
                    continue;
                }

                var parts = line.Split(" = ", 2);
                if (parts.Length != 2)
                {
                    continue;
                }

                var key = parts[0];
                var value = parts[1];

                switch (key)
                {
                    case "default_dir":
                        _DefaultDirectory = value;
                        break;
                    case "dir_format":
                        _DirectoryFormat = value;
                        break;
                    case "file_format":
                        _FileFormat = value;
                        break;
                    case "convert_heic":
                        _ConvertHeic = value == "true";
                        break;
                    case "tip_shown":
                        _TipShown = value == "true";
                        break;
                }
            }

            logger.Info($"Preferences loaded from {ConfigFilePath}");
        }

        public static void Save()
        {
            File.Delete(ConfigFilePath);
            File.WriteAllLines(ConfigFilePath, new string[]
            {
                $"default_dir = {_DefaultDirectory}",
                $"dir_format = {_DirectoryFormat}",
                $"file_format = {_FileFormat}",
                $"convert_heic = {(_ConvertHeic ? "true" : "false")}",
                $"tip_shown = {(_TipShown ? "true" : "false")}",
            });
        }

        public static void Reset()
        {
            logger.Info("Resetting settings");
            _DefaultDirectory = DefaultDefaultDirectory;
            _DirectoryFormat = DefaultDirectoryFormat;
            _FileFormat = DefaultFileFormat;
            _ConvertHeic = DefaultConvertHeic;
            _TipShown = DefaultTipShown;
            Save();
        }

        private static readonly string DefaultDefaultDirectory = "";
        private static readonly string DefaultDirectoryFormat = @"%y\%M-%MMM\";
        private static readonly string DefaultFileFormat = @"%y-%M-%d %H.%m.%s";
        private static readonly bool DefaultConvertHeic = true;
        private static readonly bool DefaultTipShown = false;

        private static string _DefaultDirectory = DefaultDefaultDirectory;
        private static string _DirectoryFormat = DefaultDirectoryFormat;
        private static string _FileFormat = DefaultFileFormat;
        private static bool _ConvertHeic = DefaultConvertHeic;
        private static bool _TipShown = DefaultTipShown;

        public static string DefaultDirectory
        {
            get
            {
                return _DefaultDirectory;
            }
            set
            {
                _DefaultDirectory = value;
                Save();
            }
        }

        public static string DirectoryFormat
        {
            get
            {
                return _DirectoryFormat;
            }
            set
            {
                _DirectoryFormat = value;
                Save();
            }
        }

        public static string FileFormat
        {
            get
            {
                return _FileFormat;
            }
            set
            {
                _FileFormat = value;
                Save();
            }
        }

        public static bool ConvertHeic
        {
            get
            {
                return _ConvertHeic;
            }
            set
            {
                _ConvertHeic = value;
                Save();
            }
        }

        public static bool TipShown
        {
            get
            {
                return _TipShown;
            }
            set
            {
                _TipShown = value;
                Save();
            }
        }
    }
}
