using System;
namespace Scryber.OpenType
{
    public class BigEndianWriter : IDisposable
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
            get { return this.BaseStream.CanWrite; }
        }

        
        public long Length
        {
            get { return this.BaseStream.Length; }
        }

        public long Position
        {
            get { return this.BaseStream.Position; }
            set { this.BaseStream.Position = value; }
        }

        private System.IO.Stream _base;

        protected System.IO.Stream BaseStream
        {
            get { return _base; }
        }



        public BigEndianWriter(System.IO.Stream basestream)
        {
            if (basestream == null)
                throw new ArgumentNullException("basestream");
            if (basestream.CanRead == false)
                throw new ArgumentException("The base stream does not support reading!");
            if (basestream.CanSeek == false)
                throw new ArgumentException("The Writer requires a stream that supports seeking (changing position within the stream) as well as forward only reading");
            if (basestream.CanWrite == false)
                throw new ArgumentException("The writer requires a stream that can be written to");

            this._base = basestream;
        }


        public void WriteByte(byte value)
        {
            this._base.WriteByte(value);
        }

        public void WriteUInt16(UInt16 value)
        {
            var both = BitConverter.GetBytes(value);

            //Reverse the order
            this._base.WriteByte(both[1]);
            this._base.WriteByte(both[0]);

        }

        public void WriteInt16(Int16 value)
        {
            var both = BitConverter.GetBytes(value);

            //Reverse the order
            this._base.WriteByte(both[1]);
            this._base.WriteByte(both[0]);

        }

        public void WriteUInt32(UInt32 value)
        {
            var both = BitConverter.GetBytes(value);

            //Reverse the order
            this._base.WriteByte(both[3]);
            this._base.WriteByte(both[2]);
            this._base.WriteByte(both[1]);
            this._base.WriteByte(both[0]);
        }

        public void WriteInt32(Int32 value)
        {
            var both = BitConverter.GetBytes(value);

            //Reverse the order
            this._base.WriteByte(both[3]);
            this._base.WriteByte(both[2]);
            this._base.WriteByte(both[1]);
            this._base.WriteByte(both[0]);

        }

        public void WriteUInt64(UInt64 value)
        {
            var both = BitConverter.GetBytes(value);

            //Reverse the order
            this._base.WriteByte(both[7]);
            this._base.WriteByte(both[6]);
            this._base.WriteByte(both[5]);
            this._base.WriteByte(both[4]);
            this._base.WriteByte(both[3]);
            this._base.WriteByte(both[2]);
            this._base.WriteByte(both[1]);
            this._base.WriteByte(both[0]);

        }

        public void WriteInt64(Int64 value)
        {
            var both = BitConverter.GetBytes(value);

            //Reverse the order
            this._base.WriteByte(both[7]);
            this._base.WriteByte(both[6]);
            this._base.WriteByte(both[5]);
            this._base.WriteByte(both[4]);
            this._base.WriteByte(both[3]);
            this._base.WriteByte(both[2]);
            this._base.WriteByte(both[1]);
            this._base.WriteByte(both[0]);

        }

        public void Write(byte[] data)
        {
            this.BaseStream.Write(data, 0, data.Length);
        }

        public void WriteASCIIChars(char[] chars)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(chars);
            this.Write(data);            
        }

        public void WriteASCIIChars(string chars)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(chars);
            this.Write(data);
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        ~BigEndianWriter()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
