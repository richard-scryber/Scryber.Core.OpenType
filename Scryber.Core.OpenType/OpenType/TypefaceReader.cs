using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if NET48
using System.Net;
#else
using System.Net.Http;
#endif

using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Scryber.OpenType.Utility;

namespace Scryber.OpenType
{
    /// <summary>
    /// Reads supported typefaces and fonts from files and streams including true type fonts, collections, open type fonts and web fonts (WOFF only).
    /// The Read Typeface methods will return information about the typeface data and fonts within it (e.g. Helvetica Black, Italic etc.).
    /// The Get Font methods will extract a particular font from the typeface data.
    /// </summary>
    /// <remarks>The TypefaceReader disinguishes between a typeface as a
    /// single data entity that can be read using the multiple ReadTypeface... methods, and can contain multiple Font programs within it.
    ///
    /// The TypefaceReader can then GetFont... programs from within the Typeface for explicit single fonts.
    ///
    /// ttf, otf and woff contain single fonts, the collection types can contain multiple fonts programs within them.
    ///
    /// The constructor for the reader accepts base paths to use as a reference when reading multiple sources from a standard loaction
    /// </remarks>
    public class TypefaceReader : IDisposable
    {

        //
        // properties
        //

        #region public virtual Utility.StreamLoader Loader {get;}

        private StreamLoader _loader;

        /// <summary>
        /// Gets the stream loader associated with this reader
        /// </summary>
        public virtual StreamLoader Loader
        {
            get { return _loader; }
        }

        #endregion

        //
        // .ctors
        //

        #region public TypefaceReader()

        /// <summary>
        /// Creates a new default Typeface reader
        /// </summary>
        public TypefaceReader()
            : this(new UnRootedStreamLoader())
        {
        }

        #endregion

        #region public TypefaceReader(Uri baseUri)

        /// <summary>
        /// Initializes a new TypefaceReader with a specific url as the root any Uri requests can be made from.
        /// </summary>
        /// <param name="baseUri">The baseUri to make relative requests from. If not null - must be an absolute Url</param>
        public TypefaceReader(Uri baseUri)
            : this(baseUri, null)
        {
            
        }

        #endregion

#if NET48

        #region public TypefaceReader(WebClient client)

        /// <summary>
        /// Initializes a new TypefaceReader without a root url, but with a WebClient that will be used for requests.
        /// </summary>
        /// <param name="client">The web client to use. If provided and not null then it is the creators responsibility to dispose of it.</param>
        public TypefaceReader(WebClient web)
            : this(new UnRootedStreamLoader(web))
        {

        }

        #endregion

        #region public TypefaceReader(Uri baseUri, WebClient client)

        /// <summary>
        /// Initializes a new TypefaceReader with a specific url as the root
        /// for any Uri requests, along with an WebClient to use for requests.
        /// </summary>
        /// <param name="baseUri">The baseUri to make relative requests from. Must be an absolute Url</param>
        /// <param name="client">The web client to use. If provided then it is the creators responsibility to dispose of it.</param>
        public TypefaceReader(Uri baseUri, WebClient client)
            : this(new RootedUriStreamLoader(baseUri, client))
        {
        }

        #endregion
#else

        #region public TypefaceReader(HttpClient client)

        /// <summary>
        /// Initializes a new TypefaceReader without a root url, but with an HttpClient that will be used for requests.
        /// </summary>
        /// <param name="client">The http client to use. If provided and not null then it is the creators responsibility to dispose of it.</param>
        public TypefaceReader(HttpClient client)
            :this(new UnRootedStreamLoader(client))
        {
        }

        #endregion

        #region public TypefaceReader(Uri baseUri, HttpClient client)

        /// <summary>
        /// Initializes a new TypefaceReader with a specific url as the root
        /// for any Uri requests, along with an HttpClient to use for requests.
        /// </summary>
        /// <param name="baseUri">The baseUri to make relative requests from. Must be an absolute Url</param>
        /// <param name="client">The http client to use. If provided then it is the creators responsibility to dispose of it.</param>
        public TypefaceReader(Uri baseUri, HttpClient client)
            : this(new RootedUriStreamLoader(baseUri, client))
        {
        }

        #endregion

#endif

        #region public TypefaceReader(DirectoryInfo baseDirectory)

        /// <summary>
        /// Initializes a new TypefaceReader with a specific Directory as the root any file requests to be made from.
        /// </summary>
        /// <param name="baseUri">The baseUri to make relative requests from. Must be an absolute Url</param>
        public TypefaceReader(DirectoryInfo baseDirectory)
            : this(new RootedFileStreamLoader(baseDirectory))
        {
            
        }

        #endregion

        #region public TypefaceReader(StreamLoader streamloader)

        /// <summary>
        /// A TypefaceReader with a specific stream loader, that can offer it's own methods of loading typefaces.
        /// </summary>
        /// <param name="streamloader"></param>
        public TypefaceReader(StreamLoader streamloader)
        {
            this._loader = streamloader ?? throw new ArgumentNullException(nameof(streamloader));
        }

        #endregion




        //
        // ReadAllTypefaces
        //

        #region public ITypefaceInfo[] ReadAllTypefaces(System.IO.DirectoryInfo directory ...)

        /// <summary>
        /// Reads a summary of all the typeface files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to scan for typeface files (children will not be scanned unless the includeSubdirectories is true)</param>
        /// <param name="matchExtension">An optional file match pattern to look for files with e.g. *.ttf|*.otf. Default is empty / null so all files will be checked. The pipe separator can be used for multiple matches</param>
        /// <param name="includeSubdirectories">An optional flag to also look for files in a subdirectory. Default is false</param>
        /// <param name="registerErrors">An optional flag to include in the results any read errors for matched files. Default is false</param>
        /// <returns>An array of all the Typefaces found in the files with References to their inner font variation(s)</returns>
        public ITypefaceInfo[] ReadAllTypefaces(System.IO.DirectoryInfo directory, string matchExtension = null, bool includeSubdirectories = false, bool registerErrors = false)
        {
            if (!directory.Exists)
                throw new DirectoryNotFoundException("Directory '" + directory.FullName + "' could not be found");

            List<FileInfo> files = new List<FileInfo>();

            Utility.PathHelper.MatchFiles(files, directory, matchExtension, includeSubdirectories);

            return this.ReadAllTypefaces(files, registerErrors);
            
        }

        #endregion

        #region public ITypefaceInfo[] ReadAllTypefaces(IEnumerable<FileInfo> files.. 


