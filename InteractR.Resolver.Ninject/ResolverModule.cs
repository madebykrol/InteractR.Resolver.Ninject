using System;
using System.Collections.Generic;
using System.Text;
using InteractorHub.Resolvers.Ninject;
using Ninject.Modules;

namespace InteractR.Resolver.Ninject
{
    public class ResolverModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IResolver>().ToMethod(context => new NinjectResolver(context.Kernel));
            Bind<IInteractorHub>().To<Hub>();
        }
    }
}
