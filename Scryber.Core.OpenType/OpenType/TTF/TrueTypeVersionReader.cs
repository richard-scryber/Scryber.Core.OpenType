using System;
using Scryber.OpenType.SubTables;
using System.Collections.Generic;
using System.IO;

namespace Scryber.OpenType.TTF
{
    public class TrueTypeVersionReader : TypefaceVersionReader
    {
        private string _versid;
        public string VersionIdentifier
        {
            get { return _versid; }
        }

        public TrueTypeVersionReader(string id, byte[] data)
            : this(id, data, DataFormat.TTF)
        {
            if (string.IsNullOrEmpty(id) || (id.Equals("TRUE", StringComparison.CurrentCultureIgnoreCase) || id.Equals("typ1", StringComparison.CurrentCultureIgnoreCase)) == false)
                throw new TypefaceReadException("The true type version must be either 'true' or 'typ1'");
        }


        protected TrueTypeVersionReader(string id, byte[] data, DataFormat format)
            : base(data, DataFormat.TTF)
        {
            this._versid = id;
        }

        public override string ToString()
        {
            return "True Type : " + this.VersionIdentifier;
        }


        public override ITypefaceInfo ReadTypefaceInfoAfterVersion(BigEndianReader reader, string source)
        {
            ushort numtables = reader.ReadUInt16();
            ushort search = reader.ReadUInt16();
            ushort entry = reader.ReadUInt16();
            ushort range = reader.ReadUInt16();

            TrueTypeTableEntryList list = new TrueTypeTableEntryList();
            bool hasOs2 = false;
            bool hasFHead = false;
            bool hasName = false;

            for (int i = 0; i < numtables; i++)
            {
                TrueTypeTableEntry dir = new TrueTypeTableEntry();
                dir.Read(reader);
                list.Add(dir);
                if (dir.Tag == Const.OS2Table)
                    hasOs2 = true;
                else if (dir.Tag == Const.FontHeaderTable)
                    hasFHead = true;
                else if (dir.Tag == Const.NameTable)
                    hasName = true;
            }

            if (!(hasOs2 || hasName) || !hasFHead)
                return new Utility.UnknownTypefaceInfo(source, "Not all the required tables (head with OS/2 or name) were found in the font file");
            else
                return ReadInfoFromTables(list, reader, source, hasOs2);

            string familyname;
            FontRestrictions restrictions;
            WeightClass weight;
            WidthClass width;
            FontSelection selection;

            var factory = this.GetTableFactory();
            var ntable = factory.ReadTable(Const.NameTable, list, reader) as SubTables.NamingTable;

            NameEntry nameEntry;
            if (ntable.Names.TryGetEntry(Const.FamilyNameID, out nameEntry))
                familyname = nameEntry.ToString();
            else
                return new Utility.UnknownTypefaceInfo(source, "The font family name could not be found in the font file");


            if (hasOs2)
            {
                SubTables.OS2Table os2table = factory.ReadTable(Const.OS2Table, list, reader) as SubTables.OS2Table;
                restrictions = os2table.FSType;
                width = os2table.WidthClass;
                weight = os2table.WeightClass;
                selection = os2table.Selection;
            }
            else if (hasFHead)
            {
                SubTables.FontHeader fhead = factory.ReadTable(Const.FontHeaderTable, list, reader) as SubTables.FontHeader;
                var mac = fhead.MacStyle;
                restrictions = FontRestrictions.InstallableEmbedding;
                weight = WeightClass.Normal;
                width = WidthClass.Medium;

                if ((mac & FontStyleFlags.Condensed) > 0)
                    width = WidthClass.Condensed;

                else if ((mac & FontStyleFlags.Extended) > 0)
                    width = WidthClass.Expanded;

                selection = 0;
                if ((mac & FontStyleFlags.Italic) > 0)
                    selection |= FontSelection.Italic;

                if ((mac & FontStyleFlags.Bold) > 0)
                {
                    selection |= FontSelection.Bold;
                    weight = WeightClass.Bold;
                }
                if ((mac & FontStyleFlags.Outline) > 0)
                    selection |= FontSelection.Outlined;

                if ((mac & FontStyleFlags.Underline) > 0)
                    selection |= FontSelection.Underscore;
            }
            else
                return new Utility.UnknownTypefaceInfo(source, "The font selections could not be found in the font file");


            return new Utility.SingleTypefaceInfo(source, DataFormat.TTF, familyname, restrictions, weight, width, selection, 0);
        }

