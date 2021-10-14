using System;
namespace Scryber.OpenType.TTF
{
    public class TrueTypeDefaultTableFactory : TrueTypeTableFactory
    {
        public TrueTypeDefaultTableFactory(bool throwonnotfound)
            : base(throwonnotfound, new string[] { "cmap", "head", "hhea", "maxp", "name", "OS/2", "post" })
        {
        }
    }
}
