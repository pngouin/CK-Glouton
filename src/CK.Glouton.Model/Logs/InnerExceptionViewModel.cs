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

            var exception = searcher.GetDocument(new TermQuery(new Term("IndexTS", doc.Get("InnerException"))));

            return new InnerExceptionViewModel
            {
                Stack = exception.Get( "Stack" ),
                Details = exception.Get( "Details" ),
                FileName = exception.Get( "Filename" )
            };
        }
    }
}
