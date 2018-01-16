using CK.Core;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Services;
using CK.Glouton.Server.Senders;
using CK.Monitoring;
using ExpressionBuilder.Common;
using ExpressionBuilder.Generics;
using System;
using System.Linq;

namespace CK.Glouton.Service
{
    public class AlertService : IAlertService
    {
        public bool AddAlert( IAlertExpressionModel alertExpression )
        {
            try
            {
                var condition = BuildAlert( alertExpression );
                var senders = alertExpression.Senders.Select( GetAlertSender ).ToList();

                return true;
            }
            catch( Exception )
            {
                return false;
            }
        }

        private static Func<ILogEntry, bool> BuildAlert( IAlertExpressionModel alertExpression )
        {
            var filter = new Filter<ILogEntry>();

            foreach( var alert in alertExpression.Expressions )
            {
                switch( alert.Field )
                {
                    case "LogType":
                        if( !Enum.TryParse( alert.Body, out LogEntryType logEntryType ) )
                            throw new ArgumentException( nameof( logEntryType ) );
                        filter.By( alert.Field, ParseOperation( alert.Operation ), logEntryType );
                        break;

                    case "LogLevel":
                        if( !Enum.TryParse( alert.Body, out LogLevel logLevel ) )
                            throw new ArgumentException( nameof( logLevel ) );
                        filter.By( alert.Field, ParseOperation( alert.Operation ), logLevel );
                        break;

                    case "GroupDepth":
                    case "LineNumber":
                        filter.By( alert.Field, ParseOperation( alert.Operation ), int.Parse( alert.Body ) );
                        break;

                    case "SourceFileName":
                    case "AppName":
                    case "Text":
                    case "Tags":
                    case "Exception.Message":
                    case "Exception.StackTrace":
                        filter.By( alert.Field, ParseOperation( alert.Operation ), alert.Body );
                        break;

                    default:
                        throw new InvalidOperationException( $"Alert field {alert.Field} is invalid." );
                }
            }

            return filter;
        }

        private IAlertSender GetAlertSender( string key )
        {
            switch( key )
            {
                case "Mail":
                    return new MailSender( new MailConfiguration() );

                default:
                    throw new ArgumentException( nameof( key ) );
            }
        }

        private static Operation ParseOperation( string operation )
        {
            return Enum.TryParse( operation, out Operation eOperation ) ? eOperation : throw new ArgumentException( nameof( operation ) );
        }
    }
}