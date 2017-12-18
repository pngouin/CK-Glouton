using CK.Glouton.Model.Lucene;
using Lucene.Net.Documents;
using System;

namespace CK.Glouton.Model.Logs
{
    public class CloseGroupViewModel : ILogViewModel
    {
        public string LogLevel { get; set; }
        public string Conclusion { get; set; }
        public ELogType LogType => ELogType.CloseGroup;
        public IExceptionViewModel Exception { get; set; }
        public string LogTime { get; set; }
        public int GroupDepth { get; set; }

        public static CloseGroupViewModel Get( ILuceneSearcher searcher, Document document )
        {
            CloseGroupViewModel obj = new CloseGroupViewModel
            {
                LogLevel = document.Get( LogField.LOG_LEVEL ),
                LogTime = DateTools.StringToDate( document.Get( LogField.LOG_TIME ) ).ToString( "dd/MM/yyyy HH:mm:ss.fff" ),
                Conclusion = document.Get( LogField.CONCLUSION ),
                GroupDepth = Int32.Parse(document.Get(LogField.GROUP_DEPTH)),
                Exception = ExceptionViewModel.Get( searcher, document)
            };

            return obj;
        }
    }
}
