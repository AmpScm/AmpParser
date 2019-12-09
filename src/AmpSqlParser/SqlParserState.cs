using System;
using System.Collections.Generic;
using System.Text;
using Amp.Linq;

namespace Amp.SqlParser
{
    public class SqlParserState 
    {
        public SqlParserState(SqlParserSettings settings, IEnumerable<PeekElement<SqlToken>> tokens)
        {
            Settings = settings;
            Tokens = tokens;
        }

        SqlParserSettings Settings { get; }

        IEnumerable<PeekElement<SqlToken>> Tokens { get; }
    }
}
