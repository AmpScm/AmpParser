using System;
using System.Collections.Generic;
using System.Text;
using Amp.Parser;

namespace Amp.SqlParser.Syntax
{
    public class SqlSelectSource : SqlSyntaxElement, ISqlParsable
    {
        public SqlExpression Expression { get; }
        public SqlToken AsToken { get; }
        public SqlToken NameToken { get; }

        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            yield return Expression;

            if (AsToken != null)
                yield return AsToken;

            if (NameToken != null)
                yield return NameToken;
        }


        #region Parser support
        protected SqlSelectSource(SqlParserState state, out AmpElement error)
        {
            Kind = SqlKind.SqlSelectSource;
            if (SqlParser.TryParse<SqlExpression>(state, out var expr))
            {
                Expression = expr;
            }

            if (state.IsKind(SqlKind.AsToken))
            {
                AsToken = state.CurrentToken;
                state.Read();
            }

            if (state.IsKind(SqlKind.IdentifierToken) || state.IsKind(SqlKind.QuotedIdentifierToken))
            {
                NameToken = state.CurrentToken;
                state.Read();
            }
            else if (AsToken != null)
            {
                error = SqlParseError.Construct(state);
                return;
            }

            error = null;
        }
        #endregion
    }
}
