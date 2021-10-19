using System;
using System.IO;

using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

#if NET48

using System.Net;
using http = System.Net.WebClient;

#else

using System.Net.Http;
using http = System.Net.Http.HttpClient;

#endif

namespace Scryber.OpenType.Utility
{
    /// <summary>
    /// Provides a generic method for opening streams for reading typefaces
    /// </summary>
    public abstract class StreamLoader : IDisposable
    {
        private http _client;
        

        /// <summary>
        /// Returns the client this instance should use for web / http requests
        /// </summary>
        public http Client
        {
            get { return _client; }
        }


        private bool _ownsClient;

        /// <summary>
        /// Returns true if this loader owns the client (and will dispose of it when the loader is disposed)
        /// </summary>
        public bool OwnsClient
        {
            get { return _ownsClient; }
        }

        // .ctor

        #region public StreamLoader(HttpClient client = null)

        /// <summary>
        /// Creates a new streamloader with an optional HttpClient instance
        /// </summary>
        /// <param name="client"></param>
        public StreamLoader(http client = null)
        {
            this._client = client;
            this._ownsClient = false;
        }

        #endregion

        //
        // String Implementation methods
        // inheritors should implement/override these to support string paths (relative or absolute)
        // As the base class cannot deterimine if these should be urls or files
        //


        /// <summary>
        /// Synchronously returns a stream from the relative or absolute path.
        /// If relative then the loader must have a eaither a base directory or Url
        /// </summary>
        /// <param name="path">The path to load from - relative or absolute.</param>
        /// <param name="ensureSeekable">If true then the loader must make sure the stream returned supports seeking (e.g. Position = 10)</param>
        /// <returns>A stream from the data loaded at the specified path</returns>
        public abstract Stream GetStream(string path, bool ensureSeekable);

        /// <summary>
        /// Asynchronously returns a stream from the relative or absolute path.
        /// If relative then the loader must have a eaither a base directory or Url
        /// </summary>
        /// <param name="path">The path to load from - relative or absolute.</param>
        /// <param name="ensureSeekable">If true then the loader must make sure the stream returned supports seeking (e.g. Position = 10)</param>
        /// <returns>A stream from the data loaded at the specified path</returns>
        public virtual async Task<Stream> GetStreamAsync(string path, bool ensureSeekable)
        {
            return GetStream(path, ensureSeekable);
        }

        //
        // File Implementation methods
        //


        #region public virtual Stream GetStream(FileInfo file, bool ensureSeekable)

        /// <summary>
        /// Syncronously loads a stream from a rooted file path, optionally making sure it is a seekable stream
        /// </summary>
        /// <param name="file">The file to load - cannot be null and must exist</param>
        /// <param name="ensureSeekable">If true then the returning stream will alwasy been seekable (can set position)</param>
        /// <returns></returns>
        public virtual Stream GetStream(FileInfo file, bool ensureSeekable)
        {
            if (null == file)
                throw new ArgumentNullException(nameof(file));

            file = EnsureAbsolute(file);

            if (!file.Exists)
                throw new FileNotFoundException("The file at path " + file.FullName + " could not be found.");

            Stream reader = file.OpenRead();

            if (ensureSeekable && !reader.CanSeek)
                reader = CopyToSeekableStream(reader, true);

            return reader;
        }

        #endregion

        #region public virtual async Task<Stream> GetStreamAsync(FileInfo file, bool ensureSeekable) + 1 Overload

        /// <summary>
        /// Loads a stream asyncronously from a rooted file path, optionally making sure it is a seekable stream
        /// </summary>
        /// <param name="file">The file to load - cannot be null and must exist</param>
        /// <param name="ensureSeekable">If true then the returning stream will alwasy been seekable (can set position)</param>
        /// <returns></returns>
        public virtual async Task<Stream> GetStreamAsync(FileInfo file, bool ensureSeekable)
        {
            //No async support, but added for completeness of abstraction
            return GetStream(file, ensureSeekable);
        }

        #endregion

        #region public bool TryGetStream(FileInfo path, bool ensureSeekable, out Stream stream, out string message)

