using System;
namespace Scryber.OpenType.Woff2
{
    public class Woff2Header : TypefaceHeader
    {

        public uint Flavour { get; set; }

        public uint Length { get; set; }

        public uint TotalFontSize { get; set; }

        public uint TotalCompressedSize { get; set; }

        public Version WoffInnerVersion { get; set; }

        public uint MetaDataOffset { get; set; }

        public uint MetaDataLength { get; set; }

        public uint MetaDataOriginalLength { get; set; }

        public uint PrivateDataOffset { get; set; }

        public uint PrivateDataLength { get; set; }

        public Woff2Header(Woff2VersionReader version, int numTables) : base(version, numTables)
        {
        }



        public static Woff2Header ReadHeader(Woff2VersionReader version, BigEndianReader reader)
        {
            uint flavour = reader.ReadUInt32();
            uint length = reader.ReadUInt32();
            ushort numTables = reader.ReadUInt16();
            var reserved = reader.ReadUInt16();
            var totalFontSize = reader.ReadUInt32();
            var compressedSize = reader.ReadUInt32();

            var major = reader.ReadUInt16();
            var minor = reader.ReadUInt16();

            Woff2Header header = new Woff2Header(version, numTables);


            header.Flavour = flavour;
            header.Length = length;
            header.TotalFontSize = totalFontSize;
            header.TotalCompressedSize = compressedSize;
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
