namespace io.ecn.Importer
{
    using io.ecn.Importer.Model;
    using MediaImporter;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class ImportManager
    {
        public readonly string mediaDirectory;
        private readonly Device importDevice;
        private readonly string[] validExtensions = { "jpg", "jpeg", "png", "gif", "mov", "mp4", "m4v", "heic", "heif", "hevc", "webp", "dng" };       

        public ImportManager(string mediaDirectory, Device importDevice)
        {
            this.mediaDirectory = mediaDirectory;
            this.importDevice = importDevice;
        }

        public Task<List<Item>> FindMedia() {
            if (this.importDevice == null)
            {
                throw new InvalidOperationException("No devices found");
            }

            this.importDevice.Refresh(this.importDevice.DeviceId);
            if (this.importDevice.DeviceItems == null || this.importDevice.DeviceItems.Count == 0)
            {
                throw new InvalidOperationException("No media found");
            }

            var t = new Task<List<Item>>(this.EnumerateDeviceItems);
            t.Start();
            return t;
        }

        private List<Item> EnumerateDeviceItems()
        {
            List<Item> items = new();
            foreach (var item in this.importDevice.DeviceItems)
            {
                recurseItemFolder(item, ref items);
            }
            return items;
        }

        private void recurseItemFolder(Item root, ref List<Item> rItems)
        {
            foreach (var item in root.DeviceItems)
            {
                switch (item.ContentType.Type)
                {
                    case WindowsPortableDeviceEnumerators.ContentType.FunctionalObject:
                    case WindowsPortableDeviceEnumerators.ContentType.Folder:
                        recurseItemFolder(item, ref rItems);
                        break;
                    case WindowsPortableDeviceEnumerators.ContentType.GenericFile:
                    case WindowsPortableDeviceEnumerators.ContentType.Image:
                    case WindowsPortableDeviceEnumerators.ContentType.Video:
                    case WindowsPortableDeviceEnumerators.ContentType.Unspecified:
                        if (item.OriginalFileName == null)
                        {
                            break;
                        }
                        var extension = item.OriginalFileName.Value.Split('.').Last().ToLower();
                        if (!this.validExtensions.Contains(extension))
                        {
                            break;
                        }
                        rItems.Add(item);
                        break;
                    default:
                        break;
                }
            }
        }

        public Task<List<string>> ImportMedia(List<Item> items, IProgress<int> progress)
        {
            return Task<List<string>>.Factory.StartNew(() =>
            {
                return DoImport(items, progress);
            });
        }

        private List<string> DoImport(List<Item> items, IProgress<int> progress)
        {
            Debug.WriteLine("Starting import...");

            // Build two maps, one used for date deduplication and the other to hold the desired file name (without extension)
            Dictionary<string, bool> dateMap = new Dictionary<string, bool>();
            Dictionary<string, string> itemNameMap = new Dictionary<string, string>();
            foreach (Item item in items)
            {
                var itemDirectory = this.itemDirectory(item);
                Directory.CreateDirectory(itemDirectory);

                var dateStr = itemDateStr(item);
                if (!dateMap.ContainsKey(dateStr))
                {
                    dateMap[dateStr] = true;
                    itemNameMap[item.Id] = dateStr;
                    continue;
                }

                var index = 1;
                var dateWithIdx = dateStr + "-" + index;
                while (dateMap.ContainsKey(dateWithIdx))
                {
                    index++;
                    dateWithIdx = dateStr + "-" + index;
                }

                dateMap[dateWithIdx] = true;
                itemNameMap[item.Id] = dateWithIdx;
            }

            this.importDevice.Connect();
            int i = 0;
            List<string> filePaths = new List<string>();
            foreach (Item item in items)
            {
                var extension = item.OriginalFileName.Value.Split('.').Last().ToLower();
                var fileName = itemNameMap[item.Id] + "." + extension;

                var itemDirectory = this.itemDirectory(item);
                string filePath = Path.Combine(itemDirectory, fileName);
                item.TransferToFile(filePath);

                File.SetCreationTime(filePath, item.Date);
                File.SetLastWriteTime(filePath, item.Date);

                Debug.WriteLine($"Imported {item.Name.Value} -> {filePath}");

                filePaths.Add(filePath);
                i++;
                progress.Report(i);
            }
            this.importDevice.Disconnect();

            Debug.WriteLine($"Imported {filePaths.Count} media items");

            return filePaths;
        }

        private string itemDirectory(Item item)
        {
            var formatStr = Preferences.DirectoryFormat;
            formatStr = formatStr.Replace("%y", item.Date.ToString("yyyy"));
            formatStr = formatStr.Replace("%MMMM", item.Date.ToString("MMMM"));
            formatStr = formatStr.Replace("%MMM", item.Date.ToString("MMM"));
            formatStr = formatStr.Replace("%M", item.Date.ToString("MM"));
            formatStr = formatStr.Replace("%dddd", item.Date.ToString("dddd"));
            formatStr = formatStr.Replace("%d", item.Date.ToString("dd"));
            formatStr = formatStr.Replace("%H", item.Date.ToString("HH"));
            formatStr = formatStr.Replace("%m", item.Date.ToString("mm"));
            formatStr = formatStr.Replace("%s", item.Date.ToString("ss"));
            return Path.Combine(this.mediaDirectory, formatStr);
        }

        private string itemDateStr(Item item)
        {
            var formatStr = Preferences.FileFormat;
            formatStr = formatStr.Replace("%y", item.Date.ToString("yyyy"));
            formatStr = formatStr.Replace("%MMMM", item.Date.ToString("MMMM"));
            formatStr = formatStr.Replace("%MMM", item.Date.ToString("MMM"));
            formatStr = formatStr.Replace("%M", item.Date.ToString("MM"));
            formatStr = formatStr.Replace("%dddd", item.Date.ToString("dddd"));
            formatStr = formatStr.Replace("%d", item.Date.ToString("dd"));
            formatStr = formatStr.Replace("%H", item.Date.ToString("HH"));
            formatStr = formatStr.Replace("%m", item.Date.ToString("mm"));
            formatStr = formatStr.Replace("%s", item.Date.ToString("ss"));
            return formatStr;
        }
    }
}
