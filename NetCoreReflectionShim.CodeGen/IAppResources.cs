using System.Collections.Generic;

namespace AbstraX
{
    public interface IAppResources
    {
        string GetResources(int componentKind);
        List<QueryInfo> GetQueries();
    }
}