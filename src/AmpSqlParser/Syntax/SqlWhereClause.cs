using System;
using System.Collections.Generic;
using System.Text;
using Amp.Parser;

namespace Amp.SqlParser.Syntax
{
    public class SqlWhereClause : SqlSyntaxElement, ISqlParsable
    {
        public SqlToken WhereToken { get; }
        public SqlExpression Expression { get; }

        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            yield return WhereToken;
            yield return Expression;
        }

        #region Parser Support
        protected SqlWhereClause(SqlParserState state, out AmpElement error)
        {
            Kind = SqlKind.WhereClause;
            if (state.IsKind(SqlKind.WhereToken))
            {
                WhereToken = state.CurrentToken;
                state.Read();
            }
            else
            {
                error = SqlParseError.Construct(state);
                return;
            }

            if (SqlParser.TryParse<SqlExpression>(state, out var expr))
            {
                Expression = expr;
            }
            else
            {
                error = state.Error;
                return;
            }

            error = null;
        }
        #endregion
    }
}
