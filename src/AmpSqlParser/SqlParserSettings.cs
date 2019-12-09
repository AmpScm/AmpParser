using System;
using System.Collections.Generic;
using System.Text;

namespace Amp.SqlParser
{
    public class SqlParserSettings
    {
        public SqlDialect Dialect { get; set; } = SqlDialect.Sql1999;
    }
}
