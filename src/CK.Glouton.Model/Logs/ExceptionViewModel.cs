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
            if( doc.GetField( LogField.EXCEPTION ) == null )
                return null;

            var exception = searcher.GetDocument(new TermQuery(new Term(LogField.INDEX_DTS, doc.Get(LogField.EXCEPTION))));

            return new ExceptionViewModel
            {
                Message = exception.Get( LogField.EXCEPTION ),
                Stack = exception.Get( LogField.STACK ),
                InnerException = InnerExceptionViewModel.Get( searcher, exception )
            };
        }
    }
}
