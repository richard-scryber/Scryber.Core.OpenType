//#define UseOpenFont
//#define Performance
#define UseLocal

using System;
using System.Threading.Tasks;
using Scryber.OpenType;
using Scryber.OpenType.SubTables;
//using Typography.OpenFont.WebFont;
using System.IO;
//using Typography.OpenFont.Extensions;

namespace Scryber.Core.OpenType.Tests
{
    class Program
    {
        static bool StopExecution = false;

        static void Main(string[] args)
        {
            var fonts = new[]
            {
                new { Name = "Hachi Maru Pop",      Include = false, LocalPath = "./fonts/HachiMaruPop.ttf",  RemotePath = "https://fonts.gstatic.com/s/hachimarupop/v2/HI_TiYoRLqpLrEiMAuO9Ysfz7rW1.ttf"},
                new { Name = "Roboto",              Include = false, LocalPath = "./fonts/Roboto.ttf", RemotePath = "https://fonts.gstatic.com/s/roboto/v20/KFOmCnqEu92Fr1Me5Q.ttf"},
                new { Name = "Open Sans Black TTF", Include = false, LocalPath = "./fonts/Open Sans Black.ttf", RemotePath = "https://fonts.gstatic.com/s/opensans/v26/memQYaGs126MiZpBA-UFUIcVXSCEkx2cmqvXlWq8tWZ0Pw86hd0Rk0ZjaVc.ttf"},
                new { Name = "Pragati Narrow",      Include = false, LocalPath = "./fonts/PragatiNarrow.ttf", RemotePath = "https://fonts.gstatic.com/s/pragatinarrow/v8/vm8vdRf0T0bS1ffgsPB7WZ-mD17_.ttf"},   
                new { Name = "Helvetica",           Include = false, LocalPath = "./fonts/Helvetica.ttf", RemotePath = "https://raw.githubusercontent.com/richard-scryber/scryber.core/svgParsing/Scryber.Drawing/Text/_FontResources/Helvetica/Helvetica.ttf"},
                new { Name = "Gill Sans ttc",       Include = false, LocalPath = "./fonts/GillSans.ttc", RemotePath = "https://raw.githubusercontent.com/richard-scryber/scryber.core.opentype/master/Scryber.Core.OpenType.Tests/fonts/GillSans.ttc"},
                new { Name = "Open Sans Black Wof", Include = false, LocalPath = "./fonts/OpenSansBlack.woff", RemotePath = "https://fonts.gstatic.com/s/opensans/v26/memQYaGs126MiZpBA-UFUIcVXSCEkx2cmqvXlWq8tWZ0Pw86hd0Rk0ZjWVAexoMUdjFXmQ.woff"},
                new { Name = "Noto TC",             Include = false,  LocalPath = "./fonts/NotoTC.otf", RemotePath = "https://fonts.gstatic.com/s/notosanstc/v20/-nF7OG829Oofr2wohFbTp9iFOQ.otf"},
                new { Name = "Festive",             Include = true, LocalPath = "./fonts/Festive.woff2", RemotePath = "https://fonts.gstatic.com/s/festive/v1/cY9Ffj6KX1xcoDWhJt_qyvPQgah_Lw.woff2"}
            };

            var path = AppContext.BaseDirectory;
            
            FontDownload loader = new FontDownload(path);

            foreach (var item in fonts)
            {

                try
                {

#if UseLocal
                    path = item.LocalPath;
#else
                    path = item.RemotePath;
#endif
                    if (item.Include && !string.IsNullOrEmpty(path))
                    {
                        Console.WriteLine("Loading the font for " + item.Name + " from " + path);
                        MeasureStringFor(loader, path).Wait();
                    }
                    else
                        Console.WriteLine("Skipped " + item.Name);

                    Console.WriteLine();
                    if(StopExecution)
                    {
                        Console.WriteLine("Stopping...");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    ExitClean("Could not measure : " + item.Name + ":" + ex.Message);

                    if (StopExecution == true)
                        break;
                }
            }



            loader.Dispose();

        }

        private async static Task MeasureStringFor(FontDownload loader, string path)
        {
            byte[] data = null;

            data = await loader.LoadFrom(path);


#if UseOpenFont

            ZlibDecompressStreamFunc zipfunc = (byte[] dataIn, byte[] output) =>
            {
                using (var streamIn = new MemoryStream(dataIn))
                {
#if NET6_0
                    ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream input = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(streamIn);
                    
                    using (var streamOut = new MemoryStream(output))
                        input.CopyTo(streamOut);

#endif
                    return true;
                }

            };
            WoffDefaultZlibDecompressFunc.DecompressHandler = zipfunc;

            BrotliDecompressStreamFunc brotlifunc = (byte[] dataIn, Stream dataOut) =>
            {
                using (var streamIn = new MemoryStream(dataIn))
                {
                    System.IO.Compression.BrotliStream deflate = new System.IO.Compression.BrotliStream(streamIn, System.IO.Compression.CompressionMode.Decompress);

                    deflate.CopyTo(dataOut);

                    return true;
                }
            };
            Woff2DefaultBrotliDecompressFunc.DecompressHandler = brotlifunc;


            using (var ms = new System.IO.MemoryStream(data))
            {
                var reader = new Typography.OpenFont.OpenFontReader();
                var preview = reader.ReadPreview(ms);

                Console.WriteLine("Loaded font reference " + preview.Name);
            }

            using (var ms = new System.IO.MemoryStream(data))
            {
                var reader = new Typography.OpenFont.OpenFontReader();
                var full = reader.Read(ms);
                string text = "This is the text to measure";
                var encoding = Scryber.OpenType.SubTables.CMapEncoding.WindowsUnicode;
                int fitted;
                var size = full.MeasureString(text, 0, 12, 10000, true, out fitted);


                Console.WriteLine("String Measured to " + size.ToString() + " and fitted " + fitted + " characters out of " + text.Length);

            }

#else

            TypefaceReader tfreader = new TypefaceReader();
            ITypefaceInfo info;

            using (var ms = new System.IO.MemoryStream(data))
            {
                info = tfreader.GetTypefaceInformation(ms, path);
                if(null == info)
                {
                    ExitClean("Could not read the info from the font file");
                    return;
                }
                else if(info.FontCount == 0)
                {
                    ExitClean("No fonts could be read from the data: " + info.ErrorMessage ?? "Unknown error");
                    return;
                }
                else
                {
                    Console.WriteLine("Read  " + info.FontCount + " typefaces from the font file " + info.Source);
                    foreach (var reference in info.Fonts)
                    {
                        Console.WriteLine("    " + reference.FamilyName + " (weight: " + reference.FontWeight.ToString() + ", width: " + reference.FontWidth + ", restrictions : " + reference.Restrictions + ", selections : " + reference.Selections.ToString() + ")");
                    }
                }

            }
            TypeMeasureOptions options = new TypeMeasureOptions();

            using (var ms = new System.IO.MemoryStream(data))
            {

                foreach (var fref in info.Fonts)
                {
                    Console.WriteLine();

                    ms.Position = 0;
                    var typeface = tfreader.GetTypeface(ms, info.Source, fref);

                    if (null == typeface || typeface.IsValid == false)
                    {
                        ExitClean("The font " + fref.ToString() + " was not valid");
                        return;
                    }
                    else
                        Console.WriteLine("Loaded font reference " + typeface.ToString());

                    var metrics = typeface.GetMetrics();

                    if (null != metrics)
                    {
                        var line = metrics.Measure("This is the text to measure", 0, 12.0, 10000, options);
                        Console.WriteLine("Measured the string to " + line.RequiredWidth + ", " + line.RequiredHeight + " points, and fitted " + line.CharsFitted + " chars" + (line.OnWordBoudary ? " breaking on the word boundary." : "."));
                    }
                    else
                    {
                        ExitClean("No metrics were returned for the font " + typeface.ToString());
                        return;
                    }

                    var ttfData = typeface.GetFileData(DataFormat.TTF);
                    if(null == ttfData)
                    {
                        ExitClean("No data was returned in TTF format for the font " + typeface.ToString());
                    }
                    else
                    {
                        Console.WriteLine("TrueType font data was extracted at " + ttfData.Length + " bytes from the original data of " + data.Length);
                    }

                    var name = typeface.FamilyName;
                    if (typeface.FontWeight != WeightClass.Normal)
                    {
                        if (name.IndexOf(typeface.FontWeight.ToString()) < 0)
                            name += " " + typeface.FontWeight.ToString();
                    }
                    if((typeface.Selections & FontSelection.Italic) > 0)
                    {
                        if (name.IndexOf("Italic") < 0)
                            name += " Italic";
                    }

                    loader.SaveToLocal("Output", name + ".ttf", ttfData);

                    
                }
            }

#if Legacy
            var ttf = new Scryber.OpenType.TTFFile(data, 0);

            Console.WriteLine("Loaded font file " + ttf.ToString());

            var encoding = Scryber.OpenType.SubTables.CMapEncoding.WindowsUnicode;
            string text = "This is the text to measure";
            int fitted;

            var size = ttf.MeasureString(encoding, text, 0, 12, 10000, true, out fitted);

            Console.WriteLine("String Measured to " + size.ToString() + " and fitted " + fitted + " characters out of " + text.Length);

#endif

#if Performance

            var stopWatch = Stopwatch.StartNew();

            MeasureStringMeasurer(ttf);

            stopWatch.Stop();

            Console.WriteLine("To measure 4 different strings " + maxRepeat + " times took " + stopWatch.Elapsed.TotalMilliseconds + "ms");
#endif

#endif

        }

        const int maxRepeat = 100000;

        static void MeasureStringsUsual(TTFFile file)
        {
            


            for(var repeat = 0; repeat < maxRepeat; repeat++)
            {
                for (var index = 0; index < AllToMeasure.Length; index++)
                {
                    var str = AllToMeasure[index];
                    int fitted;
                    var encoding = CMapEncoding.WindowsUnicode;
                    var size = file.MeasureString(encoding, str, 0, 12.0, 5061, false, out fitted);


                    if (fitted != AllFitted[index])
                        throw new InvalidOperationException("The measured number of characters " + fitted + " was not the same as the expected result " + AllFitted[index] + " for string " + index);
                }

            }

        }

        static void MeasureStringMeasurer(TTFFile file)
        {
            var measurer = Scryber.OpenType.TTF.TTFStringMeasurer.Create(file, CMapEncoding.WindowsUnicode);

            for (var repeat = 0; repeat < maxRepeat; repeat++)
            {
                for (var index = 0; index < AllToMeasure.Length; index++)
                {
                    var str = AllToMeasure[index];
                    int fitted;
                    var size = measurer.MeasureChars(str, 0, 12.0, 5061, false, out fitted); 

                    if (fitted != AllFitted[index])
                        throw new InvalidOperationException("The measured number of characters " + fitted + " was not the same as the expected result " + AllFitted[index] + " for string " + index);
                }

            }



        }

        static bool ExitClean(string message, bool error = true)
        {
            if (error)
                Console.WriteLine("AN ERROR OCCURRED");
            if (!string.IsNullOrEmpty(message))
                Console.WriteLine(message);

            Console.WriteLine();
            Console.Write("Press Y to contine, anything else to exit: ");

            var key = Console.ReadKey();
            if (key.Key != ConsoleKey.Y)
                StopExecution = true;

            Console.WriteLine();

            return error;
        }

        static int[] AllFitted = new int[]
        {
            12,
            56,
            167,
            273,
            584
        };

        static string[] AllToMeasure = new string[]
        {
            "First String",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas scelerisque porttitor urna. Duis pellentesque sem tempus magna faucibus, quis lobortis magna aliquam.",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas scelerisque porttitor urna. Duis pellentesque sem tempus magna faucibus, quis lobortis magna aliquam. Nullam eu risus facilisis sapien fermentum condimentum. Pellentesque ut placerat diam, sed suscipit nibh.",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas scelerisque porttitor urna. Duis pellentesque sem tempus magna faucibus, quis lobortis magna aliquam. Nullam eu risus facilisis sapien fermentum condimentum. Pellentesque ut placerat diam, sed suscipit nibh. Integer dictum dolor vel finibus imperdiet. Orci varius natoque penatibus et magnis disparturient montes, nascetur ridiculus mus. Integer congue turpis at varius porttitor. nec faucibus ipsum bibendum sed. Nunc tristique risus eu quam porttitor blandit. In erat mauris, imperdiet a venenatis eu, tempus a nunc."
        };
    }
}
