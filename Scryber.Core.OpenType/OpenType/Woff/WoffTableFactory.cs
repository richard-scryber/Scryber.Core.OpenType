using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
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
            using (var streamIn = new MemoryStream(reader.Read((int)table.CompressedLength)))
            {
                int pos = (int)output.Position;
                InflaterInputStream decompressor = new InflaterInputStream(streamIn);
                decompressor.CopyTo(output);
                int len = (int)(output.Position - pos);
                if (len != length)
                    return -1;
                else
                    return len;
            }

        }
    }
}
