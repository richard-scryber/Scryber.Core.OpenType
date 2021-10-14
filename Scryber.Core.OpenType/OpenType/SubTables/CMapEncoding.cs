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
using System.Linq;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    /// <summary>
    /// Encapsulates the Platform and Character encoding options for a cmap table
    /// </summary>
    /// <remarks>
    /// Use the factory methods - Mac(), Windows(), Unicode() to create specific instances of this encodings
    /// </remarks>
    public struct CMapEncoding : IEquatable<CMapEncoding>, IComparable<CMapEncoding>
    {

        #region public CharacterPlatforms Platform { get; set; }

        /// <summary>
        /// One of the standard character encoding types
        /// </summary>
        public CharacterPlatforms Platform { get; set; }

        #endregion

        #region public ushort Encoding { get; set; }

        /// <summary>
        /// One of the values from either the Windows, Unicode or Mac Character Encodings
        /// </summary>
        public ushort Encoding { get; set; }

        #endregion

        //
        // .ctors
        //

        #region public CMapEncoding(CharacterPlatforms platform, ushort encoding)

        /// <summary>
        /// Creates a new CMapEncoding with the required platform and encoding
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="encoding"></param>
        public CMapEncoding(CharacterPlatforms platform, ushort encoding)
            : this()
        {
            this.Platform = platform;
            this.Encoding = encoding;
        }

        #endregion

        //
        // methods
        //

        #region public override int GetHashCode()
        /// <summary>
        /// Returns a unique integer value based on the combination of Platform and Encoding
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (((int)Platform) * 100) + (int)Encoding;
        }

        #endregion

        #region public override bool Equals(object obj)

        /// <summary>
        /// Returns true if the obj is the same character encoding as this instance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            return this.Equals((CMapEncoding)obj);
        }

        #endregion

        #region public bool Equals(CMapEncoding enc)

        /// <summary>
        /// Returns true is this CMapEncoding is the same platform and encoding as the provided encoding
        /// </summary>
        /// <param name="enc"></param>
        /// <returns></returns>
        public bool Equals(CMapEncoding enc)
        {
            return this.Platform == enc.Platform && this.Encoding == enc.Encoding;
        }

        #endregion

        #region public int CompareTo(CMapEncoding enc)

        /// <summary>
        /// Compares this CMapEncoding to the passed encoding
        /// </summary>
        /// <param name="enc"></param>
        /// <returns></returns>
        public int CompareTo(CMapEncoding enc)
        {
            return this.GetHashCode().CompareTo(enc.GetHashCode());
        }

        #endregion

        #region public override string ToString()

        /// <summary>
        /// Overrides the base implementation to return a string 
        /// representation of this encoding
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string charEnc;
            switch (this.Platform)
            {
                case CharacterPlatforms.Unicode:
                    charEnc = ((UnicodeCharacterEncodings)this.Encoding).ToString();
                    break;
                case CharacterPlatforms.Macintosh:
                    charEnc = ((MacCharacterEncodings)this.Encoding).ToString();
                    break;
               case CharacterPlatforms.Windows:
                    charEnc = ((WindowsCharacterEncodings)this.Encoding).ToString();
                    break;
                 case CharacterPlatforms.Other:
                default:
                    charEnc = "Unsupported";
                    break;
            }
            return Platform.ToString() + ", " + charEnc;
        }

        #endregion

        //
        // operators
        //

        public static bool operator ==(CMapEncoding enc1, CMapEncoding enc2)
        {
            return enc1.Equals(enc2);
        }

        public static bool operator !=(CMapEncoding enc1, CMapEncoding enc2)
        {
            return !enc1.Equals(enc2);
        }
    

        //
        // factory methods
        //

        public static CMapEncoding Mac(MacCharacterEncodings enc)
        {
            return new CMapEncoding(CharacterPlatforms.Macintosh, (ushort)enc);
        }

        public static CMapEncoding Unicode(UnicodeCharacterEncodings enc)
        {
            return new CMapEncoding(CharacterPlatforms.Unicode, (ushort)enc);
        }

        public static CMapEncoding Windows(WindowsCharacterEncodings enc)
        {
            return new CMapEncoding(CharacterPlatforms.Windows, (ushort)enc);
        }

        //
        // Standard types
        //


        /// <summary>
        /// Standard unicode default cmap = 0.0
        /// </summary>
        public static readonly CMapEncoding UnicodeDefault = Unicode(UnicodeCharacterEncodings.Default);

        /// <summary>
        /// Unicode with the 2.0 or later semantics
        /// </summary>
        public static readonly CMapEncoding Unicode_20 = Unicode(UnicodeCharacterEncodings.Unicode_20);

        /// <summary>
        /// Standard Macintosh roman camp = 1.0
        /// </summary>
        public static readonly CMapEncoding MacRoman = Mac(MacCharacterEncodings.Roman);

        /// <summary>
        /// Refers to the windows unicode cmap = 3.1
        /// </summary>
        public static readonly CMapEncoding WindowsUnicode = Windows(WindowsCharacterEncodings.Unicode);


    }
}
