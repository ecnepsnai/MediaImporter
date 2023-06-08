namespace io.ecn.Importer.Model.Properties
{
    using System;
    using System.Runtime.InteropServices;
    using PortableDeviceApiLib;

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct PropVariant
    {
        [FieldOffset(8)]
        public double dateValue;

        public static PropVariant FromValue(tag_inner_PROPVARIANT value)
        {
            IntPtr ptrValue = Marshal.AllocHGlobal(Marshal.SizeOf(value));
            Marshal.StructureToPtr(value, ptrValue, false);
            return (PropVariant)Marshal.PtrToStructure(ptrValue, typeof(PropVariant));
        }

        public DateTime ToDate()
        {
            return DateTime.FromOADate(dateValue);
        }
    }
}