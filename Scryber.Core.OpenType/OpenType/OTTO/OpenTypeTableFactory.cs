using System;
using System.Text;
using System.Collections.Generic;

using Scryber.OpenType.TTF;
using Scryber.OpenType.SubTables;

namespace Scryber.OpenType.OTTO
{

    public class OpenTypeTableFactory : TrueTypeTableFactory
    {

        public OpenTypeTableFactory(bool throwonnotfound)
            : base(throwonnotfound, new string[] { "cmap", "head", "hhea", "maxp", "name", "OS/2", "post" })
        {
        }

        
    }
}
