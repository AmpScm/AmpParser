using System;
using System.Collections.Generic;
using System.Text;
using Amp.Parser;

namespace Amp.SqlParser.Syntax
{
    public class SqlJoinClause : SqlSyntaxElement, IAmpParsable<SqlKind>
    {
        public SqlToken SideToken { get; }
        public SqlToken OuterToken { get; }
        public SqlToken JoinToken { get; }

        public SqlSelectSource Source {get;}

        public SqlToken OnToken { get; }

        public SqlExpression OnExpression { get; }

        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            if (SideToken != null)
                yield return SideToken;
            if (OuterToken != null)
                yield return OuterToken;
            if (JoinToken != null)
                yield return JoinToken;

            yield return Source;

            if (OnToken != null)
            {
                yield return OnToken;

                yield return OnExpression;
            }
        }

        #region Parser Support

        protected SqlJoinClause(SqlParserState state, out AmpElement error)
        {
            Kind = SqlKind.JoinClause;

            if (state.IsKind(SqlKind.LeftToken) || state.IsKind(SqlKind.RightToken))
            {
                SideToken = state.CurrentToken;
                state.Read();
            }

            if (state.IsKind(SqlKind.OuterToken) || state.IsKind(SqlKind.InnerToken))
            {
                OuterToken = state.CurrentToken;
                state.Read();
            }

            if (!state.IsKind(SqlKind.JoinToken))
            {
                error = SqlParseError.Construct(state);
                return;
            }

            JoinToken = state.CurrentToken;
            state.Read();

            if (SqlParser.TryParse<SqlSelectSource>(state, out var source))
            {
                Source = source;
            }
            else
            {
                error = state.Error;
                return;
            }

            if (state.IsKind(SqlKind.OnToken))
            {
                OnToken = state.CurrentToken;
                state.Read();


                if (SqlParser.TryParse<SqlExpression>(state, out var onExpression))
                {
                    OnExpression = onExpression;
                }
                else
                {
                    error = state.Error;
                    return;
                }
            }

            error = null;
        }

        #endregion
    }
}
