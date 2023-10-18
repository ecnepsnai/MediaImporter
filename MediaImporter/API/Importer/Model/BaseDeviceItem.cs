﻿namespace io.ecn.Importer.Model
{
    using PortableDeviceApiLib;

    public class BaseDeviceItem
    {
        public Guid Guid { get; protected set; }

        public string Id { get; protected set; }

        public List<Item> DeviceItems { get; private set; }

        public BaseDeviceItem(string id)
        {
            Guid = Guid.NewGuid();
            Id = id;
            DeviceItems = new List<Item>();
        }

        /// <summary>
        /// This method enumerates/cycles through sub objects within this current object.
        /// </summary>
        /// <param name="content"></param>
        protected void LoadDeviceItems(IPortableDeviceContent content)
        {
            // Enumerate the items contained by the current object

            IEnumPortableDeviceObjectIDs objectIds;
            content.EnumObjects(0, Id, null, out objectIds);

            // Cycle through each device item and add it to the device items list.

            uint fetched = 0;
            do
            {
                string objectId;
                objectIds.Next(1, out objectId, ref fetched);

                // Check if anything was retrieved.

                if (fetched > 0)
                {
                    DeviceItems.Add(new Item(objectId, content));
                }
            } while (fetched > 0);
        }
    }
}
