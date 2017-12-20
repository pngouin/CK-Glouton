using CK.Glouton.Lucene;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ck.Glouton.Ckmon.Indexer
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            Console.WriteLine( ".ckmon need to be in ./ckmon" );
            foreach( var path in GetFiles() )
            {
                var appName = "ckmon-" + Guid.NewGuid().ToString().Substring( 0, 8 );
                Console.WriteLine( $"{path} will be indexed in ${appName}" );

                var configuration = new LuceneConfiguration
                {
                    MaxSearch = 100,
                    Directory = appName
                };
                var indexCkmon = new IndexCkmon( configuration, path );
            }
        }

        private static IEnumerable<string> GetFiles()
        {
            return Directory.GetFiles( "./ckmon", "*.ckmon", SearchOption.TopDirectoryOnly );
        }
    }
}
