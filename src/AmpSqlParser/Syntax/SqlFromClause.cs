using System;
using System.Collections.Generic;
using System.Text;
using Amp.Parser;

namespace Amp.SqlParser.Syntax
{
    public class SqlFromClause : SqlSyntaxElement, ISqlParsable
    {
        public SqlToken FromToken { get; }

        public SqlSeparatedTokenList<SqlSelectSource> Sources { get; }

        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            yield return FromToken;
            yield return Sources;
        }

        #region Parser Support

        protected SqlFromClause(SqlParserState state, out AmpElement error)
        {
            Kind = SqlKind.FromClause;
            if (state.IsKind(SqlKind.FromToken))
            {
                FromToken = state.CurrentToken;
                state.Read();
            }
            else
            {
                error = SqlParseError.Construct(state);
                return;
            }

            if (SqlParser.TryParse<SqlCommaSeparatedTokenList<SqlSelectSource>>(state, out var sources))
            {
                Sources = sources;
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
