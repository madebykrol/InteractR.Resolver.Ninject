using System;
using System.Collections.Generic;
using InteractR.Interactor;
using InteractR.Resolver;
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

        private T Resolve<T>() =>  _kernel.Get<T>();

        private object Resolve(Type t) => _kernel.Get(t);

        public IInteractor<TUseCase, TOutputPort> ResolveInteractor<TUseCase, TOutputPort>(TUseCase useCase) where TUseCase : IUseCase<TOutputPort>
        {
            return Resolve<IInteractor<TUseCase, TOutputPort>>();
        }
    }
}
