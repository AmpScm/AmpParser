using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Amp.Linq
{
    [DebuggerDisplay("{Value, nq}")]
    public sealed class PeekElement<T>
    {
        Lazy<PeekElement<T>> _peekNext;

        public PeekElement(T value, Lazy<PeekElement<T>> peekNext)
        {
            Value = value;
            peekNext = _peekNext = peekNext;
        }


        public T Value { get; }


        internal IEnumerable<PeekElement<T>> GetPeek()
        {
            var pn = _peekNext;

            while (pn != null && pn.Value != null)
            {
                yield return pn.Value;

                pn = pn.Value._peekNext;
            }
        }

        public IEnumerable<T> Peek
        {
            get => GetPeek().Select(x => x.Value);
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }

        public override bool Equals(object obj)
        {
            if (obj is PeekElement<T> other)
            {
                return Equals(other);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        public bool Equals(PeekElement<T> other)
        {
            if (other == null)
                return false;

            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public static explicit operator T(PeekElement<T> item)
        {
            return (item != null) ? item.Value : default(T);
        }

    }
}
