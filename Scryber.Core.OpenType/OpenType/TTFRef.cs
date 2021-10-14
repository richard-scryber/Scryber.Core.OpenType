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
using System.Runtime.CompilerServices;
using System.Text;
using Scryber.OpenType.SubTables;
using Scryber.OpenType.TTC;
using Scryber.OpenType.TTF;

namespace Scryber.OpenType
{
    [Obsolete("This class has been deprecated, please use the TypeFaceReader to support more formats", false)]
    public class TTFRef
    {
        
        #region public string FullPath {get;}

        private string _path;

        /// <summary>
        /// Gets the full path to the true type font file this reference points to
        /// </summary>
        public string FullPath
        {
            get { return _path; }
        }

        #endregion

        #region public bool IsValid {get; protected set;}

        private bool _isvalid;

        /// <summary>
        /// Gets the flag that identifies if the font file this reference points to is valid.
        /// </summary>
        public bool IsValid
        {
            get { return _isvalid; }
            protected set { _isvalid = value; }
        }

        #endregion

        #region public string ErrorMessage {get; protected set;}

        private string _error;
        /// <summary>
        /// Gets any error message associated with this reference if the loading for the reference failed, inheritors can set.
        /// </summary>
        public string ErrorMessage
        {
            get { return _error; }
            protected set { _error = value; }
        }

        #endregion

        #region public string FamilyName {get; protected set;}

        private string _family;

        /// <summary>
        /// Gets the family name of this font (inheritors can set)
        /// </summary>
        public string FamilyName
        {
            get
            {
                this.AssertValid();
                return _family;
            }
            protected set { this._family = value; }
        }

        #endregion

        #region public WeightClass FontWeight {get; protected set;}

        private WeightClass _fontWeight;

        /// <summary>
        /// Gets the weight of this font, inheritors can set
        /// </summary>
        public WeightClass FontWeight
        {
            get
            {
                this.AssertValid();
                return _fontWeight;
            }
            protected set { _fontWeight = value; }
        }

        #endregion

        #region public WidthClass FontWidth {get; protected set;}

        private WidthClass _fwidth;
        /// <summary>
        /// Gets the width class of this font, inheritors can set.
        /// </summary>
        public WidthClass FontWidth
        {
            get
            {
                this.AssertValid();
                return _fwidth;
            }
            protected set { _fwidth = value; }
        }

        #endregion

        #region public SubTables.FontRestrictions FontRestrictions {get; protected set;}

        private FontRestrictions _retrictions;

        /// <summary>
        /// Gets the usage restrictions of this font. inheritors can set
        /// </summary>
        public FontRestrictions FontRestrictions
        {
            get
            {
                this.AssertValid();
                return _retrictions;
            }
            protected set { this._retrictions = value; }
        }

        #endregion

        #region public SubTables.FontSelection FontSelection {get; protected set;}

        private FontSelection _fsel;

        /// <summary>
        /// Gets the font selection (italic, underscore etc) of this font, inheritors can set.
        /// </summary>
        public FontSelection FontSelection
        {
            get
            {
                this.AssertValid();
                return _fsel;
            }
            protected set { _fsel = value; }
        }

        #endregion

        #region public bool CanEmbed {get;}

        /// <summary>
        /// Returns true if this font can be embedded
        /// </summary>
        public bool CanEmbed
        {
            get
            {
                bool b = (this.FontRestrictions & FontRestrictions.NoEmbedding) == 0;
                if (b)
                    b = (this.FontRestrictions & FontRestrictions.PreviewPrintEmbedding) > 0;
                return b;
            }
        }

        #endregion

        #region public int HeadOffset {get;set;}

        private int _hoffset;

        /// <summary>
        /// Returns the offset of the OffsetTable for the file - usually zero for a ttf,
        /// for ttc these are based on the original TTC Header offsets.
        /// </summary>
        public int HeadOffset
        {
            get { return _hoffset; }
            set { _hoffset = value; }
        }

        #endregion

        //
        // constructor
        //

        #region public TTFRef(string fullpath)

        public TTFRef(string fullpath)
        {
            this._path = fullpath;
            this._isvalid = true;
        }

        #endregion

        //
        // methods
        //

        #region public string GetFullName()

        /// <summary>
        /// Returns the full name (Family name and font selection) for this font
        /// </summary>
        /// <returns></returns>
        public string GetFullName()
        {
            this.AssertValid();

            StringBuilder sb = new StringBuilder();
            sb.Append(this.FamilyName);

            if (this.FontSelection != FontSelection.Regular)
            {
                sb.Append(" ");
                sb.Append(this.FontSelection.ToString());
            }
            return sb.ToString();
        }

