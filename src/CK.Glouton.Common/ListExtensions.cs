using System;
using System.Collections.Generic;

namespace CK.Glouton.Common
{
    public static class ListExtensions
    {
        public static IEnumerable<T> TakeWhileInclusive<T>( this IEnumerable<T> enumerable, Func<T, bool> predicate )
        {
            foreach( var item in enumerable )
            {
                yield return item;
                if( !predicate( item ) )
                    break;
            }
        }
    }
}
