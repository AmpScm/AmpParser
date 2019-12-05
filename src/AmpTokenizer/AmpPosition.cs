using System;
using System.Collections.Generic;
using System.Text;

namespace AmpTokenizer
{
    public class AmpPosition : IEquatable<AmpPosition>
    {
        public AmpPosition(AmpSource source, int line, int column)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Line = line >= 0 ? line : throw new ArgumentOutOfRangeException(nameof(line));
            Column = column >= 0 ? column : throw new ArgumentOutOfRangeException(nameof(line));
        }

        public AmpSource Source { get; }
        public int Line { get; }
        public int Column { get; }

        public bool Equals(AmpPosition other)
        {
            if (other == null)
                return false;

            return other.Line == Line && other.Column == Column && other.Source.Equals(Source);
        }

        public override int GetHashCode()
        {
            return (Line * 11) ^ Column;
        }

        public override string ToString()
        {
            return $"{Line + 1}:{Column + 1}";
        }
    }
}
