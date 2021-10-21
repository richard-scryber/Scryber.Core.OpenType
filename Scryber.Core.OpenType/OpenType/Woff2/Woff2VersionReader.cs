using System;
using System.IO;
using Scryber.OpenType.TTF;

namespace Scryber.OpenType.Woff2
{
    /// <summary>
    /// NOT SUPPORTED - need to sort the glyph, loca and hmtx table transformations.
    /// </summary>
    public class Woff2VersionReader : TrueTypeVersionReader
    {
        [Obsolete("The Woff2 reading is not currently supported")]
        public Woff2VersionReader(string id, byte[] header)
            : base(id, header, DataFormat.Woff2)
        {
        }

        internal override TrueTypeTableFactory GetTableFactory()
        {
            return new Woff2TableFactory();
        }

        public override ITypefaceInfo ReadTypefaceInfoAfterVersion(BigEndianReader reader, string source)
        {
            Woff2CacheData cache;

            //Because of the complexity of extracting even the info from a Woff2 typeface we can cache the data
            //between reading.

            if (!TryGetCached(source, out cache))
            {
                cache = this.ReadCachableDataAfterVersion(reader, source);
                if(null == cache)
                    return new Utility.UnknownTypefaceInfo(source, "Could not read the typeface information from the source ");
                SetCache(source, cache);
            }

            return cache.Info;
        }


        public override ITypeface ReadTypefaceAfterVersion(BigEndianReader reader, ITypefaceReference forReference, string source)
        {
            Woff2CacheData cache;

            //Because of the complexity of extracting even the info from a Woff2 typeface we can cache the data
            //between reading.

            if (!TryGetCached(source, out cache))
            {
                cache = this.ReadCachableDataAfterVersion(reader, source);
                if (null == cache)
                    throw new TypefaceReadException("Could not read the typeface table data");
                SetCache(source, cache);
                
            }

            this.EnsureTablesDecoded(cache);
            return base.ReadTypefaceAfterVersion(reader, forReference, source);
        }


        protected virtual void EnsureTablesDecoded(Woff2CacheData cache)
        {
            if (cache.GlyphDecoded == false)
            {
                var entry = cache.Entries[TrueTypeTableNames.GlyphData];
                if (null == entry)
                    return;

                using (var ms = new MemoryStream(cache.UncompressedData))
                {
                    using (var reader = new BigEndianReader(ms))
                    {
                        reader.Position = entry.Offset;

                        this.ReconstructGlyphData(reader);
                    }
                }
            }

        }

        private void ReconstructGlyphData(BigEndianReader reader)
        {
            Woff2GlyphTransform transformer = new Woff2GlyphTransform();
            transformer.Transform(reader);
        }

        public override string ToString()
        {
            return "Woff 2 Format";
        }





        #region Woff2Cache implementation

        protected class Woff2CacheData
        {
            public byte[] UncompressedData;
            public Woff2TableEntryList Entries;
            public Woff2Header Header;
            public string Source;
            public ITypefaceInfo Info;
            public bool GlyphDecoded;
        }


        private WeakReference _cache = null;

        private Woff2CacheData ReadCachableDataAfterVersion(BigEndianReader reader, string source)
        {
            Woff2CacheData cache = null;

            Woff2Header header = Woff2Header.ReadHeader(this, reader);

            Woff2TableEntryList list = new Woff2TableEntryList();

            uint startAt = (uint)reader.Position;

            uint offset = 0;

            bool hasOs2 = false;
            bool hasFHead = false;
            bool hasName = false;

            for (var i = 0; i < header.NumberOfTables; i++)
            {
                Woff2TableEntry entry = new Woff2TableEntry(offset);
                entry.Read(reader);

                if (entry.Tag == Const.OS2Table)
                    hasOs2 = true;
                else if (entry.Tag == Const.FontHeaderTable)
                    hasFHead = true;
                else if (entry.Tag == Const.NameTable)
                    hasName = true;

                // If the table data has been transformed,
                // then that will be the offset of the next table

                if (entry.HasTransformation)
                    offset += entry.TransformedLength;
                else
                    offset += entry.Length;

                list.Add(entry);
            }

            if (!(hasOs2 || hasName))
                return null;// new Utility.UnknownTypefaceInfo(source, "Not all the required tables (head with OS/2 or name) were found in the font file");

            if (!hasFHead)
                return null;// new Utility.UnknownTypefaceInfo(source, "Not all the required tables (head with OS/2 or name) were found in the font file");

            //After the table entries, the entire table data is compressed using the Brotli alogorythm
            offset = (uint)reader.Position;

            var compressedData = reader.Read((int)header.TotalCompressedSize);
            var decompressedData = Woff2Brotli.DecompressData(compressedData);

            using (var uncompressed = new MemoryStream(decompressedData))
            {

#if RANGE_CHECK
                if (uncompressed.Length + offset != header.TotalFontSize)
                    throw new ArgumentOutOfRangeException("The header uncompressed size is not the same");
#endif


                using (var newReader = new BigEndianReader(uncompressed))
                {
                    var info = ReadInfoFromTables(list, newReader, source, hasOs2);

                    if (null != info && info.TypefaceCount > 0)
                    {
                        cache = new Woff2CacheData()
                        {
                            Entries = list,
                            Header = header,
                            Info = info,
                            Source = source,
                            UncompressedData = decompressedData
                        };
                    }
                }
            }

            return cache;
        }


        private bool TryGetCached(string source, out Woff2CacheData cache)
        {
            if (null != _cache && _cache.IsAlive)
            {
                Woff2CacheData current = _cache.Target as Woff2CacheData;
                if (null != current)
                {
                    if (current.Source == source)
                    {
                        cache = current;
                        return true;
                    }
                }
            }
            cache = null;
            return false;
        }

        private void SetCache(string source, Woff2CacheData cache)
        {
            if (null == cache)
                _cache = null;
            else
                _cache = new WeakReference(cache);
        }

        #endregion


    }
}
