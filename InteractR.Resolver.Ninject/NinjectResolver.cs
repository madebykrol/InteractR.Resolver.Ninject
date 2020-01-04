using System;
using System.Collections.Generic;
using System.Linq;
using InteractR.Interactor;
using InteractR.Resolver;
using Ninject;

namespace InteractorHub.Resolvers.Ninject
{
    public class NinjectResolver : IResolver
    {
        private readonly IReadOnlyKernel _kernel;

        public NinjectResolver(IReadOnlyKernel kernel)
        {
            _kernel = kernel;
        }

        private T Resolve<T>() =>  _kernel.Get<T>();

        public IInteractor<TUseCase, TOutputPort> ResolveInteractor<TUseCase, TOutputPort>(TUseCase useCase) where TUseCase : IUseCase<TOutputPort>
        {
            return Resolve<IInteractor<TUseCase, TOutputPort>>();
        }

        public IReadOnlyList<IMiddleware<TUseCase, TOutputPort>> ResolveMiddleware<TUseCase, TOutputPort>(TUseCase useCase) where TUseCase : IUseCase<TOutputPort>
        {
            return _kernel.GetAll<IMiddleware<TUseCase, TOutputPort>>().ToList();
        }

        public IReadOnlyList<IMiddleware> ResolveGlobalMiddleware()
        {
            return _kernel.GetAll<IMiddleware>().ToList();
        }
    }
}
