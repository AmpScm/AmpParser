using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Amp.Linq;
using Amp.Parser;
using Amp.Syntax;
using Amp.Tokenizer;

namespace Amp.SqlParser
{
    public abstract class SqlSyntaxElement : AmpSyntax<SqlKind>
    {
        public override AmpRange GetRange()
        {
            return new AmpRange(
                this.Leafs().First().GetRange().Start,
                this.Leafs().Last().GetRange().End);
        }
    }
}
