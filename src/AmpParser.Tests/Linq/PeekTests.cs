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
            Assert.AreEqual(26, n);

            var pl = line.AsPeekable();
            pl.Peek.ToArray();
            n = 0;
            foreach (var p in pl)
            {
                Assert.AreEqual(line[n], p.Value);
                n++;

                Assert.AreEqual(line.Substring(n), string.Join("", p.Peek));
            }
            Assert.AreEqual(26, n);
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

        [TestMethod]
        public void TestSkipUntil()
        {
            string line = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int n = 0;

            foreach(var p in line.AsPeekable())
            {
                Assert.AreEqual(line[n], p.Value);
                n++;

                Assert.AreEqual(line.Substring(n), string.Join("", p.Peek));

                if (p == 'E')
                {
                    p.SkipUntil('M'); // Now we continue at 'M'
                    n = 12;
                    Assert.AreEqual('M', p.Peek.First());
                }
            }
        }

        [TestMethod]
        public void TestSkipAll()
        {
            string line = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int n = 0;

            foreach (var p in line.AsPeekable())
            {
                Assert.AreEqual(line[n], p.Value);
                n++;

                Assert.AreEqual(line.Substring(n), string.Join("", p.Peek));

                if (p == 'E')
                {
                    p.SkipUntil('-'); // No more items
                    n = line.Length;
                }
            }
            Assert.AreEqual(line.Length, n);

            n = 0;
            foreach (var p in line.AsPeekable())
            {
                Assert.AreEqual(line[n], p.Value);
                n++;

                Assert.AreEqual(line.Substring(n), string.Join("", p.Peek));

                if (p == 'E')
                {
                    p.SkipUntil('Z'); // One more items
                    n = 25;
                    Assert.AreEqual('Z', p.Peek.First());
                }
            }
            Assert.AreEqual(line.Length, n);

            n = 0;
            foreach (var p in line.AsPeekable())
            {
                Assert.AreEqual(line[n], p.Value);
                n++;

                Assert.AreEqual(line.Substring(n), string.Join("", p.Peek));

                if (p == 'E')
                {
                    p.SkipUntil('E'); // No skip
                }
            }
            Assert.AreEqual(line.Length, n);
        }

        [TestMethod]
        public void TestPush()
        {
            string line = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int n = 0;

            foreach (var p in line.AsPeekable())
            {
                Assert.AreEqual(line[n], p.Value);
                n++;

                Assert.AreEqual(line.Substring(n), string.Join("", p.Peek));

                if (p == 'E')
                {
                    p.SkipUntil('M');
                    p.Push('3');
                    p.Push('2');
                    p.Push('1');

                    line = "ABCDE123MNOPQRSTUVWXYZ";
                }
            }
            Assert.AreEqual(line.Length, n);
        }
    }
}
