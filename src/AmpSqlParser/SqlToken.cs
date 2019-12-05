using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using AmpTokenizer;

namespace AmpSqlParser
{
    public class SqlToken : AmpToken<SqlKind>
    {
        readonly SqlTrivia[] _leading, _trailing;
        readonly string _value;
        readonly SqlPosition _position;

        public SqlToken(SqlKind kind, string value, SqlPosition position, IEnumerable<SqlTrivia> leading, IEnumerable<SqlTrivia> trailing)
            : base(kind)
        {
            if (leading?.Any() ?? false)
                _leading = leading.ToArray();
            if (trailing?.Any() ?? false)
                _trailing = trailing.ToArray();

            _value = value;
            _position = position;
        }

        public override AmpRange GetRange(bool includeTrivia)
        {
            return new AmpRange(_position, _position);
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
