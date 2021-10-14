using System;
namespace Scryber.OpenType
{
    public class TypeMeasureOptions
    {
        public double? CharacterSpacing { get; set; }

        public double? WordSpacing { get; set; }

        public bool BreakOnWordBoundaries { get; set; }

        public TypeMeasureOptions()
        {
        }
    }
}
