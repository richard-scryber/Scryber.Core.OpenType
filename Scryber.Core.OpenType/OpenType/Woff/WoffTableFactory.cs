using System;
using System.IO;
#if !NET6_0
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
#endif
using Scryber.OpenType.TTF;

namespace Scryber.OpenType.Woff
{
    public class WoffTableFactory : TrueTypeTableFactory
    {
        

        public WoffTableFactory()
            : base(false, new string[] { })
        {
        }


        protected override TrueTypeFontTable ReadTable(string tag, uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            var table = list[tag] as WoffTableEntry;

            if (table.CompressedLength == table.Length)
            {
                //just copy the data
                var pos = reader.Position;
                var data = reader.Read((int)length);
                table.SetDecompressedData(data);
                //and return to the original
                reader.Position = pos;

                return base.ReadTable(tag, length, list, reader);
            }
            if (table.DecompressedData == null)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    DecompressTable(ms, reader, table, length);
                    table.SetDecompressedData(ms.ToArray());
                    ms.Position = 0;

                    using (var newReader = new BigEndianReader(ms))
                    {
                        var read = base.ReadTable(tag, length, list, newReader);
                        table.SetTable(read);
                    }
                }
            }
            else
            {
                using (var ms = new System.IO.MemoryStream(table.DecompressedData))
                {
                    using (var newReader = new BigEndianReader(ms))
                    {
                        var read = base.ReadTable(tag, length, list, newReader);
                        table.SetTable(read);
                    }
                }
            }

            return table.Table;
            
            
        }

        private int DecompressTable(MemoryStream output, BigEndianReader reader, WoffTableEntry table, uint length)
        {
            int pos = (int)output.Position;

            using (var streamIn = new MemoryStream(reader.Read((int)table.CompressedLength)))
            {

#if NET6_0


                using (var compress = new System.IO.Compression.ZLibStream(streamIn, System.IO.Compression.CompressionMode.Decompress))
                {
                    compress.CopyTo(output);
                }


#else
            
                using(InflaterInputStream decompressor = new InflaterInputStream(streamIn))
                {
                    decompressor.CopyTo(output);
                }
#endif
            }

            int len = (int)(output.Position - pos);
            if (len != length)
                return -1;
            else
                return len;
        }
    }
}
