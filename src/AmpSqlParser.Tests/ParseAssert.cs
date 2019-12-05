using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmpSqlParser.Tests
{
    static class ParseAssert
    {
        public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            AreEqual(expected, actual, null);
        }
        public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message)
        {
            if (!string.IsNullOrEmpty(message))
                message = ": " + message;
            else
                message = null;

            if ((expected == null) != (actual == null))
            {   
                if (expected == null)
                    Assert.Fail("Expected value is null, while actual is not" + message);
                else
                    Assert.Fail("Expected value is not null, while actual is" + message);
            }
            else if (expected == null)
                return;

            using (var e1 = expected.GetEnumerator())
            using (var e2 = actual.GetEnumerator())
            {
                bool b1 = false, b2 = false;

                int n = 0;
                while ((b1 = e1.MoveNext()) & (b2 = e2.MoveNext())) // No short circuit '&&'
                {
                    if (!e1.Current.Equals(e2.Current))
                        Assert.AreEqual(e1.Current, e2.Current, $"Position {n}:{message}");

                    n++;
                }

                if (b1 || b2)
                    Assert.Fail((b1 ? "Expected list is longer than actual" : "Expected list is shorter than actual") + message);
            }
        }
    }
}
