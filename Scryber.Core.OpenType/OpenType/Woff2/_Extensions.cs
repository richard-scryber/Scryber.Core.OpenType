using System;
namespace Scryber.OpenType.Woff2
{
    public static class BigEndianReaderExtensions
    {

        /// <summary>
        /// Extension method that attempts to read a variable length encoding of unsigned integers for values up to 2^32 - 1
        /// </summary>
        /// <param name="reader">The bigendian reader to get the value from at the current position</param>
        /// <param name="result">Set to the value read if successful otherwise 0</param>
        /// <returns>True if the value could be read, outherwise false</returns>
        /// <remarks>
        /// A UIntBase128 encoded number is a sequence of bytes for which the most significant bit
        /// is set for all but the last byte,
        /// and clear for the last byte.
        /// The number itself is base 128 encoded in the lower 7 bits of each byte.
        /// Thus, a decoding procedure for a UIntBase128 is: 
        /// start with value = 0.
        /// Consume a byte, setting value = old value times 128 + (byte bitwise - and 127).
        /// Repeat last step until the most significant bit of byte is false.
        /// </remarks>
        public static bool TryReadUIntBase128(this BigEndianReader reader, out uint result)
        {
            uint value = 0;
            result = 0;

            for(var i = 0; i < 5; i++)
            {
                var b = reader.ReadByte();

                //No leading 0's
                if (i == 0 && b == 0x80)
                    return false;

                //Overflow check
                else if ((value & 0xFE000000) != 0)
                    return false;

                //shift 7 bits and add the current byte
                value = (uint)(value << 7) | (uint)(b & 0x7F);

                //If most significant bit is 0 we are done.
                if((b & 0x80) == 0)
                {
                    result = value;
                    return true;
                }    
            }
            // sequence exceeds 5 bytes
            return false;
        }



        /// <summary>
        /// Extension method that reads a variable length encoding of unsigned integers for values up to 2^32 - 1
        /// </summary>
        /// <param name="reader">The bigendian reader to get the value from at the current position</param>
        /// <returns>if the value could be read, outherwise 0</returns>
        /// <remarks>
        /// A UIntBase128 encoded number is a sequence of bytes for which the most significant bit
        /// is set for all but the last byte,
        /// and clear for the last byte.
        /// The number itself is base 128 encoded in the lower 7 bits of each byte.
        /// Thus, a decoding procedure for a UIntBase128 is: 
        /// start with value = 0.
        /// Consume a byte, setting value = old value times 128 + (byte bitwise - and 127).
        /// Repeat last step until the most significant bit of byte is false.
        /// </remarks>
        public static uint ReadUIntBase128(this BigEndianReader reader)
        {
            uint value = 0;

            for (var i = 0; i < 5; i++)
            {
                var b = reader.ReadByte();

                //No leading 0's on the first byte
                if (i == 0 && b == 0x80)
                    return 0;

                //Overflow check
                else if ((value & 0xFE000000) != 0)
                    return 0;

                //shift 7 bits and add the current byte
                value = (uint)(value << 7) | (uint)(b & 0x7F);

                //If most significant bit is 0 we are done.
                if ((b & 0x80) == 0)
                {
                    return value;
                }
            }
            // sequence exceeds 5 bytes
            return 0;
        }

        const byte ONE_MORE_BYTE_CODE1 = 255;
        const byte ONE_MORE_BYTE_CODE2 = 254;
        const byte WORD_CODE = 253;
        const byte LOWEST_UCODE = 253;

        /// <summary>
        /// Extension methad that reads a variable length encoded 16-bit unsigned integer
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>The decoded ushort value</returns>
        /// <remarks>255UInt16 is a variable-length encoding of an unsigned integer 
        /// in the range 0 to 65535 inclusive.
        /// This data type is intended to be used as intermediate representation of various font values,
        /// which are typically expressed as UInt16 but represent relatively small values.
        /// Depending on the encoded value, the length of the data field may be one to three bytes,
        /// where the value of the first byte either represents the small value itself or is treated as a code that defines the format of the additional byte(s).
        /// </remarks>
        public static ushort Read255UInt16(this BigEndianReader reader)
        {
            byte code = reader.ReadByte();
            if(code == WORD_CODE)
            {
                /* Read two more bytes and concatenate them to form UInt16 value*/
                int value = reader.ReadByte();
                value <<= 8;
                value &= 0xff00;

                int value2 = reader.ReadByte();
                value |= value2 & 0x00ff;

                return (ushort)value;
            }
            else if(code == ONE_MORE_BYTE_CODE1)
            {
                ushort value = (ushort)(reader.ReadByte());
                value += LOWEST_UCODE;

                return value;
            }
            else if(code == ONE_MORE_BYTE_CODE2)
            {
                ushort value = (ushort)(reader.ReadByte());
                value += (ushort)(LOWEST_UCODE * 2);

                return value;
            }
            else
            {
                return code;
            }

        }

    }
}
