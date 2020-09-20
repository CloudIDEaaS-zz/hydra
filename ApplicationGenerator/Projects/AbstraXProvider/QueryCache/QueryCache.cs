using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AbstraX.QueryCache
{
    public class AbstraXQueryCache : IDisposable
    {
        private Dictionary<string, CacheItem> cache;
        private Thread sweeperThread;
        private bool exitThread;
        private bool threadExited;

        public AbstraXQueryCache()
        {
            cache = new Dictionary<string, CacheItem>();

            sweeperThread = new Thread(SweeperThread);
            sweeperThread.Priority = ThreadPriority.Lowest;

            sweeperThread.Start();
        }

        public void AddToCache(string expression, IQueryable cachedData, TimeSpan expiration)
        {
            var cacheItem = new CacheItem(expression, cachedData, expiration);

            AddToCache(cacheItem);
        }

        public void AddToCache(string expression, IQueryable cachedData)
        {
            var cacheItem = new CacheItem(expression, cachedData);

            AddToCache(cacheItem);
        }

        public void AddToCache(CacheItem item)
        {
            lock (cache)
            {
                if (cache.ContainsKey(item.Key))
                {
                    cache.Remove(item.Key);
                }

                cache.Add(item.Key, item);
            }
        }

        // for testing
        internal void SpeedForward(TimeSpan span)
        {
            lock (cache)
            {
                cache.Values.ToList().ForEach(c => 
                {
                    c.LastAccessed -= span;
                });
            }
        }

        private void SweeperThread()
        {
            while (!exitThread)
            {
                lock (cache)
                {
                    cache.Values.Where(c => 
                    {
                        return (c.LastAccessed + c.Expiration) < DateTime.Now;

                    }).ToList().ForEach(c => 
                        {
                            cache.Remove(c.Key);
                        });
                }

                if (exitThread)
                {
                    threadExited = true;
                    return;
                }

                Thread.Sleep(1000);
            }

            lock (cache)
            {
                threadExited = true;
            }
        }

        public void ClearAll()
        {
            lock (cache)
            {
                cache.Clear();
            }
        }

        public void ClearCurrent()
        {
            lock (cache)
            {
                cache.Values.Where(c => c.IsCurrent).ToList().ForEach(c => cache.Remove(c.Key));
            }
        }

        public bool InCache(string expression, out IQueryable queryable)
        {
            bool inCache;

            lock (cache)
            {
                var dummyItem = new CacheItem(expression);

                if (cache.ContainsKey(dummyItem.Key))
                {
                    var cacheItem = cache[dummyItem.Key];

                    queryable = cacheItem.CachedData;
                    inCache = true;
                }
                else
                {
                    queryable = null;
                    inCache = false;
                }
            }

            return inCache;
        }

        public void Dispose()
        {
            var exitStarted = DateTime.Now;

            lock (cache)
            {
                cache.Clear();
                exitThread = true;
            }

            while (DateTime.Now - exitStarted < new TimeSpan(0, 5, 0))
            {
                lock (cache)
                {
                    if (threadExited)
                    {
                        break;
                    }
                }

            }
        }
    }
}
