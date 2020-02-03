using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amp.Linq;
using System.Reflection;

namespace AmpParser.Tests.Linq
{
    [TestClass]
    public class ReflectionTests
    {
        delegate void getOutBool(out bool value);

        [TestMethod]
        public void SimpleGetMethod()
        {
            var m = typeof(ReflectionTests).GetMethod<Action>(nameof(ReflectionTests.Dummy), null, BindingFlags.NonPublic);
            m();

            var mb = typeof(ReflectionTests).GetMethod<Func<bool>>(nameof(ReflectionTests.GetTrue), null, BindingFlags.NonPublic);
            Assert.IsTrue(mb());

            var mgb = typeof(ReflectionTests).GetMethod<getOutBool>(nameof(GetTrueOut), null, BindingFlags.NonPublic);
            mgb(out var v);
            Assert.IsTrue(v);
        }

        delegate DummyConstruct TestConstruct(bool b1, out bool b2);
        [TestMethod]
        public void SimpleGetConstructor()
        {
            var m = typeof(DummyConstruct).GetConstructor<Func<DummyConstruct>>(BindingFlags.Public);
            Assert.IsNotNull(m());

            var mb = typeof(DummyConstruct).GetConstructor<Func<bool, DummyConstruct>>(BindingFlags.Public);
            Assert.IsNotNull(mb(true));

            var mgb = typeof(DummyConstruct).GetConstructor<TestConstruct>(BindingFlags.NonPublic);            
            Assert.IsNotNull(mgb(true, out var v));
            Assert.IsTrue(v);
        }

        delegate void dcGetOutBool(DummyConstruct dc, out bool value);
        [TestMethod]
        public void SimpleInvokeOnMethod()
        {
            DummyConstruct dc = new DummyConstruct();
            var m = typeof(DummyConstruct).GetMethodWithInstance<Action<DummyConstruct>>(nameof(DummyConstruct.Dummy), BindingFlags.Public);
            m(dc);

            var mb = typeof(DummyConstruct).GetMethodWithInstance<Func<DummyConstruct, bool>>(nameof(DummyConstruct.GetTrue), BindingFlags.Public);
            Assert.IsTrue(mb(dc));

            var mgb = typeof(DummyConstruct).GetMethodWithInstance<dcGetOutBool>(nameof(DummyConstruct.GetTrueOut), BindingFlags.Public);
            mgb(dc, out var v);
            Assert.IsTrue(v);
        }

        #region Used by tests

        static void Dummy()
        {

        }

        static bool GetTrue()
        {
            return true;
        }

        static void GetTrueOut(out bool value)
        {
            value = true;
        }

        class DummyConstruct
        {
            public DummyConstruct()
            {

            }

            public DummyConstruct(bool v)
            {

            }

            protected DummyConstruct(bool v, out bool e)
            {
                e = true;
            }

            public void Dummy()
            {

            }

            public bool GetTrue()
            {
                return true;
            }

            public void GetTrueOut(out bool value)
            {
                value = true;
            }
        }
        #endregion
    }
}
