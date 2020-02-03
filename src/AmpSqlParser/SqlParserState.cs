using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Linq;
using Amp.Parser;

namespace Amp.SqlParser
{
    public class SqlParserState
    {
        public SqlParserState(SqlParserSettings settings, IEnumerableWithPeek<AmpElement<SqlKind>> tokens)
        {
            Settings = settings;
            Tokens = tokens.GetEnumerator();
            Tokens.MoveNext();
        }

        protected IEnumerator<PeekElement<AmpElement<SqlKind>>> Tokens { get; }

        public AmpElement<SqlKind> Current => Tokens.Current?.Value;
        public IEnumerable<AmpElement<SqlKind>> Peek => Tokens.Current?.Peek ?? Enumerable.Empty<AmpElement<SqlKind>>();


        public AmpElement<SqlKind> PeekItem(int index)
        {
            return this.Peek.Skip(index).FirstOrDefault();
        }


        public SqlToken CurrentToken => Current as SqlToken ?? throw new InvalidOperationException();

        public bool IsKind(SqlKind kind)
        {
            return Current?.Kind == kind;
        }

        public bool IsKind(params SqlKind[] kinds)
        {
            var ck = Current?.Kind;
            foreach (var k in kinds)
            {
                if (k == ck)
                    return true;
            }
            return false;
        }

        internal IEnumerable<AmpElement<SqlKind>> ReadMany(int n)
        {
            List<AmpElement<SqlKind>> tokens = new List<AmpElement<SqlKind>>();
            while (n-- > 0)
            {
                tokens.Add(Current);

                if (!Read())
                    return tokens;
            }

            return tokens;
        }

        public bool Read()
        {
            return Tokens.MoveNext();
        }

        public SqlParserSettings Settings { get; }
        public AmpElement Error
        {
            get;
            internal set;
        }

        internal bool PeekKind(SqlKind kind)
        {
            SqlKind? k = Peek.FirstOrDefault()?.Kind;

            return (kind == k);
        }

        internal bool PeekKind(params SqlKind[] kinds)
        {
            SqlKind? k = Peek.FirstOrDefault()?.Kind;

            return kinds.Any(x => x == k);
        }
    }
}
