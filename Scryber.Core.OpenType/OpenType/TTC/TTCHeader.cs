using System;
using System.Collections.Generic;

namespace Scryber.OpenType.TTC
{

    public class TTCHeader
    {

        public int NumFonts { get; set; }

        public uint[] FontOffsets { get; set; }


        public TTCHeader(TTCollectionVersionReader version, int numFonts, uint[] offsets)
        {
            if (offsets.Length != numFonts)
                throw new ArgumentOutOfRangeException("The number of fonts and their offsets do not match");
            NumFonts = numFonts;
            FontOffsets = offsets;
        }

        internal static bool TryReadHeader(BigEndianReader reader, out TTCHeader header)
        {
            header = null;
            TypefaceVersionReader vers;
            if (TypefaceVersionReader.TryGetVersion(reader, out vers) == false)
                return false;
            else if (vers is TTCollectionVersionReader ttc)
                header = ReadHeader(reader, ttc);
            else
                return false;

            return null != header;
        }

        internal static TTCHeader ReadHeader(BigEndianReader reader, TTCollectionVersionReader version)
        {
            ushort versMajor = reader.ReadUInt16();
            ushort versMinor = reader.ReadUInt16();

            int numtables = (int)reader.ReadUInt32();

            uint[] offsets = new uint[numtables];

            for (var i = 0; i < numtables; i++)
            {
                offsets[i] = reader.ReadUInt32();
            }

            return new TTCHeader(version, numtables, offsets);
        }
    }
}
