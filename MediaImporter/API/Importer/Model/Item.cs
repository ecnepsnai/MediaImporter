using System;
using System.IO;
using System.Runtime.InteropServices;
using PortableDeviceApiLib;
using io.ecn.Importer.Model.Properties;

namespace io.ecn.Importer.Model
{
    public class Item : BaseDeviceItem
    {
        public readonly ContentTypeProperty ContentType;
        public readonly NameProperty Name;
        public readonly OriginalFileNameProperty OriginalFileName;
        public readonly DateCreatedProperty DateCreated;
        public readonly DateModifiedProperty DateModified;
        public readonly DateAuthoredProperty DateAuthored;
        public readonly DateTime Date;

        public readonly IPortableDeviceContent DeviceContent;

        public Item(string objectId, IPortableDeviceContent content)
            : base(objectId)
        {
            DeviceContent = content;

            IPortableDeviceProperties properties;
            content.Properties(out properties);

            IPortableDeviceKeyCollection keys;
            properties.GetSupportedProperties(objectId, out keys);

            IPortableDeviceValues values;
            properties.GetValues(objectId, keys, out values);

            ContentType = new ContentTypeProperty(values);
            Name = new NameProperty(values);

            switch (ContentType.Type)
            {
                case WindowsPortableDeviceEnumerators.ContentType.FunctionalObject:
                    LoadDeviceItems(content);
                    break;

                case WindowsPortableDeviceEnumerators.ContentType.Folder:
                    OriginalFileName = new OriginalFileNameProperty(values);
                    LoadDeviceItems(content);
                    break;

                case WindowsPortableDeviceEnumerators.ContentType.Image:
                case WindowsPortableDeviceEnumerators.ContentType.Video:
                case WindowsPortableDeviceEnumerators.ContentType.GenericFile:
                case WindowsPortableDeviceEnumerators.ContentType.Unspecified:
                    OriginalFileName = new OriginalFileNameProperty(values);
                    DateCreated = new DateCreatedProperty(values);
                    DateModified = new DateModifiedProperty(values);
                    DateAuthored = new DateAuthoredProperty(values);

                    var oldestDate = DateTime.Now;
                    if (DateCreated.Value.HasValue && DateCreated.Value.Value < oldestDate)
                    {
                        oldestDate = DateCreated.Value.Value;
                    }
                    if (DateModified.Value.HasValue && DateModified.Value.Value < oldestDate)
                    {
                        oldestDate = DateModified.Value.Value;
                    }
                    if (DateAuthored.Value.HasValue && DateAuthored.Value.Value < oldestDate)
                    {
                        oldestDate = DateAuthored.Value.Value;
                    }
                    Date = oldestDate;

                    break;
            }
        }

        public void TransferFiles(string destinationPath, bool isKeepFolderStructure)
        {
            switch (ContentType.Type)
            {
                case WindowsPortableDeviceEnumerators.ContentType.Folder:
                case WindowsPortableDeviceEnumerators.ContentType.FunctionalObject:
                {
                    if (isKeepFolderStructure)
                    {
                        destinationPath = Path.Combine(destinationPath, Name.Value);
                        if (!Directory.Exists(destinationPath))
                        {
                            Directory.CreateDirectory(destinationPath);
                        }
                    }

                    foreach (Item item in DeviceItems)
                    {
                        item.TransferFiles(destinationPath, isKeepFolderStructure);
                    }
                }
                break;

                case WindowsPortableDeviceEnumerators.ContentType.Image:
                case WindowsPortableDeviceEnumerators.ContentType.Video:
                case WindowsPortableDeviceEnumerators.ContentType.GenericFile:
                case WindowsPortableDeviceEnumerators.ContentType.Unspecified:
                    {
                    Transfer(destinationPath);
                }
                break;
            }
        }

        /// <summary>
        /// This method copies the file from the device to the destination file path.
        /// </summary>
        /// <param name="fileName"></param>
        public void TransferToFile(string fileName)
        {
            IPortableDeviceResources resources;
            DeviceContent.Transfer(out resources);

            IStream wpdStream = null;
            uint optimalTransferSize = 0;

            var defaultResourceProperty = new _tagpropertykey
            {
                fmtid = new Guid("E81E79BE-34F0-41BF-B53F-F1A06AE87842"),
                pid = 0
            };

            System.Runtime.InteropServices.ComTypes.IStream sourceStream = null;
            try
            {
                resources.GetStream(Id, ref defaultResourceProperty, 0, ref optimalTransferSize, out wpdStream);
                sourceStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

                if (optimalTransferSize < 1024 * 1024)
                {
                    optimalTransferSize = 1024 * 1024;
                }

                FileStream targetStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                try
                {
                    var amtRead = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
                    Marshal.WriteInt32(amtRead, (int)optimalTransferSize);
                    var buffer = new byte[optimalTransferSize];
                    while (Marshal.ReadInt32(amtRead) > 0)
                    {
                        sourceStream.Read(buffer, buffer.Length, amtRead);
                        targetStream.Write(buffer, 0, Marshal.ReadInt32(amtRead));
                    }
                }
                finally
                {
                    targetStream.Flush();
                    targetStream.Close();
                }
            }
            finally
            {
                Marshal.ReleaseComObject(sourceStream);
                Marshal.ReleaseComObject(wpdStream);
            }
        }

        /// <summary>
        /// This method copies the file from the device to the destination path.
        /// </summary>
        /// <param name="destinationPath"></param>
        public void Transfer(string destinationPath)
        {
            TransferToFile(Path.Combine(destinationPath, OriginalFileName.Value));
        }
    }
}
