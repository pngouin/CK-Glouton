using CK.Glouton.Lucene;
using Lucene.Net.Documents;

namespace CK.Glouton.Model
{
    public class CloseGroupViewModel : ILogViewModel
    {
        public string LogLevel { get; set; }
        public string Conclusion { get; set; }
        public ELogType LogType => ELogType.CloseGroup;
        public IExceptionViewModel Exception { get; set; }
        public string LogTime { get; set; }

        public static CloseGroupViewModel Get( LuceneSearcher searcher, Document doc )
        {
            CloseGroupViewModel obj = new CloseGroupViewModel
            {
                LogLevel = doc.Get( "LogLevel" ),
                LogTime = doc.Get( "LogTime" ),
                Conclusion = doc.Get( "Conclusion" ),
                Exception = ExceptionViewModel.Get( searcher, doc )
            };

            return obj;
        }
    }
}
