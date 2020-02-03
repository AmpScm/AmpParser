using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Parser;
using Amp.Syntax;
using Amp.Tokenizer;

namespace Amp.SqlParser.Syntax
{
    public class SqlSeparatedTokenList<TElement> : AmpSeparatedSyntaxList<SqlKind, TElement>
        where TElement : AmpSyntax<SqlKind>
    {
        protected SqlSeparatedTokenList(IEnumerable<AmpElement<SqlKind>> items) : base(items)
        {
        }
    }

    public class SqlCommaSeparatedTokenList<TElement> : SqlSeparatedTokenList<TElement>, ISqlParsable
        where TElement : AmpSyntax<SqlKind>, ISqlParsable
    {
        protected SqlCommaSeparatedTokenList(IEnumerable<AmpElement<SqlKind>> items) : base(items)
        {
        }

        protected SqlCommaSeparatedTokenList(SqlParserState state, out AmpElement error)
            : base(Enumerable.Empty<TElement>())
        {
            List<AmpElement<SqlKind>> items = new List<AmpElement<SqlKind>>();

            while(true)
            {
                if (SqlParser.TryParse<TElement>(state, out var v))
                {
                    items.Add(v);
                }
                else
                {
                    error = state.Error;
                    return;
                }

                if (state.IsKind(SqlKind.CommaToken))
                {
                    items.Add(state.CurrentToken);
                    state.Read();
                    continue;
                }
                else
                    break;
            }
            SetItems(items);
            error = null;
        }
    }
}
