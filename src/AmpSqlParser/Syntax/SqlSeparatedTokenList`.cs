using System;
using System.Collections.Generic;
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
}
