using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amp.Parser;
using Amp.Tokenizer;

namespace Amp.SqlParser
{
    public class SqlParseError : AmpElement<SqlKind>
    {
        public SqlParseError()
        {
            IsError = true;
        }
        public override AmpRange GetRange()
        {
            return null;
        }

        public override void WriteTo(TextWriter tw, IFormatProvider formatProvider, string format, bool leading, bool following)
        {
            //
        }

        internal static SqlParseError Construct(SqlParserState state)
        {
            return new SqlParseError();
        }
    }
}
