using CK.Glouton.Model.Lucene;
using Lucene.Net.Documents;
using System.Collections.Generic;

namespace CK.Glouton.Model.Logs
{
    public class OpenGroupViewModel : ILogViewModel
    {
        public ELogType LogType => ELogType.OpenGroup;
        public string LogLevel { get; set; }
        public string LogTime { get; set; }
        public string Text { get; set; }
        public string SourceFileName { get; set; }
        public IExceptionViewModel Exception { get; set; }
        public string GroupDepth { get; set; }
        public List<ILogViewModel> GroupLogs { get; set; }

        public static OpenGroupViewModel Get( ILuceneSearcher luceneSearcher, Document document )
        {
            return new OpenGroupViewModel
            {
                LogLevel = document.Get( LogField.LOG_LEVEL ),
                GroupDepth = document.Get( LogField.GROUP_DEPTH ),
                LogTime = DateTools.StringToDate( document.Get( LogField.LOG_TIME ) ).ToString( "dd/MM/yyyy HH:mm:ss.fff" ),
                Text = document.Get( LogField.TEXT ),
                SourceFileName = document.Get( LogField.SOURCE_FILE_NAME ),
                Exception = ExceptionViewModel.Get( luceneSearcher, document )
            };
        }
    }
}