        /// <summary>
        /// Reads the summary information for all the requested files.
        /// </summary>
        /// <param name="files">An enumerable collection of Files that can be scanned and information returned on.</param>
        /// <param name="registerErrors">An optional flag that if true will add any read errors to the returned information array</param>
        /// <returns>An array of all the Typefaces found in the files with references to their inner Font(s)</returns>
        public ITypefaceInfo[] ReadAllTypefaces(IEnumerable<FileInfo> files, bool registerErrors = false)
        {
            if (null == files)
                throw new ArgumentNullException(nameof(files));

            List<ITypefaceInfo> found = new List<ITypefaceInfo>();

            foreach (var file in files)
            {
                ITypefaceInfo foundOne;

                using (var fs = this.Loader.GetStream(file, true))
                {
                    if (this.TryReadTypeface(fs, file.FullName, out foundOne))
                        found.Add(foundOne);
                    else if (registerErrors)
                    {
                        if (null != foundOne)
                            found.Add(foundOne);
                        else
                            found.Add(new Utility.UnknownTypefaceInfo(file.FullName, "The file is not a known font format"));
                    }
                    else
                        throw new TypefaceReadException("Could not load the typeface from file " + file.ToString());
                }
            }

            return found.ToArray();
        }

        #endregion

        #region public ITypefaceInfo[] ReadAllTypefaces(IEnumerable<Uri> urls.. 

        /// <summary>
        /// Reads the summary typeface information for all the requested urls.
        /// </summary>
        /// <param name="urls">An enumerable collection of full uri's that can be enumerated over inturn.</param>
        /// <param name="registerErrors">An optional flag that if true will add any read errors to the returned information array</param>
        /// <returns>An array of all the Typefaces found in the files with References to their inner font variation(s)</returns>
        public ITypefaceInfo[] ReadAllTypefaces(IEnumerable<Uri> urls, bool registerErrors = false)
        {
            if (null == urls)
                throw new ArgumentNullException(nameof(urls));

            List<ITypefaceInfo> found = new List<ITypefaceInfo>();

            foreach (var url in urls)
            {
                ITypefaceInfo foundOne;

                using (var fs = this.Loader.GetStream(url, true))
                {
                    if (this.TryReadTypeface(fs, url.ToString(), out foundOne))
                        found.Add(foundOne);
                    else if (registerErrors)
                    {
                        if (null != foundOne)
                            found.Add(foundOne);
                        else
                            found.Add(new Utility.UnknownTypefaceInfo(url.ToString(), "The file is not a known font format"));
                    }
                    else
                        throw new TypefaceReadException("Could not load the typeface from uri " + url.ToString());
                }
            }

            return found.ToArray();
        }

        #endregion


        //
        // ReadAllTypefacesAsync
        //

        #region public virtual ITypefaceInfo[] ReadAllTypefacesAsync(System.IO.DirectoryInfo directory ...)

        /// <summary>
        /// Reads a summary of all the typeface files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to scan for typeface files (children will not be scanned unless the includeSubdirectories is true)</param>
        /// <param name="matchExtension">An optional file match pattern to look for files with e.g. *.ttf|*.otf. Default is empty / null so all files will be checked</param>
        /// <param name="includeSubdirectories">An optional flag to also look for files in a subdirectory. Default is false</param>
        /// <param name="registerErrors">An optional flag include in the results any read errors for matched files. Default is false</param>
        /// <returns></returns>
        public virtual async Task<ITypefaceInfo[]> ReadAllTypefacesAsync(System.IO.DirectoryInfo directory, string matchExtension = null, bool includeSubdirectories = false, bool registerErrors = false)
        {
            if (!directory.Exists)
                throw new DirectoryNotFoundException("Directory '" + directory.FullName + "' could not be found");

            List<FileInfo> files = new List<FileInfo>();

            var info = await Task.Run(() =>
            {
                Utility.PathHelper.MatchFiles(files, directory, matchExtension, includeSubdirectories);

                return this.ReadAllTypefaces(files.ToArray(), registerErrors);
            });

            return info;
        }

        #endregion

        #region public virtual async Task<ITypefaceInfo[]> ReadAllTypefacesAsync(IEnumerable<FileInfo> files ...)

        /// <summary>
        /// Reads the summary information for all the requested files.
        /// </summary>
        /// <param name="files">An enumerable collection of Files that can be scanned and information returned on.</param>
        /// <param name="registerErrors">An optional flag that if true will add any read errors to the returned information array</param>
        /// <returns>An array of all the Typefaces found in the files with References to their inner font variation(s)</returns>
        public virtual async Task<ITypefaceInfo[]> ReadAllTypefacesAsync(IEnumerable<FileInfo> files, bool registerErrors = false)
        {
            if (null == files)
                throw new ArgumentNullException(nameof(files));

            List<ITypefaceInfo> found = new List<ITypefaceInfo>();

            await Task.Run(() =>
            {
                foreach (var file in files)
                {
                    ITypefaceInfo foundOne;

                    using (var fs = this.Loader.GetStream(file, true))
                    {
                        if (this.TryReadTypeface(fs, file.FullName, out foundOne))
                            found.Add(foundOne);

                        else if (registerErrors)
                        {
                            if (null != foundOne)
                                found.Add(foundOne);
                            else
                                found.Add(new Utility.UnknownTypefaceInfo(file.FullName, "The file is not a known font format"));
                        }
                        else
                            throw new TypefaceReadException("Could not load the typeface from file " + file.ToString());
                    }
                }
            });

            return found.ToArray();
        }


        #endregion

        #region public Task<ITypefaceInfo[]> ReadAllTypefacesAsync(IEnumerable<Uri> absoluteUrls.. 

        /// <summary>
        /// Reads the summary information for all the requested files asyncronously.
        /// </summary>
        /// <param name="urls">An enumerable collection of Files that can be scanned and information returned on.</param>
        /// <param name="registerErrors">An optional flag that if true will add any read errors to the returned information array</param>
        /// <returns>An array of all the Typefaces found in the files with References to their inner font variation(s)</returns>
        public virtual async Task<ITypefaceInfo[]> ReadAllTypefacesAsync(IEnumerable<Uri> urls, bool registerErrors = false)
        {
            if (null == urls)
                throw new ArgumentNullException(nameof(urls));

            List<ITypefaceInfo> found = new List<ITypefaceInfo>();

            await Task.Run(() =>
            {
                foreach (var url in urls)
                {
                    ITypefaceInfo foundOne;

                    using (var fs = this.Loader.GetStream(url, true))
                    {
                        if (this.TryReadTypeface(fs, url.ToString(), out foundOne))
                            found.Add(foundOne);
                        else if (registerErrors)
                        {
                            if (null != foundOne)
                                found.Add(foundOne);
                            else
                                found.Add(new Utility.UnknownTypefaceInfo(url.ToString(), "The file is not a known font format"));
                        }
                        else
                            throw new TypefaceReadException("Could not load the typeface from uri " + url.ToString());
                    }
                }
            });

            return found.ToArray();
        }

        #endregion

        //
        // TryReadTypeface
        //

