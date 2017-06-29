using System;
using System.Web;
using System.Runtime.Caching;

namespace AppMetrics.Common
{
    public class CacheHelper
    {
        private static MemoryCache cache = MemoryCache.Default; 

        /// <summary>
        /// Insert value into the cache using appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="o">Item to be cached</param>
        /// <param name="key">Name of item</param>
        public void Add<T>(T o, string key, CacheItemPolicy cacheItemPolicy)
        {
            cache.Set(key, o, cacheItemPolicy);
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        public void Remove(string key)
        {
            if (Exists(key))
                cache.Remove(key);
        }

        /// <summary>
        /// Check for item in cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return cache.Contains(key);
        }

        /// <summary>
        /// Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of cached item</param>
        /// <returns>Cached item as type</returns>
        public T Get<T>(string key) where T : class
        {
            try
            {
                return (T)cache.Get(key);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get cache reference
        /// </summary>
        /// <returns>Cache reference</returns>
        public MemoryCache Cache()
        {
            return cache;
        }
    }
}