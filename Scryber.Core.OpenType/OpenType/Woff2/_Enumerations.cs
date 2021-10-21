using System;
namespace Scryber.OpenType.Woff2
{
    public enum KnownTableIndex : int
    {
        Cmap = 0,
        Head = 1,
        HHead = 2,
        HMtx = 3,
        MaxP = 4,
        Name = 5,
        OS2 = 6,
        Post = 7,
        Cvt = 8,
        Fpgm = 9,
        Glyf = 10,
        Loca = 11,
        Prep = 12,
        CFF = 13,
        Vorg = 14,
        EBDT = 15,
        EBLC = 16,
        Gasp = 17,
        HDmx = 18,
        Kern = 19,
        LTSH = 20,
        PCLT = 21,
        VDmx = 22,
        VHea = 23,
        VMtx = 24,
        BASE = 25,
        GDEF = 26,
        GPos = 27,
        GSub = 28,
        EBSC = 29,
        JSTF = 30,
        MATH = 31,
        CBDT = 32,
        CBLC = 33,
        COLR = 34,
        CPAL = 35,
        SVG_ = 36,
        sbix = 37,
        acnt = 38,
        avar = 39,
        bdat = 40,
        bloc = 41,
        bsln = 42,
        cvar = 43,
        fdsc = 44,
        feat = 45,
        fmtx = 46,
        fvar = 47,
        gvar = 48,
        hsty = 49,
        just = 50,
        lcar = 51,
        mort = 52,
        morx = 53,
        opbd = 54,
        prop = 55,
        trak = 56,
        Zapf = 57,
        Silf = 58,
        Glat = 59,
        Gloc = 60,
        Feat = 61,
        Sill = 62,
        UNKN = 63
    }

    [Flags]
    internal enum SimpleGlyphFlag : byte
    {
        OnCurve = 1,
        XByte = 1 << 1,
        YByte = 1 << 2,
        Repeat = 1 << 3,
        XSignOrSame = 1 << 4,
        YSignOrSame = 1 << 5
    }


    [Flags]
    internal enum CompositeGlyphFlags : ushort
    {
        //These are the constants for the flags field:
        //Bit   Flags 	 	            Description
        //0     ARG_1_AND_2_ARE_WORDS  	If this is set, the arguments are words; otherwise, they are bytes.
        //1     ARGS_ARE_XY_VALUES 	  	If this is set, the arguments are xy values; otherwise, they are points.
        //2     ROUND_XY_TO_GRID 	  	For the xy values if the preceding is true.
        //3     WE_HAVE_A_SCALE 	 	This indicates that there is a simple scale for the component. Otherwise, scale = 1.0.
        //4     RESERVED 	        	This bit is reserved. Set it to 0.
        //5     MORE_COMPONENTS 	    Indicates at least one more glyph after this one.
        //6     WE_HAVE_AN_X_AND_Y_SCALE 	The x direction will use a different scale from the y direction.
        //7     WE_HAVE_A_TWO_BY_TWO 	  	There is a 2 by 2 transformation that will be used to scale the component.
        //8     WE_HAVE_INSTRUCTIONS 	 	Following the last component are instructions for the composite character.
        //9     USE_MY_METRICS 	 	        If set, this forces the aw and lsb (and rsb) for the composite to be equal to those from this original glyph. This works for hinted and unhinted characters.
        //10    OVERLAP_COMPOUND 	 	    If set, the components of the compound glyph overlap. Use of this flag is not required in OpenType — that is, it is valid to have components overlap without having this flag set. It may affect behaviors in some platforms, however. (See Apple’s specification for details regarding behavior in Apple platforms.)
        //11    SCALED_COMPONENT_OFFSET 	The composite is designed to have the component offset scaled.
        //12    UNSCALED_COMPONENT_OFFSET 	The composite is designed not to have the component offset scaled.

        ARG_1_AND_2_ARE_WORDS = 1,
        ARGS_ARE_XY_VALUES = 1 << 1,
        ROUND_XY_TO_GRID = 1 << 2,
        WE_HAVE_A_SCALE = 1 << 3,
        RESERVED = 1 << 4,
        MORE_COMPONENTS = 1 << 5,
        WE_HAVE_AN_X_AND_Y_SCALE = 1 << 6,
        WE_HAVE_A_TWO_BY_TWO = 1 << 7,
        WE_HAVE_INSTRUCTIONS = 1 << 8,
        USE_MY_METRICS = 1 << 9,
        OVERLAP_COMPOUND = 1 << 10,
        SCALED_COMPONENT_OFFSET = 1 << 11,
        UNSCALED_COMPONENT_OFFSET = 1 << 12
    }
}