        #region public bool TryReadTypeface(FileInfo fromFile, out ITypefaceInfo info)

        /// <summary>
        /// Attempts to read the typeface information from a file
        /// </summary>
        /// <param name="fromFile">The file to read the info from</param>
        /// <param name="info">Set to the info for the typeface if successful, or an error message</param>
        /// <returns>True if successfully read the information from file, otherwise false</returns>
        public bool TryReadTypeface(FileInfo fromFile, out ITypefaceInfo info)
        {
            Stream stream;
            string message;
            bool result = false;

            if (null == fromFile)
            {
                info = new Utility.UnknownTypefaceInfo("", "The File Info was null");
                result = false;
            }
            else if (!this.Loader.TryGetStream(fromFile, true, out stream, out message))
            {
                info = new Utility.UnknownTypefaceInfo(fromFile.FullName, message);
                result = false;
            }
            else if (null == stream)
            {
                info = new Utility.UnknownTypefaceInfo(fromFile.FullName, "No stream was returned for the requested file");
                result = false;
            }
            else
            {
                try
                {
                    result = this.TryReadTypeface(stream, fromFile.FullName, out info);
                }
                finally
                {
                    if (null != stream)
                        stream.Dispose();

                }
            }

            return result;
        }

        #endregion

        #region public bool TryReadTypeface(Uri fromUrl, out ITypefaceInfo info)

        /// <summary>
        /// Attempts to read the typeface information from a url
        /// </summary>
        /// <param name="fromUrl">The url to read the info from</param>
        /// <param name="info">Set to the info for the typeface if successful, or an error message</param>
        /// <returns>True if successfully read the information from the url, otherwise false</returns>
        public bool TryReadTypeface(Uri fromUrl, out ITypefaceInfo info)
        {
            Stream stream;
            string message;
            bool result = false;

            if (null == fromUrl)
            {
                info = new Utility.UnknownTypefaceInfo("", "The Url was null");
                result = false;
            }
            else if (!this.Loader.TryGetStream(fromUrl, true, out stream, out message))
            {
                info = new Utility.UnknownTypefaceInfo(fromUrl.ToString(), message);
                result = false;
            }
            else if (null == stream)
            {
                info = new Utility.UnknownTypefaceInfo(fromUrl.ToString(), "No stream was returned for the requested file");
                result = false;
            }
            else
            {
                try
                {
                    result = this.TryReadTypeface(stream, fromUrl.ToString(), out info);

                }
                finally
                {
                    if (null != stream)
                        stream.Dispose();
                }
            }

            return result;
        }

        #endregion

        #region public bool TryReadTypeface(Stream seekableStream, string source, out ITypefaceInfo info)

        /// <summary>
        /// Attempts to read the typeface information from a seekable stream
        /// </summary>
        /// <param name="seekableStream">The stream to read the info from</param>
        /// <param name="source">The name or path for the information</param>
        /// <param name="info">Set to the info for the typeface if successful, or an error message</param>
        /// <returns>True if successfully read the information from the url, otherwise false</returns>
        public bool TryReadTypeface(Stream seekableStream, string source, out ITypefaceInfo info)
        {
            info = null;
            if (null == seekableStream)
            {
                info = new Utility.UnknownTypefaceInfo(source, " The provided stream was null. Ensure there is a valid stream to read from");
                return false;
            }
            if (seekableStream.CanSeek == false)
            {
                info = new Utility.UnknownTypefaceInfo(source, "A stream must be seekable to read font information so the position can be set, for example a FileStream or MemoryStream, and this one is not seekable.");
                return false;
            }

            return DoTryReadTypeface(seekableStream, source, out info);
        }

        #endregion

        //
        // DoTryReadTypeface
        //



        //
        // ReadTypeface
        //

        #region public ITypefaceInfo ReadTypeface(string path)

        /// <summary>
        /// Reads the available typeface information from the requested path.
        /// This can either be an absolute Url or File path, or relative if a base path was provided in the constructor.
        /// If the base was a Url and the path is relative, it is assumed to be a relative url (not a relative file path).
        /// </summary>
        /// <param name="path">The absolute or relative path</param>
        /// <returns></returns>
        public ITypefaceInfo ReadTypeface(string path)
        {
            using (var stream = this.Loader.GetStream(path, true))
            {
                return DoReadTypeface(stream, path);
            }

        }

        #endregion

        #region public ITypefaceInfo ReadTypeface(Uri path)

        /// <summary>
        /// Reads the available typeface information from the requested path. This can either be an absolute Url, or relative if a base path was provided in the constructor
        /// </summary>
        /// <param name="path">The absolute or relative url</param>
        /// <returns></returns>
        public ITypefaceInfo ReadTypeface(Uri path)
        {
            using (var stream = this.Loader.GetStream(path, true))
            {
                return DoReadTypeface(stream, path.ToString());
            }

        }

        #endregion

        #region public ITypefaceInfo ReadTypeface(FileInfo path)

        /// <summary>
        /// Reads the available typeface information from the requested path. This can either be an absolute File path, or relative if a base path was provided in the constructor
        /// </summary>
        /// <param name="path">The absolute or relative file path</param>
        /// <returns></returns>
        public ITypefaceInfo ReadTypeface(FileInfo path)
        {
            using (var stream = this.Loader.GetStream(path, true))
            {
                return DoReadTypeface(stream, path.ToString());
            }

        }

        #endregion

        #region public ITypefaceInfo ReadTypeface(Stream seekableStream, string source)

        /// <summary>
        /// Reads the available typeface information from the requested SEEKABLE stream with the specified source name
        /// </summary>
        /// <param name="seekableStream">The seekable stream to read the info from</param>
        /// <param name="source">Can be any name or path to identify the returned info with</param>
        /// <returns>The information read from the typeface stream</returns>
        public ITypefaceInfo ReadTypeface(Stream seekableStream, string source)
        {
            if (null == seekableStream)
                throw new ArgumentNullException(nameof(seekableStream));

            if (seekableStream.CanSeek == false)
            {
                throw new ArgumentOutOfRangeException(nameof(seekableStream), "The provided stream cannot be positioned, use a FileStream or a MemoryStream if needed to create a seekable stream");
            }

            return DoReadTypeface(seekableStream, source);
        }


        #endregion

        //
        // ReadTypefaceAsync
        //

        #region public async Task<ITypefaceInfo> ReadTypefaceAsync(string path)

        /// <summary>
        /// Asyncronously reads the available typeface information from the requested path.
        /// This can either be an absolute Url or File path, or relative if a base path was provided in the constructor.
        /// If the base was a Url and the path is relative, it is assumed to be a relative url (not a relative file path).
        /// </summary>
        /// <param name="path">The absolute or relative path</param>
        /// <returns></returns>
        public async Task<ITypefaceInfo> ReadTypefaceAsync(string path)
        {
            using (var stream = await this.Loader.GetStreamAsync(path, true))
            {
                return DoReadTypeface(stream, path);
            }
        }

