using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmpParser.Tests.Linq
{
    [TestClass]
    public class TreeLinqTests
    {
        public class TreeString : ITree<TreeString>
        {
            public string Value { get; set; }

            public static implicit operator TreeString(string value)
            {
                return new TreeString() { Value = value };
            }

            public static implicit operator TreeString(TreeString[] items)
            {
                return new TreeStringContainer(items);
            }

            public override string ToString()
            {
                return Value;
            }
        }

        sealed class TreeStringContainer : TreeString, IEnumerable<TreeString>
        {
            readonly IEnumerable<TreeString> _items;
            public TreeStringContainer(params TreeString[] items)
            {
                Value = items.First().Value;
                _items = items.AsEnumerable().Skip(1);
            }

            public IEnumerator<TreeString> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        static TreeString root = "A";
        static TreeString simpleList = new TreeStringContainer("A", "B", "C");
        static TreeString simpleTree = (TreeStringContainer)new TreeString[]
        {
            "A",
            new TreeString[] {
                "B", "C", "D",
                new TreeString[] {
                    "E", "F", "G"
                },
                new TreeString[] {"H", "I", "J", "K"},
                "L",
                new TreeString[] {"M", "N", "O", "P"},
            },
            "Q",
            "R",
            new TreeString[] {
                "S", "T",
                new TreeString[] {
                    "U", "V", "W"
                },
                "X",
                new TreeString[] {"Y"},
            },
            "Z"
        };
        static TreeString heavyTreeTree = new TreeString[]
        {
            "A",
            new TreeString[] {
                "B", "C", "D",               
                new TreeString[] {"M", "N", "O", "P"},
                simpleTree,
            },
            "Q",
            "R",
            new TreeString[] {
                "S", "T",
                "X",
                new TreeString[] {"Y"},
            },
            simpleTree
        };


        [TestMethod]
        public void SimpleDescendantsTest()
        {
            Assert.AreEqual("ABC",
                string.Join("", simpleList.TreeDescendantsAndSelf().Select(x => x.Value)));
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                string.Join("", simpleTree.TreeDescendantsAndSelf().Select(x => x.Value)));

            Assert.AreEqual(26, simpleTree.TreeDescendantsAndSelf().Count());
            int n = 0;
            foreach (var i in simpleTree.TreeDescendantsAndSelf())
            {
                int iPreceding = i.Preceding.Count();
                int iDescendantsAndSelf = i.DescendantsAndSelf.Count();
                int iFollowing = i.Following.Count();

                Assert.AreEqual(26, iPreceding + iDescendantsAndSelf + iFollowing, "Can walk tree via preceding, descendants and following");
                Assert.AreEqual(iDescendantsAndSelf - 1, i.Descendants.Count());

                int nSiblings = i.Ancestors.FirstOrDefault()?.Children.Count() ?? 1;
                Assert.AreEqual(nSiblings, i.PrecedingSiblings.Count() + i.FollowingSiblings.Count() + 1);

                Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                    string.Join("",
                        i.Preceding.Reverse().Concat(
                            i.DescendantsAndSelf).Concat(
                                i.Following).Select(x => x.Value)));
                n++;
            }
            Assert.AreEqual(26, n);
        }

        [TestMethod]
        public void TestQ()
        {
            var Q = simpleTree.TreeDescendantsAndSelf().First(x => x.Value.Value == "Q");
           
            Assert.AreEqual("A", string.Join("", Q.Ancestors));
            Assert.AreEqual("QA", string.Join("", Q.AncestorsAndSelf));
            Assert.AreEqual("", string.Join("", Q.Children));
            Assert.AreEqual("", string.Join("", Q.Descendants));
            Assert.AreEqual("Q", string.Join("", Q.DescendantsAndSelf));
            Assert.AreEqual("RSTUVWXYZ", string.Join("", Q.Following));
            Assert.AreEqual("RSZ", string.Join("", Q.FollowingSiblings));
            Assert.AreEqual("QRSZ", string.Join("", Q.FollowingSiblingsAndSelf));
            Assert.AreEqual("PONMLKJIHGFEDCBA", string.Join("", Q.Preceding));
            Assert.AreEqual("B", string.Join("", Q.PrecedingSiblings));
            Assert.AreEqual("QB", string.Join("", Q.PrecedingSiblingsAndSelf));
            Assert.AreEqual("A", string.Join("", Q.Root));
            Assert.AreEqual("Q", string.Join("", Q.Value));
        }

        [TestMethod]
        public void TestS()
        {
            var S = simpleTree.TreeDescendantsAndSelf().First(x => x.Value.Value == "S");

            Assert.AreEqual("A", string.Join("", S.Ancestors));
            Assert.AreEqual("SA", string.Join("", S.AncestorsAndSelf));
            Assert.AreEqual("TUXY", string.Join("", S.Children));
            Assert.AreEqual("TUVWXY", string.Join("", S.Descendants));
            Assert.AreEqual("STUVWXY", string.Join("", S.DescendantsAndSelf));
            Assert.AreEqual("Z", string.Join("", S.Following));
            Assert.AreEqual("Z", string.Join("", S.FollowingSiblings));
            Assert.AreEqual("SZ", string.Join("", S.FollowingSiblingsAndSelf));
            Assert.AreEqual("RQPONMLKJIHGFEDCBA", string.Join("", S.Preceding));
            Assert.AreEqual("RQB", string.Join("", S.PrecedingSiblings));
            Assert.AreEqual("SRQB", string.Join("", S.PrecedingSiblingsAndSelf));
            Assert.AreEqual("A", string.Join("", S.Root));
            Assert.AreEqual("S", string.Join("", S.Value));
        }

        [TestMethod]
        public void HeavyTree()
        {
            Assert.AreEqual(66, heavyTreeTree.TreeDescendantsAndSelf().Count());
            int n = 0;
            foreach (var i in heavyTreeTree.TreeDescendantsAndSelf())
            {
                int iPreceding = i.Preceding.Count();
                int iDescendantsAndSelf = i.DescendantsAndSelf.Count();
                int iFollowing = i.Following.Count();

                Assert.AreEqual(66, iPreceding + iDescendantsAndSelf + iFollowing, "Can walk tree via preceding, descendants and following");
                Assert.AreEqual(iDescendantsAndSelf - 1, i.Descendants.Count());

                int nSiblings = i.Ancestors.FirstOrDefault()?.Children.Count() ?? 1;
                Assert.AreEqual(nSiblings, i.PrecedingSiblings.Count() + i.FollowingSiblings.Count() + 1);

                Assert.AreEqual("ABCDMNOPABCDEFGHIJKLMNOPQRSTUVWXYZQRSTXYABCDEFGHIJKLMNOPQRSTUVWXYZ",
                    string.Join("",
                        i.Preceding.Reverse().Concat(
                            i.DescendantsAndSelf).Concat(
                                i.Following).Select(x => x.Value)));
                n++;
            }
            Assert.AreEqual(66, n);
        }
    }
}
