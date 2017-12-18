using CK.Glouton.Lucene;
using System;
using System.IO;
using System.Reflection;

namespace Ck.Glouton.Ckmon.Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".ckmon need to be in ./ckmon");
            foreach (var path in GetFiles())
            {
                string appName = "ckmon-" + Guid.NewGuid().ToString().Substring(0, 8);
                LuceneConfiguration configuration = new LuceneConfiguration
                {
                    MaxSearch = 100,
                    Directory = appName
                };
                IndexCkmon readCkmon = new IndexCkmon(configuration, path);
            }
        }

        private static string[] GetFiles()
        {
            return Directory.GetFiles("./ckmon", "*.ckmon", SearchOption.TopDirectoryOnly);
        }
    }
}
