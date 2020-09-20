using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Utils;
using System.Xml.Serialization;

namespace AbstraX.QueryCache
{
    [Serializable]
    public class CacheItem
    {
        public string Expression { get; set; }
        public string UserCookie { get; set; }
        [XmlIgnore]
        public TimeSpan Expiration { get; set; }
        [XmlIgnore]
        public DateTime StartDate { get; set; }
        [XmlIgnore]
        public DateTime LastAccessed { get; set; }
        [XmlIgnore]
        private IQueryable cachedData;
        [XmlIgnore]
        private string key;

        public CacheItem()
        {
        }

        public static bool operator== (CacheItem item1, CacheItem item2)
        {
            return item1.Key == item2.Key;
        }

        public static bool operator !=(CacheItem item1, CacheItem item2)
        {
            return item1.Key != item2.Key;
        }

        public CacheItem(string expression) : this(expression, null, new TimeSpan(0, 0, 0))
        {
        }

        public CacheItem(string expression, IQueryable cachedData) : this(expression, cachedData, new TimeSpan(0, 15, 0))
        {
        }

        public CacheItem(string expression, IQueryable cachedData, TimeSpan expiration)
        {
            if (HttpContext.Current != null)
            {
                this.UserCookie = HttpContext.Current.Session.SessionID;
            }
            else
            {
                this.UserCookie = Environment.UserName;
            }

            this.StartDate = DateTime.Now;
            this.LastAccessed = DateTime.Now;

            this.Expression = expression;
            this.CachedData = cachedData;
            this.Expiration = expiration;
        }

        [XmlIgnore]
        public bool IsCurrent
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return this.UserCookie == HttpContext.Current.Session.SessionID;
                }
                else
                {
                    return this.UserCookie == Environment.UserName;
                }
            }
        }

        [XmlIgnore]
        public IQueryable CachedData 
        {
            get
            {
                this.LastAccessed = DateTime.Now;
                return cachedData;
            }

            set
            {
                cachedData = value;
            }
        }

        [XmlIgnore]
        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = MD5HashGenerator.GenerateKey(this);
                }

                return key;
            }
        }
    }
}
