﻿namespace io.ecn.Importer.Model.Properties.Device
{
    using System;
    using PortableDeviceApiLib;

    public class ModelProperty : BaseWPDProperties
    {
        public string Value { get; private set; }

        public ModelProperty(IPortableDeviceValues deviceProperties)
            : base(deviceProperties)
        {
            FormatId = new Guid("26D4979A-E643-4626-9E2B-736DC0C92FDC");
            PositionId = 8;
            Value = GetStringPropertyValue(FormatId, PositionId);
        }
    }
}
