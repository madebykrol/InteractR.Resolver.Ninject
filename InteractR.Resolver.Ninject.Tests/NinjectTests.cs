using System;
using System.Threading;
using System.Threading.Tasks;
using InteractorHub.Resolvers.Ninject;
using InteractR;
using InteractR.Interactor;
using InteractR.Resolver.AutoFac.Tests.Mocks;
using Ninject;
using NSubstitute;
using NUnit.Framework;

namespace InteractorHub.Tests.Resolvers.Ninject
{
    [TestFixture]
    public class NinjectTests
    {
        private IInteractorHub _interactorHub;
        private IInteractor<MockUseCase, IMockOutputPort> _useCaseInteractor;

        private IMiddleware<MockUseCase, IMockOutputPort> _middleware1;
        private IMiddleware<MockUseCase, IMockOutputPort> _middleware2;

        private IMiddleware _globalMiddleware;

        [SetUp]
        public void Setup()
        {

            var kernelConfig = new KernelConfiguration();
            _useCaseInteractor = Substitute.For<IInteractor<MockUseCase, IMockOutputPort>>();

            _globalMiddleware = Substitute.For<IMiddleware>();
                _globalMiddleware.Execute(
                    Arg.Any<MockUseCase>(),
                    d => Task.FromResult(new UseCaseResult(true)),
                    Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(x => new UseCaseResult(true))
                .AndDoes(x => x.Arg<Func<MockUseCase, Task<UseCaseResult>>>().Invoke(x.Arg<MockUseCase>()));

            _middleware1 = Substitute.For<IMiddleware<MockUseCase, IMockOutputPort>>();
            _middleware1.Execute(
                    Arg.Any<MockUseCase>(),
                    Arg.Any<IMockOutputPort>(),
                    d => Task.FromResult(new UseCaseResult(true)),
                    Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(x => new UseCaseResult(true))
                .AndDoes(x => x.Arg<Func<MockUseCase, Task<UseCaseResult>>>().Invoke(x.Arg<MockUseCase>()));

            _middleware2 = Substitute.For<IMiddleware<MockUseCase, IMockOutputPort>>();
            _middleware2.Execute(
                    Arg.Any<MockUseCase>(),
                    Arg.Any<IMockOutputPort>(),
                    d => Task.FromResult(new UseCaseResult(true)),
                    Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(x => new UseCaseResult(true))
                .AndDoes(x => x.Arg<Func<MockUseCase, Task<UseCaseResult>>>().Invoke(x.Arg<MockUseCase>()));

            kernelConfig.Bind<IMiddleware<MockUseCase, IMockOutputPort>>()
                .ToMethod(context => _middleware1);

            kernelConfig.Bind<IMiddleware<MockUseCase, IMockOutputPort>>()
                .ToMethod(context => _middleware2);

            kernelConfig.Bind<IInteractor<MockUseCase, IMockOutputPort>>()
                .ToMethod(context => _useCaseInteractor);

            kernelConfig.Bind<IMiddleware>()
                .ToMethod(context => _globalMiddleware);

            _interactorHub = new Hub(new NinjectResolver(kernelConfig.BuildReadonlyKernel()));
        }

        [Test]
        public async Task Test_AutoFac_Resolver()
        {
            await _interactorHub.Execute(new MockUseCase(), (IMockOutputPort)new MockOutputPort());
            await _useCaseInteractor.Received().Execute(Arg.Any<MockUseCase>(), Arg.Any<IMockOutputPort>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task Test_Pipeline()
        {
            await _interactorHub.Execute(new MockUseCase(), (IMockOutputPort)new MockOutputPort());
            await _middleware2.ReceivedWithAnyArgs().Execute(Arg.Any<MockUseCase>(), Arg.Any<IMockOutputPort>(), Arg.Any<Func<MockUseCase, Task<UseCaseResult>>>(),
                Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task Interactor_Returns_UseCaseResult()
        {
            var result = await _interactorHub.Execute(new MockUseCase(), (IMockOutputPort)new MockOutputPort());
            Assert.AreEqual(true, result.Success);

        }
    }
}