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

        private IKernel _kernel;
        private IInteractorHub _interactorHub;
        private IInteractor<MockUseCase, IMockOutputPort> _useCaseInteractor;

        private IMiddleware<MockUseCase, IMockOutputPort> _middleware1;
        private IMiddleware<MockUseCase, IMockOutputPort> _middleware2;

        [SetUp]
        public void Setup()
        {

            _kernel = new StandardKernel();
            _useCaseInteractor = Substitute.For<IInteractor<MockUseCase, IMockOutputPort>>();

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

            _kernel.Bind<IMiddleware<MockUseCase, IMockOutputPort>>()
                .ToMethod(context => _middleware1);

            _kernel.Bind<IMiddleware<MockUseCase, IMockOutputPort>>()
                .ToMethod(context => _middleware2);

            _kernel.Bind<IInteractor<MockUseCase, IMockOutputPort>>()
                .ToMethod(context => _useCaseInteractor);

            _interactorHub = new Hub(new NinjectResolver(_kernel));
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
    }
}