        #endregion

        #region public async Task<ITypefaceInfo> ReadTypefaceAsync(Uri path)

        /// <summary>
        /// Asyncronously reads the available typeface information from the requested path.
        /// This can either be an absolute Url or File path, or relative if a base path was provided in the constructor.
        /// If the base was a Url and the path is relative, it is assumed to be a relative url (not a relative file path).
        /// </summary>
        /// <param name="path">The absolute or relative path</param>
        /// <returns></returns>
        public async Task<ITypefaceInfo> ReadTypefaceAsync(Uri path)
        {
            using (var stream = await this.Loader.GetStreamAsync(path, true))
            {
                return DoReadTypeface(stream, path.ToString());
            }
        }

        #endregion

        #region public async Task<ITypefaceInfo> ReadTypefaceAsync(FileInfo path)

        /// <summary>
        /// Asyncronously reads the available typeface information from the requested path.
        /// This can either be an absolute Url or File path, or relative if a base path was provided in the constructor.
        /// If the base was a Url and the path is relative, it is assumed to be a relative url (not a relative file path).
        /// </summary>
        /// <param name="path">The absolute or relative path</param>
        /// <returns></returns>
        public async Task<ITypefaceInfo> ReadTypefaceAsync(FileInfo path)
        {
            using (var stream = await this.Loader.GetStreamAsync(path, true))
            {
                return DoReadTypeface(stream, path.FullName);
            }
        }

        #endregion



        //
        // GetFonts
        //

        #region public IEnumerable<ITypefaceFont> GetFonts(ITypefaceInfo forInfo)

        /// <summary>
        /// Gets all the typeface fonts in the typeface information
        /// </summary>
        /// <param name="forInfo">The typeface to load the font programs for</param>
        /// <returns>An enumerable collection of font programmes or empty if there were none.</returns>
        /// <remarks>For single font files (ttf, otf or woff) this will be just one, for collections (ttc, otc or woff2) this could be more than one</remarks>
        public IEnumerable<ITypefaceFont> GetFonts(ITypefaceInfo forInfo)
        {
            if (null == forInfo)
                throw new ArgumentNullException(nameof(forInfo));

            if (forInfo.Fonts == null)
                return new ITypefaceFont[] { };

            if (forInfo.FontCount == 0 && (forInfo.Fonts == null || forInfo.Fonts.Length == 0))
                return new ITypefaceFont[] { };

            if (forInfo.FontCount != forInfo.Fonts.Length)
                throw new ArgumentOutOfRangeException(nameof(forInfo.FontCount), "The number of references in the information does not match the specified TypefaceCount");

            if (string.IsNullOrEmpty(forInfo.Source))
                throw new ArgumentNullException(nameof(forInfo.Source), "The path must be set on the typeface info, so the reader can load the file");

            if (forInfo.FontCount == 1)
            {
                return new ITypefaceFont[]
                {
                    GetFont(forInfo, forInfo.Fonts[0])
                };
            }
            else
            {
                List<ITypefaceFont> loaded = new List<ITypefaceFont>();

                using (var stream = this.Loader.GetStream(forInfo.Source, true))
                {
                    foreach (var reference in forInfo.Fonts)
                    {
                        stream.Position = 0;
                        var one = GetFont(forInfo, reference);
                        loaded.Add(one);
                    }
                }
                return loaded;
            }
        }

        #endregion

        #region public virtual IEnumerable<ITypefaceFont> GetFonts(Uri uri)

        /// <summary>
        /// Gets all the typeface fonts in the data returned from the relative or absolute url
        /// </summary>
        /// <param name="uri">The relative or absolute url containing one or more typefaces</param>
        /// <returns>An enumerable collection of typefaces or null if there were none.</returns>
        /// <remarks>For single font files (ttf, otf or woff) this will be just one, for collections (ttc, otc or woff2) this could be more than one</remarks>
        public IEnumerable<ITypefaceFont> GetFonts(Uri uri)
        {
            if (null == uri)
                throw new ArgumentNullException(nameof(uri));

            using (var stream = this.Loader.GetStream(uri, true))
            {
                return DoGetFonts(stream, uri.ToString());
            }
        }

        #endregion

        #region public virtual IEnumerable<ITypefaceFont> GetFonts(FileInfo path)

        /// <summary>
        /// Gets all the typeface fonts defined in the data returned from the relative or absolute filepath
        /// </summary>
        /// <param name="uri">The relative or absolute url containing one or more typeface fonts</param>
        /// <returns>An enumerable collection of typefaces or null if there were none.</returns>
        /// <remarks>For single font files (ttf, otf or woff) this will be just one, for collections (ttc, otc or woff2) this could be more than one</remarks>
        public IEnumerable<ITypefaceFont> GetFonts(FileInfo path)
        {
            if (null == path)
                throw new ArgumentNullException(nameof(path));

            using (var stream = this.Loader.GetStream(path, true))
            {
                return DoGetFonts(stream, path.FullName);
            }
        }

        #endregion

        #region public IEnumerable<ITypefaceFont> GetFonts(Stream seekableStream, string source)

        /// <summary>
        /// Gets all the typeface fonts defined in the provided SEEKABLE stream with the specified source set.
        /// </summary>
        /// <param name="seekableStream">The stream that supports seeking, having a position explicily set.</param>
        /// <returns>An enumerable collection of typefaces or null if there were none.</returns>
        /// <remarks>For single font files (ttf, otf or woff) this will be just one, for collections (ttc, otc or woff2) this could be more than one</remarks>
        public IEnumerable<ITypefaceFont> GetFonts(Stream seekableStream, string source)
        {
            if (null == seekableStream)
                throw new ArgumentNullException(nameof(seekableStream));
            if (!seekableStream.CanSeek)
                throw new ArgumentException("The stream must support seeking (setting the position)", nameof(seekableStream));

            return DoGetFonts(seekableStream, source);
        }

        #endregion


        //
        // GetFontsAsync
        //

        #region public async Task<IEnumerable<ITypefaceFont>> GetFontsAsync(Uri uri)

