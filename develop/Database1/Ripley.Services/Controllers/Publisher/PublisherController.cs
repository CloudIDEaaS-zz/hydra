using Ripley.Entities;
using Ripley.Services.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using Publisher = Ripley.Services.Models.Publisher;

namespace Ripley.Services.Controllers
{
    [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
    public class PublisherController : ApiController
    {
        private IRipleyEntities ripleyEntities;

        public PublisherController(IRipleyEntities ripleyEntities)
        {
            this.ripleyEntities = ripleyEntities;
        }


        [HttpGet]
        [Route("~/api/publishers")]
        public Publisher[] GetPublishers()
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var publishers = ripleyEntities.Publishers;

                return publishers.Select(r => new Publisher
                {
                    PublisherId = r.PublisherId,
                    PublisherName = r.PublisherName,
                }).OrderBy(c => c.PublisherName).ToArray();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }


        [HttpGet]
        [Route("~/api/publisher/{id:guid}")]
        public Publisher GetPublisher(Guid id)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var publishers = ripleyEntities.Publishers;

                return publishers.Where(r => r.PublisherId == id).Select(r => new Publisher
                {
                    PublisherId = r.PublisherId,
                    PublisherName = r.PublisherName,
                }).Single();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [HttpPost]
        public void CreatePublisher(Publisher publisher)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var publishers = ripleyEntities.Publishers;

                publishers.Add(new Entities.Publisher
                {
                    PublisherId = Guid.NewGuid(),
                    PublisherName = publisher.PublisherName,
                });

                ripleyEntities.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [HttpPut]
        public void UpdatePublisher(Publisher publisher)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var updatePublisher = ripleyEntities.Publishers.Single(r => r.PublisherId == publisher.PublisherId);

                updatePublisher.PublisherName = publisher.PublisherName;

                ripleyEntities.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [HttpDelete]
        public void DeletePublisher(Guid id)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var publishers = ripleyEntities.Publishers;
                var deletePublisher = publishers.Single(r => r.PublisherId == id);

                publishers.Remove(deletePublisher);

                ripleyEntities.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
