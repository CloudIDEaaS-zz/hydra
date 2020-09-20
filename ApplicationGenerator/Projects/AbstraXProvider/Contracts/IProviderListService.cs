using System;
using AbstraX.Contracts;
using System.Linq;

namespace AbstraX.Contracts
{
    public interface IProviderListService
    {
        void DeleteProvider(ProviderListItem listItem);
        IDomainHostApplication DomainServiceHostApplication { get; set; }
        IQueryable<ProviderListItem> GetProviderList();
        void InsertProvider(ProviderListItem listItem);
        void UpdateProvider(ProviderListItem listItem);
    }
}