        /// <summary>
        /// Asyncronously gets all the fonts defined in the typeface font data returned from the relative or absolute url
        /// </summary>
        /// <param name="uri">The url pointing to a data stream with one or more fonts programs in a typeface. Should be absolute, or relative if the reader was initialised with a root url.</param>
        /// <returns>An enumerable collection of typefaces or null if there were none.</returns>
        public async Task<IEnumerable<ITypefaceFont>> GetFontsAsync(Uri uri)
        {
            if (null == uri)
                throw new ArgumentNullException(nameof(uri));

            using (var stream = await this.Loader.GetStreamAsync(uri, true))
            {
                var info = this.DoReadTypeface(stream, uri.ToString());

                if (null != info && info.Fonts.Length > 0)
                {
                    //reset the stream, rather than reloading
                    //as we know it is seekable
                    stream.Position = 0;

                    if (info.Fonts.Length == 1)
                    {
                        var one = this.GetFont(stream, uri.ToString(), info.Fonts[0]);
                        return new ITypefaceFont[] { one };
                    }
                    else
                    {
                        List<ITypefaceFont> typefaces = new List<ITypefaceFont>();

                        foreach (var tfref in info.Fonts)
                        {
                            stream.Position = 0;
                            var one = this.GetFont(stream, info.Source, tfref);
                            typefaces.Add(one);
                        }

                        return typefaces;
                    }

                }
                return null;
            }
        }

        #endregion


        #region public async Task<IEnumerable<ITypefaceFont>> GetFontsAsync(FileInfo path)

        /// <summary>
        /// Asyncronously gets all the fonts defined in the typeface font data returned from the relative or rooted file path
        /// </summary>
        /// <param name="path">The file path containing one or more typefaces. Should be absolute, or relative if the reader was initialised with a directory.</param>
        /// <returns>An enumerable collection of typefaces or null if there were none.</returns>
        public async Task<IEnumerable<ITypefaceFont>> GetFontsAsync(FileInfo path)
        {
            if (null == path)
                throw new ArgumentNullException(nameof(path));

            using (var stream = await this.Loader.GetStreamAsync(path, true))
            {
                var info = this.DoReadTypeface(stream, path.FullName);

                if (null != info && info.Fonts.Length > 0)
                {
                    //reset the stream, rather than reloading
                    //as we know it is seekable
                    stream.Position = 0;

                    if (info.Fonts.Length == 1)
                    {
                        var one = this.GetFont(stream, info.Source, info.Fonts[0]);
                        return new ITypefaceFont[] { one };
                    }
                    else
                    {
                        List<ITypefaceFont> typefaces = new List<ITypefaceFont>();

                        foreach (var tfref in info.Fonts)
                        {
                            stream.Position = 0;
                            var one = this.GetFont(stream, info.Source, tfref);
                            
                            typefaces.Add(one);
                        }

                        return typefaces;
                    }

                }
                return null;
            }
        }

        #endregion

        #region public async Task<IEnumerable<ITypefaceFont>> GetFontsAsync(ITypefaceInfo forInfo)

        /// <summary>
        /// Asyncronously gets all the fonts defined in the typeface information returned from one fo the ReadTypeface methods.
        /// </summary>
        /// <param name="forInfo">The typeface to get all the font programs from</param>
        /// <returns>An enumerable collection of fonts or an empty array if there were no fonts in the typeface.</returns>
        public async Task<IEnumerable<ITypefaceFont>> GetFontsAsync(ITypefaceInfo forInfo)
        {
            if (null == forInfo)
                throw new ArgumentNullException(nameof(forInfo));

            if (forInfo.Fonts == null)
                return new ITypefaceFont[] { };

            if (forInfo.FontCount == 0 && (forInfo.Fonts == null || forInfo.Fonts.Length == 0))
                return new ITypefaceFont[] { };

            if (forInfo.FontCount != forInfo.Fonts.Length)
                throw new ArgumentOutOfRangeException(nameof(forInfo.FontCount), "The number of references in the information does not match the specified TypefaceCount");

            if (string.IsNullOrEmpty(forInfo.Source))
                throw new ArgumentNullException(nameof(forInfo.Source), "The path must be set on the typeface info, so the reader can load the file");


            if (forInfo.FontCount == 1)
            {
                using (var stream = await this.Loader.GetStreamAsync(forInfo.Source, true))
                {
                    var one = GetFont(stream, forInfo.Source, forInfo.Fonts[0]);
                    return new ITypefaceFont[] { one };
                }
            }
            else
            {
                List<ITypefaceFont> loaded = new List<ITypefaceFont>();
                using (var stream = await this.Loader.GetStreamAsync(forInfo.Source, true))
                {
                    foreach (var reference in forInfo.Fonts)
                    {
                        stream.Position = 0;
                        var one = GetFont(stream, forInfo.Source, reference);
                        loaded.Add(one);
                    }
                }
                return loaded;
            }
        }

        #endregion

        //
        // GetFirstFont
        //

        #region public ITypefaceFont GetFirstFont(Uri uri)

        /// <summary>
        /// Asyncronously gets the first typeface defined in the data returned from the relative or absolute url. NOT recommended for TTC or Woff2
        /// </summary>
        /// <param name="uri">The relative or absolute url containing one or more typefaces</param>
        /// <returns>An enumerable collection of typefaces or null if there were none.</returns>
        /// <remarks>For single font files (ttf, otf or woff) this will be the font typeface, for collections (ttc, otc or woff2) this will be the first in the file</remarks>
        public ITypefaceFont GetFirstFont(Uri uri)
        {
            if (null == uri)
                throw new ArgumentNullException(nameof(uri));

            using (var stream = this.Loader.GetStream(uri, true))
            {
                var info = this.DoReadTypeface(stream, uri.ToString());

                if (null != info && info.Fonts.Length > 0)
                {
                    //reset the stream, rather than reloading
                    //as we know it is seekable

                    stream.Position = 0;

                    var one = this.GetFont(stream, info.Source, info.Fonts[0]);
                    return one;

                }
                else
                    throw new TypefaceReadException("No fonts were returned from the typeface at path " + uri.ToString());
            }
        }

        #endregion

        #region public ITypefaceFont GetFirstFont(FileInfo file)

        /// <summary>
        /// Asyncronously gets the first typeface defined in the data returned from the relative or absolute url. NOT recommended for TTC or Woff2
        /// </summary>
        /// <param name="uri">The relative or absolute url containing one or more typefaces</param>
        /// <returns>An enumerable collection of typefaces or null if there were none.</returns>
        /// <remarks>For single font files (ttf, otf or woff) this will be the font typeface, for collections (ttc, otc or woff2) this will be the first in the file</remarks>
        public ITypefaceFont GetFirstFont(FileInfo file)
        {
            if (null == file)
                throw new ArgumentNullException(nameof(file));

            using (var stream = this.Loader.GetStream(file, true))
            {
                var info = this.DoReadTypeface(stream, file.FullName);

                if (null != info && info.Fonts.Length > 0)
                {
                    //reset the stream, rather than reloading
                    //as we know it is seekable

                    stream.Position = 0;

                    var one = this.GetFont(stream, info.Source, info.Fonts[0]);
                    return one;

                }
                else
                    throw new TypefaceReadException("No fonts were returned from the typeface at path " + file.FullName.ToString());
            }
        }

        #endregion

        //
        // GetFirstFontAsync
        //

        #region public async Task<ITypefaceFont> GetFirstFontAsync(Uri uri)

