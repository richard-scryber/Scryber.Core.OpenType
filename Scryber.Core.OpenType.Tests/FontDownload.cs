using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Scryber.Core.OpenType.Tests
{
    public class FontDownload : IDisposable
    {
        private string LocalDirectory;
        private HttpClient Http = new HttpClient();

        public FontDownload(string rootPath)
        {
            this.LocalDirectory = rootPath;
        }

        

        public async Task<byte[]> LoadFrom(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            else if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                return await DoDownloadAsync(path);
            }
            else
            {
                path = System.IO.Path.Combine(LocalDirectory, path);
                path = System.IO.Path.GetFullPath(path);

                if (!System.IO.File.Exists(path))
                    throw new ArgumentException("The font file could not be found at path " + path);

                return await DoReadFile(path);
            }
        }

        protected async Task<byte[]> DoReadFile(string path)
        {
            return await System.IO.File.ReadAllBytesAsync(path);
        }
        
        
        protected async Task<byte[]> DoDownloadAsync(string path)
        {
            return await Http.GetByteArrayAsync(path);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != this.Http)
                    this.Http.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }


        ~FontDownload()
        {
            this.Dispose(false);
        }
    }
}
