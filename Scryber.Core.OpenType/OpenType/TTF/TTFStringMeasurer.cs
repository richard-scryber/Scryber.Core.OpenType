using System;
using System.Collections.Generic;

using Scryber.OpenType.SubTables;

namespace Scryber.OpenType.TTF
{

    public delegate double Measurer(string chars, int startOffset, double emsize, double available, bool wordboundary, out int fitted);

    public class TTFStringMeasurer : ITypefaceMetrics
    {

        private CMAPSubTable _offsets;
        private List<HMetric> _metrics;
        private int unitsPerEm;
        private int _lineHeight;
        private TrueTypeFile _fontfile;
        private CMapEncoding _cMapEncoding;
        private Dictionary<char, HMetric> _lookup;

        private TTFStringMeasurer(int unitsPerEm, CMAPSubTable offsets, OS2Table oS2, List<HMetric> metrics, TrueTypeFile font, CMapEncoding encoding, int lineheight)
        {
            this.unitsPerEm = unitsPerEm;
            this._offsets = offsets;
            this._metrics = metrics;
            this._lookup = new Dictionary<char, HMetric>();
            _lineHeight = lineheight;
            this._fontfile = font;
            this._cMapEncoding = encoding;
        }

        public double MeasureChars(string chars, int startOffset, double emsize, double available, bool wordboundary, out int fitted)
        {
            int totalUnits = (int)((available / emsize) * this.unitsPerEm);

            int measured = 0;
            int count = 0;

            int lastWordLen = 0;
            int lastWordCount = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];

                if(char.IsWhiteSpace(c))
                {
                    lastWordLen = measured;
                    lastWordCount = count;
                }
                HMetric metric;

                if (_lookup.TryGetValue(c, out metric) == false)
                {
                    int moffset = _offsets.GetCharacterGlyphOffset(c);

                    if (moffset >= _metrics.Count)
                        moffset = _metrics.Count - 1;

                    metric = _metrics[moffset];
                    _lookup.Add(c, metric);
                }

                if (i == 0)
                    measured -= metric.LeftSideBearing;

                measured += metric.AdvanceWidth;

                if (measured > totalUnits)
                {
                    lastWordLen -= metric.AdvanceWidth;
                    break;
                }
                else
                    count++;
            }

            if(count + startOffset < chars.Length)
            {
                if (wordboundary && lastWordLen > 0)
                {
                    //Not everything fitted so go bask to the last full word
                    //(if there was one).
                    measured = lastWordLen;
                    count = lastWordCount;
                }
            }

            double w = (measured * emsize) / (double)unitsPerEm;
            fitted = count;
            return w;

        }

        LineSize ITypefaceMetrics.Measure(string chars, int startOffset, double emSize, double maxWidth, TypeMeasureOptions options)
        {
            int fitted;

            //If we have spacing, we cannot take advantage of caching, so fall back to the main font file measure
            if (options.WordSpacing.HasValue || options.CharacterSpacing.HasValue)
                return _fontfile.MeasureString(this._cMapEncoding, chars, startOffset, emSize, maxWidth, options.WordSpacing, options.CharacterSpacing.HasValue ? options.CharacterSpacing.Value : TrueTypeFile.NoCharacterSpace, TrueTypeFile.NoHorizontalScale, false, options.BreakOnWordBoundaries, out fitted);
            else
            {
                double required = this.MeasureChars(chars, startOffset, emSize, maxWidth, options.BreakOnWordBoundaries, out fitted);
                double lineh = ((double)this._lineHeight * emSize) / (double)unitsPerEm;

                return new LineSize(required, lineh, fitted, (fitted < chars.Length) && options.BreakOnWordBoundaries);
            }
        }


        public static TTFStringMeasurer Create(TrueTypeFile forfont, CMapEncoding encoding)
        {
            HorizontalMetrics table = forfont.Directories["hmtx"].Table as HorizontalMetrics;
            CMAPTable cmap = forfont.Directories["cmap"].Table as CMAPTable;
            OS2Table os2 = forfont.Directories["OS/2"].Table as OS2Table;
            FontHeader head = forfont.Directories["head"].Table as FontHeader;
            HorizontalHeader hhead = forfont.Directories["hhea"].Table as HorizontalHeader;

            CMAPSubTable map = cmap.GetOffsetTable(encoding);
            if (map == null)
            {
                encoding = CMapEncoding.Unicode_20;
                map = cmap.GetOffsetTable(CMapEncoding.Unicode_20);
            }

            if (map == null)
            {
                encoding = CMapEncoding.MacRoman;
                map = cmap.GetOffsetTable(CMapEncoding.MacRoman);
            }
            
            int lineHeight;
            bool useTypo = (FontSelection.UseTypographicSizes & os2.Selection) > 0;

            if (useTypo)
                lineHeight = os2.TypoAscender - os2.TypoDescender + os2.TypoLineGap;
            else
                lineHeight = hhead.Ascender - hhead.Descender + hhead.LineGap; 


            return new TTFStringMeasurer(head.UnitsPerEm, map, os2, table.HMetrics, forfont, encoding, lineHeight);
        }
    }

    
}
