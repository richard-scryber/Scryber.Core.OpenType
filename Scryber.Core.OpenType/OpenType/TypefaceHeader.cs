using System;
namespace Scryber.OpenType
{
    public class TypefaceHeader
    {

        private TypefaceVersionReader _vers;
        public TypefaceVersionReader Version
        {
            get { return _vers; }
            set { this._vers = value; }
        }

        private int _numtables;

        public int NumberOfTables
        {
            get { return _numtables; }
            set { _numtables = value; }
        }

        public TypefaceHeader(TypefaceVersionReader version, int numTables)
        {
            this.Version = version;
            this.NumberOfTables = numTables;
        }
    }
}
