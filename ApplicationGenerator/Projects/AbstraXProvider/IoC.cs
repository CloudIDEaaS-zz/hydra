using System;
using Microsoft.Practices.Unity;
using System.Collections.Generic;

namespace AbstraX
{
    public static class IoC
    {
        private static IUnityContainer container;

        static IoC()
        {
            if (container == null)
            {
                container = new UnityContainer();
            }
        }

        public static IUnityContainer RegisterInstance<TInterface>(TInterface instance)
        {
            return container.RegisterInstance<TInterface>(instance);
        }

        public static IUnityContainer RegisterInstance<TInterface>(string name, TInterface instance)
        {
            return container.RegisterInstance<TInterface>(name, instance);
        }

        public static IUnityContainer RegisterInstance<TInterface>(TInterface instance, LifetimeManager lifetimeManager)
        {
            return container.RegisterInstance<TInterface>(instance, lifetimeManager);
        }

        public static IUnityContainer RegisterInstance(Type t, object instance)
        {
            return container.RegisterInstance(t, instance);
        }

        public static IUnityContainer RegisterInstance<TInterface>(string name, TInterface instance, LifetimeManager lifetimeManager)
        {
            return container.RegisterInstance<TInterface>(name, instance, lifetimeManager);
        }

        public static IUnityContainer RegisterInstance(Type t, object instance, LifetimeManager lifetimeManager)
        {
            return container.RegisterInstance(t, null, instance, lifetimeManager);
        }

        public static IUnityContainer RegisterInstance(Type t, string name, object instance)
        {
            return container.RegisterInstance(t, name, instance);
        }

        public static IUnityContainer RegisterType<TFrom, TTo>(LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return container.RegisterType<TFrom, TTo>(lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterType<TFrom, TTo>(params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return container.RegisterType<TFrom, TTo>(injectionMembers);
        }

        public static IUnityContainer RegisterType(Type from, Type to, params InjectionMember[] injectionMembers)
        {
            return container.RegisterType(from, to, injectionMembers);
        }

        public static T Resolve<T>(params ResolverOverride[] overrides)
        {
            return container.Resolve<T>(overrides);
        }

        public static T Resolve<T>(string name, params ResolverOverride[] overrides)
        {
            return container.Resolve<T>(name, overrides);
        }

        public static object Resolve(Type t, params ResolverOverride[] overrides)
        {
            return container.Resolve(t, overrides);
        }

        public static LifetimeManager CreateSingleton()
        {
            return new ContainerControlledLifetimeManager();
        }

        public static IEnumerable<T> ResolveAll<T>(params ResolverOverride[] resolverOverrides)
        {
            return container.ResolveAll<T>(resolverOverrides);
        }

        public static IUnityContainer Container
        {
            get
            {
                return container;
            }
        }
    }
}
