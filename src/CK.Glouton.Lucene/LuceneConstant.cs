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
        /// <param name="dirName">The name of the directory where the data will be indexed</param>
        /// </summary>
        public static string GetPath(string dirName)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Glouton", "Logs", dirName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Test in the directory of the indexer.
        /// On windows in appdata, in other os in home.
        /// If he doesn't find anything create the directory.
        /// </summary>
        /// <param name="dirNames"> the string array that contain the AppName, the name of the machine executing the app and the app GUID </param>
        /// <returns></returns>
        public static string GetPath(string[] dirNames)
        {
            if (dirNames.Length > 3) throw new ArgumentException("The array is too big.", nameof(dirNames));
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Glouton", "Logs", dirNames[0], dirNames[1], dirNames[2]);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
