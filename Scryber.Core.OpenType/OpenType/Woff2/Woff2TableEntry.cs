using System;
using System.Collections.Generic;
using Scryber.OpenType.TTF;

namespace Scryber.OpenType.Woff2
{
    public class Woff2TableEntry : TTF.TrueTypeTableEntry
    {
        public uint TransformedLength { get; set; }

        public bool HasTransformation { get; set; }

        public byte[] DecompressedData { get; protected set; }

        public Woff2TableEntry(uint offset)
        {
            this.Offset = offset;
        }

        public override void Read(BigEndianReader reader)
        {
            byte tableFlags = 0;
            uint origLen = 0;
            byte preprocess = 0;
            string tag;

            tableFlags = reader.ReadByte();
            //First 7 bits for a known table
            KnownTableIndex known = (KnownTableIndex)(tableFlags & 0x1F);

            if (known < KnownTableIndex.UNKN)
                tag = GetKnownTable(known);
            else //not known so read the next tag.
                tag = reader.ReadString(4);


            //Get the pre-processing transformation value
            //from bits 6 and 7 to versions 0-3.
            preprocess = (byte)((tableFlags >> 5) & 0x3);

            origLen = reader.ReadUIntBase128();

            if (IsTransformedTable(tag) && preprocess == 0)
            {
                this.HasTransformation = true;
                this.TransformedLength = reader.ReadUIntBase128();
            }

            this.Tag = tag;
            this.Length = origLen;
        }

        protected virtual bool IsTransformedTable(string tag)
        {
            return tag == TrueTypeTableNames.GlyphData || tag == TrueTypeTableNames.LocationIndex;
        }


        private string GetKnownTable(KnownTableIndex known)
        {
            var found = "";
            switch (known)
            {
                case KnownTableIndex.Cmap:
                    found = TrueTypeTableNames.CharacterMapping;
                    break;
                case KnownTableIndex.Head:
                    found = TrueTypeTableNames.FontHeader;
                    break;
                case KnownTableIndex.HHead:
                    found = TrueTypeTableNames.HorizontalHeader;
                    break;
                case KnownTableIndex.HMtx:
                    found = TrueTypeTableNames.HorizontalMetrics;
                    break;
                case KnownTableIndex.MaxP:
                    found = TrueTypeTableNames.MaximumProfile;
                    break;
                case KnownTableIndex.Name:
                    found = TrueTypeTableNames.NamingTable;
                    break;
                case KnownTableIndex.OS2:
                    found = TrueTypeTableNames.WindowsMetrics;
                    break;
                case KnownTableIndex.Post:
                    found = TrueTypeTableNames.PostscriptInformation;
                    break;
                case KnownTableIndex.Cvt:
                    found = TrueTypeTableNames.ControlValue;
                    break;
                case KnownTableIndex.Fpgm:
                    found = TrueTypeTableNames.FontProgram;
                    break;
                case KnownTableIndex.Glyf:
                    found = TrueTypeTableNames.GlyphData;
                    break;
                case KnownTableIndex.Loca:
                    found = TrueTypeTableNames.LocationIndex;
                    break;
                case KnownTableIndex.Prep:
                    found = TrueTypeTableNames.CVTProgram;
                    break;
                case KnownTableIndex.CFF:
                    found = TrueTypeTableNames.PostscriptProgram;
                    break;
                case KnownTableIndex.Vorg:
                    found = TrueTypeTableNames.VerticalOrigin;
                    break;
                case KnownTableIndex.EBDT:
                    found = TrueTypeTableNames.EmbeddedBitmapData;
                    break;
                case KnownTableIndex.EBLC:
                    found = TrueTypeTableNames.EmbeddedBitmapLocationData;
                    break;
                case KnownTableIndex.Gasp:
                    found = TrueTypeTableNames.GridFittingAndScanConversion;
                    break;
                case KnownTableIndex.HDmx:
                    found = TrueTypeTableNames.HorizontalDeviceMetrics;
                    break;
                case KnownTableIndex.Kern:
                    found = TrueTypeTableNames.Kerning;
                    break;
                case KnownTableIndex.LTSH:
                    found = TrueTypeTableNames.LinearThresholdData;
                    break;
                case KnownTableIndex.PCLT:
                    found = TrueTypeTableNames.PCL5Data;
                    break;
                case KnownTableIndex.VDmx:
                    found = TrueTypeTableNames.VerticalDeviceMetrics;
                    break;
                case KnownTableIndex.VHea:
                    found = TrueTypeTableNames.VerticalMetricsHeader;
                    break;
                case KnownTableIndex.VMtx:
                    found = TrueTypeTableNames.VerticalMetrics;
                    break;
                case KnownTableIndex.BASE:
                    found = TrueTypeTableNames.BaseLineData;
                    break;
                case KnownTableIndex.GDEF:
                    found = TrueTypeTableNames.GlyphDefinitionData;
                    break;
                case KnownTableIndex.GPos:
                    found = TrueTypeTableNames.GlyphPositionData;
                    break;
                case KnownTableIndex.GSub:
                    found = TrueTypeTableNames.GlyphSubstitutionData;
                    break;
                case KnownTableIndex.EBSC:
                    found = TrueTypeTableNames.EmbeddedBitmapScanningData;
                    break;
                case KnownTableIndex.JSTF:
                    found = TrueTypeTableNames.JustificationData;
                    break;
                case KnownTableIndex.MATH:
                case KnownTableIndex.CBDT:
                case KnownTableIndex.CBLC:
                case KnownTableIndex.COLR:
                case KnownTableIndex.CPAL:
                case KnownTableIndex.SVG_:
                case KnownTableIndex.sbix:
                case KnownTableIndex.acnt:
                case KnownTableIndex.avar:
                case KnownTableIndex.bdat:
                case KnownTableIndex.bloc:
                case KnownTableIndex.bsln:
                case KnownTableIndex.cvar:
                case KnownTableIndex.fdsc:
                case KnownTableIndex.feat:
                case KnownTableIndex.fmtx:
                case KnownTableIndex.fvar:
                case KnownTableIndex.gvar:
                case KnownTableIndex.hsty:
                case KnownTableIndex.just:
                case KnownTableIndex.lcar:
                case KnownTableIndex.mort:
                case KnownTableIndex.morx:
                case KnownTableIndex.opbd:
                case KnownTableIndex.prop:
                case KnownTableIndex.trak:
                case KnownTableIndex.Zapf:
                case KnownTableIndex.Silf:
                case KnownTableIndex.Glat:
                case KnownTableIndex.Gloc:
                case KnownTableIndex.Feat:
                case KnownTableIndex.Sill:
                    found = known.ToString().Replace('_', ' ');
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(known));
            }

            return found;
        }


    }


    public class Woff2TableEntryList : TTF.TrueTypeTableEntryList
    {
        public Woff2TableEntryList()
            : base()
        {
        }

        

        public Woff2TableEntryList(IEnumerable<Woff2TableEntry> entries)
            : base(entries)
        {
        }
    }
}
