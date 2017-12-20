using System.Collections.Generic;

namespace CK.Glouton.Service
{
    internal static class Extensions
    {
        public static List<T> RemoveAndGetRange<T>( this List<T> @this, int index, int count )
        {
            var range = @this.GetRange( index, count );
            @this.RemoveRange( index, count );
            return range;
        }
    }
}
