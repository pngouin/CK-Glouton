using CK.Glouton.Model.Server.Handlers;
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

        public AlertTableMock( string path )
        {
            if( string.IsNullOrEmpty( path ) )
                throw new ArgumentNullException( nameof( path ) );

            _path = path;
            if( Directory.Exists( path ) )
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
                var filePath = Path.Combine( _path, Guid.NewGuid().ToString() );
                using( var streamWriter = new StreamWriter( filePath ) )
                {
                    streamWriter.WriteLine( "Expressions:" );
                    foreach( var expression in alertExpressionModel.Expressions )
                        streamWriter.WriteLine( $"Log.{expression.Field} {expression.Operation} {expression.Body}" );
                    streamWriter.WriteLine( "Senders:" );
                    foreach( var sender in alertExpressionModel.Senders )
                        streamWriter.WriteLine( $"{sender.SenderType}" );
                }
            }
        }

        public IAlertExpressionModel GetAll()
        {
            var files = Directory.EnumerateFiles( _path );
            if( files == null )
                return null;

            return null;
        }
    }
}