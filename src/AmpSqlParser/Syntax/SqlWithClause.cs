using System;
using System.Collections.Generic;
using System.Text;
using Amp.Parser;

namespace Amp.SqlParser.Syntax
{
    public class SqlWithClause : SqlSyntaxElement, ISqlParsable
    {
        protected SqlWithClause(SqlParserState state, out AmpElement error)
        {
            error = SqlParseError.Construct(state);
        }

        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
