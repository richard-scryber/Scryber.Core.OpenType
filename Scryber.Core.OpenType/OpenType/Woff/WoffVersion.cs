using System;
using Scryber.OpenType.SubTables;
using Scryber.OpenType.TTF;

namespace Scryber.OpenType.Woff
{
    public class WoffVersion : TypefaceVersionReader
    {
        
        public WoffVersion(string id, byte[] header)
            : base(header, DataFormat.Woff)
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

        public override string ToString()
        {
            return "Woff Format";
        }

        protected virtual WoffTableFactory GetTableFactory()
        {
            return new WoffTableFactory();
        }

        public override ITypeface ReadTypefaceAfterVersion(BigEndianReader reader, ITypefaceReference forReference)
        {
            throw new NotImplementedException();
        }
    }
}
