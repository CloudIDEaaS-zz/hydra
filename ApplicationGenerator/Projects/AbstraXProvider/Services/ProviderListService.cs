
namespace AbstraX.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using AbstraX.Contracts;

    [EnableClientAccess()]
    public class ProviderListService : DomainService, IProviderListService
    {
        public IDomainHostApplication DomainServiceHostApplication { get; set; }

        [Query]
        public IQueryable<ProviderListItem> GetProviderList()
        {
            return this.DomainServiceHostApplication.ProviderList.AsQueryable();
        }

        [Delete]
        public void DeleteProvider(ProviderListItem listItem)
        {
        }

        [Update]
        public void UpdateProvider(ProviderListItem listItem)
        {
        }

        [Insert]
        public void InsertProvider(ProviderListItem listItem)
        {
        }
    }
}


