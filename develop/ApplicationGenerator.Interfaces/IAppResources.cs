using AbstraX.DataAnnotations;
using System.Collections.Generic;

namespace AbstraX
{
    public interface IAppResources
    {
        dynamic GetResources(UIKind componentKind);
        List<QueryInfo> GetQueries();
    }
}