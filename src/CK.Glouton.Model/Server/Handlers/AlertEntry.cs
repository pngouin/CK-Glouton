using System;
using System.Collections.Generic;
using CK.Core;
using CK.Monitoring;

namespace CK.Glouton.Model.Server.Handlers
{
    public class AlertEntry : IAlertEntry
    {
        public IMulticastLogEntry MulticastLogEntry { get; set; }
        public string AppName { get; set; }

        public void WriteLogEntry( CKBinaryWriter w )
        {
            MulticastLogEntry.WriteLogEntry( w );
        }

        public LogEntryType LogType => MulticastLogEntry.LogType;

        public LogLevel LogLevel => MulticastLogEntry.LogLevel;

        public string Text => MulticastLogEntry.Text;

        public CKTrait Tags => MulticastLogEntry.Tags;

        public DateTimeStamp LogTime => MulticastLogEntry.LogTime;

        public CKExceptionData Exception => MulticastLogEntry.Exception;

        public string FileName => MulticastLogEntry.FileName;

        public int LineNumber => MulticastLogEntry.LineNumber;

        public IReadOnlyList<ActivityLogGroupConclusion> Conclusions => MulticastLogEntry.Conclusions;

        public Guid MonitorId => MulticastLogEntry.MonitorId;

        public ILogEntry CreateUnicastLogEntry()
        {
            return MulticastLogEntry.CreateUnicastLogEntry();
        }

        public int GroupDepth => MulticastLogEntry.GroupDepth;

        public LogEntryType PreviousEntryType => MulticastLogEntry.PreviousEntryType;

        public DateTimeStamp PreviousLogTime => MulticastLogEntry.PreviousLogTime;

        public AlertEntry( IMulticastLogEntry multicastLogEntry, string appName )
        {
            MulticastLogEntry = multicastLogEntry;
            AppName = appName;
        }
    }
}