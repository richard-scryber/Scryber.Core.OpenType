using System;

namespace Scryber.OpenType.Woff2.Numerics
{
    public readonly struct Bounds
    {

        
        public static readonly Bounds Zero = new Bounds(0, 0, 0, 0);

        public Bounds(short xmin, short ymin, short xmax, short ymax)
        {
            XMin = xmin;
            YMin = ymin;
            XMax = xmax;
            YMax = ymax;
        }

        public short XMin { get; }
        public short YMin { get; }
        public short XMax { get; }
        public short YMax { get; }

        public override string ToString()
        {
            return "(" + XMin + "," + YMin + "," + XMax + "," + YMax + ")";
        }

    }
}