        /// <summary>
        /// Syncronously attempts to load a stream from a file, setting the stream and returning true if successful.
        /// </summary>
        /// <param name="path">The file to load</param>
        /// <param name="ensureSeekable">If true, then, if the captured stream is not seekable, it will be converted to a seekable stream before returning</param>
        /// <param name="stream">Set to the opened stream if successful, otherwise null</param>
        /// <param name="message">Set to any errors</param>
        /// <returns>True if the stream to the file could be opened</returns>
        public bool TryGetStream(FileInfo path, bool ensureSeekable, out Stream stream, out string message)
        {
            bool result = false;
            stream = null;
            FileInfo absolute;

            if (null == path)
            {
                stream = null;
                message = "The file info is null";
            }
            else if (!TryEnsureAbsolute(path, out absolute))
            {
                stream = null;
                message = "The provided file path '" + path.FullName + "' could not be resolved to a file";
            }
            else if(!absolute.Exists)
            {
                stream = null;
                message = "The file at the provided path '" + path.FullName + "' does not exist";
            }
            else
            {
                try
                {
                    stream = GetStream(absolute, ensureSeekable);
                    message = (null == stream) ? "No errors, but the returned stream was null" : null;
                    result = (null != stream);
                }
                catch (Exception ex)
                {
                    if (stream != null)
                        stream.Dispose();
                    stream = null;
                    message = "Could not open the stream : " + ex.Message;
                    result = false;
                }
            }
            return result;
        }

        #endregion

        #region protected virtual FileInfo EnsureAbsolute(FileInfo file)

        /// <summary>
        /// Makes sure the file provided is rooted. Inheritors can override to provide their own implementation
        /// </summary>
        /// <param name="file">The file path to check</param>
        /// <returns>The absolute file info if rooted</returns>
        /// <exception cref="ArgumentOutOfRangeException" >Thrown if the file is not rooted</exception>
        protected virtual FileInfo EnsureAbsolute(FileInfo file)
        {
            if (!IsRootedFile(file.FullName, out file))
                throw new ArgumentOutOfRangeException(nameof(file), "The file '" + file.FullName + "' is not a rooted path");

            return file;
        }

        #endregion

        #region protected virtual bool TryEnsureAbsolute(FileInfo file, out FileInfo absolute)

        /// <summary>
        /// Checks to make sure the stream is absolute. Inheritors can override the implementation
        /// </summary>
        /// <param name="file"></param>
        /// <param name="absolute"></param>
        /// <returns></returns>
        protected virtual bool TryEnsureAbsolute(FileInfo file, out FileInfo absolute)
        {
            if (!IsRootedFile(file.FullName, out absolute))
                return false;
            else
                return true;
        }

        #endregion

        //
        // Uri Implementation methods
        //

        #region public virtual Stream GetStream(Uri file, bool ensureSeekable)

        /// <summary>
        /// Syncronously loads a stream from an absolute Url, or a relative url for rooted base urls, optionally making sure it is a seekable stream
        /// </summary>
        /// <param name="uri">The uri to load - cannot be null and must be absolute and exist</param>
        /// <param name="ensureSeekable">If true then the returning stream will alwasy been seekable (can set position)</param>
        /// <returns></returns>
        public virtual Stream GetStream(Uri uri, bool ensureSeekable)
        {
            if (null == uri)
                throw new ArgumentNullException(nameof(uri));

            uri = EnsureAbsolute(uri);

            var client = this.GetHttpClient();

            Stream stream;

            //In old school .net use the DownloadData with a web client
#if NET48
            byte[] data = client.DownloadData(uri);
            stream = new System.IO.MemoryStream(data);
            //No need to check for seek.
#else
            stream = client.GetStreamAsync(uri).Result;

            if (ensureSeekable && !stream.CanSeek)
                stream = CopyToSeekableStream(stream, true);
#endif
            return stream;
        }


        #endregion

        #region public bool TryGetStream(Uri forUrl, bool ensureSeekable, out Stream stream, out string message)

