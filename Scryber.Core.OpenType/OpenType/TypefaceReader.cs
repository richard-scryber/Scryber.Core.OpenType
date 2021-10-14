using System;
using System.IO;


namespace Scryber.OpenType
{
    public class TypefaceReader
    {
        public TypefaceReader()
        {
        }

        public ITypefaceInfo GetInfo(Stream stream, string source)
        {
            using(var reader = new BigEndianReader(stream))
            {
                TypefaceVersionReader version;
                if (!TypefaceVersionReader.TryGetVersion(reader, out version))
                    return new Utility.UnknownTypefaceInfo(source, "Could not identify the version of the font source");

                else
                {
                    var info = version.ReadTypefaceInfoAfterVersion(reader, source);
                    return info;
                }
            }
        }

        public ITypeface GetTypeface(Stream stream, ITypefaceReference theReference)
        {
            using (var reader = new BigEndianReader(stream))
            {
                TypefaceVersionReader version;
                if (!TypefaceVersionReader.TryGetVersion(reader, out version))
                    throw new NullReferenceException("Could not identify the typeface in the provided stream");

                else
                {
                    var typeface = version.ReadTypefaceAfterVersion(reader, theReference);
                    
                    return typeface;
                }
            }

        }


        
        
    }

}
