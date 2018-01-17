using CK.Core;
using CK.Monitoring;
using System;
using System.Collections.Generic;

namespace CK.Glouton.Model.Server.Handlers
{
    public class AlertEntry : IAlertEntry
    {
        public IMulticastLogEntry MulticastMulticastLogEntry { get; set; }
        public string AppName { get; set; }

        public void WriteLogEntry( CKBinaryWriter w )
        {
            MulticastMulticastLogEntry.WriteLogEntry( w );
        }

        public LogEntryType LogType => MulticastMulticastLogEntry.LogType;

        public LogLevel LogLevel => MulticastMulticastLogEntry.LogLevel;

        public string Text => MulticastMulticastLogEntry.Text;

        public CKTrait Tags => MulticastMulticastLogEntry.Tags;

        public DateTimeStamp LogTime => MulticastMulticastLogEntry.LogTime;

        public CKExceptionData Exception => MulticastMulticastLogEntry.Exception;

        public string FileName => MulticastMulticastLogEntry.FileName;

        public int LineNumber => MulticastMulticastLogEntry.LineNumber;

        public IReadOnlyList<ActivityLogGroupConclusion> Conclusions => MulticastMulticastLogEntry.Conclusions;

        public Guid MonitorId => MulticastMulticastLogEntry.MonitorId;

        public ILogEntry CreateUnicastLogEntry()
        {
            return MulticastMulticastLogEntry.CreateUnicastLogEntry();
        }

        public int GroupDepth => MulticastMulticastLogEntry.GroupDepth;

        public LogEntryType PreviousEntryType => MulticastMulticastLogEntry.PreviousEntryType;

        public DateTimeStamp PreviousLogTime => MulticastMulticastLogEntry.PreviousLogTime;

        public AlertEntry( IMulticastLogEntry multicastLogEntry, string appName )
        {
            MulticastMulticastLogEntry = multicastLogEntry;
            AppName = appName;
        }
    }
}