        /// <summary>
        /// Asyncronously gets the first typeface defined in the data returned from the relative or absolute url. NOT recommended for TTC or Woff2
        /// </summary>
        /// <param name="uri">The relative or absolute url containing one or more typefaces</param>
        /// <returns>An enumerable collection of typefaces or null if there were none.</returns>
        /// <remarks>For single font files (ttf, otf or woff) this will be the font typeface, for collections (ttc, otc or woff2) this will be the first in the file</remarks>
        public async Task<ITypefaceFont> GetFirstFontAsync(Uri uri)
        {
            if (null == uri)
                throw new ArgumentNullException(nameof(uri));

            using (var stream = await this.Loader.GetStreamAsync(uri, true))
            {
                var info = this.DoReadTypeface(stream, uri.ToString());

                if (null != info && info.Fonts.Length > 0)
                {
                    //reset the stream, rather than reloading
                    //as we know it is seekable

                    stream.Position = 0;

                    var one = this.GetFont(stream, info.Source, info.Fonts[0]);
                    return one;

                }
                else
                    throw new TypefaceReadException("No fonts were returned from the typeface at path " + uri.ToString());
            }
        }

        #endregion

        #region public async Task<ITypefaceFont> GetFirstFontAsync(FileInfo file)

        /// <summary>
        /// Asyncronously gets the first typeface defined in the data returned from the relative or absolute url. NOT recommended for TTC or Woff2
        /// </summary>
        /// <param name="uri">The relative or absolute url containing one or more typefaces</param>
        /// <returns>An enumerable collection of typefaces or null if there were none.</returns>
        /// <remarks>For single font files (ttf, otf or woff) this will be the font typeface, for collections (ttc, otc or woff2) this will be the first in the file</remarks>
        public async Task<ITypefaceFont> GetFirstFontAsync(FileInfo file)
        {
            if (null == file)
                throw new ArgumentNullException(nameof(file));

            using (var stream = await this.Loader.GetStreamAsync(file, true))
            {
                var info = this.DoReadTypeface(stream, file.FullName);

                if (null != info && info.Fonts.Length > 0)
                {
                    //reset the stream, rather than reloading
                    //as we know it is seekable

                    stream.Position = 0;

                    var one = this.GetFont(stream, info.Source, info.Fonts[0]);
                    return one;

                }
                else
                    throw new TypefaceReadException("No fonts were returned from the typeface at path " + file.FullName.ToString());
            }
        }

        #endregion


        //
        // GetFont
        //

        #region public ITypefaceFont GetFont(ITypefaceInfo forInfo, int fontIndex)

        /// <summary>
        /// Gets the font program data at the specified index in the typeface information returned from the ReadTypeface method(s)
        /// </summary>
        /// <param name="forInfo">The typeface information returned from a ReadTypeface</param>
        /// <param name="fontIndex">This index of the font program to get from the Fonts</param>
        /// <returns>The font prgram matching the font info at the specified index</returns>
        public ITypefaceFont GetFont(ITypefaceInfo forInfo, int fontIndex)
        {
            if (null == forInfo)
                throw new ArgumentNullException(nameof(forInfo));

            if (forInfo.Fonts == null)
                throw new ArgumentNullException(nameof(forInfo.Fonts));

            if (forInfo.Fonts.Length <= fontIndex)
                throw new ArgumentOutOfRangeException(nameof(fontIndex));

            return this.GetFont(forInfo, forInfo.Fonts[fontIndex]);

        }

        #endregion

        #region public ITypefaceFont GetFont(ITypefaceInfo info, IFontInfo forReference)

        /// <summary>
        /// Gets the font program data for the reference in the typeface information returned from the ReadTypeface method(s)
        /// </summary>
        /// <param name="forInfo">The typeface information returned from a ReadTypeface</param>
        /// <param name="forReference">This font reference, that must also be in the typeface information</param>
        /// <returns>The font program matching the font info</returns>
        public ITypefaceFont GetFont(ITypefaceInfo info, IFontInfo forReference)
        {
            if (null == info)
                throw new ArgumentNullException(nameof(info));

            if (null == forReference)
                throw new ArgumentNullException(nameof(forReference));

            if (Array.IndexOf(info.Fonts, forReference) < 0)
                throw new IndexOutOfRangeException("The font reference was not found in the typeface information");
            
            using (var stream = this.Loader.GetStream(info.Source, true))
            {
                return DoGetFont(stream, info.Source, forReference);
            }
        }

        #endregion

        #region public ITypefaceFont GetFont(Uri uri, int forIndex)

        /// <summary>
        /// Gets the font program data for the reference in the data returned from the Uri
        /// </summary>
        /// <param name="uri">The absolute or relative url of the typeface that will contain the font program</param>
        /// <param name="forReference">This font reference, that the is in the url</param>
        /// <returns>The font program matching the font info</returns>
        public ITypefaceFont GetFont(Uri uri, int forIndex)
        {
            if (null == uri)
                throw new ArgumentNullException(nameof(uri));

            using (var stream = this.Loader.GetStream(uri, true))
            {
                var info = DoReadTypeface(stream, uri.ToString());
                if (null == info || info.FontCount == 0 || null == info.Fonts || info.Fonts.Length <= forIndex)
                    throw new ArgumentOutOfRangeException(nameof(forIndex), "The typeface information did not contain a font reference at index " + forIndex);

                stream.Position = 0;
                return DoGetFont(stream, uri.ToString(), info.Fonts[forIndex]);
            }
        }

        #endregion

        #region  public ITypefaceFont GetFont(FileInfo file, int forIndex)

        /// <summary>
        /// Gets the font program data for the reference in the data returned from the file path
        /// </summary>
        /// <param name="file">The absolute or relative file path of the typeface that will contain the font program</param>
        /// <param name="forReference">This font reference, that the is in the file</param>
        /// <returns>The font program matching the font info</returns>
        public ITypefaceFont GetFont(FileInfo file, int forIndex)
        {
            if (null == file)
                throw new ArgumentNullException(nameof(file));

            using (var stream = this.Loader.GetStream(file, true))
            {
                var info = DoReadTypeface(stream, file.FullName);

                if (null == info || info.FontCount == 0 || null == info.Fonts || info.Fonts.Length <= forIndex)
                    throw new ArgumentOutOfRangeException(nameof(forIndex), "The typeface information did not contain a font reference at index " + forIndex);

                stream.Position = 0;
                return DoGetFont(stream, file.FullName, info.Fonts[forIndex]);
            }
        }

        #endregion

        #region public ITypefaceFont GetFont(Uri uri, IFontInfo forReference)

