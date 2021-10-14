/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType
{
    public class BigEndianReader : IDisposable
    {
        public bool CanSeek
        {
            get { return this.BaseStream.CanSeek; }
        }

        public bool CanRead
        {
            get { return this.BaseStream.CanRead; }
        }

        public bool CanWrite
        {
            get { return false; }
        }
        
        public long Position
        {
            get { return this.BaseStream.Position; }
            set { this.BaseStream.Position = value; }
        }

        public long Length
        {
            get { return this.BaseStream.Length; }
        }

        private System.IO.Stream _base;
        public System.IO.Stream BaseStream
        {
            get { return _base; }
        }

        public BigEndianReader(System.IO.Stream basestream)
        {
            if (basestream == null)
                throw new ArgumentNullException("basestream");
            if (basestream.CanRead == false)
                throw new ArgumentException("The base stream does not support reading!");
            if (basestream.CanSeek == false)
                throw new ArgumentException("The Reader requires a stream that supports seeking (changing position within the stream) as well as forward only reading");
            this._base = basestream;
        }

        public byte ReadByte()
        {
            return (byte)this.BaseStream.ReadByte();
        }

        public ushort ReadUInt16()
        {
            //byte[] data = new byte[2];
            byte b1 = (byte)this.BaseStream.ReadByte();
            byte b2 = (byte)this.BaseStream.ReadByte();

            //this.BaseStream.Read(data, 0, 2);
            BigEnd16 word = new BigEnd16(b1, b2);
            
            return word.UnsignedValue;
        }

        public short ReadInt16()
        {
            //byte[] data = new byte[2];
            byte b1 = (byte)this.BaseStream.ReadByte();
            byte b2 = (byte)this.BaseStream.ReadByte();

            //this.BaseStream.Read(data, 0, 2);
            BigEnd16 word = new BigEnd16(b1, b2);

            return word.SignedValue;
        }

        public uint ReadUInt32()
        {
            byte[] data = new byte[4];
            this.BaseStream.Read(data, 0, 4);
            BigEnd32 l = new BigEnd32(data, 0);

            return l.UnsignedValue;
        }

        public int ReadInt32()
        {
            byte[] data = new byte[4];
            this.BaseStream.Read(data, 0, 4);
            BigEnd32 l = new BigEnd32(data, 0);

            return l.SignedValue;
        }

        public ulong ReadUInt64()
        {
            byte[] data = new byte[8];
            this.BaseStream.Read(data, 0, 8);
            BigEnd64 l = new BigEnd64(data, 0);
            return l.UnsignedValue;
        }

        public long ReadInt64()
        {
            byte[] data = new byte[8];
            this.BaseStream.Read(data, 0, 8);
            BigEnd64 l = new BigEnd64(data, 0);
            return l.SignedLong;
        }

        public char[] ReadChars(int len)
        {
            byte[] data = new byte[len];
            this.BaseStream.Read(data, 0, len);
            char[] str = new char[len];
            for (int i = 0; i < len; i++)
            {
                str[i] = (char)data[i];
            }
            return str;
        }

        public string ReadString(int len)
        {
            return new string(this.ReadChars(len));
        }

        private StringBuilder _unicodechars = new StringBuilder();
        private bool _unicodelocked = false;

        public string ReadUnicodeString(int len)
        {
            if (_unicodelocked)
                throw new NotSupportedException("Cannot handle multitheaded use of a single instance");
            _unicodelocked = true;

            byte[] data = new byte[len];
            this.BaseStream.Read(data, 0, len);

            _unicodechars.Clear();
            len = data.Length % 2 == 0 ? data.Length : data.Length - 1;
            
            for (int i = 0; i < len; i+= 2)
            {
                BigEnd16 word = new BigEnd16(data, i);
                ushort character = word.UnsignedValue;
                _unicodechars.Append((char)character);
            }
            string value = _unicodechars.ToString();
            _unicodelocked = false;

            return value;
        }

        public string ReadPascalString()
        {
            int len = this.BaseStream.ReadByte();
            return this.ReadString(len);
        }

        

        public byte[] Read(int len)
        {
            byte[] data = new byte[len];
            this.BaseStream.Read(data, 0, len);
            return data;
        }

        public Version ReadUShortVersion()
        {
            ushort major = this.ReadUInt16();

            return new Version((int)major, 0);
        }

        public float ReadFixed1616()
        {
            short major = this.ReadInt16();
            ushort minor = this.ReadUInt16();
            float mf = ((float)minor) / ((float)ushort.MaxValue);
            return ((float)major) + mf;
        }

        public Version ReadFixedVersion()
        {
            ushort major = this.ReadUInt16();
            ushort minor = this.ReadUInt16();
            return new Version((int)major, (int)minor);
        }

        private static DateTime _dateoffsetbase = new DateTime(1904, 1, 1, 0, 0, 0);
        public static DateTime DateOffsetBase
        {
            get { return _dateoffsetbase; }
        }

        public DateTime ReadDateTime()
        {
            ulong l = this.ReadUInt64();
            DateTime dt = DateOffsetBase;

            dt = dt.AddSeconds(l);

            return dt;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        ~BigEndianReader()
        {
            this.Dispose(false);
        }
        #endregion

        
    }
}
