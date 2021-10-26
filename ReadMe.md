<h1 align='center'>
    <img height='150' src='https://raw.githubusercontent.com/richard-scryber/scryber.core/master/ScyberLogo2_alpha_small.png'/>
    <br/>
    <span style='font-weight:900'> scryber.core</span> <span style='font-weight:300'>opentype</span>
</h1>


This repository is the OpenType library capability for Scryber, but placed independently in case it helps others.

It allows the...

* Reading of multiple font files from various sources,
* Measuring lines of characters within available space using that font.
* The conversion from ttc or woff to open type byte arrays or streams.
* Both syncronous and asyncronous loading ofr typefaces and fonts.


# Installing Scryber.Core.OpenType

The easiest way is to add the Nuget package [Scryber.Core.OpenType](https://www.nuget.org/packages/Scryber.Core.OpenType/) to your project via the CLI

```
    > dotnet add package Scryber.Core.OpenType
```

This will add the latest version (6.0.0-alpha at the time of writing) and supports the following frameworks

* .NET Standard 2.0
* .NET Standard 2.1
* .NET Core 3.1
* .NET 5.0
* .NET 6.0 (RC1)
* .NET Framework 4.8


# Supported Typeface Formats

The ``Scryber.Core.OpenType`` library supports reading typefaces and font programmes in the following formats:

- [x] True Type fonts.
- [x] Open Type fonts (including ccf).
- [x] True Type collections
- [x] Woff fonts (version 1)
- [ ] <del>Woff2 is coming, but need to resolve the glyph tables back into true type format first.</del>




# Using the library

If you want to use the library in your own projects or build on the base library, please feel free.
And if you have any troublesome fonts, please just call them out.
Happy to help, as it helps us too.


> In this document we use the terms 'Typeface' to refer to a single file or stream,
> and 'Font' to refer to the one or more font programmes in that file or stream


# 1. Create a TypefaceReader

The TypefaceReader (``Scryber.OpenType.TypefaceReader``) is the primary entry point to the library and there are multiple constructors for the disposable class that can read a typeface from either a url, file, stream or custom StreamLoader (``Scryber.OpenType.Utility.StreamLoader``).
The overloads also allow a base path to be defined and also an 'HttpClient or WebClient' to be passed rather than a new one be created on demand by each reader instance.


```csharp

   //using Scryber.OpenType;

   using(var reader = new TypefaceReader())
   {
   }

```

Constructor                             | Support       | Description
----------------------------------------|---------------|------------
new TypefaceReader()                    | All           | Parameterless constructor without any base paths or network clients.
new TypefaceReader(Uri)                 | All           | Takes an **Absolute** url to use as the base path for any relative Uri requests.
new TypefaceReader(HttpClient)          | NOT .Net4.8   | Takes an HttpClient that will be used for Url requests, but will not be disposed of by the reader.
new TypefaceReader(Uri, HttpClient)     | NOT .Net4.8   | Takes an **Absolute** url to use as the base path for any relative Uri requests, and a **Web**client that will be used for those requests, but will not be disposed of by the reader.
new TypefaceReader(WebClient)           | .Net4.8 Only  | Takes a WebClient that will be used for Url requests, but will not be disposed of by the reader.
new TypefaceReader(Uri, WebClient)      | .Net4.8 Only  | Takes an **Absolute** url to use as the base path for any relative Uri requests, and a **Web**client that will be used for those requests, but will not be disposed of by the reader.
new TypefaceReader(DirectoryInfo)       | All           | Takes a directory to use as the base for any relative path requests.
new TypefaceReader(Utility.StreamLoader)| All           | Allows for the passing of a custom stream loader that will be used for **All** reaquests.

# 2. Reading the information in a typeface file.

Once a reader is created it can be used to read one or more typefaces from a stream, file or url using the ``ReadTypeface`` or ``ReadTypefaceAsync`` variants.


```csharp

   //using Scryber.OpenType

   using(var reader = new TypefaceReader())
   {
        var file = new FileInfo("[path to font]"); //new Uri("[absolute or relative url]");
        var face = reader.ReadTypeface(file); //await reader.ReadTypefaceAsync(url);
   }

```

The library uses the .net framework `FileInfo` and `Uri` classes along with the Stream classes to distinguish easily between file paths and urls, rather than trusting strings.

It also offers various overloads for each of the methods.

Method                                                               | Overloads                        | Async Support | Description
---------------------------------------------------------------------|----------------------------------|---------------|----------------------------------------------------------------------------
ReadAllTypefaces(DirectoryInfo):ITypefaceInfo[]                      | extension Pattern / include subs | Yes           | Reads all the typefaces in a directory (optionally matching a specific pattern and or including subdirectories)
ReadAllTypefaces(IEnumerable\<FileInfo\>):ITypefaceInfo[]            | File or Uri                      | Yes           | Reads all the typefaces in the list of files or urls provided.
TryReadTypeface(FileInfo, out ITypefaceInfo):bool                    | File, Uri or Stream              | No            | Attempts to read the typeface in a file url or stream, setting the error message on any info's if not possible
ReadTypeface(FileInfo):ITypefaceInfo                                 | File, Uri or Stream              | Yes           | Reads the typeface, from the location or stream, throwing an exception if not possible

If a base path was provided in the reader's constructor it will be used if the argument is a file info and the base is a directory, or the argument is a relative url and the base was an absolute url.

## 2.1. The ITypefaceInfo instance

The returned ``ITypefaceInfo`` provides information on the loaded source, along with the ``Fonts`` contained in the typeface.


Property    | Type          | Description
------------|---------------|------------------------------------------------------------
Source      | String        | The path to the typeface that was loaded.
SourceFormat| DataFormat    | The underlying data format for the source e.g. TTF, WOFF etc.
FontCount   | int           | The number of font programmes within the typeface source.
Fonts       | IFontInfo[]   | Information on each of the fonts within the source.
ErrorMessage| string        | If the ``TryReadTypeface`` method could not load some fonts, then any errors will be here.

## 2.2 The IFontInfo instance

The ``Fonts`` property for the typeface can be looped over to find each of the font programmes in the source based on their families, weights, widths, selections (italic etc.) and any file restrictions (embedding).


Property    | Type             | Description
------------|------------------|---------------------
FamilyName  | String           | The postscript family for the font programme.
FontWeight  | WeightClass      | How heavy the rendered font would be (light to black, 100 to 900).
FontWidth   | WidthClass       | How wide the rendered font would be (condensed to expanded - not to be relied on, as this often forms part of the family name - e.g. Arial Condensed).
Selections  | FontSelection    | Information on the type of font this is - Italic, Strikeout, Outlined etc.
Restrictions| FontRestrictions | What restrictions are placed on the font usage - can it be embedded in other applications or documents.


**ttf and otf files should only contain one font, ttc can be multiple fonts, usually from the same family**

# 3. Reading font programmes

A ``TypefaceReader`` also reads the font programmes within the the files or streams with the ``GetFont`` and similar methods, allowing both synchronous and asyncronous loading of font programmes.
All methods return either a single or a collection of ``ITypefaceFont``(s) from a previous ITypefaceInfo or IFontInfo, or paths or streams.

```csharp

   using(var reader = new TypefaceReader())
   {
        var file = new FileInfo("[path to font]"); //new Uri("[absolute or relative url]");
        var font = reader.GetFonts(file); //await reader.GetFontsAsync(url);
   }

```

The returned ``ITypefaceFont`` is also an ``IFontInfo`` interface so can be used interchangably but add functionality for extracting tables, data and getting the font metrics.


Method                                                               | Overloads                          | Async Support     | Description
---------------------------------------------------------------------|------------------------------------|-------------------|----------------------------------------------------------------------------
GetFonts(ITypefaceInfo):IEnumerable<ITypefaceFont>                   | Typeface, Uri, FileInfo, Stream    | Yes (exc. Stream) | Reads all the fonts in the typeface source data, and returns as a collection
GetFirstFont(Uri):ITypefaceFont                                      | File or Uri                        | Yes               | Reads the first font programme from a source, that can either be a single font, or a collection.
GetFont(ITypefaceInfo, int):ITypefaceFont                            | Typeface, Uri, FileInfo, Stream    | Yes (exc. Stream) | Reads the font programme from the typeface info or a source, at the specified index in the collection or 0 for a single font file.
GetFont(ITypefaceInfo, IFontInfo):ITypefaceFont                      | Typeface, File, Uri or Stream      | Yes (exc. Stream) | Reads the font programme for the font info in the typeface or in a source.


There are no TryGetFont methods as the info access should be used to check fonts, and any other failures would point to an invalid font (or an issue with the library).

## 3.1. The ITypefaceFont

The ITypefaceFont has a ``GetFileData`` method that will return a byte[] for the data that can be saved to another stream matching the format requested, **if that is supported**
The ``CanGetFileData`` will return true if the conversion would be supported.

It also supports the font metrics using the ``GetMetrics`` method.

The returned instances can also support the IOpenTypeFont. This interface enables access to the inner tables in an open/true type font programme, along with any decoded ``SubTables``.



## 3.2. Extracting the font data

Primarily the ONLY fully supported format is ``DataFormat.TTF``, and will support the conversion if the original source was OpenType, TrueType, TrueTypeCollection, or Woff v1.


```csharp

   //using System.IO;
   
   using(var reader = new TypefaceReader())
   {
        var file = new FileInfo("[path to font]"); //new Uri("[absolute or relative url]");
        var font = reader.GetFont(file, 0); //await reader.GetFontsAsync(url);

        if(font.CanGetFileData(DataFormat.TTF))
            File.WriteAllBytes("[new path].ttf", font.GetFileData(DataFormat.TTF);
   }

```

## 3.3 Supported conversion

The following table shows the input data type and supported output type for an ITypefaceFont.

Source Data   | Output data types
--------------|--------------------
True Type     | TTF
Open Type     | TTF
Collection    | TTF or TTC
Woff          | TTF or WOFF

It may be that we will add more in the future, but it meets the needs of our usage at the moment.


# 3.4 Measuring strings

The font metrics returned from the ``ITypefaceFont`` enables the measurement of strings of characters in a font programme.
At the moment it is fairly basic, only supporting horizontal characters, word and character spacing, and word boundaries, nor it does not use the kerning tables.


However it is fast.


```csharp

   //using System.IO;
   
   using(var reader = new TypefaceReader())
   {
        var file = new FileInfo("[path to font]"); //new Uri("[absolute or relative url]");
        var font = reader.GetFont(file, 0); //await reader.GetFontsAsync(url);

        var options = new TypeMeasureOptions() { BreakOnWordBoundaries = true };

        //Get the standard font metrics for the font.
        var metrics = font.GetMetrics(options);

        var chars = "This is my line of text";
        var start = 0;
        var fontSize = 12.0;
        var max = 200.0;
 
        var size = metrics.MeasureLine(chars, start, fontSize, max, options);

        Console.WriteLine("Fitted " + size.CharsFitted + " in "
                                    + size.RequiredWidth
                                    + (OnWordBoundary ? ", breaking on a word boundary" : ""));
   }

```

The font metrics instance holds a reference to the font programme, but can also add lookup tables for common and used characters.
A single instance can be held to measure any number of font sizes. If multiple fonts are used, they, and the data they contain will not be garbage collected if the measure is still referenced.


# 3.5. OpenType Table access

If the ITypefaceFont implements the ``IOpenTypeFont`` interface the sub stables that make up the Open Type font format can be accessed. Some of these have dedicated sub table implementations
in the ``Scryber.OpenType.SubTables`` namespace.

Use the ``TableCount`` and ``TableKeys`` properties of the IOpenTypeFont interface to get the tables in the font.
Use the  ``TryGetTable`` to access any decoded tables in the font.

```csharp

   //using System.Collections.Generic;
   //using Scryber.OpenType.SubTables;

   using(var reader = new TypefaceReader())
   {
        var file = new FileInfo("[path to font]"); //new Uri("[absolute or relative url]");
        var font = reader.GetFont(file, 0); //await reader.GetFontsAsync(url);

        if(ofnt is IOpenTypeFont ot)
        {
            List<string> names = new List<string>(ot.TableKeys);

            OS2Table os2;

            if(ot.TryGetTable("OS/2", out os2))
            {
                Console.WriteLine("Superscript y offset : " + os2.SuperscriptYOffset);
                Console.WriteLine("Superscript y size : " + os2.SuperScriptYSize);
            }
        }
   }

```


# 4. Data Caching

**Note: the library DOES NOT implement any form of data caching, so urls used for the original load maybe re-loaded again to request the actual font data.**

Many use cases are to read information from many sources initially but these may ultimately never be used again.
Any data held in cache would be consuming space that would NEVER be needed, and there could be a significant number of entries.

The xxxInfo classes are lightweight and can be manipulated as desired without large memory constraints.

It may be done in the future, but if caching is needed, then the stream methods can be called with cached data and allow effecient use of memory data for multiple requests.
This includes the Uri methods, and any Http response stream must be read completely and used as a seekable memory stream.

# 5. With thanks

We have referred to a lot of other open source software and libraries to get this code together, and it is with thanks to them that we
can have been able to do this.

* iTextSharp in it's various releases,
* SharpZipLib for decompression where decomression is not available
* Typography.OpenFont for the Woff and Woff2 formats

These are brilliant libraries and assets to the opensource community.

# 6. Contributing

If you want to use the code or refer to it in your own libraries, feel free.
If you want to contribute, we would love help to use vertical fonts, arabic and otehr right to left scripts, and Woff2 decompression.

Happy to let anyone fork the repository, contribute and we will pull.

### Happy coding.


