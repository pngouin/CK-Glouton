using CK.Glouton.Model.Lucene;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace CK.Glouton.Model.Logs
{
    public class InnerExceptionViewModel : IInnerExceptionViewModel
    {
        public string StackTrace { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }

        public static IInnerExceptionViewModel Get( ILuceneSearcher searcher, Document doc )
        {
            if( doc.GetField( LogField.INNER_EXCEPTION ) == null )
                return null;

            var exception = searcher.GetDocument( new TermQuery( new Term( LogField.INDEX_DTS, doc.Get( LogField.INNER_EXCEPTION ) ) ), 999 );

            return new InnerExceptionViewModel
            {
                StackTrace = exception.Get( LogField.STACKTRACE ),
                Message = exception.Get( LogField.MESSAGE ),
                FileName = exception.Get( LogField.SOURCE_FILE_NAME )
            };
        }
    }
}
