//#define UseBrotliLib

using System;
using System.IO;

namespace Scryber.OpenType.Woff2
{
    /// <summary>
    /// Supports the BrotliCompression in various frameworks
    /// </summary>
    public static class Woff2Brotli
    {

        public static byte[] DecompressData(byte[] dataIn)
        {
            using (var streamIn = new MemoryStream(dataIn))
            {
                var streamOut = Decompress(streamIn) as MemoryStream;

                if (null == streamOut)
                    throw new NullReferenceException("Expected a memory stream as the return from the internal Decompress(stream):stream method");

                return streamOut.ToArray();
            }
        }

        public static Stream Decompress(byte[] dataIn)
        {
            using (var streamIn = new MemoryStream(dataIn))
                return Decompress(streamIn);

        }

        public static Stream Decompress(Stream dataIn)
        {

#if UseBrotliLib

            MemoryStream decompressed = new MemoryStream();
            using (var decompressor = new BrotliSharpLib.BrotliStream(dataIn, System.IO.Compression.CompressionMode.Decompress))
                decompressor.CopyTo(decompressed);
            return decompressed;

#elif NET48

            throw new NotSupportedException("The Brotli decompression is not supported in .Net 4.8");

#elif NETSTANDARD2_0

            throw new NotSupportedException("The Brotli decompression is not supported in .Net 4.8");
#else
            using (var decoder = new System.IO.Compression.BrotliStream(dataIn, System.IO.Compression.CompressionMode.Decompress))
            {
                var dataOut = new MemoryStream();
                decoder.CopyTo(dataOut);
                decoder.Flush();
                return dataOut;
            }

#endif
        }
    }
}
