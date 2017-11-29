using CK.Glouton.Lucene;
using Lucene.Net.Documents;

namespace CK.Glouton.Model.Logs
{
    public class LineViewModel : ILogViewModel
    {
        public ELogType LogType => ELogType.Line;
        public string LogTime { get; set; }
        public string Text { get; set; }
        public string Tags { get; set; }
        public string SourceFileName { get; set; }
        public string LineNumber { get; set; }
        public IExceptionViewModel Exception { get; set; }
        public string LogLevel { get; set; }
        public string MonitorId { get; set; }
        public string GroupDepth { get; set; }
        public string PreviousEntryType { get; set; }
        public string PreviousLogTime { get; set; }
        public string AppId { get; set; }

        public static LineViewModel Get( LuceneSearcher luceneSearcher, Document document )
        {
            return new LineViewModel
            {
                MonitorId = document.Get( "MonitorId" ),
                GroupDepth = document.Get( "GroupDepth" ),
                PreviousEntryType = document.Get( "PreviousEntryType" ),
                PreviousLogTime = document.Get( "PreviousLogTime" ),
                LogLevel = document.Get( "LogLevel" ),
                Text = document.Get( "Text" ),
                Tags = document.Get( "Tags" ),
                SourceFileName = document.Get( "FileName" ),
                LineNumber = document.Get( "LineNumber" ),
                LogTime = DateTools.StringToDate( document.Get( "LogTime" ) ).ToString( "dd/MM/yyyy HH:mm:ss.fff" ),
                Exception = ExceptionViewModel.Get( luceneSearcher, document ),
                AppId = document.Get( "AppName" )
            };
        }
    }
}
