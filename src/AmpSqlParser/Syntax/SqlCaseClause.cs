using System.Collections.Generic;
using Amp.Parser;

namespace Amp.SqlParser.Syntax
{
    public class SqlCaseClause : SqlSyntaxElement, ISqlParsable
    {
        public SqlToken CaseToken { get; }
        public SqlExpression CaseExpression { get; }
        public SqlSyntaxList<SqlWhenClause> Whens { get; }

        public SqlToken ElseToken { get; }
        public SqlExpression ElseExpression { get; }
        public SqlToken EndToken { get; }

        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            yield return CaseToken;

            if (CaseExpression != null)
                yield return CaseExpression;

            yield return Whens;

            if (ElseToken != null)
            {
                yield return ElseToken;
                yield return ElseExpression;
            }

            yield return EndToken;
        }

        #region Parser
        protected SqlCaseClause(SqlParserState state, out AmpElement error)
        {
            Kind = SqlKind.SqlCaseClause;
            if (!state.IsKind(SqlKind.CaseToken))
            {
                error = SqlParseError.Construct(state);
                return;
            }

            CaseToken = state.CurrentToken;
            state.Read();

            if (!state.IsKind(SqlKind.WhenToken, SqlKind.ElseToken, SqlKind.EndToken))
            {
                if (!SqlParser.TryParse<SqlExpression>(state, out var caseExpr))
                {
                    error = SqlParseError.Construct(state);
                    return;
                }
                CaseExpression = caseExpr;
            }

            List<SqlWhenClause> whens = new List<SqlWhenClause>();
            while(state.IsKind(SqlKind.WhenToken))
            {
                SqlToken whenToken = state.CurrentToken;
                state.Read();

                if (!SqlParser.TryParse<SqlExpression>(state, out var whenExpr))
                {
                    error = SqlParseError.Construct(state);
                    return;
                }

                if (!state.IsKind(SqlKind.ThenToken))
                {
                    error = SqlParseError.Construct(state);
                    return;
                }
                SqlToken thenToken = state.CurrentToken;
                state.Read();


                if (!SqlParser.TryParse<SqlExpression>(state, out var thenExpr))
                {
                    error = SqlParseError.Construct(state);
                    return;
                }

                whens.Add(new SqlWhenClause(whenToken, whenExpr, thenToken, thenExpr));
            }
            Whens = SqlSyntaxList<SqlWhenClause>.FromItems(whens);

            if (state.IsKind(SqlKind.ElseToken))
            {
                ElseToken = state.CurrentToken;
                state.Read();

                if (!SqlParser.TryParse<SqlExpression>(state, out var elseExpr))
                {
                    error = SqlParseError.Construct(state);
                    return;
                }
                ElseExpression = elseExpr;
            }

            if (state.IsKind(SqlKind.EndToken))
            {
                EndToken = state.CurrentToken;
                state.Read();
                error = null;
            }
            else
            {
                error = SqlParseError.Construct(state);
            }
        }
        #endregion
    }

    public class SqlWhenClause : SqlSyntaxElement
    {
        public SqlToken WhenToken { get; }
        public SqlExpression WhenExpression { get; }
        public SqlToken ThenToken { get; }
        public SqlExpression ThenExpression { get; }

        internal SqlWhenClause(SqlToken whenToken, SqlExpression whenExpression, SqlToken thenToken, SqlExpression thenExpression)
        {
            Kind = SqlKind.WhenClause;
            WhenToken = whenToken;
            WhenExpression = whenExpression;
            ThenToken = thenToken;
            ThenExpression = thenExpression;
        }

        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            yield return WhenToken;
            yield return WhenExpression;
            yield return ThenToken;
            yield return ThenExpression;
        }
    }
}