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
            var dataOut = BrotliSharpLib.Brotli.DecompressBuffer(dataIn, 0, dataIn.Length);
            return dataOut;
        }

        public static Stream Decompress(byte[] dataIn)
        {
            var dataOut = BrotliSharpLib.Brotli.DecompressBuffer(dataIn, 0, dataIn.Length);
            MemoryStream decompressed = new MemoryStream(dataOut);
            return decompressed;
        }

        public static Stream Decompress(Stream dataIn)
        {
            MemoryStream decompressed = new MemoryStream();
            using (var decompressor = new BrotliSharpLib.BrotliStream(dataIn, System.IO.Compression.CompressionMode.Decompress))
                decompressor.CopyTo(decompressed);
            return decompressed;
        }
    }
}
