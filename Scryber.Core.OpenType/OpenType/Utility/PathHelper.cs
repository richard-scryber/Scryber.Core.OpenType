using System;
using System.IO;
using System.Collections.Generic;

namespace Scryber.OpenType.Utility
{
    public static class PathHelper
    {

        internal static int MatchFiles(List<FileInfo> addTo, DirectoryInfo directory, string matchExtension, bool includeSubdirectories)
        {
            int start = addTo.Count;

            FileInfo[] files;
            if (!string.IsNullOrEmpty(matchExtension))
            {
                if (matchExtension.IndexOf('|') > 0)
                {
                    //Special case to support multiple matches - recurrsively calls itself
                    return MatchFiles(addTo, directory, matchExtension.Split('|'), includeSubdirectories);
                }
                else
                {
                    files = directory.GetFiles(matchExtension);
                    addTo.AddRange(files);
                }
            }
            else
            {
                files = directory.GetFiles();
                addTo.AddRange(files);
            }

            if (includeSubdirectories)
            {
                DirectoryInfo[] directories = directory.GetDirectories();
                if (null != directories && directories.Length > 0)
                {
                    foreach (var child in directories)
                    {
                        MatchFiles(addTo, child, matchExtension, includeSubdirectories);
                    }
                }
            }

            return addTo.Count - start;
        }

        /// <summary>
        /// Private implementation to support multiple extensions
        /// </summary>
        /// <param name="addTo"></param>
        /// <param name="directory"></param>
        /// <param name="matchExtension"></param>
        /// <param name="includeSubdirectories"></param>
        /// <returns></returns>
        private static int MatchFiles(List<FileInfo> addTo, DirectoryInfo directory, string[] matchExtension, bool includeSubdirectories)
        {
            int start = addTo.Count;

            FileInfo[] files;
            foreach (var ext in matchExtension)
            {
                files = directory.GetFiles(ext.Trim());

                if (files != null && files.Length > 0)
                    addTo.AddRange(files);
            }

            if (includeSubdirectories)
            {
                DirectoryInfo[] directories = directory.GetDirectories();
                if (null != directories && directories.Length > 0)
                {
                    foreach (var child in directories)
                    {
                        MatchFiles(addTo, child, matchExtension, includeSubdirectories);
                    }
                }
            }

            return addTo.Count - start;
        }
    }
}
