using System;
using System.IO;

namespace CK.Glouton.Lucene
{
    public class LuceneConstant
    {
        internal const int MaxSearch = 10;

        /// <summary>
        /// Test in the directory of the indexer.
        /// On windows in appdata, in other os in home.
        /// If he doesn't find anything create the directory.
        /// </summary>
        public static string GetPath()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Glouton", "Logs", "Default");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Test in the directory of the indexer.
        /// On windows in appdata, in other os in home.
        /// If he doesn't find anything create the directory.
        /// </summary>
        public static string GetPath(string dirName)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Glouton", "Logs", dirName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
