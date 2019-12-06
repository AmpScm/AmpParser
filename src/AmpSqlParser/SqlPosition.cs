using System;
using System.Collections.Generic;
using System.Text;
using Amp.Tokenizer;

namespace Amp.SqlParser
{
    public class SqlPosition : AmpPosition
    {
        internal SqlPosition(AmpPosition position)
            : this(position.Source, position.Line, position.Column)
        {
        }

        public SqlPosition(AmpSource source, int line, int column) : base(source, line, column)
        {
        }
    }
}
