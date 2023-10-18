namespace io.ecn.Importer.Model.Properties
{
    using PortableDeviceApiLib;

    public class DateCreatedProperty : BaseWPDProperties
    {
        public readonly DateTime? Value;

        public DateCreatedProperty(IPortableDeviceValues deviceProperties): base(deviceProperties)
        {
            FormatId = new Guid("EF6B490D-5CD8-437A-AFFC-DA8B60EE4A3C");
            PositionId = 18;
            Value = GetDateTimePropertyValue(FormatId, PositionId);
        }
    }
}
