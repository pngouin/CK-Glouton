using CK.Glouton.Model.Lucene;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace CK.Glouton.Model.Logs
{
    public class InnerExceptionViewModel : IInnerExceptionViewModel
    {
        public string Stack { get; set; }
        public string Details { get; set; }
        public string FileName { get; set; }

        public static IInnerExceptionViewModel Get( ILuceneSearcher searcher, Document doc )
        {
            if( doc.GetField( "InnerException" ) == null )
                return null;

            var query = new TermQuery( new Term( "IndexTS", doc.Get( "InnerException" ) ) );
            var exceptionDoc = searcher.Search( query );
            var exception = searcher.GetDocument( exceptionDoc.ScoreDocs[ 0 ] );

            return new InnerExceptionViewModel
            {
                Stack = exception.Get( "Stack" ),
                Details = exception.Get( "Details" ),
                FileName = exception.Get( "Filename" )
            };
        }
    }
}
