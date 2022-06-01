using Microsoft.Extensions.Caching.Memory;

using System.Collections;
using System.Reflection;

namespace AccReporting.Server.Services
{
    public static class MemoryCacheExtensions
    {
        private static readonly Func<MemoryCache, object> GetEntriesCollection = Delegate.CreateDelegate(
            typeof(Func<MemoryCache, object>),
            typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true),
            throwOnBindFailure: true) as Func<MemoryCache, object>;

        public static IEnumerable<string> GetKeysForDb(this IMemoryCache memoryCache, string prefix)
        {
            var en = ((IDictionary)GetEntriesCollection((MemoryCache)memoryCache)).Keys;
            return en.Cast<string>().Where(x => x.StartsWith(prefix));
        }
        public static IEnumerable<T> GetKeys<T>(this IMemoryCache memoryCache, string prefix) =>
            GetKeysForDb(memoryCache, prefix).OfType<T>();
    }
}
