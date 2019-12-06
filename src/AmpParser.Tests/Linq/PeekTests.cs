using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmpParser.Tests.Linq
{
    [TestClass]
    public class PeekTests
    {
        [TestMethod]
        public void WalkSome()
        {
            string line = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int n = 0;

            foreach (var p in line.AsPeekable())
            {
                Assert.AreEqual(line[n], p.Value);
                n++;

                Assert.AreEqual(line.Substring(n), string.Join("", p.Peek));
            }
        }

        [TestMethod]
        public void WalkAZCallback()
        {
            string line = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int n = 0;
            int ii = 0;

            Func<char?> getNext = () =>
            {
                if (ii < line.Length)
                    return (char?)line[ii++];
                else
                    return null;
            };

            foreach (var p in getNext.AsPeekable())
            {
                Assert.AreEqual(line[n], p.Value);
                n++;

                Assert.AreEqual(line.Substring(n), string.Join("", p.Peek));
            }
        }


        [TestMethod]
        public void WalkAZCallbackRef()
        {
            string line = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int n = 0;
            int ii = 0;

            Func<string> getNext = () =>
            {
                if (ii < line.Length)
                    return line[ii++].ToString();
                else
                    return null;
            };

            foreach (var p in getNext.AsPeekable())
            {
                Assert.AreEqual(line.Substring(n, 1), p.Value);
                n++;

                Assert.AreEqual(line.Substring(n), string.Join("", p.Peek));
            }
        }
    }
}
