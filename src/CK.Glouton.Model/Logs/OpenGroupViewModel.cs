﻿using CK.Glouton.Model.Lucene;
using Lucene.Net.Documents;

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

        public static OpenGroupViewModel Get( ILuceneSearcher luceneSearcher, Document document )
        {
            return new OpenGroupViewModel
            {
                LogLevel = document.Get( LogField.LOG_LEVEL ),
                LogTime = document.Get( LogField.LOG_TIME ),
                Text = document.Get( LogField.TEXT ),
                SourceFileName = document.Get( LogField.SOURCE_FILE_NAME ),
                Exception = ExceptionViewModel.Get( luceneSearcher, document )
            };
        }
    }
}
