using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmpSqlParser.Tests
{
    [TestClass]
    public class ParserTests
    {
        public TestContext TestContext { get; set; }
        [TestMethod]
        public void ScanAll()
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(typeof(ParserTests).Assembly.Location), "sql-files"));

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
    }
}