        /// <summary>
        /// Syncronously attempts to load a stream from a relative (from the loaders base url) or absolute url, setting the stream and returning true if successful.
        /// </summary>
        /// <param name="forUrl">The absolute url, or a url relative to the base url</param>
        /// <param name="ensureSeekable">If true, then, if the captured stream is not seekable, it will be converted to a seekable stream before returning</param>
        /// <param name="stream">Set to the opened stream if successful, otherwise null</param>
        /// <param name="message">Set to any errors</param>
        /// <returns>True if the stream to the url could be opened</returns>
        public bool TryGetStream(Uri forUrl, bool ensureSeekable, out Stream stream, out string message)
        {
            bool result = false;
            Uri resolved;

            stream = null;

            if(null == forUrl)
            {
                stream = null;
                message = "The url is null";
            }
            else if(!TryEnsureAbsolute(forUrl, out resolved))
            {
                stream = null;
                message = "The provided relative url '" + forUrl.ToString() + "' could not be resolved to an absolute url";
            }
            else
            {
                try
                {
                    stream = GetStream(resolved, ensureSeekable);
                    message = (null == stream) ? "No errors, but the returned stream was null" : null;
                    result = (null != stream);
                }
                catch(Exception ex)
                {
                    if (stream != null)
                        stream.Dispose();
                    stream = null;
                    message = "Could not open the stream : " + ex.Message;
                    result = false;
                }
            }
            return result;
        }

        #endregion

        #region public virtual async Task<Stream> GetStreamAsync(Uri uri, bool ensureSeekable)

        /// <summary>
        /// Loads a stream asyncronously from an Absolute Url, optionally making sure it is a seekable stream
        /// </summary>
        /// <param name="uri">The uri to load - cannot be null and must be absolute and exist</param>
        /// <param name="ensureSeekable">If true then the returning stream will alwasy been seekable (can set position)</param>
        /// <returns></returns>
        public virtual async Task<Stream> GetStreamAsync(Uri uri, bool ensureSeekable)
        {
            if (null == uri)
                throw new ArgumentNullException(nameof(uri));

            uri = EnsureAbsolute(uri);

            var client = this.GetHttpClient();

            Stream stream;

            //In old school .net use the DownloadData with a web client
#if NET48
            byte[] data = await client.DownloadDataTaskAsync(uri);
            stream = new System.IO.MemoryStream(data);

#else
            stream = await client.GetStreamAsync(uri);

            if (ensureSeekable && !stream.CanSeek)
                stream = CopyToSeekableStream(stream, true);
#endif
            return stream;
        }

        #endregion

        #region protected virtual Uri EnsureAbsolute(Uri uri)

        /// <summary>
        /// Makes sure the uri provided is rooted. Inheritors can override to provide their own implementation
        /// </summary>
        /// <param name="file">The uri path to check</param>
        /// <returns>The absolute uri info if rooted</returns>
        /// <exception cref="ArgumentOutOfRangeException" >Thrown if the uri is not rooted</exception>
        protected virtual Uri EnsureAbsolute(Uri uri)
        {
            if (!IsRootedUri(uri.ToString(), out uri))
                throw new InvalidOperationException("The file '" + uri.ToString() + "' is not a rooted path");

            return uri;
        }

        #endregion

        #region protected virtual bool TryEnsureAbsolute(Uri uri, out Uri absolute)

        /// <summary>
        /// Checks to make sure the stream is absolute. Inheritors can override the implementation
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="absolute"></param>
        /// <returns></returns>
        protected virtual bool TryEnsureAbsolute(Uri uri, out Uri absolute)
        {
            if (!IsRootedUri(uri.ToString(), out uri))
            {
                absolute = null;
                return false;
            }
            else
            {
                absolute = uri;
                return true;
            }
        }

        #endregion

        //
        // support methods
        //


        #region protected HttpClient GetHttpClient()

        /// <summary>
        /// returns an HttpClient that can be used for
        /// loading data from a remote Uri either synchronously or async
        /// </summary>
        /// <returns></returns>
        /// <remarks>This can be set on the constructor, and if so, a new one will not be created.
        /// If one is created, then it will be owned and disposed when this instance is disposed.</remarks>
        protected http GetHttpClient()
        {
            if(null == this._client)
            {
                this._client = new http();
                _ownsClient = true;
            }

            return this._client;
        }


        #endregion

        #region protected Stream CopyToSeekableStream(Stream baseStream, bool disposeBase)

