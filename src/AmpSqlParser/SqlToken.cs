using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Amp.Parser;
using Amp.Tokenizer;

namespace Amp.SqlParser
{
    public class SqlToken : AmpToken<SqlKind>
    {
        readonly SqlTrivia[] _leading, _trailing;
        readonly string _value;

        public SqlToken(SqlKind kind, string value, SqlPosition startPosition, IEnumerable<SqlTrivia> leading, IEnumerable<SqlTrivia> trailing)
            : base(kind)
        {
            if (leading?.Any() ?? false)
                _leading = leading.ToArray();
            if (trailing?.Any() ?? false)
                _trailing = trailing.ToArray();

            _value = value;
            StartPosition = startPosition;
        }

        public SqlPosition StartPosition { get; }
        public SqlPosition EndPosition => new SqlPosition(StartPosition.Source, StartPosition.Line, StartPosition.Column + _value.Length);


        public override AmpRange GetRange()
        {
            return new AmpRange(StartPosition, EndPosition);
        }
        
        public new IEnumerable<SqlTrivia> LeadingTrivia => _leading ?? Enumerable.Empty<SqlTrivia>();
        public new IEnumerable<SqlTrivia> TrailingTrivia => _trailing ?? Enumerable.Empty<SqlTrivia>();

        protected override IEnumerable<AmpTrivia> GetLeadingTrivia() => LeadingTrivia;
        protected override IEnumerable<AmpTrivia> GetTrailingTrivia() => TrailingTrivia;

        public override void WriteToken(TextWriter tw, IFormatProvider formatProvider, string format)
        {
            tw.Write(_value);
        }
    }
}
