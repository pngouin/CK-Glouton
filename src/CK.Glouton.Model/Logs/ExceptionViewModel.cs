using CK.Glouton.Lucene;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace CK.Glouton.Model.Logs
{
    public class ExceptionViewModel : IExceptionViewModel
    {
        public IInnerExceptionViewModel InnerException { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }

        public static IExceptionViewModel Get( LuceneSearcher searcher, Document doc )
        {
            if( doc.GetField( "Exception" ) == null )
                return null;
            var query = new TermQuery( new Term( "IndexTS", doc.Get( "Exception" ) ) );
            var exceptionDoc = searcher.Search( query );
            var exception = searcher.GetDocument( exceptionDoc.ScoreDocs[ 0 ] );

            return new ExceptionViewModel
            {
                Message = exception.Get( "Exception" ),
                Stack = exception.Get( "Stack" ),
                InnerException = InnerExceptionViewModel.Get( searcher, exception )
            };
        }
    }
}
