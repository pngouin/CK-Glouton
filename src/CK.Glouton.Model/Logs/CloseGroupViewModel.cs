using CK.Glouton.Model.Lucene;
using Lucene.Net.Documents;

namespace CK.Glouton.Model.Logs
{
    public class CloseGroupViewModel : ILogViewModel
    {
        public string LogLevel { get; set; }
        public string Conclusion { get; set; }
        public ELogType LogType => ELogType.CloseGroup;
        public IExceptionViewModel Exception { get; set; }
        public string LogTime { get; set; }

        public static CloseGroupViewModel Get( ILuceneSearcher searcher, Document doc )
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
