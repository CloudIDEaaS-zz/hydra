using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Utils
{
    public static class MockExtensions
    {
        public static void Setup<T>(this Mock<DbSet<T>> mock, IQueryable<T> queryable) where T : class
        {
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        }

        public static void Setup<T>(this Mock<DbSet<T>> mock, List<T> list) where T : class
        {
            var queryable = list.AsQueryable();

            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            mock.Setup(m => m.Add(It.IsAny<T>())).Callback<T>((e) =>
            {
                list.Add(e);
            });
        }

        public static Mock<IObserver<T>> OnNext<T>(this Mock<IObserver<T>> observer, Action<T> onNext)
        {
            observer.Setup(m => m.OnNext(It.IsAny<T>())).Callback((T result) => onNext(result));

            return observer;
        }

        public static Mock<IObserver<T>> OnError<T>(this Mock<IObserver<T>> observer, Action<Exception> onError)
        {
            observer.Setup(m => m.OnError(It.IsAny<Exception>())).Callback((Exception ex) => onError(ex));

            return observer;
        }

        public static Mock<IObserver<T>> OnCompleted<T>(this Mock<IObserver<T>> observer, Action onCompleted)
        {
            observer.Setup(m => m.OnCompleted()).Callback(() => onCompleted());

            return observer;
        }
    }
}