        protected ITypefaceInfo ReadInfoFromTables(TrueTypeTableEntryList list, BigEndianReader reader, string source, bool hasOs2)
        {
            string familyname;
            FontRestrictions restrictions;
            WeightClass weight;
            WidthClass width;
            FontSelection selection;

            var factory = this.GetTableFactory();
            var ntable = factory.ReadTable(Const.NameTable, list, reader) as SubTables.NamingTable;

            NameEntry nameEntry;
            if (ntable.Names.TryGetEntry(Const.FamilyNameID, out nameEntry))
                familyname = nameEntry.ToString();
            else
                return new Utility.UnknownTypefaceInfo(source, "The font family name could not be found in the font file");


            if (hasOs2)
            {
                SubTables.OS2Table os2table = factory.ReadTable(Const.OS2Table, list, reader) as SubTables.OS2Table;
                restrictions = os2table.FSType;
                width = os2table.WidthClass;
                weight = os2table.WeightClass;
                selection = os2table.Selection;
            }
            else
            {
                SubTables.FontHeader fhead = factory.ReadTable(Const.FontHeaderTable, list, reader) as SubTables.FontHeader;
                var mac = fhead.MacStyle;
                restrictions = FontRestrictions.InstallableEmbedding;
                weight = WeightClass.Normal;
                width = WidthClass.Medium;

                if ((mac & FontStyleFlags.Condensed) > 0)
                    width = WidthClass.Condensed;

                else if ((mac & FontStyleFlags.Extended) > 0)
                    width = WidthClass.Expanded;

                selection = 0;
                if ((mac & FontStyleFlags.Italic) > 0)
                    selection |= FontSelection.Italic;

                if ((mac & FontStyleFlags.Bold) > 0)
                {
                    selection |= FontSelection.Bold;
                    weight = WeightClass.Bold;
                }
                if ((mac & FontStyleFlags.Outline) > 0)
                    selection |= FontSelection.Outlined;

                if ((mac & FontStyleFlags.Underline) > 0)
                    selection |= FontSelection.Underscore;
            }
            

            return new Utility.SingleTypefaceInfo(source, DataFormat.TTF, familyname, restrictions, weight, width, selection, 0);
        }

        public override ITypeface ReadTypefaceAfterVersion(BigEndianReader reader, ITypefaceReference forReference)
        {
            long startOffset = reader.BaseStream.Position - 4;

            TrueTypeHeader header;
            if (TrueTypeHeader.TryReadHeaderAfterVersion(reader, this, false, out header) == false)
                throw new TypefaceReadException("Could not read the TrueType header for " + forReference.ToString() + " font");

            List<TrueTypeTableEntry> dirs;
            try
            {
                dirs = new List<TrueTypeTableEntry>();

                for (int i = 0; i < header.NumberOfTables; i++)
                {
                    TrueTypeTableEntry dir = new TrueTypeTableEntry();
                    dir.Read(reader);
                    dirs.Add(dir);
                }

                dirs.Sort(delegate (TrueTypeTableEntry one, TrueTypeTableEntry two) { return one.Offset.CompareTo(two.Offset); });

                var entries = new TrueTypeTableEntryList(dirs);

                TrueTypeFile file = new TrueTypeFile(header, entries);

                TrueTypeTableFactory factory = this.GetTableFactory();

                foreach (TrueTypeTableEntry dir in dirs)
                {
                    TrueTypeFontTable tbl = factory.ReadTable(dir, entries, reader);
                    if (tbl != null)
                        dir.SetTable(tbl);
                }

                file.EnsureReferenceMatched(forReference);
                

                byte[] data = CopyStreamData(reader.BaseStream, startOffset);
                file.SetFileData(data, this.DataFormat);

                return file;
            }
            catch (Exception ex) { throw new TypefaceReadException("Could not read the TTF File", ex); }
        }

        private byte[] CopyStreamData(Stream fromStream, long fromOffset)
        {
            if (fromOffset == 0 && fromStream is MemoryStream msOrig)
                return msOrig.ToArray();
            else
            {
                byte[] data = null;

                var endOffset = fromStream.Position;
                int capacity = (int)(endOffset - fromOffset);
                fromStream.Position = fromOffset;

                using (var ms = new MemoryStream(capacity))
                {
                    ExtractData(fromStream, ms);
                    ms.Position = 0;
                    data = ms.ToArray();
                }

                //Set the stream position back to where we were before returning
                fromStream.Position = endOffset;
                return data;
            }
        }

        private void ExtractData(Stream stream, MemoryStream into)
        {
            //Copy the stream to a private data array
            byte[] buffer = new byte[4096];
            int count;
            while ((count = stream.Read(buffer, 0, 4096)) > 0)
            {
                into.Write(buffer, 0, count);
            }
        }

        internal virtual TrueTypeTableFactory GetTableFactory()
        {
            return new TrueTypeDefaultTableFactory(true);
        }
    }
}