        /// <summary>
        /// Takes an existing stream and copies the content to new seekable stream, optionally disposing of the original base stream
        /// </summary>
        /// <param name="baseStream"></param>
        /// <param name="disposeBase"></param>
        /// <returns></returns>
        protected Stream CopyToSeekableStream(Stream baseStream, bool disposeBase)
        {
            var ms = new MemoryStream();

            try
            {
                baseStream.CopyTo(ms);
                ms.Flush();
                ms.Position = 0;
            }
            finally
            {
                if (disposeBase)
                    baseStream.Dispose();
            }

            return ms;
        }

        #endregion

        //
        // Disposable implementation
        //

        #region protected virtual void Dispose(bool disposing)

        /// <summary>
        /// Primary disposal method to release reasources, inheritors should override to
        /// dispose of any unmanaged resources, calling the base implementation afterwards.
        /// </summary>
        /// <param name="disposing">true if this was called from the public Dispose method</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != _client && _ownsClient)
                    _client.Dispose();
            }
            _client = null;

        }

        #endregion

        #region public void Dispose()

        /// <summary>
        /// Disposes of this loader along with any owned unmanaged resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #region ~StreamLoader()

        /// <summary>
        /// Finalizer to relase references without explicitly disposing
        /// </summary>
        ~StreamLoader()
        {
            this.Dispose(false);
        }

        #endregion

        //
        // static methods
        //

        #region public static bool IsRootedUri(string path, out Uri uri)

        /// <summary>
        /// Returns true if the path provided is an absolute url
        /// </summary>
        /// <param name="path"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool IsRootedUri(string path, out Uri uri)
        {
            if(Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                uri = new Uri(path);
                return true;
            }
            else
            {
                uri = null;
                return false;
            }
        }

        #endregion

        #region public static bool IsRootedDirectory(string path, out DirectoryInfo dir)

        /// <summary>
        /// Returns true if the path provided is a rooted file directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool IsRootedDirectory(string path, out DirectoryInfo dir)
        {
            if(System.IO.Path.IsPathRooted(path))
            {
                dir = new DirectoryInfo(path);
                return true;
            }
            else
            {
                dir = null;
                return false;
            }
        }

        #endregion

        #region public static bool IsRootedFile(string path, out FileInfo dir)

        /// <summary>
        /// Returns true if the path provided is a rooted file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsRootedFile(string path, out FileInfo file)
        {
            if (System.IO.Path.IsPathRooted(path))
            {
                file = new FileInfo(path);
                return true;
            }
            else
            {
                file = null;
                return false;
            }
        }

        #endregion

        #region public static StreamLoader GetLoaderForPath(string unknownPath)

        /// <summary>
        /// Based on a string path, this method will attempt to discover if the path is empty (Unbased) the path is a rooted Uri, or rooted Directory.
        /// And return the appropriacte concrete stream loader for the path.
        /// </summary>
        /// <param name="unknownPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException" >Thrown if the path is not a rooted url or a rooted directory.</exception>
        public static StreamLoader GetLoaderForPath(string unknownPath)
        {
            if (string.IsNullOrEmpty(unknownPath))
                return new UnRootedStreamLoader();

            else if (IsRootedUri(unknownPath, out Uri uri))
                return new RootedUriStreamLoader(uri);

            else if (IsRootedDirectory(unknownPath, out DirectoryInfo dir))
                return new RootedFileStreamLoader(dir);

            else
                throw new ArgumentException("The provided path" + unknownPath + " is not a recognisable absolute url or rooted directory path. Cannot determine the base path");

        }

        #endregion


    }



    /// <summary>
    /// Implements the StreamLoader with a known base Url
    /// </summary>
    public class RootedUriStreamLoader : StreamLoader
    {
        private Uri _baseUri;

        public Uri BaseUri
        {
            get { return _baseUri; }
            set
            {
                if (null == value)
                    throw new ArgumentNullException("The base uri cannot be null");

                if (!value.IsAbsoluteUri)
                    throw new ArgumentException("The baseUri must be an absolute url");

                _baseUri = value;
            }
        }
        

        public RootedUriStreamLoader(Uri uri)
            : this(uri, null)
        {
        }

        public RootedUriStreamLoader(Uri uri, http client)
            : base(client)
        {
            this.BaseUri = uri;
        }

        public override Stream GetStream(string path, bool ensureSeekable)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Uri full;
            if (!IsRootedUri(path, out full))
                full = CombineUri(this._baseUri, path);

            return GetStream(full, ensureSeekable);
            
        }


        public override async Task<Stream> GetStreamAsync(string path, bool ensureSeekable)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Uri full;
            if (!IsRootedUri(path, out full))
                full = CombineUri(this._baseUri, path);

            return await GetStreamAsync(full, ensureSeekable);
        }


        public Uri CombineUri(Uri baseUri, string path)
        {
            
            var full = baseUri.ToString();
            if (full.EndsWith("/") || path.StartsWith("/"))
                return new Uri(full + path);
            else if (full.EndsWith("\\") || full.StartsWith("\\"))
                return new Uri(full + path);
            else
                return new Uri(full + "/" + path);
        }

        protected override Uri EnsureAbsolute(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                uri = CombineUri(this.BaseUri, uri.ToString());

            return base.EnsureAbsolute(uri);
        }

        protected override bool TryEnsureAbsolute(Uri uri, out Uri absolute)
        {
            if (!uri.IsAbsoluteUri)
                uri = CombineUri(this.BaseUri, uri.ToString());

            return base.TryEnsureAbsolute(uri, out absolute);
        }


    }


    /// <summary>
    /// Implements the StreamLoader with a known base directory
    /// </summary>
    public class RootedFileStreamLoader : StreamLoader
    {
        private DirectoryInfo _baseDirectory;

        public DirectoryInfo BaseDirectory
        {
            get { return _baseDirectory; }
            set { _baseDirectory = value; }
        }

        public RootedFileStreamLoader(DirectoryInfo dir)
            : base(null)
        {
            if (null == dir)
                throw new ArgumentNullException("The base directory cannot be null");

            this._baseDirectory = dir;
        }

        public override Stream GetStream(string path, bool ensureSeekable)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            FileInfo full;
            if (!IsRootedFile(path, out full))
                full = CombinePath(this._baseDirectory, path);

            return GetStream(full, ensureSeekable);

        }


        public override async Task<Stream> GetStreamAsync(string path, bool ensureSeekable)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            FileInfo full;
            if (!IsRootedFile(path, out full))
                full = CombinePath(this._baseDirectory, path);

            return await GetStreamAsync(full, ensureSeekable);
        }

        public FileInfo CombinePath(DirectoryInfo baseDir, string path)
        {
            var full = baseDir.FullName;
            full = Path.Combine(full, path);
            full = Path.GetFullPath(full);

            return new FileInfo(full);
        }

        protected override FileInfo EnsureAbsolute(FileInfo file)
        {
            if (!IsRootedFile(file.FullName, out file))
                file = CombinePath(this._baseDirectory, file.FullName);

            return base.EnsureAbsolute(file);
        }

        protected override bool TryEnsureAbsolute(FileInfo file, out FileInfo absolute)
        {
            if (!IsRootedFile(file.FullName, out absolute))
                file = CombinePath(this._baseDirectory, file.FullName);

            return base.TryEnsureAbsolute(file, out absolute);
        }

    }



    /// <summary>
    /// Implements the StreamLoader without a base.
    /// </summary>
    public class UnRootedStreamLoader : StreamLoader
    {

        public UnRootedStreamLoader() : this(null)
        {
        }

        public UnRootedStreamLoader(http client)
            : base(client)
        {
        }

        public override Stream GetStream(string path, bool ensureSeekable)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Uri uri;
            FileInfo file;

            if (IsRootedFile(path, out file))
                return GetStream(file, ensureSeekable);

            else if (IsRootedUri(path, out uri))
                return GetStream(uri, ensureSeekable);

            else
                throw new ArgumentOutOfRangeException("The path " + path + " was not recognised as a rooted file path, or absolute url");

        }


        public override async Task<Stream> GetStreamAsync(string path, bool ensureSeekable)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Uri uri;
            FileInfo file;

            if (IsRootedFile(path, out file))
                return await GetStreamAsync(file, ensureSeekable);

            else if (IsRootedUri(path, out uri))
                return await GetStreamAsync(uri, ensureSeekable);

            else
                throw new ArgumentOutOfRangeException("The path " + path + " was not recognised as a rooted file path, or absolute url");

        }


    }
}
