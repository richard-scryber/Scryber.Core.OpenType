using System;
using System.Collections.Generic;
using Scryber.OpenType.TTF;

namespace Scryber.OpenType.TTC
{
    public static class TTCollectionFile
    {
        private const byte Zero = (byte)0;

        public static byte[] ExtractTTFfromTTC(System.IO.Stream ttc, int ttfHeadOffset)
        {

            using (System.IO.MemoryStream ttf = new System.IO.MemoryStream())
            {
                BigEndianReader reader = new BigEndianReader(ttc);
                reader.Position = ttfHeadOffset;

                TrueTypeHeader header;
                if (TrueTypeHeader.TryReadHeader(reader, out header) == false)
                    throw new NotSupportedException("The current stream is not a supported OpenType or TrueType font file");


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
                }
                catch (TypefaceReadException) { throw; }
                catch (Exception ex) { throw new TypefaceReadException("Could not read the TTF File", ex); }

                BigEndianWriter writer = new BigEndianWriter(ttf);
                writer.Write(header.Version.HeaderData);
                writer.WriteUInt16((ushort)header.NumberOfTables);
                writer.WriteUInt16((ushort)header.SearchRange);
                writer.WriteUInt16((ushort)header.EntrySelector);
                writer.WriteUInt16((ushort)header.RangeShift);

                long[] dirOffsets = new long[dirs.Count]; //Set to the byte position of the Offset32 in the header to point to the table
                long[] tableOffsets = new long[dirs.Count]; //Set to the byte position of the table in the file

                for(var i = 0; i < dirs.Count; i++)
                {
                    var dir = dirs[i];
                    
                    writer.WriteASCIIChars(dir.Tag);
                    writer.WriteUInt32(dir.CheckSum);

                    //Write zero as the offset initially and then we will come back and update
                    dirOffsets[i] = writer.Position;
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(dir.Length);

                }

                for (var i = 0; i < dirs.Count; i++)
                {
                    var dir = dirs[i];

                    while (writer.Position % 4 != 0)
                        writer.WriteByte(Zero);

                    //Remember the start position of the table

                    tableOffsets[i] = writer.Position;
                    reader.Position = dir.Offset;

                    //we can improve this
                    var data = reader.Read((int)dir.Length);
                    writer.Write(data);
                }



                for (int i = 0; i < dirs.Count; i++)
                {
                    writer.Position = dirOffsets[i];
                    writer.WriteUInt32((uint)tableOffsets[i]);
                }



                writer.Position = 0;
                var fileData = ttf.ToArray();
                return fileData;

            }
        }
    }
}