        /// <summary>
        /// Gets the font program data for the reference in the data returned from the Uri
        /// </summary>
        /// <param name="uri">The absolute or relative url of the typeface that will contain the font program</param>
        /// <param name="forReference">This font reference, that the is in the url</param>
        /// <returns>The font program matching the font info</returns>
        public ITypefaceFont GetFont(Uri uri, IFontInfo forReference)
        {
            if (null == uri)
                throw new ArgumentNullException(nameof(uri));

            if (null == forReference)
                throw new ArgumentNullException(nameof(forReference));

            using (var stream = this.Loader.GetStream(uri, true))
            {
                return DoGetFont(stream, uri.ToString(), forReference);
            }
        }

        #endregion

        #region  public ITypefaceFont GetFont(FileInfo file, IFontInfo forReference)

        /// <summary>
        /// Gets the font program data for the reference in the data returned from the file path
        /// </summary>
        /// <param name="file">The absolute or relative file path of the typeface that will contain the font program</param>
        /// <param name="forReference">This font reference, that the is in the file</param>
        /// <returns>The font program matching the font info</returns>
        public ITypefaceFont GetFont(FileInfo file, IFontInfo forReference)
        {
            if (null == file)
                throw new ArgumentNullException(nameof(file));

            if (null == forReference)
                throw new ArgumentNullException(nameof(forReference));

            using (var fs = this.Loader.GetStream(file, true))
            {
                return DoGetFont(fs, file.FullName, forReference);
            }
        }

        #endregion

        #region public ITypefaceFont GetFont(Stream seekableStream, string source, IFontInfo forReference)

        /// <summary>
        /// Gets the font program data for the reference in the data returned from the stream
        /// </summary>
        /// <param name="seekableStream">The seekable stream that will contain the font program</param>
        /// <param name="source">A path to the stream, that will be used as the source property on the font program</param>
        /// <param name="forReference">This font reference, that the is in the url</param>
        /// <returns>The font program matching the font info</returns>
        public ITypefaceFont GetFont(Stream seekableStream, string source, IFontInfo forReference)
        {
            if (null == seekableStream)
                throw new ArgumentNullException(nameof(seekableStream));

            if (null == forReference)
                throw new ArgumentNullException(nameof(forReference));

            if (seekableStream.CanSeek == false)
            {
                throw new ArgumentOutOfRangeException(nameof(seekableStream), "The provided stream cannot be positioned, use a FileStream or a MemoryStream if needed to create a seekable stream");
            }

            return DoGetFont(seekableStream, source, forReference);
        }

        #endregion


        //
        // GetFontAsync
        //


        #region public async Task<ITypefaceFont> GetFontAsyncGetFont(ITypefaceInfo forInfo, int fontIndex)

        /// <summary>
        /// Gets the font program data at the specified index in the typeface information returned from the ReadTypeface method(s)
        /// </summary>
        /// <param name="forInfo">The typeface information returned from a ReadTypeface</param>
        /// <param name="fontIndex">This index of the font program to get from the Fonts</param>
        /// <returns>The font prgram matching the font info at the specified index</returns>
        public async Task<ITypefaceFont> GetFontAsync(ITypefaceInfo forInfo, int fontIndex)
        {
            if (null == forInfo)
                throw new ArgumentNullException(nameof(forInfo));

            if (forInfo.Fonts == null)
                throw new ArgumentNullException(nameof(forInfo.Fonts));

            if (forInfo.Fonts.Length <= fontIndex)
                throw new ArgumentOutOfRangeException(nameof(fontIndex));

            return await this.GetFontAsync(forInfo, forInfo.Fonts[fontIndex]);
        }

        #endregion

        #region public async Task<ITypefaceFont> GetFontAsync(ITypefaceInfo info, IFontInfo forReference)

        /// <summary>
        /// Asyncronously gets the font program data for the reference in the typeface information returned from the ReadTypeface method(s)
        /// </summary>
        /// <param name="forInfo">The typeface information returned from a ReadTypeface</param>
        /// <param name="forReference">This font reference, that must also be in the typeface information</param>
        /// <returns>The font program matching the font info</returns>
        public async Task<ITypefaceFont> GetFontAsync(ITypefaceInfo info, IFontInfo forReference)
        {
            if (null == info)
                throw new ArgumentNullException(nameof(info));

            if (null == forReference)
                throw new ArgumentNullException(nameof(forReference));

            if (Array.IndexOf(info.Fonts, forReference) < 0)
                throw new IndexOutOfRangeException("The font reference was not found in the typeface information");

            using (var stream = await this.Loader.GetStreamAsync(info.Source, true))
            {
                return DoGetFont(stream, info.Source, forReference);
            }
        }

        #endregion

        #region public async Task<ITypefaceFont> GetFontAsync(Uri uri, IFontInfo forReference)

        /// <summary>
        /// Asyncronously gets the font program data for the reference in the data returned from the Uri
        /// </summary>
        /// <param name="uri">The absolute or relative url of the typeface that will contain the font program</param>
        /// <param name="forIndex">The index of the font within the typeface collection</param>
        /// <returns>The font program at the index in the collection</returns>
        public async Task<ITypefaceFont> GetFontAsync(Uri uri, int forIndex)
        {
            if (null == uri)
                throw new ArgumentNullException(nameof(uri));

            
            using (var stream = await this.Loader.GetStreamAsync(uri, true))
            {
                var info = DoReadTypeface(stream, uri.ToString());
                if (null == info || info.FontCount == 0 || null == info.Fonts || info.Fonts.Length <= forIndex)
                    throw new ArgumentOutOfRangeException(nameof(forIndex), "The typeface information did not contain a font reference at index " + forIndex);

                stream.Position = 0;
                return DoGetFont(stream, info.Source, info.Fonts[forIndex]);
            }
        }

        #endregion

        #region public async Task<ITypefaceFont> GetFontAsync(FileInfo file, IFontInfo forReference)


        /// <summary>
        /// Asyncronously gets the font program data for the reference in the data returned from the file path
        /// </summary>
        /// <param name="file">The absolute or relative file path of the typeface that will contain the font program</param>
        /// <param name="forIndex">This font reference, that the is in the file</param>
        /// <returns>The font program matching the font info</returns>
        public async Task<ITypefaceFont> GetFontAsync(FileInfo file, int forIndex)
        {
            if (null == file)
                throw new ArgumentNullException(nameof(file));


            using (var stream = await this.Loader.GetStreamAsync(file, true))
            {
                var info = DoReadTypeface(stream, file.FullName);

                if (null == info || info.FontCount == 0 || null == info.Fonts || info.Fonts.Length <= forIndex)
                    throw new ArgumentOutOfRangeException(nameof(forIndex), "The typeface information did not contain a font reference at index " + forIndex);

                stream.Position = 0;
                return DoGetFont(stream, info.Source, info.Fonts[forIndex]);
            }
        }


        #endregion

        #region public async Task<ITypefaceFont> GetFontAsync(Uri uri, IFontInfo forReference)

