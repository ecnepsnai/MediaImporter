namespace io.ecn.Importer.Model.Properties
{
    using System;
    using PortableDeviceApiLib;

    public class BaseWPDProperties
    {
        public IPortableDeviceValues DeviceProperties { get; private set; }

        public Guid FormatId { get; set; }
        public uint PositionId { get; set; }

        public BaseWPDProperties(IPortableDeviceValues deviceProperties)
        {
            DeviceProperties = deviceProperties;
        }

        public string GetStringPropertyValue(Guid formatId, uint positionId)
        {
            _tagpropertykey property = CreateProperty(formatId, positionId);

            string propertyValue;
            DeviceProperties.GetStringValue(ref property, out propertyValue);
            return propertyValue;
        }

        public Guid GetGUIDPropertyValue(Guid formatId, uint positionId)
        {
            _tagpropertykey property = CreateProperty(formatId, positionId);

            Guid propertyValue;
            DeviceProperties.GetGuidValue(ref property, out propertyValue);
            return propertyValue;
        }

        public uint GetUIntPropertyValue(Guid formatId, uint positionId)
        {
            _tagpropertykey property = CreateProperty(formatId, positionId);

            uint propertyValue;
            DeviceProperties.GetUnsignedIntegerValue(ref property, out propertyValue);
            return propertyValue;
        }

        public DateTime? GetDateTimePropertyValue(Guid formatId, uint positionId)
        {
            _tagpropertykey property = CreateProperty(formatId, positionId);
            tag_inner_PROPVARIANT propertyValue;
            try
            {
                DeviceProperties.GetValue(ref property, out propertyValue);
            }
            catch
            {
                return null;
            }

            if (propertyValue.vt != 7) // VT_DATE
            {
                return null;
            }

            return PropVariant.FromValue(propertyValue).ToDate();
        }

        public static _tagpropertykey CreateProperty(Guid formatId, uint positionId)
        {
            return new PortableDeviceApiLib._tagpropertykey()
            {
                fmtid = formatId,
                pid = positionId,
            };
        }
    }
}
