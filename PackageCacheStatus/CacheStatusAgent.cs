using AbstraX;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace PackageCacheStatus
{
    public class CacheStatusAgent : BaseThreadedService
    {
        private string url = ConfigurationManager.AppSettings["WebApiUrl"];
        private IManagedLockObject lockObject;
        private DateTime lastAttemptedUpdate;
        private string lastAttemptedError;

        public event EventHandlerT<AbstraX.PackageCache.PackageCacheStatus> OnCacheStatus;
        public event EventHandlerT<AbstraX.PackageCache.PackageInstallsFromCacheStatus> OnInstallFromCacheStatus;

        public CacheStatusAgent() : base(ThreadPriority.BelowNormal, 5000, 30000, 15000)
        {
            lockObject = LockManager.CreateObject();
        }

        public DateTime LastAttemptedUpdate
        {
            get
            {
                DateTime update;

                using (lockObject.Lock())
                {
                    update = this.lastAttemptedUpdate;
                }

                return update;
            }
        }

        public string LastAttemptedError
        {
            get
            {
                string error;

                using (lockObject.Lock())
                {
                    error = this.lastAttemptedError;
                }

                return error;
            }
        }

        public override void DoWork(bool stopping)
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), url + "/api/Status/GetCacheStatus?mode=Agent"))
                {

                    try
                    {
                        using (var response = client.SendAsync(request).Result)
                        { 
                            var cacheStatus = response.Content.ReadAsAsync<AbstraX.PackageCache.PackageCacheStatus>();

                            this.lastAttemptedError = string.Empty;

                            OnCacheStatus.Raise(this, cacheStatus.Result);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "One or more errors occurred.")
                        {
                            this.lastAttemptedError = ex.InnerException.Message;
                        }
                        else
                        {
                            this.lastAttemptedError = ex.Message;
                        }
                    }
                }
            }

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), url + "/api/Status/GetInstallFromCacheStatus?mode=Agent"))
                {

                    try
                    {
                        using (var response = client.SendAsync(request).Result)
                        {
                            var cacheStatus = response.Content.ReadAsAsync<AbstraX.PackageCache.PackageInstallsFromCacheStatus>();

                            this.lastAttemptedError = string.Empty;

                            OnInstallFromCacheStatus.Raise(this, cacheStatus.Result);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "One or more errors occurred.")
                        {
                            this.lastAttemptedError = ex.InnerException.Message;
                        }
                        else
                        {
                            this.lastAttemptedError = ex.Message;
                        }
                    }
                }
            }

            using (lockObject.Lock())
            {
                this.lastAttemptedUpdate = DateTime.Now;
            }
        }
    }
}
