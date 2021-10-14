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
using System.Text;
using Scryber.OpenType.SubTables;
using Scryber.OpenType.TTF;

namespace Scryber.OpenType
{
    [Obsolete("This class has been deprecated, please use the TypeFaceReader to support more formats", false)]
    public class TTFFile : TTF.TrueTypeFile
    {

        public TTFFile(byte[] data, int headOffset)
            : base(data, headOffset)
        {
        }

        public TTFFile(string path, int headOffset)
            : base(GetData(path, headOffset), headOffset)
        {
        }


        private static byte[] GetData(string path, int headOffset)
        {
            var fi = new System.IO.FileInfo(path);
            if (fi.Exists == false)
                throw new System.IO.FileNotFoundException("The font file at '" + fi.FullName + "' does not exist");

            return System.IO.File.ReadAllBytes(path);
        }


        public void Read(string path, int headOffset)
        {
            this.Read(new System.IO.FileInfo(path), headOffset);
        }

        public void Read(System.IO.FileInfo fi, int headOffset)
        {
            if (fi.Exists == false)
                throw new System.IO.FileNotFoundException("The font file at '" + fi.FullName + "' does not exist");

            using (System.IO.FileStream fs = fi.OpenRead())
            {
                this.Read(fs, headOffset);
            }
        }

        public void Read(System.IO.Stream stream, int headOffset)
        {
            System.IO.MemoryStream ms = null;


            try
            {
                if (stream.CanSeek && stream.Length < (long)int.MaxValue)
                    ms = new System.IO.MemoryStream((int)stream.Length);
                else
                    ms = new System.IO.MemoryStream();

                //Copy the stream to a private data array
                byte[] buffer = new byte[4096];
                int count;
                while ((count = stream.Read(buffer, 0, 4096)) > 0)
                {
                    ms.Write(buffer, 0, count);
                }

                if (headOffset > 0)
                {
                    byte[] data = TTC.TTCollectionFile.ExtractTTFfromTTC(ms, headOffset);
                    ms.Dispose();
                    ms = new System.IO.MemoryStream(data);

                }

                ms.Position = 0;
                using (BigEndianReader reader = new BigEndianReader(ms))
                {
                    this.Read(reader);
                }
                this.FileData = ms.ToArray();
            }
            catch (Exception ex)
            {
                throw new System.IO.IOException("Could not load the font file from the stream. " + ex.Message);
            }
            finally
            {
                if (null != ms)
                {
                    ms.Dispose();
                    ms = null;
                }
            }
        }

    }

}
