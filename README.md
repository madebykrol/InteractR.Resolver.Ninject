# InteractR.Resolver.StructureMap
For documentation see [InteractR](https://github.com/madebykrol/InteractR)

Install from nuget.
```PowerShell
PM > Install-Package InteractR.Resolver.Ninject -Version 2.1.1
```

# Usage

Either you can register the resolver on your own

```Csharp
 Bind<IResolver>().ToMethod(context => new NinjectResolver(context.Kernel));
            Bind<IInteractorHub>().To<Hub>();
```

Or you can use the provided Module.

```Csharp
var kernelConfig = new KernelConfiguration(new ResolverModule());

```