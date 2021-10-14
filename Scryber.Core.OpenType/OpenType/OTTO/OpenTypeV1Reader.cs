using System;
namespace Scryber.OpenType.OTTO
{
    public class OpenType1VersionReader : TTF.TrueTypeVersionReader
    {
        private Version _innervers;
        protected Version InnerVersion
        {
            get { return _innervers; }
        }

        public OpenType1VersionReader(UInt16 major, UInt16 minor, byte[] data)
            : this(new Version((int)major, (int)minor), data, DataFormat.TTF)
        {
            if (this._innervers != new Version("1.0"))
                throw new TypefaceReadException("The open type version can only be version 1.0");
        }

        protected OpenType1VersionReader(Version vers, byte[] data, DataFormat format)
            : base(vers.ToString(), data, format)
        {
            this._innervers = vers;
        }


        public override string ToString()
        {
            return "Open Type " + InnerVersion.ToString();
        }

        internal override TTF.TrueTypeTableFactory GetTableFactory()
        {
            return new OpenTypeTableFactory(false);
        }
    }
}
