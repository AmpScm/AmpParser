using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Parser;
using Amp.Syntax;

namespace Amp.SqlParser.Syntax
{
    public class SqlExpression : SqlSyntaxElement, ISqlParsable
    {
        List<AmpElement<SqlKind>> items = new List<AmpElement<SqlKind>>();

        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            return items.GetEnumerator();
        }


        #region Parser
        protected SqlExpression(SqlParserState state, out AmpElement error)
        {
            if (state.IsKind(SqlKind.NotToken, SqlKind.MinusOperatorToken, SqlKind.PlusOperatorToken, SqlKind.TildeOperatorToken))
            {
                items.Add(state.CurrentToken);
                state.Read();
            }

            if (state.IsKind(SqlKind.ExistsToken) && state.PeekKind(SqlKind.OpenParenToken))
            {
                items.Add(state.CurrentToken);
                state.Read();
                // Fall through in OpenParenToken
            }

            if (state.IsKind(SqlKind.OpenParenToken))
            {
                items.Add(state.CurrentToken);
                state.Read();

                if (state.IsKind(SqlKind.SelectToken)
                    && SqlParser.TryParse<SqlSelect>(state, out var subSelect))
                {
                    items.Add(subSelect);
                }
                else if (SqlParser.TryParse<SqlExpression>(state, out var expr))
                {
                    items.Add(expr);
                }
                else
                {
                    error = state.Error ?? SqlParseError.Construct(state);
                    return;
                }

                if (state.IsKind(SqlKind.CloseParenToken))
                {
                    items.Add(state.CurrentToken);
                    state.Read();
                }
                else
                {
                    error = state.Error ?? SqlParseError.Construct(state);
                    return;
                }
            }
            else if (state.IsKind(SqlKind.IdentifierToken) || state.IsKind(SqlKind.QuotedIdentifierToken))
            {
                if (state.PeekItem(0)?.Kind == SqlKind.DotToken 
                    && (state.PeekItem(1)?.Kind == SqlKind.IdentifierToken || state.PeekItem(1)?.Kind == SqlKind.QuotedIdentifierToken))
                {
                    items.Add(new AmpElementList<SqlKind>(state.ReadMany(3)));
                }
                else
                {
                    items.Add(state.CurrentToken);
                    state.Read();
                }

                if (state.IsKind(SqlKind.OpenParenToken))
                {
                    items.Add(state.CurrentToken);
                    state.Read();

                    if (!SqlParser.TryParse<SqlCommaSeparatedTokenList<SqlExpression>>(state, out var arguments))
                    {
                        error = state.Error;
                        return;
                    }

                    items.Add(arguments);

                    if (state.IsKind(SqlKind.CloseParenToken))
                    {
                        items.Add(state.CurrentToken);
                        state.Read();
                    }
                    else
                    {
                        error = SqlParseError.Construct(state);
                        return;
                    }
                }
            }
            else if (state.IsKind(SqlKind.NumericValueToken, SqlKind.DoubleValueToken, SqlKind.StringToken, SqlKind.NullToken))
            {
                items.Add(state.CurrentToken);
                state.Read();
            }
            else if (state.IsKind(SqlKind.CaseToken))
            {
                if (!SqlParser.TryParse<SqlCaseClause>(state, out var @case))
                {
                    error = state.Error;
                    return;
                }
                items.Add(@case);
            }
            else
            {
                error = state.Error ?? SqlParseError.Construct(state);
                return;
            }

            switch (state.Current.Kind)
            {
                case SqlKind.IsNullToken:
                case SqlKind.NotNullToken:
                    items.Add(state.CurrentToken);
                    state.Read();
                    break;
                case SqlKind.NotToken when state.PeekKind(SqlKind.NullToken):
                    items.Add(state.CurrentToken);
                    state.Read();
                    items.Add(state.CurrentToken);
                    state.Read();
                    break;
            }

            switch (state.Current.Kind)
            {
                case SqlKind.EqualOperatorToken:
                case SqlKind.NotEqualToken:
                case SqlKind.LessThanOrEqualToken:
                case SqlKind.LessThanToken:
                case SqlKind.GreaterThanOrEqualToken:
                case SqlKind.GreaterThanToken:
                case SqlKind.AndToken:
                case SqlKind.OrToken:
                case SqlKind.LikeToken:
                case SqlKind.ConcatToken:
                case SqlKind.PlusOperatorToken:
                case SqlKind.MinusOperatorToken:
                case SqlKind.AsteriksOperatorToken:
                case SqlKind.DivOperatorToken:
                case SqlKind.ShiftLeftToken:
                case SqlKind.InToken:
                case SqlKind.ShiftRightToken:
                case SqlKind.BetweenToken:
                case SqlKind.IsToken:
                    if (state.Current.Kind == SqlKind.IsToken && state.PeekKind(SqlKind.NotToken))
                    {
                        items.Add(state.CurrentToken);
                        state.Read();
                    }
                    items.Add(state.CurrentToken);
                    state.Read();

                    if (SqlParser.TryParse<SqlExpression>(state, out var subExpression))
                    {
                        items.Add(subExpression);
                    }
                    else
                    {
                        error = state.Error;
                        return;
                    }
                    break;
                case SqlKind.NotToken when state.PeekKind(SqlKind.BetweenToken, SqlKind.InToken):
                    // TODO: Many more
                    items.Add(state.CurrentToken);
                    state.Read();
                    goto case SqlKind.BetweenToken;

                default:
                    break;
            }

            error = null;
        }
        #endregion
    }
}
