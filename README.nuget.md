# Scryber.Core.OpenType

**Reads, measures, converts and subsets TrueType and OpenType fonts in .NET.**

This is the font library used by [Scryber.Core](https://www.nuget.org/packages/Scryber.Core/) and [Paperwork.Core](https://www.nuget.org/packages/Paperwork.Core/) to generate PDF documents, but it does not depend on either, so you can use it on its own.

- Read the family, weight, style and embedding restrictions of the fonts in a file, stream or url.
- Read `.ttf`, `.otf` (including CFF outlines), `.ttc` collections and WOFF (version 1).
- Get the font program as TrueType data, including a single font from a collection or a WOFF file.
- Measure how many characters of a line fit in a width, at any font size.
- Subset a TrueType font to only the glyphs you use, to embed a much smaller font.
- Access the OpenType tables (`cmap`, `OS/2`, `hhea` and more) directly.

It targets .NET Standard 2.0, .NET 6, 8, 9 and 10, and works in Blazor WebAssembly, with async loading from urls.

## Install

```bash
dotnet add package Scryber.Core.OpenType
```

## Read the fonts in a file

```csharp
using Scryber.OpenType;

using (var reader = new TypefaceReader())
{
    var info = reader.ReadTypeface(new FileInfo("fonts/Roboto-Regular.ttf"));

    foreach (var font in info.Fonts)
        Console.WriteLine(font.FamilyName + " " + font.FontWeight + " " + font.Restrictions);
}
```

`ReadTypefaceAsync` reads from a `Uri`, and `ReadAllTypefaces` reads every font in a directory.

## Measure a line of text

```csharp
using (var reader = new TypefaceReader())
{
    var font = reader.GetFont(new FileInfo("fonts/Roboto-Regular.ttf"), 0);

    var options = new TypeMeasureOptions() { BreakOnWordBoundaries = true };
    var metrics = font.GetMetrics(options);

    //how much of the text fits in 200pt at 12pt
    var size = metrics.MeasureLine("This is my line of text", 0, 12.0, 200.0, options);
    Console.WriteLine("Fitted " + size.CharsFitted + " characters in " + size.RequiredWidth + "pt");
}
```

## Subset a font

`FontSubsetter.Subset` keeps only the outlines of the glyphs you pass. The glyph numbers stay the same, so text that refers to glyphs by number is unchanged. Glyph 0 and the parts of composite glyphs (such as the letter and the accent of an accented character) are always kept.

```csharp
using Scryber.OpenType.SubTables;

using (var reader = new TypefaceReader())
{
    var font = reader.GetFont(new FileInfo("fonts/Roboto-Regular.ttf"), 0);
    var data = font.GetFileData(DataFormat.TTF);

    //the glyph numbers of the characters used, from the cmap table
    var ot = (IOpenTypeFont)font;
    ot.TryGetTable<CMAPTable>("cmap", out var cmap);
    var map = cmap.GetOffsetTable(CMapEncoding.Windows(WindowsCharacterEncodings.Unicode));
    var glyphs = "Hello world".Select(c => map.GetCharacterGlyphOffset(c));

    var result = FontSubsetter.Subset(data, glyphs);

    if (result.Success)
        File.WriteAllBytes("Roboto-Subset.ttf", result.Data);
    else
        Console.WriteLine("Not subset: " + result.Reason);
}
```

A font with CFF outlines, a collection, or a font whose licence does not allow subsetting (OS/2 `fsType` 0x0100) is not subset. The result gives the reason, and you should embed the whole font instead. `FontSubsetter.CanSubset` checks this first.

## Documentation

The [read me on GitHub](https://github.com/richard-scryber/Scryber.Core.OpenType) covers each reader method, the conversions supported and table access. The library is open source under the LGPL 3.0.
