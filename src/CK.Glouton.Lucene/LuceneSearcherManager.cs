using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;

namespace CK.Glouton.Lucene
{
    public class LuceneSearcherManager
    {
        private LuceneConfiguration _configuration;
        private readonly Dictionary<string, IndexReader> _readers;

        public LuceneSearcherManager( LuceneConfiguration configuration )
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
                var directoryInfo = new System.IO.DirectoryInfo( _configuration.Path );
                var dirs = new HashSet<string>();

                foreach( var info in directoryInfo.GetDirectories() )
                {
                    if( IndexExists( info.Name ) )
                        dirs.Add( info.Name );
                }

                return dirs;
            }
        }

        /// <summary>
        /// Get a new <see cref="LuceneSearcher"/> that gonna search on all app names index.
        /// </summary>
        /// <param name="appNames"></param>
        /// <returns></returns>
        public LuceneSearcher GetSearcher( params string[] appNames )
        {
            var readers = new List<IndexReader>();
            foreach( var appName in appNames )
            {
                if( !IndexExists( appName ) )
                    continue;
                var reader = GetReader( appName );
                if( reader != null )
                    readers.Add( reader );
            }

            return readers.Count == 0 ? null : new LuceneSearcher( new MultiReader( readers.ToArray() ) );
        }

        private IndexReader GetReader( string appName )
        {
            if( _readers.ContainsKey( appName ) )
            {
                UpdateReader( appName );
                return _readers[ appName ];
            }

            IndexReader reader;
            try
            {
                reader = DirectoryReader.Open( GetDirectory( appName ) );
            }
            catch( Exception )
            {
                return null;
            }

            _readers.Add( appName, reader );
            return reader;
        }

        /// <summary>
        /// An <see cref="IndexReader"/> is an instance of the index at a given point in time
        /// We need to update this Reader by reopen the <see cref="IndexReader"/>
        /// Maybe change this method later ?
        /// </summary>
        /// <param name="appName"></param>
        private void UpdateReader( string appName )
        {
            _readers[ appName ] = DirectoryReader.OpenIfChanged( _readers[ appName ] as DirectoryReader ) ?? _readers[ appName ];
        }

        private Directory GetDirectory( string appName )
        {
            return FSDirectory.Open( _configuration.Path + "\\" + appName );
        }

        private bool IndexExists (string appname)
        {
            return DirectoryReader.IndexExists( GetDirectory( appname ) );
        }
    }
}
