using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AmpTokenizer
{
    public abstract class AmpTokenizer<TToken, TKind> : IEnumerable<AmpTokenItem<TToken, TKind>>, IDisposable
        where TToken : AmpToken<TKind>
        where TKind : struct, Enum
    {
        protected AmpTokenizer(AmpReader reader)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        protected AmpReader Reader { get; }

        IEnumerator<AmpTokenItem<TToken, TKind>> IEnumerable<AmpTokenItem<TToken, TKind>>.GetEnumerator()
        {
            return GetTokenStream();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetTokenStream();
        }

        public virtual AmpTokenStream<TToken, TKind> GetTokenStream()
        {
            if (ReadNext(out var nextToken))
            {
                return new AmpTokenStream<TToken, TKind>(new AmpTokenItem<TToken, TKind>(nextToken, this));
            }
            else
                return new AmpTokenStream<TToken, TKind>(null, this);
        }

        IEnumerator<TToken> _getTokens;        

        internal bool ReadNext(out TToken nextToken)
        {
            if (_getTokens == null)
                _getTokens = GetTokens().GetEnumerator();

            if (_getTokens.MoveNext())
            {
                nextToken = _getTokens.Current;
                return true;
            }
            else
            {
                nextToken = null;
                return false;
            }
        }

        protected abstract IEnumerable<TToken> GetTokens();

        protected abstract TKind? TokenForIdentifier(string value);

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Reader.Dispose();
            }
        }
    }

    public sealed class AmpTokenStream<TToken, TKind> : IEnumerator<AmpTokenItem<TToken, TKind>>
        where TToken : AmpToken<TKind>
        where TKind : struct, Enum
    {
        readonly AmpTokenizer<TToken, TKind> _tokenizer;
        AmpTokenItem<TToken, TKind> _current;
        bool _started;

        public AmpTokenStream(AmpTokenItem<TToken, TKind> current)
            : this(current ?? throw new ArgumentNullException(nameof(current)),
                   current?.Tokenizer)
        {
        }

        public AmpTokenStream(AmpTokenItem<TToken, TKind> current, AmpTokenizer<TToken, TKind> tokenizer)
        {
            _current = current;
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        }

        public AmpTokenItem<TToken, TKind> Current
        {
            get => _started ? _current : null;
        }

        public bool MoveNext()
        {
            if (!_started)
            {
                _started = true;
            }
            else
                _current = Current.GetNext();

            return (_current != null);
        }

        #region IEnumerator legacy support
        object IEnumerator.Current => Current;

#pragma warning disable CA1063 // Implement IDisposable Correctly
        void IDisposable.Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
