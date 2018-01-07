using CK.Glouton.Model.Lucene;
using Lucene.Net.Documents;
using System.Collections.Generic;
using System.Linq;

namespace CK.Glouton.Model.Logs
{
    public class ExceptionViewModel : IExceptionViewModel
    {
        public IInnerExceptionViewModel InnerException { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public List<IExceptionViewModel> AggregatedExceptions { get; set; }

        public static IExceptionViewModel Get( ILuceneSearcher searcher, Document doc )
        {
            if( doc.GetField( LogField.EXCEPTION ) == null )
                return null;

            var exception = searcher.GetDocument( LogField.INDEX_DTS, doc.Get( LogField.EXCEPTION ), 999 );

            return new ExceptionViewModel
            {
                Message = exception.Get( LogField.MESSAGE ),
                StackTrace = exception.Get( LogField.STACKTRACE ),
                InnerException = InnerExceptionViewModel.Get( searcher, exception ),
                AggregatedExceptions = GetAggregatedExceptions( searcher, doc )
            };
        }

        private static List<IExceptionViewModel> GetAggregatedExceptions( ILuceneSearcher searcher, Document doc )
        {
            var exception = searcher.GetDocument( LogField.INDEX_DTS, doc.Get( LogField.EXCEPTION ), 999 );

            if( string.IsNullOrEmpty( exception.Get( LogField.AGGREGATED_EXCEPTIONS ) ) )
                return null;

            var list = new List<IExceptionViewModel>();
            if( exception.Get( LogField.EXCEPTION_TYPE_NAME ) != LogField.AGGREGATE_EXCEPTION )
                return list;

            var ids = exception.Get( LogField.AGGREGATED_EXCEPTIONS ).Split( ';' );
            list.AddRange( from id in ids where !string.IsNullOrEmpty( id ) select GetAggregateException( searcher, id ) );

            return list;
        }

        private static IExceptionViewModel GetAggregateException( ILuceneSearcher searcher, string id )
        {
            var exception = searcher.GetDocument( LogField.INDEX_DTS, id, 999 );
            if( exception.GetField( LogField.EXCEPTION_TYPE_NAME ) == null )
                return null;

            return new ExceptionViewModel
            {
                Message = exception.Get( LogField.MESSAGE ),
                StackTrace = exception.Get( LogField.STACKTRACE ),
                InnerException = InnerExceptionViewModel.Get( searcher, exception ),
            };
        }
    }
}
