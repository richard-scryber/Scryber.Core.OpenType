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
                files = directory.GetFiles(matchExtension);
            else
                files = directory.GetFiles();

            addTo.AddRange(files);

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
