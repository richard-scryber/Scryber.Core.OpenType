using System;
using System.IO;
using System.Linq;
using Scryber.OpenType.TTF;

namespace Scryber.OpenType.Woff
{
    public class WoffFontFile : TrueTypeFile
    {
        private byte[] _convertedTTF;
        private const byte Zero = (byte)0;

        public WoffFontFile(TypefaceHeader header, WoffTableEntryList entries, int offset)
            : base(header, entries, offset)
        {
        }


        public override byte[] GetFileData(DataFormat format)
        {
            if (format == DataFormat.Woff)
                return base.GetFileData(format);
            else if (format == DataFormat.TTF)
                return ConvertWoffToTTF();
            else
                throw new NotSupportedException("Cannot convert a Woff font to " + format.ToString());
        }

        public override bool CanGetFileData(DataFormat format)
        {
            if (format == DataFormat.Woff || format == DataFormat.TTF)
                return null != this.FileData;
            else
                return base.CanGetFileData(format);
        }

        private byte[] ConvertWoffToTTF()
        {
            if(_convertedTTF == null)
            {
                using (var ms = new MemoryStream())
                {
                    var expected = ((WoffHeader)this.Head).TotalFontSize;
                    this.DoWriteWoffToTTF(ms);

                    ms.Position = 0; //not strictly nescessary

                    _convertedTTF = ms.ToArray();
                    if (_convertedTTF.Length != expected)
                        throw new InvalidDataException("The decompressed data for the Woff font was " + _convertedTTF.Length + ", however expected length was " + expected + " either something went wrong, or the original font is invalid.");
                }
            }
            return _convertedTTF;
        }

        private void DoWriteWoffToTTF(MemoryStream ms)
        {
            var dirs = this.Directories.ToArray();

            BigEndianWriter writer = new BigEndianWriter(ms);
            writer.Write(TypefaceVersionReader.TrueTypeHeaderBytes);
            writer.WriteUInt16((ushort)dirs.Length);

            var checkOffset = ms.Position;
            
            ushort max2 = 2;
            while (max2 * 2 <= this.Directories.Count)
                max2 *= 2;

            ushort search = (ushort)(max2 * 16);
            ushort entry = (ushort)Math.Log(max2, 2);
            ushort range = (ushort)((dirs.Length * 16) - search);

            writer.WriteUInt16(search);
            writer.WriteUInt16(entry);
            writer.WriteUInt16(range);

            var offset = writer.Position;
            var dirOffsets = new long[dirs.Length];
            var tableOffsets = new long[dirs.Length];

            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i] as WoffTableEntry;

                if (dir.DecompressedData == null)
                    throw new InvalidOperationException("The directrory " + dir.Tag + " does not have any decompressed data");

                writer.WriteASCIIChars(dir.Tag);
                writer.WriteUInt32(dir.CheckSum);

                //Write zero as the offset initially and then we will come back and update
                dirOffsets[i] = writer.Position;
                writer.WriteUInt32(0);

                writer.WriteUInt32(dir.Length);
            }

            for(var i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i] as WoffTableEntry;

                //pad to 4 bytes
                while (writer.Position % 4 != 0)
                    writer.WriteByte(Zero);

                //remember the position and write the decompressed data
                tableOffsets[i] = writer.Position;
                writer.Write(dir.DecompressedData);

            }

            //Now go back and update the positions

            for (int i = 0; i < dirs.Length; i++)
            {
                writer.Position = dirOffsets[i];
                writer.WriteUInt32((uint)tableOffsets[i]);
            }
        }

        public override void SetFileData(byte[] data, DataFormat format)
        {
            base.SetFileData(data, format);
        }
    }
}
