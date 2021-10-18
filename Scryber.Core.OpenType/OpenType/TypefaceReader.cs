using System;
using System.Collections.Generic;
using System.IO;

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
    /// Reads supported typefaces from files and streams. Start by reading the summary ITypefaceInfo for a file,
    /// and then read the ITypeface for a Reference returned for the file, as some typefaces can have multiple fonts within them 
    /// </summary>
    public class TypefaceReader : IDisposable
    {

        #region public Utility.StreamLoader Loader {get;}

        private StreamLoader _loader;

        /// <summary>
        /// Gets the stream loader associated with this reader
        /// </summary>
        public Utility.StreamLoader Loader
        {
            get { return _loader; }
        }

        #endregion

        //
        // .ctor
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
        // Directory loading
        //

        #region public ITypefaceInfo[] GetAllTypefaceInformation(System.IO.DirectoryInfo directory ...)

        /// <summary>
        /// Reads a summary of all the typeface files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to scan for typeface files (children will not be scanned unless the includeSubdirectories is true)</param>
        /// <param name="matchExtension">An optional file match pattern to look for files with e.g. *.ttf|*.otf. Default is empty / null so all files will be checked</param>
        /// <param name="includeSubdirectories">An optional flag to also look for files in a subdirectory. Default is false</param>
        /// <param name="registerErrors">An optional flag to include in the results any read errors for matched files. Default is false</param>
        /// <returns>An array of all the Typefaces found in the files with References to their inner font variation(s)</returns>
        public ITypefaceInfo[] GetAllTypefaceInformation(System.IO.DirectoryInfo directory, string matchExtension = null, bool includeSubdirectories = false, bool registerErrors = false)
        {
            if (!directory.Exists)
                throw new DirectoryNotFoundException("Directory '" + directory.FullName + "' could not be found");

            List<FileInfo> files = new List<FileInfo>();

            Utility.PathHelper.MatchFiles(files, directory, matchExtension, includeSubdirectories);

            return this.GetAllTypefaceInformation(files, registerErrors);
            
        }

        #endregion

        #region public ITypefaceInfo[] GetAllTypefaceInformation(IEnumerable<FileInfo> files.. 


        /// <summary>
        /// Reads the summary information for all the requested files.
        /// </summary>
        /// <param name="files">An enumerable collection of Files that can be scanned and information returned on.</param>
        /// <param name="registerErrors">An optional flag that if true will add any read errors to the returned information array</param>
        /// <returns>An array of all the Typefaces found in the files with References to their inner font variation(s)</returns>
        public ITypefaceInfo[] GetAllTypefaceInformation(IEnumerable<FileInfo> files, bool registerErrors = false)
        {
            if (null == files)
                throw new ArgumentNullException(nameof(files));

            List<ITypefaceInfo> found = new List<ITypefaceInfo>();

            foreach (var file in files)
            {
                ITypefaceInfo foundOne;

                using (var fs = _loader.GetStream(file, true))
                {
                    if (this.TryGetTypefaceInformation(fs, file.FullName, out foundOne))
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

        #region public ITypefaceInfo[] GetAllTypefaceInformation(IEnumerable<Uri> files.. 

        /// <summary>
        /// Reads the summary information for all the requested urls.
        /// </summary>
        /// <param name="absoluteUrls">An enumerable collection of full uri's that can be enumerated over inturn.</param>
        /// <param name="registerErrors">An optional flag that if true will add any read errors to the returned information array</param>
        /// <returns>An array of all the Typefaces found in the files with References to their inner font variation(s)</returns>
        public ITypefaceInfo[] GetAllTypefaceInformation(IEnumerable<Uri> absoluteUrls, bool registerErrors = false)
        {
            if (null == absoluteUrls)
                throw new ArgumentNullException(nameof(absoluteUrls));

            List<ITypefaceInfo> found = new List<ITypefaceInfo>();

            foreach (var url in absoluteUrls)
            {
                ITypefaceInfo foundOne;

                using (var fs = _loader.GetStream(url, true))
                {
                    if (this.TryGetTypefaceInformation(fs, url.ToString(), out foundOne))
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
        // Async Directory loading
        //

        #region public virtual ITypefaceInfo[] GetAllTypefaceInformationAsync(System.IO.DirectoryInfo directory ...)

        /// <summary>
        /// Reads a summary of all the typeface files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to scan for typeface files (children will not be scanned unless the includeSubdirectories is true)</param>
        /// <param name="matchExtension">An optional file match pattern to look for files with e.g. *.ttf|*.otf. Default is empty / null so all files will be checked</param>
        /// <param name="includeSubdirectories">An optional flag to also look for files in a subdirectory. Default is false</param>
        /// <param name="registerErrors">An optional flag include in the results any read errors for matched files. Default is false</param>
        /// <returns></returns>
        public virtual async Task<ITypefaceInfo[]> GetAllTypefaceInformationAsync(System.IO.DirectoryInfo directory, string matchExtension = null, bool includeSubdirectories = false, bool registerErrors = false)
        {
            if (!directory.Exists)
                throw new DirectoryNotFoundException("Directory '" + directory.FullName + "' could not be found");

            List<FileInfo> files = new List<FileInfo>();

            var info = await Task.Run(() =>
            {
                Utility.PathHelper.MatchFiles(files, directory, matchExtension, includeSubdirectories);

                return this.GetAllTypefaceInformation(files.ToArray(), registerErrors);
            });

            return info;
        }

        #endregion

        #region public virtual async Task<ITypefaceInfo[]> GetAllTypefaceInformationAsync(IEnumerable<FileInfo> files ...)

        /// <summary>
        /// Reads the summary information for all the requested files.
        /// </summary>
        /// <param name="files">An enumerable collection of Files that can be scanned and information returned on.</param>
        /// <param name="registerErrors">An optional flag that if true will add any read errors to the returned information array</param>
        /// <returns>An array of all the Typefaces found in the files with References to their inner font variation(s)</returns>
        public virtual async Task<ITypefaceInfo[]> GetAllTypefaceInformationAsync(IEnumerable<FileInfo> files, bool registerErrors = false)
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
                        if (this.TryGetTypefaceInformation(fs, file.FullName, out foundOne))
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

        #region public ITypefaceInfo[] GetAllTypefaceInformationAsync(IEnumerable<FileInfo> files.. 

        /// <summary>
        /// Reads the summary information for all the requested files asyncronously.
        /// </summary>
        /// <param name="files">An enumerable collection of Files that can be scanned and information returned on.</param>
        /// <param name="registerErrors">An optional flag that if true will add any read errors to the returned information array</param>
        /// <returns>An array of all the Typefaces found in the files with References to their inner font variation(s)</returns>
        public virtual async Task<ITypefaceInfo[]> GetAllTypefaceInformationAsync(IEnumerable<Uri> absoluteUrls, bool registerErrors = false)
        {
            if (null == absoluteUrls)
                throw new ArgumentNullException(nameof(absoluteUrls));

            List<ITypefaceInfo> found = new List<ITypefaceInfo>();

            await Task.Run(() =>
            {
                foreach (var url in absoluteUrls)
                {
                    ITypefaceInfo foundOne;

                    using (var fs = _loader.GetStream(url, true))
                    {
                        if (this.TryGetTypefaceInformation(fs, url.ToString(), out foundOne))
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



        //Uri Loading

        //Async Uri loading


        //string loading

        public ITypefaceInfo GetTypefaceInformation(string path)
        {
            using (var stream = _loader.GetStream(path, true))
            {
                return DoGetTypefaceInformation(stream, path);
            }
            
        }


        

        public bool TryGetTypefaceInformation(Stream seekableStream, string source, out ITypefaceInfo info)
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

            return DoTryGetTypefaceInformation(seekableStream, source, out info);
        }

        
        


        //
        // GetTypefaceInformation
        //

        public ITypefaceInfo GetTypefaceInformation(Stream seekableStream, string source)
        {
            if (null == seekableStream)
                throw new ArgumentNullException(nameof(seekableStream));

            if (seekableStream.CanSeek == false)
            {
                throw new ArgumentOutOfRangeException(nameof(seekableStream), "The provided stream cannot be positioned, use a FileStream or a MemoryStream if needed to create a seekable stream");
            }

            return DoGetTypefaceInformation(seekableStream, source);
        }

        /// <summary>
        /// Trys to get the specific typeface Information from the provided stream, which must be seekable (supports the Postion setting)
        /// </summary>
        /// <param name="seekableStream">The Seekable stream to load the font information from</param>
        /// <param name="source">The original source for the stream (for reference identification only)</param>
        /// <param name="info">Set to the typeface information with References to specific font faces within if successful</param>
        /// <returns>True if reading of the stream was successful</returns>
        protected virtual bool DoTryGetTypefaceInformation(Stream seekableStream, string source, out ITypefaceInfo info)
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

        protected virtual ITypefaceInfo DoGetTypefaceInformation(Stream seekableStream, string source)
        {
            using (var reader = new BigEndianReader(seekableStream))
            {
                TypefaceVersionReader version;
                if (!TypefaceVersionReader.TryGetVersion(reader, out version))
                    throw new TypefaceReadException("Could not identify the version of the font the source " + (source ?? ""));

                else
                {
                    var info = version.ReadTypefaceInfoAfterVersion(reader, source);
                    if (null == info)
                        throw new TypefaceReadException("Could not extract the information for the font source " + (source ?? ""));

                    else if (!string.IsNullOrEmpty(info.ErrorMessage))
                        throw new TypefaceReadException("Could not extract the information for the font source " + (source ?? "") + ": " + info.ErrorMessage);

                    return info;
                }
            }
        }


        // ITypeface returns

        public async Task<IEnumerable<ITypeface>> GetTypefacesAsync(Uri uri)
        {
            return null;
        }

        public virtual IEnumerable<ITypeface> GetTypefaces(Uri uri)
        {
            return null;
        }

        

        public async Task<IEnumerable<ITypeface>> GetTypefacesAsync(ITypefaceInfo forInfo)
        {
            if (null == forInfo)
                throw new ArgumentNullException(nameof(forInfo));

            if (forInfo.References == null)
                return new ITypeface[] { };

            if (forInfo.TypefaceCount == 0 && (forInfo.References == null || forInfo.References.Length == 0))
                return new ITypeface[] { };

            if (forInfo.TypefaceCount != forInfo.References.Length)
                throw new ArgumentOutOfRangeException(nameof(forInfo.TypefaceCount), "The number of references in the information does not match the specified TypefaceCount");

            if (string.IsNullOrEmpty(forInfo.Path))
                throw new ArgumentNullException(nameof(forInfo.Path), "The path must be set on the typeface info, so the reader can load the file");

            if (forInfo.TypefaceCount == 1)
            {
                using (var stream = await this._loader.GetStreamAsync(forInfo.Path, true))
                {
                    var one = GetTypeface(stream, forInfo.References[0]);
                    return new ITypeface[] { one };
                }
            }
            else
            {
                List<ITypeface> loaded = new List<ITypeface>();

                foreach (var reference in forInfo.References)
                {
                    using (var stream = await this._loader.GetStreamAsync(forInfo.Path, true))
                    {
                        var one = GetTypeface(stream, reference);
                        loaded.Add(one);
                    }
                }
                return loaded;
            }
        }

        public virtual IEnumerable<ITypeface> GetTypefaces(ITypefaceInfo forInfo)
        {
            if (null == forInfo)
                throw new ArgumentNullException(nameof(forInfo));

            if (forInfo.References == null)
                return new ITypeface[] { };

            if (forInfo.TypefaceCount == 0 && (forInfo.References == null || forInfo.References.Length == 0))
                return new ITypeface[] { };

            if (forInfo.TypefaceCount != forInfo.References.Length)
                throw new ArgumentOutOfRangeException(nameof(forInfo.TypefaceCount), "The number of references in the information does not match the specified TypefaceCount");

            if (string.IsNullOrEmpty(forInfo.Path))
                throw new ArgumentNullException(nameof(forInfo.Path), "The path must be set on the typeface info, so the reader can load the file");

            if(forInfo.TypefaceCount == 1)
            {
                return new ITypeface[]
                {
                    GetTypeface(forInfo, forInfo.References[0])
                };
            }
            else
            {
                List<ITypeface> loaded = new List<ITypeface>();

                foreach (var reference in forInfo.References)
                {
                    var one = GetTypeface(forInfo, reference);
                    loaded.Add(one);
                }
                return loaded;
            }
        }

        public async Task<ITypeface> GetTypefaceAsync(ITypefaceInfo info, ITypefaceReference forReference)
        {
            throw new NotImplementedException();
        }


        public virtual ITypeface GetTypeface(ITypefaceInfo info, ITypefaceReference forReference)
        {
            throw new NotImplementedException();
        }

        public virtual ITypeface GetTypeface(Uri uri, ITypefaceReference forReference)
        {
            throw new NotImplementedException();
        }

        public virtual ITypeface GetTypeface(FileInfo file, ITypefaceReference theReference)
        {
            if (null == file)
                throw new ArgumentNullException(nameof(file));

            if (!file.Exists)
                throw new FileNotFoundException("The file at path " + file.FullName + " no longer exists");

            using(var fs = file.OpenRead())
            {
                return GetTypeface(fs, theReference);
            }
        }


        public virtual ITypeface GetTypeface(Stream seekableStream, ITypefaceReference theReference)
        {
            if (null == seekableStream)
                throw new ArgumentNullException(nameof(seekableStream));

            if (seekableStream.CanSeek == false)
            {
                throw new ArgumentOutOfRangeException(nameof(seekableStream), "The provided stream cannot be positioned, use a FileStream or a MemoryStream if needed to create a seekable stream");
            }

            using (var reader = new BigEndianReader(seekableStream))
            {
                TypefaceVersionReader version;
                if (!TypefaceVersionReader.TryGetVersion(reader, out version))
                    throw new NullReferenceException("Could not identify the typeface in the provided stream");

                else
                {
                    var typeface = version.ReadTypefaceAfterVersion(reader, theReference);
                    
                    return typeface;
                }
            }

        }



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
