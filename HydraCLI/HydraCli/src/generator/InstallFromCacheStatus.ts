export class InstallFromCache {
    public StatusText : string;
    public StatusIsError : boolean;
    public StatusIsSuccess : boolean;
    public CachePath : string;
    public CacheInstall : string;
    public string : string;
    public TimeReported : string;
}

export class InstallsFromCacheStatus {
    public Total : number;
    public TotalRemaining : number;
    public Requested : number;
    public RequestedRemaining : number;
    public InstallFromCacheStatus : InstallFromCache[];
    public StatusSummary : string;
    public StatusText : string;
    public StatusIsError : boolean;
    public StatusIsSuccess : boolean;
    public NothingToPoll : boolean;
}