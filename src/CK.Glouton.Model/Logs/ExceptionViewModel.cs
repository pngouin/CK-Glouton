using CK.Glouton.Model.Lucene;
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

        public static IExceptionViewModel Get( ILuceneSearcher searcher, Document doc )
        {
            if( doc.GetField( "Exception" ) == null )
                return null;

            var exception = searcher.GetDocument(new TermQuery(new Term("IndexTS", doc.Get("Exception"))));

            return new ExceptionViewModel
            {
                Message = exception.Get( "Exception" ),
                Stack = exception.Get( "Stack" ),
                InnerException = InnerExceptionViewModel.Get( searcher, exception )
            };
        }
    }
}
