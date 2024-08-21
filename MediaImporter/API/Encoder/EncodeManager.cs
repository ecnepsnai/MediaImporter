namespace io.ecn.Encoder
{
    using ImageMagick;
    using io.ecn.MediaImporter;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class EncodeManager
    {
        private readonly object semaphore = new object();
        private static readonly Logger logger = new Logger(typeof(EncodeManager));

        private List<string>? mediaToConvert;
        private IProgress<int>? progress;
        private int currentMediaIdx = 0;
        private int convertedMediaCount = 0;

        private readonly string[] extensions = new string[]
        {
            "jpg",
            "jpeg",
            "heif",
            "heic",
            "png",
            "dng",
            "gif"
        };

        public Task<bool> EncodeMedia(List<string> mediaToConvert, IProgress<int> progress)
        {
            this.mediaToConvert = mediaToConvert;
            this.progress = progress;

            return Task<bool>.Factory.StartNew(() =>
            {
                return DoEncodeMedia();
            });
        }

        private bool DoEncodeMedia()
        {
            logger.Info($"Starting encoder across {Environment.ProcessorCount} threads");
            List<Thread> threads = new();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                threads.Add(new Thread(ConvertNextMedia));
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            logger.Info($"Finished encoding {this.mediaToConvert!.Count} items");
            return true;
        }

        private void ConvertNextMedia()
        {
            while (true)
            {
                string file;
                lock (this.semaphore)
                {
                    if (this.currentMediaIdx >= this.mediaToConvert!.Count)
                    {
                        return;
                    }

                    file = this.mediaToConvert[this.currentMediaIdx];
                    this.currentMediaIdx++;
                }

                var extension = file.ToLower().Split(".").Last();
                if (!this.extensions.Contains(extension))
                {
                    lock (this.semaphore)
                    {
                        convertedMediaCount++;
                        this.progress!.Report(convertedMediaCount);
                    }
                    logger.Info($"Skip convert {file}");
                    continue;
                }

                try
                {
                    var format = GetMediaFormat(file);
                    switch (format)
                    {
                        case MagickFormat.Heic:
                        case MagickFormat.Heif:
                        case MagickFormat.Dng:
                            ConvertToJpg(file, "."+extension);
                            break;
                        default:
                            logger.Info($"Skip convert {file} with format {format}");
                            break;
                    }
                    lock (this.semaphore)
                    {
                        convertedMediaCount++;
                        this.progress!.Report(convertedMediaCount);
                    }
                }
                catch (Exception e)
                {
                    logger.Error($"Exception converting media {file}: {e}");
                }
            }
        }

        private static MagickFormat GetMediaFormat(string filePath)
        {
            using (var image = new MagickImage(filePath))
            {
                return image.Format;
            }
        }

        private static string ConvertToJpg(string filePath, string oldExtension)
        {
            var newPath = filePath.Replace(oldExtension, ".jpg");

            using (var image = new MagickImage(filePath))
            {
                image.Format = MagickFormat.Jpg;
                image.AutoOrient();
                image.Write(newPath);
            };

            logger.Info($"Converted {filePath} -> {newPath}");
            File.Delete(filePath);
            return newPath;
        }
    }
}
