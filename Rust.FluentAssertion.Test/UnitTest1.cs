namespace Rust.FluentAssertion.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GeneralTest
    {
        private class MyClass1
        {
            public MyClass2 Class2 { get; set; }
        }

        private class MyClass2
        {
            public int Value { get; set; }
        }

        [TestMethod]
        public void ShouldNotBeC2Test()
        {
            var c2 = new MyClass2();

            var c1 = new MyClass1 { Class2 = new MyClass2() };

            c1.On(x => x.Class2, "c1").Should().Not.Be(c2);
        }

        [TestMethod]
        public void ShouldBeC2Test()
        {
            var c2 = new MyClass2();

            var c1 = new MyClass1 { Class2 = c2 };

            c1.On(x => x.Class2, "c1").Should().Be(c2);
        }

        [ExpectedException(typeof(AssertFailedException))]
        [TestMethod]
        public void ShouldNotBeC2ExceptionTest()
        {
            var c2 = new MyClass2();

            var c1 = new MyClass1 { Class2 = c2 };

            c1.On(x => x.Class2, "c1").Should().Not.Be(c2);
        }

        [TestMethod]
        public void ExtensionAndTest()
        {
            var c2 = new MyClass2 { Value = 5 };

            var c1 = new MyClass1 { Class2 = c2 };

            c1.On(x => x.Class2, "c1").Should().Not.Be(null).And(x => x.Value).Be(5);
        }
    }
}
