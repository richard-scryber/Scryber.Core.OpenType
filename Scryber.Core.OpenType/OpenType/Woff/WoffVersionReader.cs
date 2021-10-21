using System;
using System.Collections.Generic;
using Scryber.OpenType.SubTables;
using Scryber.OpenType.TTF;

namespace Scryber.OpenType.Woff
{
    public class WoffVersionReader : TrueTypeVersionReader
    {

        private const long VersionByteSize = 4;

        public WoffVersionReader(string id, byte[] header)
            : base(id, header, DataFormat.Woff)
        {
        }

        

        public override ITypefaceInfo ReadTypefaceInfoAfterVersion(BigEndianReader reader, string source)
        {
            WoffHeader header = WoffHeader.ReadHeader(this, reader);
            WoffTableEntryList list = new WoffTableEntryList();

            bool hasOs2 = false;
            bool hasFHead = false;
            bool hasName = false;

            for (var i = 0; i < header.NumberOfTables; i++)
            {
                WoffTableEntry entry = new WoffTableEntry();
                entry.Read(reader);
                list.Add(entry);

                if (entry.Tag == Const.OS2Table)
                    hasOs2 = true;
                else if (entry.Tag == Const.FontHeaderTable)
                    hasFHead = true;
                else if (entry.Tag == Const.NameTable)
                    hasName = true;
            }

            if (!(hasOs2 || hasName) || !hasFHead)
                return new Utility.UnknownTypefaceInfo(source, "Not all the required tables (head with OS/2 or name) were found in the font file");

            return ReadInfoFromTables(list, reader, source, hasOs2);
            
        }


        

        public override string ToString()
        {
            return "Woff Format";
        }

        internal override TrueTypeTableFactory GetTableFactory()
        {
            return new WoffTableFactory();
        }

        public override ITypeface ReadTypefaceAfterVersion(BigEndianReader reader, ITypefaceReference forReference, string source)
        {
            long startOffset = reader.Position - VersionByteSize;

            WoffHeader header = WoffHeader.ReadHeader(this, reader);
            List<WoffTableEntry> list = new List<WoffTableEntry>();



            for (var i = 0; i < header.NumberOfTables; i++)
            {
                WoffTableEntry entry = new WoffTableEntry();
                entry.Read(reader);
                list.Add(entry);
            }

            list.Sort(delegate (WoffTableEntry one, WoffTableEntry two) { return one.Offset.CompareTo(two.Offset); });

            var entries = new WoffTableEntryList(list);
            var file = new WoffFontFile(header, entries);

            var factory = this.GetTableFactory();

            foreach (var entry in entries)
            {
                var tbl = factory.ReadTable(entry.Tag, entries, reader);
                if (null != tbl)
                    entry.SetTable(tbl);

            }

            file.EnsureReferenceMatched(forReference);

            byte[] data = CopyStreamData(reader.BaseStream, startOffset);
            file.SetFileData(data, DataFormat.Woff);


            return file;
        }
    }
}
