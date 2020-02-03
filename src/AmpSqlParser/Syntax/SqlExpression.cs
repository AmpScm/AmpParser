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
            if (state.IsKind(SqlKind.NotToken) || state.IsKind(SqlKind.MinusOperatorToken))
            {
                items.Add(state.CurrentToken);
                state.Read();
            }

            if (state.IsKind(SqlKind.OpenParenToken))
            {
                int n = 1;

                do
                {
                    items.Add(state.CurrentToken);
                    state.Read();

                    if (state.IsKind(SqlKind.CloseParenToken))
                    {
                        if (--n == 0)
                        {
                            items.Add(state.CurrentToken);
                            state.Read();
                            break;
                        }
                        else
                            continue;
                    }
                    else if (state.IsKind(SqlKind.OpenParenToken))
                        n++;

                }
                while (state.Peek.Any());
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

            switch(state.Current.Kind)
            {
                case SqlKind.EqualOperatorToken:
                case SqlKind.IsToken:
                case SqlKind.NotEqualToken:
                case SqlKind.LessThanOrEqualToken:
                case SqlKind.LessThanToken:
                case SqlKind.GreaterThanOrEqualToken:
                case SqlKind.GreaterThanToken:
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
            }

            error = null;
        }
        #endregion
    }
}
