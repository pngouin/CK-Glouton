using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;
using CK.Glouton.Model.Server.Sender.Implementation;
using System;
using System.Collections.Generic;
using System.IO;

namespace CK.Glouton.Database
{
    /// <summary>
    /// This class is only for sample usage, it's really not meant for production.
    /// </summary>
    public sealed class AlertTableMock
    {
        private readonly string _path;

        private const string ExpressionsDelimiter = "Expressions:";
        private const string SendersDelimiter = "Senders:";

        public AlertTableMock( string path )
        {
            if( string.IsNullOrEmpty( path ) )
                throw new ArgumentNullException( nameof( path ) );

            _path = path;
            if( !Directory.Exists( path ) )
                Directory.CreateDirectory( path );
        }

        public void Create( IList<IAlertExpressionModel> alertExpressionModels )
        {
            if( alertExpressionModels == null )
                throw new ArgumentNullException( nameof( alertExpressionModels ) );

            foreach( var file in Directory.EnumerateFiles( _path ) )
                File.Delete( file );

            foreach( var alertExpressionModel in alertExpressionModels )
            {
                var filePath = Path.Combine( _path, $"{Guid.NewGuid().ToString()}.txt" );
                using( var streamWriter = new StreamWriter( filePath ) )
                {
                    streamWriter.WriteLine( ExpressionsDelimiter );
                    foreach( var expression in alertExpressionModel.Expressions )
                        streamWriter.WriteLine( $"Log.{expression.Field} {expression.Operation} {expression.Body}" );
                    streamWriter.WriteLine( SendersDelimiter );
                    foreach( var sender in alertExpressionModel.Senders )
                        streamWriter.WriteLine( $"{sender.SenderType}" );
                }
            }
        }

        public IList<IAlertExpressionModel> GetAll()
        {
            var files = Directory.EnumerateFiles( _path );
            if( files == null )
                return null;

            var alertExpressionModels = new List<IAlertExpressionModel>();
            foreach( var file in files )
            {
                var expressionModels = new List<ExpressionModel>();
                var alertSenderConfigurations = new List<AlertSenderConfiguration>();

                using( var streamReader = new StreamReader( file ) )
                {
                    var currentLine = streamReader.ReadLine();
                    if( !currentLine.Equals( ExpressionsDelimiter ) )
                        throw new InvalidOperationException( "File content is invalid" );

                    while( !( currentLine = streamReader.ReadLine() ).Equals( SendersDelimiter ) )
                    {
                        var fields = currentLine.Split( ' ' );
                        if( fields.Length < 3 )
                            throw new InvalidOperationException( "File content is invalid" );

                        expressionModels.Add( new ExpressionModel
                        {
                            Field = fields[ 0 ],
                            Operation = fields[ 1 ],
                            Body = fields[ 2 ]
                        } );
                    }

                    while( ( currentLine = streamReader.ReadLine() ) != null )
                        alertSenderConfigurations.Add( new AlertSenderConfiguration { SenderType = currentLine } );
                }

                alertExpressionModels.Add( new AlertExpressionModel
                {
                    Expressions = expressionModels.ToArray(),
                    Senders = alertSenderConfigurations.ToArray()
                } );
            }

            return alertExpressionModels;
        }

        internal class AlertExpressionModel : IAlertExpressionModel
        {
            public IExpressionModel[] Expressions { get; set; }
            public IAlertSenderConfiguration[] Senders { get; set; }
        }
    }
}