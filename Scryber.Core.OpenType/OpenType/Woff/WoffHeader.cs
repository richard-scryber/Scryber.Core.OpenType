using System;


namespace Scryber.OpenType.Woff
{
    public class WoffHeader
    {
        public WoffVersion Version { get; private set; }

        public uint Flavour { get; set; }

        public uint Length { get; set; }

        public uint NumberOfTables { get; set; }

        public uint TotalFontSize { get; set; }

        public Version WoffInnerVersion { get; set; }

        public uint MetaDataOffset { get; set; }

        public uint MetaDataLength { get; set; }

        public uint MetaDataOriginalLength { get; set; }

        public uint PrivateDataOffset { get; set; }

        public uint PrivateDataLength { get; set; }

        public WoffHeader(WoffVersion version)
        {
            this.Version = version;
        }


        public static WoffHeader ReadHeader(WoffVersion version, BigEndianReader reader)
        {
            WoffHeader header = new WoffHeader(version);
            header.Flavour = reader.ReadUInt32();
            header.Length = reader.ReadUInt32();
            header.NumberOfTables = reader.ReadUInt16();
            var reserved = reader.ReadUInt16();
            header.TotalFontSize = reader.ReadUInt32();

            var major = reader.ReadUInt16();
            var minor = reader.ReadUInt16();

            header.WoffInnerVersion = new Version(major, minor);

            header.MetaDataOffset = reader.ReadUInt32();
            header.MetaDataLength = reader.ReadUInt32();
            header.MetaDataOriginalLength = reader.ReadUInt32();

            header.PrivateDataOffset = reader.ReadUInt32();
            header.PrivateDataLength = reader.ReadUInt32();

            return header;

        }
    }
}
