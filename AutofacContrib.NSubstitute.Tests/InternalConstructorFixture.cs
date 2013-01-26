using NSubstitute;
using NUnit.Framework;

namespace AutofacContrib.NSubstitute.Tests
{
    [TestFixture]
    internal class InternalConstructorFixture
    {
        [Test]
        public void Example_test_with_standard_resolve()
        {
            const int val = 3;
            var AutoSubstitute = new AutoSubstitute();
            AutoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val);
            AutoSubstitute.Resolve<IDependency1>().SomeMethod(val).Returns(c => c.Arg<int>());

            var result = AutoSubstitute.Resolve<MyClassWithInternalConstructor>().AMethod();

            Assert.That(result, Is.EqualTo(val));
        }

        [Test]
        public void Example_test_with_concrete_type_provided()
        {
            const int val = 3;
            var AutoSubstitute = new AutoSubstitute();
            AutoSubstitute.Resolve<IDependency2>().SomeOtherMethod().Returns(val);
                // This shouldn't do anything because of the next line
            AutoSubstitute.Provide<IDependency2, Dependency2>();
            AutoSubstitute.Resolve<IDependency1>().SomeMethod(Arg.Any<int>()).Returns(c => c.Arg<int>());

            var result = AutoSubstitute.Resolve<MyClassWithInternalConstructor>().AMethod();

            Assert.That(result, Is.EqualTo(Dependency2.Value));
        }
    }

    public class MyClassWithInternalConstructor
    {
        private readonly IDependency1 _d1;
        private readonly IDependency2 _d2;

        internal MyClassWithInternalConstructor(IDependency1 d1, IDependency2 d2)
        {
            _d1 = d1;
            _d2 = d2;
        }

        public int AMethod()
        {
            return _d1.SomeMethod(_d2.SomeOtherMethod());
        }
    }

}
