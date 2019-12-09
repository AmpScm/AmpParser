using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Parser;
using Amp.Syntax;

namespace Amp.SqlParser.Syntax
{
    public abstract class SqlSyntaxList<TElement> : AmpSyntaxList<SqlKind, TElement>
        where TElement : AmpSyntax<SqlKind>
    {
        public override int Count => throw new NotImplementedException();        
    }
}
