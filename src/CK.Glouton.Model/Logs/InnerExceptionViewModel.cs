﻿using CK.Glouton.Model.Lucene;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace CK.Glouton.Model.Logs
{
    public class InnerExceptionViewModel : IInnerExceptionViewModel
    {
        public string Stack { get; set; }
        public string Details { get; set; }
        public string FileName { get; set; }

        public static IInnerExceptionViewModel Get( ILuceneSearcher searcher, Document doc )
        {
            if( doc.GetField( LogField.INNER_EXCEPTION ) == null )
                return null;

            var exception = searcher.GetDocument(new TermQuery(new Term(LogField.INDEX_DTS, doc.Get(LogField.INNER_EXCEPTION))));

            return new InnerExceptionViewModel
            {
                Stack = exception.Get( LogField.STACK ),
                Details = exception.Get( LogField.DETAILS ),
                FileName = exception.Get( LogField.SOURCE_FILE_NAME )
            };
        }
    }
}