        #endregion

        #region protected virtual void AssertValid()

        /// <summary>
        /// Checks this font reference and raises a TTFReadException if it is not valid
        /// </summary>
        protected virtual void AssertValid()
        {
            if (this.IsValid == false)
                throw new TypefaceReadException(this.ErrorMessage);
        }

        #endregion

        //
        // static factory methods
        //

        #region public static TTFRef[] LoadRefs(System.IO.DirectoryInfo dir)

        /// <summary>
        /// Gets an array of all the font file references in the specified directory.
        /// If any files cannot be loaded, then it will be included as a TTFRef with it's invalid flag set, and error message set appropriately.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static TTFRef[] LoadRefs(System.IO.DirectoryInfo dir)
        {
            List<TTFRef> matched = new List<TTFRef>();
            List<System.IO.FileInfo> fis = new List<System.IO.FileInfo>();

            fis.AddRange(dir.GetFiles("*.ttf"));
            fis.AddRange(dir.GetFiles("*.otf"));

            if (fis.Count > 0)
            {
                for (int i = 0; i < fis.Count; i++)
                {
                    TTFRef aref = LoadRef(fis[i]);
                    if (aref != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Loaded Font : " + aref.GetFullName());
                        matched.Add(aref);
                    }
                }
            }

            List<System.IO.FileInfo> cols = new List<System.IO.FileInfo>();
            cols.AddRange(dir.GetFiles("*.ttc"));

            if(cols.Count > 0)
            {
                for(int i = 0; i < cols.Count; i++)
                {
                    TTFRef[] all = LoadCollectionRefs(cols[i]);
                    if(null != all)
                    {
                        foreach (var tref in all)
                        {
                            System.Diagnostics.Debug.WriteLine("Loaded Font : " + tref.GetFullName());
                            matched.Add(tref);
                        }
                    }
                }
            }


            return matched.ToArray();
        }

        #endregion

        #region public static TTFRef LoadRef(string path)

        /// <summary>
        /// Loads a specific font reference from the specified path. Will return null if the file does not exist.
        /// If the file cannot be loaded, then it will return a TTFRef with it's invalid flag set, and error message set appropriately.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TTFRef LoadRef(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            return LoadRef(new System.IO.FileInfo(path));
        }

        #endregion

        #region public static TTFRef LoadRef(System.IO.FileInfo fi)

        /// <summary>
        /// Loads a specific font reference from the specified path in the file info. Will return null if the file does not exist.
        /// If the file cannot be loaded, then it will return a TTFRef with it's invalid flag set, and error message set appropriately.
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        public static TTFRef LoadRef(System.IO.FileInfo fi)
        {
            if (null == fi)
                throw new ArgumentNullException("fi");

            if (fi.Exists == false)
                return null;

            using (System.IO.FileStream fs = fi.OpenRead())
            {
                return LoadRef(fs, fi.FullName);
            }
        }

        #endregion

        #region public static TTFRef LoadRef(System.IO.FileStream fs, string fullpath)

        /// <summary>
        /// Loads a specific font reference from the specified filestream with the full path.
        /// If the file cannot be loaded, then it will return a TTFRef with it's invalid flag set, and error message set appropriately.
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static TTFRef LoadRef(System.IO.FileStream fs, string fullpath)
        {
            if (null == fs)
                throw new ArgumentNullException("fs");

            using (BigEndianReader reader = new BigEndianReader(fs))
            {
                return LoadRef(reader, fullpath);
            }
        }

        #endregion

        #region public static TTFRef LoadRef(BigEndianReader reader, string fullpath)

        /// <summary>
        /// Loads a specific font reference using the BigEndianReader and the specified file path.
        /// If the file cannot be loaded, then it will return a TTFRef with it's invalid flag set, and error message set appropriately.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static TTFRef LoadRef(BigEndianReader reader, string fullpath)
        {
            TTFRef loaded;
            try
            {
                loaded = DoLoadRef(reader,fullpath);
            }
            catch (Exception ex)
            {
                TTFRef invalid = new TTFRef(fullpath);
                invalid.IsValid = false;
                invalid.ErrorMessage = ex.Message;
                invalid.FamilyName = "INVALID FONT FILE";
                loaded = invalid;
            }

            return loaded;
        }

        #endregion

