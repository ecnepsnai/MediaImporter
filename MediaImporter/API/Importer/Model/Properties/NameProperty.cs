using System;
using PortableDeviceApiLib;

namespace io.ecn.Importer.Model.Properties
{
    public class NameProperty : BaseWPDProperties
    {
        public string Value { get; private set; }

        public NameProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid("EF6B490D-5CD8-437A-AFFC-DA8B60EE4A3C");
            PositionId = 4;
            try
            {
                Value = GetStringPropertyValue(FormatId, PositionId);
            }
            catch {
                Value = "UNKNOWN";
            }
        }
    }
}
