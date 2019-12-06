using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using K = AmpSqlParser.SqlKind;

namespace AmpSqlParser.Tests
{
    [TestClass]
    public class TokenizerTests
    {
        public TestContext TestContext { get; set; }
        [TestMethod]
        public void ScanAll()
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(typeof(TokenizerTests).Assembly.Location), "sql-files"));

            foreach (FileInfo fi in dir.GetFiles("*.sql", SearchOption.AllDirectories))
            {
                string file = fi.FullName;

                if (fi.Name.Equals("PROJECTCONTROLS.sql"))
                    continue;
                using (StreamReader sr = File.OpenText(file))
                using (SqlReader rdr = new SqlReader(sr, new SqlSource(file)))
                using (SqlTokenizer tk = new SqlTokenizer(rdr))
                {
                    tk.Dialect = SqlDialect.Oracle;

                    Console.WriteLine($"== {file} ==");
                    int n = 0;

                    List<SqlToken> tokens = new List<SqlToken>();

                    foreach (var v in tk)
                    {
                        switch (v.Kind)
                        {
                            case SqlKind.UnknownCharToken:
                                Console.WriteLine($"{n++:000}: {v.Kind}: {v.Token} ({v.Position})");
                                break;
                        }
                        tokens.Add(v);
                    }

                    string expected = File.ReadAllText(file).Replace("\r", "");
                    string actual = string.Join("", tokens.Select(x => x.ToFullString()));

                    Assert.AreEqual(expected, actual);
                }
            }
        }

        [TestMethod]
        public void TestQueryWithJoin()
        {
            string expected;
            using (StringReader sr = new StringReader(expected = @"
                    SELECT * From MyTable m
                    LEFT JOIN OtherTable o on m.OtherId = o.Id
                    WHERE q=r and s=1
                    ORDER BY Something"))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                Assert.AreEqual(28, kinds.Length);

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.AsteriksToken, K.FromToken, K.IdentifierToken, K.IdentifierToken,
                    K.LeftToken, K.JoinToken, K.IdentifierToken, K.IdentifierToken, K.OnToken, K.IdentifierToken, K.DotToken, K.IdentifierToken, K.EqualOperatorToken, K.IdentifierToken, K.DotToken, K.IdentifierToken,
                    K.WhereToken, K.IdentifierToken, K.EqualOperatorToken, K.IdentifierToken, K.AndToken, K.IdentifierToken, K.EqualOperatorToken, K.IntValueToken,
                    K.OrderToken, K.ByToken, K.IdentifierToken},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestEof1()
        {
            string expected;
            using (StringReader sr = new StringReader(expected = @"
                    /*
                    */ SELECT 1"))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.IntValueToken},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestEof2()
        {
            string expected;
            using (StringReader sr = new StringReader(expected = @"
                    /*
                    */ SELECT 1 /* QQQQ"))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.IntValueToken},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestEof3()
        {
            string expected;
            using (StringReader sr = new StringReader(expected = @"
                    /*
                    */ SELECT 1 /* QQQQ
                    "))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.IntValueToken},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestEof4()
        {
            string expected;
            using (StringReader sr = new StringReader(expected = @"
                    /*
                    */ SELECT 1 /* QQQQ
                    */ --"))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.IntValueToken},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestEof5()
        {
            string expected;
            using (StringReader sr = new StringReader(expected = @"
                    /*
                    */ SELECT 1 /* QQQQ
                    */" + "\r\n"))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.IntValueToken},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestEof5A()
        {
            string expected;
            using (StringReader sr = new StringReader(expected = @"
                    /*
                    */ SELECT 1 /* QQQQ
                    */" + "\r\n "))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.IntValueToken, K.EndOfStream},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestEof6()
        {
            string expected;
            using (StringReader sr = new StringReader(expected = @"
                    /*
                    */ SELECT 1 -- QQQQ"))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.IntValueToken},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestEof6A()
        {
            string expected;
            using (StringReader sr = new StringReader(expected = @"
                    /*
                    */ SELECT 1 -- B" + "\r\n"))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.IntValueToken},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestEof6B()
        {
            string expected;
            using (StringReader sr = new StringReader(expected = @"
                    /*
                    */ SELECT 1 -- C" + "\r\n "))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.IntValueToken, K.EndOfStream},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestSomeOps()
        {
            string expected;

            using (StringReader sr = new StringReader(expected = @"
                    SELECT 1+2.5*3.4E5/4.5e-6, case when 8=9 then :p5 else 'qq' end--QQ
                    FROM DUAL--R"))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                tk.Dialect = SqlDialect.Oracle;

                var tokens = tk.Select(x => x.Token).ToArray().AsQueryable();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                Assert.AreEqual(22, kinds.Length);

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken,
                        K.IntValueToken,  K.PlusOperatorToken,
                        K.DoubleValueToken, K.AsteriksToken,
                        K.DoubleValueToken, K.DivToken,
                        K.DoubleValueToken, K.CommaToken,
                        K.CaseToken, K.WhenToken,
                        K.IntValueToken, K.EqualOperatorToken,
                        K.IntValueToken,
                        K.ThenToken,
                        K.ColonOperatorToken, K.IdentifierToken,
                        K.ElseToken, K.StringToken, K.EndToken,
                    K.FromToken, K.IdentifierToken},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }
    }
}
