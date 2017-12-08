using CK.Glouton.Model.Logs;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Glouton.Lucene
{
    public class LuceneSearcherManager
    {
        private LuceneConfiguration _configuration;
        private readonly Dictionary<string, IndexReader> _readers;

        public LuceneSearcherManager(LuceneConfiguration configuration)
        {
            _configuration = configuration;
            _readers = new Dictionary<string, IndexReader>();
        }

        /// <summary>
        /// Get all app name indexed with Lucene.
        /// </summary>
        public ISet<string> AppName
        {
            get
            {
                var directoryInfo = new System.IO.DirectoryInfo(_configuration.Path);
                var dirs = new HashSet<string>();

                foreach (var info in directoryInfo.GetDirectories())
                    dirs.Add(info.Name);

                return dirs;
            }
        }

        /// <summary>
        /// Get a new <see cref="LuceneSearcher"/> that gonna search on all app names index.
        /// </summary>
        /// <param name="appNames"></param>
        /// <returns></returns>
        public LuceneSearcher GetSearcher (params string[] appNames)
        {
            List<IndexReader> readers = new List<IndexReader>();
            foreach (string appName in appNames)
            {
                bool coucou = System.IO.Directory.Exists(_configuration.Path + "\\" + appName);
                if (!System.IO.Directory.Exists(_configuration.Path + "\\" + appName))
                    continue;
                readers.Add(GetReader(appName));
            }
            return new LuceneSearcher(new MultiReader(readers.ToArray()));
        }

        private IndexReader GetReader (string appName)
        {
            if (_readers.ContainsKey(appName))
            {
                UpdateReader(appName);
                return _readers[appName];
            }

            IndexReader reader = DirectoryReader.Open(GetDirectory(appName));
            _readers.Add(appName,  reader);
            return reader;
        }

        /// <summary>
        /// An <see cref="IndexReader"/> is an instance of the index at a given point in time
        /// We need to update this Reader by reopen the <see cref="IndexReader"/>
        /// Maybe change this method later ?
        /// </summary>
        /// <param name="appName"></param>
        private void UpdateReader (string appName)
        {
            _readers[appName] = DirectoryReader.Open(GetDirectory(appName));
        }

        private Directory GetDirectory (string appName)
        {
            return FSDirectory.Open(_configuration.Path + "\\" + appName);
        }
    }
}
