using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class RedisCollectionExtensions
    {
        public static async ValueTask<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> collection)
        {
            IList<T> ts = await collection.ToListAsync();
            return ts.ToArray();

        }


        public static async ValueTask<IList<T>> ToListAsync<T>(this IAsyncEnumerable<T> collection)
        { 
            List<T> ts = new ();
            await foreach (var item in collection)
            {
                ts.Add(item);
            }

            return ts;
        }

        public static async ValueTask<ImmutableList<T>> ToImmutableListAsync<T>(this IAsyncEnumerable<T> collection)
        { 
            ImmutableList<T> ts = ImmutableList<T>.Empty;
            await foreach (var item in collection)
            {
                ts = ts.Add(item);
            }

            return ts;
        }

        public static async ValueTask<ImmutableArray<T>> ToImmutableArrayAsync<T>(this IAsyncEnumerable<T> collection)
        { 
            ImmutableArray<T> ts = ImmutableArray<T>.Empty;
            await foreach (var item in collection)
            {
                ts = ts.Add(item);
            }

            return ts;
        }
    }
}
