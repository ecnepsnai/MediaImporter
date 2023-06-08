using System;
using PortableDeviceApiLib;

namespace io.ecn.Importer.Model.Properties
{
    public class DateAuthoredProperty : BaseWPDProperties
    {
        public readonly DateTime? Value;

        public DateAuthoredProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid("EF6B490D-5CD8-437A-AFFC-DA8B60EE4A3C");
            PositionId = 20;
            Value = GetDateTimePropertyValue(FormatId, PositionId);
        }
    }
}
