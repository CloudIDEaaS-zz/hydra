export class PackageCacheStatus {
    public CacheStatus : string;
    public CurrentActionVerb : string;
    public AddingToCacheCount : number;
    public ProcessingCount : number;
    public AddedToCacheCount : number;
    public WithErrorsCount : number;
    public InstallsFromCache : string[];
    public CopiedToCache : string[];
    public InstallErrorsFromCache : string[];
    public StatusText : string;
}