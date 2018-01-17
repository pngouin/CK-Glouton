using CK.Core;
using CK.Glouton.Model.Server.Handlers;
using CK.Monitoring;
using ExpressionBuilder.Common;
using ExpressionBuilder.Generics;
using System;

namespace CK.Glouton.Service.Common
{
    public static class AlertBuilder
    {
        /// <summary>
        /// Build a <see cref="Func{TResult}"/> from a given <see cref="IExpressionModel"/> array.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Will be thrown if an enum cannot be parsed.</exception>
        /// <exception cref="InvalidOperationException">Will be thrown if an invalid field is encountered.</exception>
        public static Func<AlertEntry, bool> Build( this IExpressionModel[] @this )
        {
            var filter = new Filter<AlertEntry>();

            foreach( var alert in @this )
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

                    case "FileName":
                    case "AppName":
                    case "Text":
                    case "Exception.Message":
                    case "Exception.StackTrace":
                        filter.By( alert.Field, Operation.IsNotNull ).And
                            .By( alert.Field, ParseOperation( alert.Operation ), alert.Body );
                        break;

                    case "Tags":
                        var traitContext = new CKTraitContext( "AlertParsing", ';' );
                        filter.By( alert.Field, Operation.IsNotNull ).And
                            .By( alert.Field, ParseOperation( alert.Operation ), traitContext.FindOrCreate( alert.Body ) );
                        break;

                    default:
                        throw new InvalidOperationException( $"Alert field {alert.Field} is invalid." );
                }
            }

            return filter;
        }

        private static Operation ParseOperation( string operation )
        {
            return Enum.TryParse( operation, out Operation eOperation ) ? eOperation : throw new ArgumentException( nameof( operation ) );
        }
    }
}