namespace io.ecn.Importer
{
    using System.Collections.Generic;
    using PortableDeviceApiLib;
    using io.ecn.Importer.Model;
    using io.ecn.MediaImporter;

    public class Utility
    {
        private readonly static Logger logger = new Logger(typeof(Utility));

        public static List<Device> Get()
        {
            List<Device> connectedPortableDevices = new List<Device>();
            PortableDeviceManager manager = new PortableDeviceManager();

            manager.RefreshDeviceList();
            uint count = 1;
            manager.GetDevices(null, ref count);

            if (count == 0)
            {
                logger.Info("No devices returned");
                return connectedPortableDevices;
            }
            logger.Info($"Found {count} devices");

            // Call the above again because we now know how many devices there are.

            string[] deviceIds = new string[count];
            manager.GetDevices(ref deviceIds[0], ref count);

            ExtractDeviceInformation(deviceIds, connectedPortableDevices);
            return connectedPortableDevices;
        }

        private static void ExtractDeviceInformation(string[] deviceIds, List<Device> connectedPortableDevices)
        {
            foreach (string deviceId in deviceIds)
            {
                connectedPortableDevices.Add(new Device(deviceId));
            }            
        }
    }
}
