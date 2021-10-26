/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Scryber.OpenType.SubTables;

namespace Scryber.OpenType
{
    /// <summary>
    /// Base class for all versions of an OpenType Typeface file that is based on 4 bytes identifier and BigEndian ordering
    /// </summary>
    public abstract class TypefaceVersionReader
    {
        public byte[] HeaderData { get; private set; }

        public DataFormat DataFormat { get; private set; }

        public static readonly byte[] TrueTypeHeaderBytes = new byte[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e' };
        public static readonly byte[] OpenTypeHeaderBytes = new byte[] { (byte)'O', (byte)'T', (byte)'T', (byte)'O' };
        public static readonly byte[] Type1HeaderBytes = new byte[] { (byte)'t', (byte)'y', (byte)'p', (byte)'1' };
        public static readonly byte[] TrueTypeCollectionHeaderBytes = new byte[] { (byte)'t', (byte)'t', (byte)'c', (byte)'f' };
        public static readonly byte[] WoffHeaderBytes = new byte[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e' };
        public static readonly byte[] Woff2HeaderBytes = new byte[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e' };


        protected TypefaceVersionReader(byte[] header, DataFormat format)
        {
            this.HeaderData = header;
            this.DataFormat = format;
        }


        public abstract ITypefaceInfo ReadTypefaceInfoAfterVersion(BigEndianReader reader, string source);

        public abstract ITypefaceFont ReadTypefaceAfterVersion(BigEndianReader reader, IFontInfo forReference, string source);

        public abstract override string ToString();


        protected byte[] CopyStreamData(Stream fromStream, long fromOffset)
        {
            if (fromOffset == 0 && fromStream is MemoryStream msOrig)
                return msOrig.ToArray();
            else
            {
                byte[] data = null;

                var endOffset = fromStream.Position;
                int capacity = (int)(endOffset - fromOffset);
                fromStream.Position = fromOffset;

                using (var ms = new MemoryStream(capacity))
                {
                    ExtractData(fromStream, ms);
                    ms.Position = 0;
                    data = ms.ToArray();
                }

                //Set the stream position back to where we were before returning
                fromStream.Position = endOffset;
                return data;
            }
        }

        private void ExtractData(Stream stream, MemoryStream into)
        {
            //Copy the stream to a private data array
            byte[] buffer = new byte[4096];
            int count;
            while ((count = stream.Read(buffer, 0, 4096)) > 0)
            {
                into.Write(buffer, 0, count);
            }
        }

        /// <summary>
        /// Gets a version reader for a typeface from the reader at the current position
        /// </summary>
        /// <param name="reader">The bigendian reader to read the version from</param>
        /// <param name="vers">Set to the version reader if known</param>
        /// <returns>True if the version is known, otherwise false</returns>
        /// <remarks>This will check the current reader for a known version and move the position on 4 bytes</remarks>
        public static bool TryGetVersion(BigEndianReader reader, out TypefaceVersionReader vers, bool thrownOnUnsupported = false)
        {
            vers = null;
            byte[] data = reader.Read(4);
            char[] chars = ConvertToChars(data, 4);

            if (chars[0] == 'O' && chars[1] == 'T' && chars[2] == 'T' && chars[3] == 'O')        //OTTO
            {
                vers = new OTTO.CCFOpenTypeVersionReader(new string(chars), data);
            }
            else if (chars[0] == 't' && chars[1] == 'r' && chars[2] == 'u' && chars[3] == 'e')   //true
                vers = new TTF.TrueTypeVersionReader(new string(chars), data);

            else if (chars[0] == 't' && chars[1] == 'y' && chars[2] == 'p' && chars[3] == '1')   //typ1
                vers = new TTF.TrueTypeVersionReader(new string(chars), data);

            else if (chars[0] == 't' && chars[1] == 't' && chars[2] == 'c' && chars[3] == 'f')   //ttcf
                vers = new TTC.TTCollectionVersionReader(new string(chars), data);

            else if (chars[0] == 'w' && chars[1] == 'O' && chars[2] == 'F' && chars[3] == 'F')   //wOFF
                vers = new Woff.WoffVersionReader(new string(chars), data);

            else if (chars[0] == 'w' && chars[1] == 'O' && chars[2] == 'F' && chars[3] == '2')   //wOF2
                vers = null;// new Woff2.Woff2VersionReader(new string(chars), data);
                //throw new NotSupportedException("The Woff2 format is not currently supported.");
            else                                                                                 //1.0
            {
                BigEnd16 wrd1 = new BigEnd16(data, 0);
                BigEnd16 wrd2 = new BigEnd16(data, 2);

                if (((int)wrd1.UnsignedValue) == 1 && ((int)wrd2.UnsignedValue) == 0)
                    vers = new OTTO.OpenType1VersionReader(wrd1.UnsignedValue, wrd2.UnsignedValue, data);


            }

            return vers != null;
        }

        private static char[] ConvertToChars(byte[] data, int count)
        {
            char[] chars = new char[count];

            for (int i = 0; i < count; i++)
            {
                chars[i] = (char)data[i];
            }
            return chars;
        }


    }

}
