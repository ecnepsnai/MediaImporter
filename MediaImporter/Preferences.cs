﻿namespace MediaImporter
{
    using System;
    using System.IO;

    internal class Preferences
    {
        private static string ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MediaImporter", "config.txt");

        public static void Load()
        {
            var dirName = Path.GetDirectoryName(ConfigFilePath);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            if (!File.Exists(ConfigFilePath))
            {
                Save();
            }

            var lines = File.ReadAllLines(ConfigFilePath);
            foreach (var line in lines)
            {
                var parts = " = ".Split(line);
                if (parts.Length != 2) {
                    continue;
                }
                var key = parts[0];
                var value = parts[1];

                switch (key) {
                    case "dir_format":
                        _DirectoryFormat = value;
                        break;
                    case "file_format":
                        _FileFormat = value;
                        break;
                    case "convert_heic":
                        _ConvertHeic = value == "true";
                        break;
                }
            }
        }

        public static void Save()
        {
            File.Delete(ConfigFilePath);
            File.WriteAllLines(ConfigFilePath, new string[]
            {
                $"dir_format = {_DirectoryFormat}",
                $"file_format = {_FileFormat}",
                $"convert_heic = {(_ConvertHeic ? "true" : "false")}",
            });
        }

        public static void Reset()
        {
            _DirectoryFormat = DefaultDirectoryFormat;
            _FileFormat = DefaultFileFormat;
            _ConvertHeic = DefaultConvertHeic;
            Save();
        }

        private static readonly string DefaultDirectoryFormat = @"%y\%M-%MMM\";
        private static readonly string DefaultFileFormat = @"%y-%M-%d %H.%m.%s";
        private static readonly bool DefaultConvertHeic = true;

        private static string _DirectoryFormat = DefaultDirectoryFormat;
        private static string _FileFormat = DefaultFileFormat;
        private static bool _ConvertHeic = DefaultConvertHeic;

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
    }
}