        private static TTFRef[] LoadCollectionRefs(string path)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            if (file.Exists)
                return LoadCollectionRefs(file);
            else
                return null;
        }

        private static TTFRef[] LoadCollectionRefs(System.IO.FileInfo fi)
        {
            using(var stream = new System.IO.FileStream(fi.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                return LoadCollectionRefs(stream, fi.FullName);
            }
        }

        private static TTFRef[] LoadCollectionRefs(System.IO.Stream stream, string fullPath)
        {
            using (BigEndianReader reader = new BigEndianReader(stream))
            {
                return LoadCollectionRefs(reader, fullPath);
            }
        }

        private static TTFRef[] LoadCollectionRefs(BigEndianReader reader, string fullPath)
        {
            TTCHeader ttc;
            List<TTFRef> refs = new List<TTFRef>();

            if (TTCHeader.TryReadHeader(reader, out ttc))
            {
                for (var f = 0; f < ttc.NumFonts; f++)
                {
                    reader.Position = ttc.FontOffsets[f];

                    var tref = DoLoadRef(reader, fullPath);
                    
                    if (null == tref)
                        continue;

                    tref.HeadOffset = (int)ttc.FontOffsets[f];
                    refs.Add(tref);
                }
            }
            return refs.ToArray();
        }

        /// <summary>
        /// Performs the actual reading and loading of the file with the BigEndian reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        private static TTFRef DoLoadRef(BigEndianReader reader, string fullpath)
        {
            TrueTypeHeader head;
            if (TrueTypeHeader.TryReadHeader(reader, out head) == false)
                return null;

            TrueTypeTableEntryList list = new TrueTypeTableEntryList();
            bool hasOs2 = false;
            bool hasFHead = false;
            bool hasName = false;

            for (int i = 0; i < head.NumberOfTables; i++)
            {
                TrueTypeTableEntry dir = new TrueTypeTableEntry();
                dir.Read(reader);
                list.Add(dir);
                if (dir.Tag == Const.OS2Table)
                    hasOs2 = true;
                else if (dir.Tag == Const.FontHeaderTable)
                    hasFHead = true;
                else if (dir.Tag == Const.NameTable)
                    hasName = true;
            }


            TrueTypeTableFactory fact = (head.Version as TrueTypeVersionReader).GetTableFactory();


            SubTables.NamingTable ntable = null;

            if (hasName)
                ntable = fact.ReadTable(Const.NameTable, list, reader) as SubTables.NamingTable;
            else
                throw new ArgumentNullException("The required '" + TrueTypeTableNames.NamingTable + "' is not present in this font file. The OpenType file is corrupt");


            //if (fhead == null)
            //    throw new ArgumentNullException("The required '" + FontHeaderTable + "' is not present in this font file. The OpenType file is corrupt");



            TTFRef ttfref = new TTFRef(fullpath);
            NameEntry entry;
            if (ntable.Names.TryGetEntry(Const.FamilyNameID, out entry))
            {
                ttfref.FamilyName = entry.ToString();
            }

            if (hasOs2)
            {
                SubTables.OS2Table os2table = fact.ReadTable(TrueTypeTableNames.WindowsMetrics, list, reader) as SubTables.OS2Table;
                ttfref.FontRestrictions = os2table.FSType;
                ttfref.FontWidth = os2table.WidthClass;
                ttfref.FontWeight = os2table.WeightClass;
                ttfref.FontSelection = os2table.Selection;
            }
            else if (hasFHead)
            {
                SubTables.FontHeader fhead = fact.ReadTable(TrueTypeTableNames.FontHeader, list, reader) as SubTables.FontHeader;
                var mac = fhead.MacStyle;
                ttfref.FontRestrictions = FontRestrictions.InstallableEmbedding;
                ttfref.FontWeight = WeightClass.Normal;

                if ((mac & FontStyleFlags.Condensed) > 0)
                    ttfref.FontWidth = WidthClass.Condensed;

                else if ((mac & FontStyleFlags.Extended) > 0)
                    ttfref.FontWidth = WidthClass.Expanded;

                ttfref.FontSelection = 0;
                if ((mac & FontStyleFlags.Italic) > 0)
                    ttfref.FontSelection |= FontSelection.Italic;

                if ((mac & FontStyleFlags.Bold) > 0)
                {
                    ttfref.FontSelection |= FontSelection.Bold;
                    ttfref.FontWeight = WeightClass.Bold;
                }
                if ((mac & FontStyleFlags.Outline) > 0)
                    ttfref.FontSelection |= FontSelection.Outlined;

                if ((mac & FontStyleFlags.Underline) > 0)
                    ttfref.FontSelection |= FontSelection.Underscore;
            }
            else
                throw new ArgumentNullException("The required '" + Const.OS2Table + "' or '" + Const.FontHeaderTable + " are not present in this font file. The OpenType file is corrupt");


            return ttfref;

        }


        
    }
}
