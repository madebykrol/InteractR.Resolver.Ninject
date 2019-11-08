using System;
using System.Collections.Generic;
using InteractorHub.Notification;
using InteractorHub.Pipeline;
using InteractorHub.Resolver;
using Ninject;

namespace InteractorHub.Resolvers.Ninject
{
    public class NinjectResolver : IResolver
    {
        private readonly IKernel _kernel;

        public NinjectResolver(IKernel kernel)
        {
            _kernel = kernel;
        } 

        public TInteractor ResolveInteractor<TInteractor>() => Resolve<TInteractor>();

        public object ResolveInteractor(Type interactorType) => Resolve(interactorType);

        private T Resolve<T>() =>  _kernel.Get<T>();

        private object Resolve(Type t) => _kernel.Get(t);

        private IEnumerable<T> ResolveMultiple<T>() => _kernel.GetAll<T>();
    }
}
