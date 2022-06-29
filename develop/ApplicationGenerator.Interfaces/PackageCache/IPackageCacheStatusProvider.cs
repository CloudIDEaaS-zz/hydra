using AbstraX.PackageCache;

namespace AbstraX.PackageCache
{
    public interface IPackageCacheStatusProvider
    {
        PackageCacheStatusInfo GetCacheStatus(string mode, bool setAsReported = false);
        PackageInstallsFromCacheStatus GetInstallFromCacheStatus(string mode);
    }
}