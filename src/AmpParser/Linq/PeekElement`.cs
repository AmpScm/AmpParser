using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Amp.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{Value, nq}")]
    public sealed class PeekElement<T> : IEquatable<PeekElement<T>>, IEquatable<T>
    {
        Lazy<PeekElement<T>> _peekNext;

        /// <summary>
        /// Creates a new PeekElement instance
        /// </summary>
        /// <param name="value"></param>
        /// <param name="peekNext"></param>
        public PeekElement(T value, Lazy<PeekElement<T>> peekNext)
        {
            Value = value;
            peekNext = _peekNext = peekNext;
        }

        /// <summary>
        /// Gets the current value
        /// </summary>
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

        /// <summary>
        /// Gets an enumerable containing the next items
        /// </summary>
        public IEnumerable<T> Peek
        {
            get => GetPeek().Select(x => x.Value);
        }

        /// <summary>
        /// Reads items until the next item is <paramref name="untilItem"/>
        /// </summary>
        /// <param name="untilItem"></param>
        public void SkipUntil(T untilItem)
        {
            if ((object)untilItem == null)
                throw new ArgumentNullException(nameof(untilItem));

            if (Equals(untilItem))
                return;

            while (!_peekNext.Value?.Equals(untilItem) ?? false)
            {
                _peekNext = _peekNext.Value._peekNext;
            }
        }

        /// <summary>
        /// Reads
        /// </summary>
        /// <param name="untilItem"></param>
        public void SkipUntilAfter(T untilItem)
        {
            SkipUntil(untilItem);

            if (Equals(GetPeek().FirstOrDefault(), untilItem))
            {
                _peekNext = _peekNext.Value._peekNext;
            }
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }

        #region Equatable support
        public override bool Equals(object obj)
        {
            if (obj is PeekElement<T> other)
            {
                return Equals(other);
            }
            else if (obj is T otherValue)
            {
                return Equals(otherValue);
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

        public bool Equals(T other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other);
        }

        public void Push(T newHead)
        {
            var lastPeek = _peekNext; // Must be local var, to allow capturing in lambda


            _peekNext = new Lazy<PeekElement<T>>(()=> new PeekElement<T>(newHead, lastPeek));
        }

        public static implicit operator T(PeekElement<T> item)
        {
            return (item != null) ? item.Value : default(T);
        }
        #endregion
    }
}
