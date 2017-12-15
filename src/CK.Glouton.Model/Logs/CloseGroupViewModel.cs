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

        public static CloseGroupViewModel Get( ILuceneSearcher searcher, Document document )
        {
            CloseGroupViewModel obj = new CloseGroupViewModel
            {
                LogLevel = document.Get( LogField.LOG_LEVEL ),
                LogTime = DateTools.StringToDate( document.Get( LogField.LOG_TIME ) ).ToString( "dd/MM/yyyy HH:mm:ss.fff" ),
                Conclusion = document.Get( LogField.CONCLUSION ),
                Exception = ExceptionViewModel.Get( searcher, document )
            };

            return obj;
        }
    }
}
