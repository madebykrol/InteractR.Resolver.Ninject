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
        [SetUp]
        public void Setup()
        {

            _kernel = new StandardKernel();
            _useCaseInteractor = Substitute.For<IInteractor<MockUseCase, IMockOutputPort>>();
            _kernel.Bind<IInteractor<MockUseCase, IMockOutputPort>>()
                .ToMethod(context => _useCaseInteractor);

            _interactorHub = new Hub(new NinjectResolver(_kernel));
        }

        [Test]
        public async Task Test_Ninject_Resolver()
        {
            await _interactorHub.Execute(new MockUseCase(), (IMockOutputPort)new MockOutputPort());
            await _useCaseInteractor.Received().Execute(Arg.Any<MockUseCase>(),Arg.Any<IMockOutputPort>(), Arg.Any<CancellationToken>());
        }
    }
}