using System;
using System.Collections.Generic;
using CK.Glouton.Lucene;
using CK.Glouton.Model;
using System.IO;

namespace CK.Glouton.Service
{
    public class LuceneSearcherService : ILuceneSearcherService
    {
        public List<ILogViewModel> Search( string directory, string query )
        {
            if( query == "*" )
                return GetAll( directory, 25 );

            var result = new List<ILogViewModel>();
            var luceneSearcher = new LuceneSearcher( LuceneConstant.GetPath(directory) ,new[] { "LogLevel", "Exception" } );
            var hits = luceneSearcher.Search( query );

            foreach( var scoreDocument in hits.ScoreDocs )
            {
                var document = luceneSearcher.GetDocument( scoreDocument );
                switch( document.Get( "LogType" ) )
                {
                    case "OpenGroup":
                        result.Add( OpenGroupViewModel.Get( luceneSearcher, document ) );
                        break;
                    case "Line":
                        result.Add( LineViewModel.Get( luceneSearcher, document ) );
                        break;
                    case "CloseGroup":
                        result.Add( CloseGroupViewModel.Get( luceneSearcher, document ) );
                        break;
                    default:
                        throw new ArgumentException( nameof( document ) );
                }
            }
            return result;
        }

        public List<ILogViewModel> GetAll( string directory, int max )
        {
            var result = new List<ILogViewModel>();
            var searcher = new LuceneSearcher( LuceneConstant.GetPath(directory) ,new[] { "LogLevel" } );
            var hits = searcher.GetAllLog( max );

            if (hits == null)
                return null;

            foreach( var scoreDoc in hits.ScoreDocs )
            {
                var document = searcher.GetDocument( scoreDoc );
                switch( document.Get( "LogType" ) )
                {
                    case "OpenGroup":
                        result.Add( OpenGroupViewModel.Get( searcher, document ) );
                        break;
                    case "Line":
                        result.Add( LineViewModel.Get( searcher, document ) );
                        break;
                    case "CloseGroup":
                        result.Add( CloseGroupViewModel.Get( searcher, document ) );
                        break;
                    default:
                        throw new ArgumentException( nameof( document ) );
                }
            }
            return result;
        }

        public List<ILogViewModel> GetLogWithFilters( string monitorId, string appName, DateTime start, DateTime end, string[] fields, string[] logLevel, string keyword )
        {
            var result = new List<ILogViewModel>();
            var searcher = new LuceneSearcher( new[] { "LogLevel" } );
            var hits = searcher.Search( searcher.CreateQuery( monitorId, appName, fields, logLevel, start, end, keyword ) );

            foreach( var scoreDoc in hits.ScoreDocs )
            {
                var doc = searcher.GetDocument( scoreDoc );
                switch( doc.Get( "LogType" ) )
                {
                    case "OpenGroup":
                        result.Add( OpenGroupViewModel.Get( searcher, doc ) );
                        break;
                    case "Line":
                        result.Add( LineViewModel.Get( searcher, doc ) );
                        break;
                    case "CloseGroup":
                        result.Add( CloseGroupViewModel.Get( searcher, doc ) );
                        break;
                    default:
                        throw new ArgumentException( nameof( doc ) );
                }
            }
            return result;

        }

        public ISet<string> GetMonitorIdList()
        {
            return new LuceneSearcher( new string[] { } ).MonitorIdList;
        }

        /// <summary>
        /// Return of the App Name indexed by Lucene.
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetAppNameList()
        {
            DirectoryInfo dir = new DirectoryInfo(LuceneConstant.GetPath());
            HashSet<string> dirs = new HashSet<string>();

            foreach (DirectoryInfo d in dir.GetDirectories())
                dirs.Add(d.Name);

            return dirs;
        }
    }
}
