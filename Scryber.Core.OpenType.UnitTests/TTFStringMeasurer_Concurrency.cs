using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    /// <summary>
    /// A measurer memoizes glyph metrics per character. Consumers cache one measurer per font and
    /// share it, so those memoized entries are written from several threads at once whenever
    /// documents are laid out in parallel.
    /// </summary>
    [TestClass]
    public class TTFStringMeasurer_Concurrency
    {
        //Deliberately wide, so a cold measurer takes many distinct characters and writes many
        //cache entries while the threads are running. A narrow alphabet fills the cache almost
        //immediately and hides the problem.
        private static readonly string[] Lines = new[]
        {
            "The quick brown fox jumps over the lazy dog",
            "Sphinx of black quartz, judge my vow",
            "Pack my box with five dozen liquor jugs",
            "How vexingly quick daft zebras jump",
            "Waltz, bad nymph, for quick jigs vex",
            "0123456789 !\"#$%&'()*+,-./:;<=>?@[]^_`{|}~",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "abcdefghijklmnopqrstuvwxyz",
        };

        private const double FontSize = 12.0;
        private const double Available = 1000.0;

        /// <summary>
        /// Regression test. The per-character metric cache used to be a plain Dictionary, so two
        /// threads inserting at once corrupted it. That surfaced as InvalidOperationException
        /// ("Operations that change non-concurrent collections must have exclusive access"),
        /// ArgumentException, or NullReferenceException raised from inside MeasureChars.
        /// </summary>
        [TestMethod("1. Measure concurrently on a shared measurer")]
        public void MeasureLine_SharedMeasurerIsSafeAcrossThreads()
        {
            var path = new FileInfo(Path.Combine(Environment.CurrentDirectory, ValidateHelvetica.UrlPath));

            //Several rounds, because the cache only takes writes while it is cold. Once every
            //character has been seen the calls are pure reads and the race can no longer happen,
            //so a single round with a warm measurer would pass even against the broken version.
            const int Rounds = 25;

            var errors = new ConcurrentQueue<string>();
            var threads = Math.Max(4, Environment.ProcessorCount);

            using (var reader = new TypefaceReader())
            {
                var font = reader.GetFirstFont(path);

                //Reference results, measured one at a time so nothing can interfere with them.
                var expected = new List<LineSize>();
                var reference = font.GetMetrics(TypeMeasureOptions.Default);

                foreach (var line in Lines)
                    expected.Add(reference.MeasureLine(line, 0, FontSize, Available, TypeMeasureOptions.Default));

                for (var round = 0; round < Rounds; round++)
                {
                    //A new measurer each round, so every round starts with an empty cache.
                    var metrics = font.GetMetrics(TypeMeasureOptions.Default);

                    Parallel.For(0, threads, new ParallelOptions { MaxDegreeOfParallelism = threads }, _ =>
                    {
                        for (var i = 0; i < Lines.Length; i++)
                        {
                            try
                            {
                                var size = metrics.MeasureLine(Lines[i], 0, FontSize, Available, TypeMeasureOptions.Default);

                                if (size.CharsFitted != expected[i].CharsFitted)
                                    errors.Enqueue("Fitted " + size.CharsFitted + " characters instead of " + expected[i].CharsFitted + ".");

                                else if (Math.Abs(size.RequiredWidth - expected[i].RequiredWidth) > 0.001)
                                    errors.Enqueue("Measured " + size.RequiredWidth + " wide instead of " + expected[i].RequiredWidth + ".");
                            }
                            catch (Exception ex)
                            {
                                errors.Enqueue(ex.GetType().Name + ": " + ex.Message);
                            }
                        }
                    });
                }
            }

            Assert.AreEqual(0, errors.Count,
                "Concurrent measurement on a shared measurer failed. First failure: " +
                (errors.TryDequeue(out var first) ? first : string.Empty));
        }

        /// <summary>
        /// The memoized values must still be the ones a single threaded caller would get, so a
        /// warm measurer has to agree with a cold one character for character.
        /// </summary>
        [TestMethod("2. Memoized metrics match a cold measurer")]
        public void MeasureLine_CachedMetricsMatchAFreshMeasurer()
        {
            var path = new FileInfo(Path.Combine(Environment.CurrentDirectory, ValidateHelvetica.UrlPath));

            using (var reader = new TypefaceReader())
            {
                var font = reader.GetFirstFont(path);
                var warm = font.GetMetrics(TypeMeasureOptions.Default);

                foreach (var line in Lines)
                {
                    //Warm the cache for this line, then measure it again on the same instance.
                    var first = warm.MeasureLine(line, 0, FontSize, Available, TypeMeasureOptions.Default);
                    var second = warm.MeasureLine(line, 0, FontSize, Available, TypeMeasureOptions.Default);

                    //And compare against an instance that has never seen these characters.
                    var cold = font.GetMetrics(TypeMeasureOptions.Default)
                        .MeasureLine(line, 0, FontSize, Available, TypeMeasureOptions.Default);

                    Assert.AreEqual(first.CharsFitted, second.CharsFitted, "Repeat measurement changed the fitted count for '" + line + "'");
                    Assert.AreEqual(first.RequiredWidth, second.RequiredWidth, "Repeat measurement changed the width for '" + line + "'");
                    Assert.AreEqual(cold.CharsFitted, first.CharsFitted, "A warm measurer disagreed with a cold one on the fitted count for '" + line + "'");
                    Assert.AreEqual(cold.RequiredWidth, first.RequiredWidth, "A warm measurer disagreed with a cold one on the width for '" + line + "'");
                }
            }
        }
    }
}
