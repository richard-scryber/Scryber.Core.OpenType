using System;
using System.Collections.Generic;

namespace Scryber.OpenType.Woff2
{
    public class Woff2GlyphTransform
    {
        private struct TempGlyph
        {
            public readonly ushort glyphIndex;
            public readonly short numContour;

            public ushort instructionLen;
            public bool compositeHasInstructions;

            public TempGlyph(ushort glyphIndex, short contourCount)
            {
                this.glyphIndex = glyphIndex;
                this.numContour = contourCount;

                instructionLen = 0;
                compositeHasInstructions = false;
            }

            public override string ToString()
            {
                return glyphIndex + " " + numContour;
            }

        }

        public Woff2GlyphTransform()
        {
        }


        public void Transform(BigEndianReader reader)
        {

            //the glyf table is split into several substreams, to group like data together. 

            //The transformed table consists of a number of fields specifying the size of each of the substreams,
            //followed by the substreams in sequence.

            //During the decoding process the reverse transformation takes place,
            //where data from various separate substreams are recombined to create a complete glyph record
            //for each entry of the original glyf table.

            //Transformed glyf Table

            //Data-Type Semantic                Description and value type(if applicable)
            //Fixed     version                 = 0x00000000
            //UInt16    numGlyphs               Number of glyphs
            //UInt16    indexFormatOffset      format for loca table, 
            //                                 should be consistent with indexToLocFormat of 
            //                                 the original head table(see[OFF] specification)

            //UInt32    nContourStreamSize      Size of nContour stream in bytes
            //UInt32    nPointsStreamSize       Size of nPoints stream in bytes
            //UInt32    flagStreamSize          Size of flag stream in bytes
            //UInt32    glyphStreamSize         Size of glyph stream in bytes(a stream of variable-length encoded values, see description below)
            //UInt32    compositeStreamSize     Size of composite stream in bytes(a stream of variable-length encoded values, see description below)
            //UInt32    bboxStreamSize          Size of bbox data in bytes representing combined length of bboxBitmap(a packed bit array) and bboxStream(a stream of Int16 values)
            //UInt32    instructionStreamSize   Size of instruction stream(a stream of UInt8 values)

            //Int16     nContourStream[]        Stream of Int16 values representing number of contours for each glyph record
            //255UInt16 nPointsStream[]         Stream of values representing number of outline points for each contour in glyph records
            //UInt8     flagStream[]            Stream of UInt8 values representing flag values for each outline point.
            //Vary      glyphStream[]           Stream of bytes representing point coordinate values using variable length encoding format(defined in subclause 5.2)
            //Vary      compositeStream[]       Stream of bytes representing component flag values and associated composite glyph data
            //UInt8     bboxBitmap[]            Bitmap(a numGlyphs-long bit array) indicating explicit bounding boxes
            //Int16     bboxStream[]            Stream of Int16 values representing glyph bounding box data
            //UInt8     instructionStream[]	    Stream of UInt8 values representing a set of instructions for each corresponding glyph


            uint version = reader.ReadUInt32();
            ushort numGlyphs = reader.ReadUInt16();
            ushort indexFormatOffset = reader.ReadUInt16();

            uint nContourStreamSize = reader.ReadUInt32(); //in bytes
            uint nPointsStreamSize = reader.ReadUInt32(); //in bytes
            uint flagStreamSize = reader.ReadUInt32(); //in bytes
            uint glyphStreamSize = reader.ReadUInt32(); //in bytes
            uint compositeStreamSize = reader.ReadUInt32(); //in bytes
            uint bboxStreamSize = reader.ReadUInt32(); //in bytes
            uint instructionStreamSize = reader.ReadUInt32(); //in bytes


            long expected_nCountStartAt = reader.BaseStream.Position;
            long expected_nPointStartAt = expected_nCountStartAt + nContourStreamSize;
            long expected_FlagStreamStartAt = expected_nPointStartAt + nPointsStreamSize;
            long expected_GlyphStreamStartAt = expected_FlagStreamStartAt + flagStreamSize;
            long expected_CompositeStreamStartAt = expected_GlyphStreamStartAt + glyphStreamSize;

            long expected_BboxStreamStartAt = expected_CompositeStreamStartAt + compositeStreamSize;
            long expected_InstructionStreamStartAt = expected_BboxStreamStartAt + bboxStreamSize;
            long expected_EndAt = expected_InstructionStreamStartAt + instructionStreamSize;

            Glyph[] glyphs = new Glyph[numGlyphs];

            TempGlyph[] allGlyphs = new TempGlyph[numGlyphs];
            List<ushort> compositeGlyphs = new List<ushort>();

            int contourCount = 0;

            for (ushort i = 0; i < numGlyphs; ++i)
            {
                short numContour = reader.ReadInt16();
                allGlyphs[i] = new TempGlyph(i, numContour);
                if (numContour > 0)
                {
                    contourCount += numContour;
                    //>0 => simple glyph
                    //-1 = compound
                    //0 = empty glyph
                }
                else if (numContour < 0)
                {
                    //composite glyph, resolve later
                    compositeGlyphs.Add(i);
                }
                else
                {

                }
            }

            ushort[] pntPerContours = new ushort[contourCount];
            for (int i = 0; i < contourCount; ++i)
            {
                // Each of these is the number of points of that contour.
                pntPerContours[i] = reader.Read255UInt16();
            }

            byte[] flagStream = reader.Read((int)flagStreamSize);

            //TODO: Read more from line 296 in OpenFoint.Woff2Reader


        }
    }
}
