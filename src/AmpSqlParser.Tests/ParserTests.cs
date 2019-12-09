using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amp.SqlParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amp.Linq;
using System.Linq;
using Amp.SqlParser.Syntax;

namespace AmpSqlParser.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseAllSelects()
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


                    var peekable = tk.AsPeekable();

                    if (peekable.Peek.FirstOrDefault()?.Kind != SqlKind.SelectToken)
                        continue;


                    Console.WriteLine($"== {file} ==");

                    var state = new SqlParserState(new SqlParserSettings { Dialect = SqlDialect.Oracle }, peekable);

                    //Assert.IsTrue(SqlParser.TryParse<SqlSelect>(state, out var result), $"Parsing succeeed on {file}");
                    //
                    //var rest = state.P

                    Assert.AreEqual(SqlKind.SelectToken, peekable.First().Value.Kind);

                    GC.KeepAlive(peekable);


                }

            }
        }
    }
}
