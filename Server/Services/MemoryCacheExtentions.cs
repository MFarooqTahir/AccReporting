using Microsoft.Extensions.Caching.Memory;

using System.Collections;
using System.Reflection;

namespace AccReporting.Server.Services
{
    public static class MemoryCacheExtensions
    {
        private static readonly Func<MemoryCache, object> GetEntriesCollection = Delegate.CreateDelegate(
            type: typeof(Func<MemoryCache, object>),
            method: typeof(MemoryCache).GetProperty(name: "EntriesCollection", bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(nonPublic: true),
            throwOnBindFailure: true) as Func<MemoryCache, object>;

        public static IEnumerable<string> GetKeysForDb(this IMemoryCache memoryCache, string prefix)
        {
            var en = ((IDictionary)GetEntriesCollection(arg: (MemoryCache)memoryCache)).Keys;
            return en.Cast<string>().Where(predicate: x => x.StartsWith(value: prefix));
        }
        public static IEnumerable<T> GetKeys<T>(this IMemoryCache memoryCache, string prefix) =>
            GetKeysForDb(memoryCache: memoryCache, prefix: prefix).OfType<T>();
    }
}
