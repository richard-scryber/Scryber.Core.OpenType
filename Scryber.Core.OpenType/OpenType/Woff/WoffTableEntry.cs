using System;
using System.Collections.Generic;

namespace Scryber.OpenType.Woff
{
    public class WoffTableEntry : TTF.TrueTypeTableEntry
    {
        public uint CompressedLength { get; set; }

        public uint ExpectedStart { get; set; }

        public byte[] DecompressedData { get; protected set; }

        public WoffTableEntry()
        {
        }


        public override void Read(BigEndianReader reader)
        {
            this.Tag = reader.ReadString(4);
            this.Offset = reader.ReadUInt32();
            this.CompressedLength = reader.ReadUInt32();
            this.Length = reader.ReadUInt32();
            this.CheckSum = reader.ReadUInt32();
        }

        public void SetDecompressedData(byte[] data)
        {
            if (null == data)
                throw new ArgumentNullException(nameof(data));
            if (data.Length != this.Length)
                throw new ArgumentOutOfRangeException(nameof(data), "The decompressed data is not the expected length");
            this.DecompressedData = data;
        }

        public void ResetDecompressedData()
        {
            this.DecompressedData = null;
        }
    }

    public class WoffTableEntryList : TTF.TrueTypeTableEntryList
    {
        public WoffTableEntryList()
            : base()
        {
        }

        public WoffTableEntryList(IEnumerable<WoffTableEntry> entries)
            : base(entries)
        {
        }
    }
}
