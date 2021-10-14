using System;
using System.Collections;

namespace Scryber.OpenType
{
    public interface ITypefaceReference
    {
        string FamilyName { get; }

        WeightClass FontWeight { get; }

        WidthClass FontWidth { get; }

        FontRestrictions Restrictions { get; }

        FontSelection Selections { get; }
    }

    public interface ITypefaceInfo
    {
        string Path { get; }

        int TypefaceCount { get; }

        ITypefaceReference[] References { get; }

        DataFormat SourceFormat { get; }

        string ErrorMessage { get; }
    }

    public interface ITypeface
    {
        bool IsValid { get; }

        DataFormat SourceFormat { get; }

        ITypefaceReference Reference { get; }

        byte[] GetFileData(DataFormat format);

        ITypefaceMetrics GetMetrics();
    }


    public interface ITypefaceMetrics
    {

        LineSize Measure(string chars, int startOffset, double emSize, double maxWidth, TypeMeasureOptions options);
    }

}
