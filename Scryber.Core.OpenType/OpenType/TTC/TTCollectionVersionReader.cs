using System;
using System.Collections.Generic;

namespace Scryber.OpenType.TTC
{

    public class TTCollectionVersionReader : TypefaceVersionReader
    {

        public string VersionIdentifier
        {
            get;
            private set;
        }

        public TTCollectionVersionReader(string type, byte[] data) : base(data, DataFormat.TTC)
        {
            if (string.IsNullOrEmpty(type) || (type.Equals("ttcf", StringComparison.OrdinalIgnoreCase) == false))
                throw new TypefaceReadException("The True Type collection version must be ttcf");

            this.VersionIdentifier = type;
        }

        public override ITypefaceFont ReadTypefaceAfterVersion(BigEndianReader reader, IFontInfo forReference, string source)
        {
            this.EnsureSeekable(reader);
            Utility.SingleTypefaceInfo found = null;
            if(forReference is Utility.SingleTypefaceInfo single)
            {
                found = single;
            }
            else
            {
                reader.Position = 0;
                var collection = this.ReadCollection(reader, null);

                for (int i = 0; i < collection.Count; i++)
                {
                    var reference = collection[i];
                    if (Utility.SingleTypefaceInfo.AreEqual(forReference, reference))
                    {
                       found = (Utility.SingleTypefaceInfo)reference;
                    }
                }
            }

            if (null != found)
            {
                byte[] data = TTCollectionFile.ExtractTTFfromTTC(reader.BaseStream, (int)found.OffsetInFile);
                if (null != data && data.Length > 0)
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
                        return new TypefaceReader().GetFont(ms, source, forReference);
                }
            }

            return null;
        }

        private void EnsureSeekable(BigEndianReader reader)
        {
            if (!reader.CanSeek)
                throw new InvalidOperationException("The current reader is not seekable, so cannot be positioned");
        }

        public override string ToString()
        {
            return "TT Collection : " + this.VersionIdentifier;
        }


        public override ITypefaceInfo ReadTypefaceInfoAfterVersion(BigEndianReader reader, string source)
        {
            var found = ReadCollection(reader, source);
            return new Utility.CollectionTypefaceInfo(source, DataFormat.TTC, found.ToArray());
        }

        protected virtual List<IFontInfo> ReadCollection(BigEndianReader reader, string source)
        {
            var header = TTCHeader.ReadHeader(reader, this);
            List<IFontInfo> found = new List<IFontInfo>();
            
            for (int f = 0; f < header.NumFonts; f++)
            {
                TypefaceVersionReader innerVers;
                
                reader.Position = header.FontOffsets[f];
                if (TryGetVersion(reader, out innerVers))
                {
                    var info = innerVers.ReadTypefaceInfoAfterVersion(reader, source);
                    if (info is Utility.SingleTypefaceInfo sti)
                    {
                        sti.OffsetInFile = header.FontOffsets[f];
                    }
                    if (null != info && info.FontCount == 1)
                        found.Add(info.Fonts[0]);
                }
            }

            return found;
        }
    }
}