        /// <summary>
        /// Asyncronously gets the font program data for the reference in the data returned from the Uri
        /// </summary>
        /// <param name="uri">The absolute or relative url of the typeface that will contain the font program</param>
        /// <param name="forReference">This font reference, that the is in the url</param>
        /// <returns>The font program matching the font info</returns>
        public async Task<ITypefaceFont> GetFontAsync(Uri uri, IFontInfo forReference)
        {
            if (null == uri)
                throw new ArgumentNullException(nameof(uri));

            if (null == forReference)
                throw new ArgumentNullException(nameof(forReference));

            using (var stream = await this.Loader.GetStreamAsync(uri, true))
            {
                return DoGetFont(stream, uri.ToString(), forReference);
            }
        }

        #endregion

        #region public async Task<ITypefaceFont> GetFontAsync(FileInfo file, IFontInfo forReference)


        /// <summary>
        /// Asyncronously gets the font program data for the reference in the data returned from the file path
        /// </summary>
        /// <param name="file">The absolute or relative file path of the typeface that will contain the font program</param>
        /// <param name="forReference">This font reference, that the is in the file</param>
        /// <returns>The font program matching the font info</returns>
        public async Task<ITypefaceFont> GetFontAsync(FileInfo file, IFontInfo forReference)
        {
            if (null == file)
                throw new ArgumentNullException(nameof(file));

            if (null == forReference)
                throw new ArgumentNullException(nameof(forReference));

            using (var fs = await this.Loader.GetStreamAsync(file, true))
            {
                return DoGetFont(fs, file.FullName, forReference);
            }
        }


        #endregion

        //
        // Protected implementation methods
        //


        #region protected virtual ITypefaceFont DoGetFont(Stream seekableStream, string source, IFontInfo theReference)

        /// <summary>
        /// Main implementation method for getting a font program from a stream. Inheritors can override.
        /// </summary>
        /// <param name="seekableStream"></param>
        /// <param name="source"></param>
        /// <param name="theReference"></param>
        /// <returns></returns>
        protected virtual ITypefaceFont DoGetFont(Stream seekableStream, string source, IFontInfo theReference)
        {

            using (var reader = new BigEndianReader(seekableStream))
            {
                TypefaceVersionReader version;
                if (!TypefaceVersionReader.TryGetVersion(reader, out version))
                    throw new NullReferenceException("Could not identify the typeface in the provided stream");

                else
                {
                    var typeface = version.ReadTypefaceAfterVersion(reader, theReference, source);

                    return typeface;
                }
            }

        }

        #endregion


        #region protected virtual IEnumerable<ITypefaceFont> DoGetFonts(Stream stream, string source)

        /// <summary>
        /// Main method to read all the typeface fonts defined in a stream. Inheritors can override.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ITypefaceFont> DoGetFonts(Stream stream, string source)
        {
            var info = this.DoReadTypeface(stream, source);

            if (null != info && info.Fonts.Length > 0)
            {
                //reset the stream, rather than reloading
                //as we know it is seekable

                stream.Position = 0;

                if (info.Fonts.Length == 1)
                {
                    var one = this.GetFont(stream, info.Source, info.Fonts[0]);
                    return new ITypefaceFont[] { one };
                }
                else
                {
                    List<ITypefaceFont> typefaces = new List<ITypefaceFont>();

                    foreach (var tfref in info.Fonts)
                    {
                        stream.Position = 0;
                        var one = this.GetFont(stream, info.Source, tfref);
                        typefaces.Add(one);
                    }

                    return typefaces;
                }

            }
            return null;
        }

        #endregion


        #region protected virtual ITypefaceInfo DoReadTypeface(Stream seekableStream, string source)

        /// <summary>
        /// Gets the specific typeface Information from the provided stream, which must be seekable (supports the Postion setting)
        /// </summary>
        /// <param name="seekableStream">The Seekable stream to load the font information from</param>
        /// <param name="source">The original source for the stream (for reference identification only)</param>
        /// <returns>The typeface information for the stream</returns>
        protected virtual ITypefaceInfo DoReadTypeface(Stream seekableStream, string source)
        {
            using (var reader = new BigEndianReader(seekableStream))
            {
                TypefaceVersionReader version;
                if (!TypefaceVersionReader.TryGetVersion(reader, out version))
                    throw new TypefaceReadException("Could not identify the version of the font for the source " + (source ?? ""));

                else
                {
                    var info = version.ReadTypefaceInfoAfterVersion(reader, source);
                    if (null == info)
                        throw new TypefaceReadException("Could not extract the information for the font from the source " + (source ?? ""));

                    else if (!string.IsNullOrEmpty(info.ErrorMessage))
                        throw new TypefaceReadException("Could not extract the information for the font from the source " + (source ?? "") + ": " + info.ErrorMessage);

                    return info;
                }
            }
        }

        #endregion


        #region protected virtual bool DoTryReadTypeface(Stream seekableStream, string source, out ITypefaceInfo info)

        /// <summary>
        /// Trys to get the specific typeface Information from the provided stream, which must be seekable (supports the Postion setting)
        /// </summary>
        /// <param name="seekableStream">The Seekable stream to load the font information from</param>
        /// <param name="source">The original source for the stream (for reference identification only)</param>
        /// <param name="info">Set to the typeface information with References to specific font faces within if successful</param>
        /// <returns>True if reading of the stream was successful</returns>
        protected virtual bool DoTryReadTypeface(Stream seekableStream, string source, out ITypefaceInfo info)
        {
            using (var reader = new BigEndianReader(seekableStream))
            {
                TypefaceVersionReader version;
                if (!TypefaceVersionReader.TryGetVersion(reader, out version))
                {
                    info = new Utility.UnknownTypefaceInfo(source, "Could not identify the typeface version");
                    return false;
                }
                else
                {
                    bool success = false;
                    info = null;

                    try
                    {
                        info = version.ReadTypefaceInfoAfterVersion(reader, source);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        info = new Utility.UnknownTypefaceInfo(source, "The typeface information could not be read : " + ex.Message);
                        success = false;
                    }
                    return success;
                }
            }
        }

        #endregion

        //
        // IDisposeable Implementation
        //

        #region protected virtual void Dispose(bool disposing)

        /// <summary>
        /// Dispose method that inheritors can override to add their own implelmentation, but should call the base method.
        /// </summary>
        /// <param name="disposing">If true then we are disposing, otherwise we are finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (null != _loader)
                    _loader.Dispose();
            }
            _loader = null;
        }

        #endregion

        #region public void Dispose()

        /// <summary>
        /// Disposes of this reader, and any owned resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #region ~TypefaceReader()

        /// <summary>
        /// Finalizer method (calls dispose false)
        /// </summary>
        ~TypefaceReader()
        {
            this.Dispose(false);
        }

        #endregion

    }

}
