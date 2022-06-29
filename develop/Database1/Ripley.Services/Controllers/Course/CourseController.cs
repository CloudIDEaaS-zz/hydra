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
using Course = Ripley.Services.Models.Course;
using Publisher = Ripley.Services.Models.Publisher;

namespace Ripley.Services.Controllers
{
    [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
    public class CourseController : ApiController
    {
        private IRipleyEntities ripleyEntities;

        public CourseController(IRipleyEntities ripleyEntities)
        {
            this.ripleyEntities = ripleyEntities;
        }

        [Route("~/api/getPublisherForCurrentUser")]
        [HttpGet]
        public Publisher GetPublisherForCurrentUser()
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var userName = identity.Name;
                var publisher = ripleyEntities.Users.Single(u => (u.UserName == userName)).UserToPublishers.Single().Publisher;

                return new Publisher
                {
                    PublisherId = publisher.PublisherId,
                    PublisherName = publisher.PublisherName,
                };
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [HttpGet]
        [Route("~/api/courses")]
        public Course[] GetCourses()
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var courses = ripleyEntities.Courses;

                return courses.Select(r => new Course
                {
                    CourseId = r.CourseId,
                    PublisherId = r.PublisherId,
                    CourseName = r.CourseName,
                    CourseDescription = r.CourseDescription,
                }).OrderBy(c => c.CourseName).ToArray();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [Route("~/api/Publishers/{publisherId:guid}/courses")]
        [HttpGet]
        public Course[] GetCoursesForPublisher(Guid publisherId)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var courses = ripleyEntities.Courses;

                return courses.Where(r => r.PublisherId == publisherId).Select(r => new Course
                {
                    CourseId = r.CourseId,
                    PublisherId = r.PublisherId,
                    CourseName = r.CourseName,
                    CourseDescription = r.CourseDescription,
                }).OrderBy(r => r.CourseName).ToArray();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [HttpGet]
        [Route("~/api/course/{id:guid}")]
        public Course GetCourse(Guid id)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var courses = ripleyEntities.Courses;

                return courses.Where(r => r.CourseId == id).Select(r => new Course
                {
                    CourseId = r.CourseId,
                    PublisherId = r.PublisherId,
                    CourseName = r.CourseName,
                    CourseDescription = r.CourseDescription,
                }).Single();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [HttpPost]
        public void CreateCourse(Course course)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var courses = ripleyEntities.Courses;

                courses.Add(new Entities.Course
                {
                    CourseId = Guid.NewGuid(),
                    PublisherId = course.PublisherId,
                    CourseName = course.CourseName,
                    CourseDescription = course.CourseDescription,
                });

                ripleyEntities.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [HttpPut]
        public void UpdateCourse(Course course)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var updateCourse = ripleyEntities.Courses.Single(r => r.CourseId == course.CourseId);

                updateCourse.PublisherId = course.PublisherId;
                updateCourse.CourseName = course.CourseName;
                updateCourse.CourseDescription = course.CourseDescription;

                ripleyEntities.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [HttpDelete]
        public void DeleteCourse(Guid id)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var courses = ripleyEntities.Courses;
                var deleteCourse = courses.Single(r => r.CourseId == id);

                courses.Remove(deleteCourse);

                ripleyEntities.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
