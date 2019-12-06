using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amp.Tokenizer;

namespace Amp.SqlParser
{
    public enum SqlTriviaKind
    {
        Whitespace,
        LineComment,
        BlockComment
    }
    public class SqlTrivia : AmpTrivia
    {
        public string Value { get; }
        public SqlTriviaKind Kind { get; }

        public SqlTrivia(SqlTriviaKind kind, string value)
        {
            Kind = kind;
            Value = value;
        }

        public override void WriteTo(TextWriter tw, IFormatProvider formatProvider, string format)
        {
            tw.Write(Value);
        }
    }
}
