using CK.Glouton.Lucene;
using System;
using System.Reflection;

namespace Ck.Glouton.Ckmon.Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".ckmon need to be in ./ckmon");
            LuceneConfiguration configuration = new LuceneConfiguration
            {
                MaxSearch = 100,
                Directory = Assembly.GetExecutingAssembly().GetName().Name
            };
            IndexCkmon readCkmon = new IndexCkmon(configuration);
        }
    }
}
