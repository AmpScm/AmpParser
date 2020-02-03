using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amp.SqlParser;
using K = Amp.SqlParser.SqlKind;

namespace AmpSqlParser.Tests
{
    [TestClass]
    public class TokenizerTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestEmpty()
        {
            using (StringReader sr = new StringReader(""))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                Assert.AreEqual(0, kinds.Length);

                ParseAssert.AreEqual(new SqlKind[] {
                    },
                    kinds);

                Assert.AreEqual("", string.Join("", tokens.Select(x => x.ToFullString())));
            }

            using (StringReader sr = new StringReader(" "))
            using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
            using (SqlTokenizer tk = new SqlTokenizer(rdr))
            {
                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                Assert.AreEqual(1, kinds.Length);

                ParseAssert.AreEqual(new SqlKind[] {
                        K.EndOfStream,
                    },
                    kinds);

                Assert.AreEqual(" ", string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }


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
                                Console.WriteLine($"{n++:000}: {v.Kind}: {v} ({v.Position})");
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                Assert.AreEqual(28, kinds.Length);

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.AsteriksOperatorToken, K.FromToken, K.IdentifierToken, K.IdentifierToken,
                    K.LeftToken, K.JoinToken, K.IdentifierToken, K.IdentifierToken, K.OnToken, K.IdentifierToken, K.DotToken, K.IdentifierToken, K.EqualOperatorToken, K.IdentifierToken, K.DotToken, K.IdentifierToken,
                    K.WhereToken, K.IdentifierToken, K.EqualOperatorToken, K.IdentifierToken, K.AndToken, K.IdentifierToken, K.EqualOperatorToken, K.NumericValueToken,
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.NumericValueToken},
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.NumericValueToken},
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.NumericValueToken},
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.NumericValueToken},
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.NumericValueToken},
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.NumericValueToken, K.EndOfStream},
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.NumericValueToken},
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.NumericValueToken},
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken, K.NumericValueToken, K.EndOfStream},
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

                var tokens = tk.ToArray();

                var kinds = tokens.Select(x => x.Kind).ToArray();

                Assert.AreEqual(22, kinds.Length);

                ParseAssert.AreEqual(new SqlKind[] {
                    K.SelectToken,
                        K.NumericValueToken,  K.PlusOperatorToken,
                        K.DoubleValueToken, K.AsteriksOperatorToken,
                        K.DoubleValueToken, K.DivOperatorToken,
                        K.DoubleValueToken, K.CommaToken,
                        K.CaseToken, K.WhenToken,
                        K.NumericValueToken, K.EqualOperatorToken,
                        K.NumericValueToken,
                        K.ThenToken,
                        K.ColonOperatorToken, K.IdentifierToken,
                        K.ElseToken, K.StringToken, K.EndToken,
                    K.FromToken, K.IdentifierToken},
                    kinds);

                Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
            }
        }

        [TestMethod]
        public void TestIdentifier()
        {
            foreach (SqlDialect dialect in Enum.GetValues(typeof(SqlDialect)))
            {
                string expected;

                using (StringReader sr = new StringReader(expected = @"SELECT aa"))
                using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
                using (SqlTokenizer tk = new SqlTokenizer(rdr))
                {
                    tk.Dialect = dialect;
                    var tokens = tk.ToArray();

                    var kinds = tokens.Select(x => x.Kind).ToArray();

                    Assert.AreEqual(2, kinds.Length);

                    ParseAssert.AreEqual(new SqlKind[] {
                        K.SelectToken, K.IdentifierToken},
                    kinds);

                    Assert.AreEqual("aa", tokens.Last().ToString());
                    Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
                }

                using (StringReader sr = new StringReader(expected = @"SELECT ""aa"""))
                using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
                using (SqlTokenizer tk = new SqlTokenizer(rdr))
                {
                    tk.Dialect = dialect;

                    var tokens = tk.ToArray();

                    var kinds = tokens.Select(x => x.Kind).ToArray();

                    Assert.AreEqual(2, kinds.Length);

                    ParseAssert.AreEqual(new SqlKind[] {
                        K.SelectToken, K.QuotedIdentifierToken},
                    kinds);

                    Assert.AreEqual("\"aa\"", tokens.Last().ToString());
                    Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
                }

                using (StringReader sr = new StringReader(expected = @"SELECT ""aa"))
                using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
                using (SqlTokenizer tk = new SqlTokenizer(rdr))
                {
                    tk.Dialect = dialect;

                    var tokens = tk.ToArray();

                    var kinds = tokens.Select(x => x.Kind).ToArray();

                    Assert.AreEqual(2, kinds.Length);

                    ParseAssert.AreEqual(new SqlKind[] {
                        K.SelectToken, K.IncompleteQuotedIdentifierToken},
                    kinds);

                    Assert.AreEqual("\"aa", tokens.Last().ToString());
                    Assert.IsTrue(tokens.Last().IsError);
                    Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
                }

                using (StringReader sr = new StringReader(expected = @"SELECT [aa"))
                using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
                using (SqlTokenizer tk = new SqlTokenizer(rdr))
                {
                    tk.Dialect = dialect;

                    var tokens = tk.ToArray();

                    var kinds = tokens.Select(x => x.Kind).ToArray();

                    if (dialect == SqlDialect.SqLite || dialect == SqlDialect.SqlServer)
                    {
                        Assert.AreEqual(2, kinds.Length);

                        ParseAssert.AreEqual(new SqlKind[] {
                            K.SelectToken, K.IncompleteQuotedIdentifierToken},
                        kinds);

                        Assert.AreEqual("[aa", tokens.Last().ToString());
                        Assert.IsTrue(tokens.Last().IsError);
                    }
                    else
                    {
                        Assert.AreEqual(3, kinds.Length);

                        ParseAssert.AreEqual(new SqlKind[] {
                            K.SelectToken, K.OpenBracket, K.IdentifierToken},
                        kinds);

                        Assert.AreEqual("aa", tokens.Last().ToString());
                        Assert.IsFalse(tokens.Last().IsError);
                    }
                    Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
                }

                using (StringReader sr = new StringReader(expected = @"SELECT [aa]"))
                using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
                using (SqlTokenizer tk = new SqlTokenizer(rdr))
                {
                    tk.Dialect = dialect;

                    var tokens = tk.ToArray();

                    var kinds = tokens.Select(x => x.Kind).ToArray();

                    if (dialect == SqlDialect.SqLite || dialect == SqlDialect.SqlServer)
                    {
                        Assert.AreEqual(2, kinds.Length);

                        ParseAssert.AreEqual(new SqlKind[] {
                            K.SelectToken, K.QuotedIdentifierToken},
                        kinds);

                        Assert.AreEqual("[aa]", tokens.Last().ToString());
                        Assert.IsFalse(tokens.Last().IsError);
                    }
                    else
                    {
                        Assert.AreEqual(4, kinds.Length);

                        ParseAssert.AreEqual(new SqlKind[] {
                            K.SelectToken, K.OpenBracket, K.IdentifierToken, K.CloseBracket},
                        kinds);

                        Assert.AreEqual("aa", tokens.Reverse().Skip(1).First().ToString());
                        Assert.IsFalse(tokens.Last().IsError);
                    }
                    Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
                }
            }
        }

        [TestMethod]
        public void LegacyJoin()
        {
            foreach (SqlDialect dialect in Enum.GetValues(typeof(SqlDialect)))
            {
                string expected;

                using (StringReader sr = new StringReader(expected = @"SELECT a from A, B WHERE a.Id(+) = b.Id"))
                using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
                using (SqlTokenizer tk = new SqlTokenizer(rdr))
                {
                    tk.Dialect = dialect;
                    var tokens = tk.ToArray();
                    var kinds = tokens.Select(x => x.Kind).ToArray();

                    if (dialect != SqlDialect.Oracle)
                    {
                        Assert.AreEqual(17, kinds.Length);

                        ParseAssert.AreEqual(new SqlKind[] {
                            K.SelectToken, K.IdentifierToken, K.FromToken, K.IdentifierToken,
                            K.CommaToken,
                            K.IdentifierToken,
                            K.WhereToken, K.IdentifierToken, K.DotToken, K.IdentifierToken,
                            //
                            K.OpenParenToken, K.PlusOperatorToken, K.CloseParenToken,
                            //
                            K.EqualOperatorToken,
                            K.IdentifierToken, K.DotToken, K.IdentifierToken
                        },
                        kinds);
                    }
                    else
                    {
                        Assert.AreEqual(15, kinds.Length);

                        ParseAssert.AreEqual(new SqlKind[] {
                        K.SelectToken, K.IdentifierToken, K.FromToken, K.IdentifierToken,
                        K.CommaToken,
                        K.IdentifierToken,
                        K.WhereToken, K.IdentifierToken, K.DotToken, K.IdentifierToken,
                        //
                        K.OuterJoinToken,
                        //
                        K.EqualOperatorToken,
                        K.IdentifierToken, K.DotToken, K.IdentifierToken
                        },
                        kinds);
                    }

                    Assert.AreEqual(expected.Replace("\r", ""), string.Join("", tokens.Select(x => x.ToFullString())));
                }


                using (StringReader sr = new StringReader(expected = @"SELECT a from A, B WHERE a.Id(+ = b.Id"))
                using (SqlReader rdr = new SqlReader(sr, new SqlSource("<memory>")))
                using (SqlTokenizer tk = new SqlTokenizer(rdr))
                {
                    tk.Dialect = dialect;
                    var tokens = tk.ToArray();
                    var kinds = tokens.Select(x => x.Kind).ToArray();

                    Assert.AreEqual(16, kinds.Length);

                    ParseAssert.AreEqual(new SqlKind[] {
                            K.SelectToken, K.IdentifierToken, K.FromToken, K.IdentifierToken,
                            K.CommaToken,
                            K.IdentifierToken,
                            K.WhereToken, K.IdentifierToken, K.DotToken, K.IdentifierToken,
                            //
                            K.OpenParenToken, K.PlusOperatorToken,
                            //
                            K.EqualOperatorToken,
                            K.IdentifierToken, K.DotToken, K.IdentifierToken
                        },
                    kinds);
                }
            }
        }
    }
}
