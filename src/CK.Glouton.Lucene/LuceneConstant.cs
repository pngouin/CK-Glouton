using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
#if !NET461
            string path = Path.Combine(Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "LocalAppData" : "Home"), "Glouton", "Logs");
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            return path;
#else
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Glouton", "Logs");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
#endif
        }
    }
}
