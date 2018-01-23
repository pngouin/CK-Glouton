using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.AlertSender.Sender
{
    public class HttpSender : IAlertSender
    {
        private static readonly HttpClient Client = new HttpClient();

        private readonly string _url;

        public string SenderType { get; set; } = "Http";

        public HttpSender( HttpSenderConfiguration httpSenderConfiguration )
        {
            if( httpSenderConfiguration.SenderType != SenderType )
                throw new ArgumentException( nameof( httpSenderConfiguration ) );
            _url = httpSenderConfiguration.Url;
        }


        public bool Match( IAlertSenderConfiguration configuration )
        {
            return configuration is HttpSenderConfiguration httpSenderConfiguration
                && httpSenderConfiguration.Url.Equals( _url );
        }

        public void Send( AlertEntry logEntry )
        {
            if( logEntry == null )
                throw new ArgumentNullException( nameof( logEntry ) );

            var values = new Dictionary<string, string>
            {
                { "AppName", logEntry.AppName },
                { "LogType", logEntry.LogType.ToString() },
                { "LogLevel", logEntry.LogLevel.ToString() },
                { "Text", logEntry.Text },
                { "Tags", logEntry.Tags.ToString() },
                { "LogTime", logEntry.LogTime.ToString() },
                { "FileName", logEntry.FileName },
                { "LineNumber", logEntry.LineNumber.ToString() },
                { "MonitorId", logEntry.MonitorId.ToString() },
                { "GroupDepth", logEntry.GroupDepth.ToString() },
                { "PreviousEntryType", logEntry.PreviousEntryType.ToString() },
                { "PreviousLogTime", logEntry.PreviousLogTime.ToString() }
            };

            if( logEntry.Exception != null )
            {
                values.Add( "Exception.Message", logEntry.Exception.Message );
                values.Add( "Exception.StackTrace", logEntry.Exception.StackTrace );
                values.Add( "Exception.FileName", logEntry.Exception.FileName );
            }

            if( logEntry.Conclusions != null )
            {
                var stringBuilder = new StringBuilder();
                foreach( var conclusion in logEntry.Conclusions )
                    stringBuilder.Append( $"{conclusion};" );
                values.Add( "Conclusions", stringBuilder.ToString() );
            }

            Client.PostAsync( _url, new FormUrlEncodedContent( values ) ).GetAwaiter().GetResult();
        }
    }
}