using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;


namespace TripleZero.Helper.Caching
{
    public static class Caching
    {
        //private static MemoryCache _cache = new MemoryCache("ExampleCache");

        //public static object GetItem(string key)
        //{
        //    return AddOrGetExisting(key, () => InitItem(key));
        //}

        //private static T AddOrGetExisting<T>(string key, Func<T> valueFactory)
        //{
        //    var newValue = new Lazy<T>(valueFactory);
        //    var oldValue = _cache.AddOrGetExisting(key, newValue, new CacheItemPolicy()) as Lazy<T>;
        //    try
        //    {
        //        return (oldValue ?? newValue).Value;
        //    }
        //    catch
        //    {
        //        // Handle cached lazy exception by evicting from cache. Thanks to Denis Borovnev for pointing this out!
        //        _cache.Remove(key);
        //        throw;
        //    }
        //}
        //private static object InitItem(string key)
        //{
        //    // Do something expensive to initialize item
        //    return new { Value = key.ToUpper() };
        //}

        public static T Get<T>() where T : ICaching<T>, new()
        {
            var cache = new T();
            return Get(cache);
        }

        private static T Get<T>(T cache) where T : ICaching<T>, new()
        {
            var init = new Lazy<T>(cache.Init);
            var result =
                (Lazy<T>)
                    MemoryCache.Default.AddOrGetExisting(cache.Name(), init, DateTimeOffset.Now.AddMinutes(cache.Minutes()));

            return (result ?? init).Value;
        }

        public static T Get<T>(Func<T> create) where T : ICaching<T>, new()
        {
            var cache = create();
            return Get(cache);

        }

        public static void Reset<T>() where T : ICaching<T>, new()
        {
            var cache = new T();
            var name = cache.Name();
            Reset(name);
        }

        public static void Reset(string name)
        {
            MemoryCache.Default.Remove(name);
        }

        public static IEnumerable<ICaching> Find()
        {
            var caches = typeof(ICaching).Assembly.GetTypes().Where(p => typeof(ICaches).IsAssignableFrom(p) && !p.IsInterface).Select(
                cache => Activator.CreateInstance(cache) as ICaches).Where(cache => cache != null);
            return caches;
        }

        public static void ResetAll()
        {
            var caches = Find();
            foreach (var cache in caches)
            {
                Reset(cache.Name());
            }
        }
    }
}
