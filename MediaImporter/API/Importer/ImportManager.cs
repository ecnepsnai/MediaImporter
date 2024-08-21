namespace io.ecn.Importer
{
    using io.ecn.Importer.Model;
    using io.ecn.MediaImporter;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class ImportManager
    {
        private readonly static Logger logger = new Logger(typeof(ImportManager));
        private readonly string[] validExtensions = { "jpg", "jpeg", "png", "gif", "mov", "mp4", "m4v", "heic", "heif", "hevc", "webp", "dng" };       

        public List<Item> FindMedia(Device importDevice) {
            logger.Info("Enumerating device items...");

            if (importDevice == null)
            {
                logger.Error("No import device selected");
                throw new InvalidOperationException("No devices found");
            }

            importDevice.Refresh(importDevice.DeviceId);
            if (importDevice.DeviceItems == null || importDevice.DeviceItems.Count == 0)
            {
                logger.Error("No media found");
                throw new InvalidOperationException("No media found");
            }

            return this.EnumerateDeviceItems(importDevice);
        }

        private List<Item> EnumerateDeviceItems(Device importDevice)
        {
            List<Item> items = new();
            foreach (var item in importDevice.DeviceItems)
            {
                RecurseItemFolder(item, ref items);
            }
            logger.Info($"Found {items.Count} media items");
            return items;
        }

        private void RecurseItemFolder(Item root, ref List<Item> rItems)
        {
            foreach (var item in root.DeviceItems)
            {
                switch (item.ContentType.Type)
                {
                    case WindowsPortableDeviceEnumerators.ContentType.FunctionalObject:
                    case WindowsPortableDeviceEnumerators.ContentType.Folder:
                        RecurseItemFolder(item, ref rItems);
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
                            logger.Info($"Unsupported file extension {extension}");
                            break;
                        }
                        logger.Info($"Found media item {item.OriginalFileName.Value}");
                        rItems.Add(item);
                        break;
                    default:
                        logger.Info($"Unknown item content type {item.ContentType.Type}");
                        break;
                }
            }
        }

        public Task<List<string>> ImportMedia(Device importDevice, string importDirectory, List<Item> items, IProgress<int> progress)
        {
            return Task<List<string>>.Factory.StartNew(() =>
            {
                return DoImport(importDevice, importDirectory, items, progress);
            });
        }

        private List<string> DoImport(Device importDevice, string importDirectory, List<Item> items, IProgress<int> progress)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            logger.Info("Starting import...");
            long totalImported = 0;

            // Build two maps, one used for date deduplication and the other to hold the desired file name (without extension)
            Dictionary<string, bool> dateMap = new Dictionary<string, bool>();
            Dictionary<string, string> itemNameMap = new Dictionary<string, string>();
            foreach (Item item in items)
            {
                var itemDirectory = ItemDirectory(importDirectory, item);
                try
                {
                    Directory.CreateDirectory(itemDirectory);
                }
                catch (Exception e)
                {
                    logger.Error($"Error making item directory {itemDirectory}: {e.Message}");
                    continue;
                }

                var dateStr = ItemDateStr(item);
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

            int i = 0;
            List<string> filePaths = new();

            try
            {
                importDevice.Connect();
            }
            catch (Exception e)
            {
                logger.Error($"Error connecting to device: {e.Message}");
                return filePaths;
            }

            foreach (Item item in items)
            {
                var extension = item.OriginalFileName.Value.Split('.').Last().ToLower();
                var fileName = itemNameMap[item.Id] + "." + extension;

                var itemDirectory = ItemDirectory(importDirectory, item);
                string filePath = Path.Combine(itemDirectory, fileName);
                try
                {
                    item.TransferToFile(filePath);
                }
                catch (Exception e)
                {
                    logger.Error($"Error transferring item to file: {filePath}: {e.Message}");
                    i++;
                    progress.Report(i);
                    continue;
                }

                File.SetCreationTime(filePath, item.Date);
                File.SetLastWriteTime(filePath, item.Date);
                long size = (new FileInfo(filePath)).Length;
                totalImported += size;

                logger.Info($"Imported {item.Id} to {filePath} ({size}B)");

                filePaths.Add(filePath);
                i++;
                progress.Report(i);
            }

            try
            {
                importDevice.Disconnect();
            }
            catch
            {
                // Pass
            }

            stopwatch.Stop();
            double mbps = (totalImported * 8.0 / 1000000) / stopwatch.Elapsed.TotalSeconds;

            logger.Info($"Imported {filePaths.Count} media items totalling {totalImported/1024/1024}MB in {stopwatch.Elapsed.TotalSeconds} seconds at {mbps}Mbps");

            return filePaths;
        }

        private static string ItemDirectory(string mediaDirectory, Item item)
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
            return Path.Combine(mediaDirectory, formatStr);
        }

        private static string ItemDateStr(Item item)
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
