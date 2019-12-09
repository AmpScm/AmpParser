using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Amp.Parser;

namespace Amp.Tokenizer
{
    public abstract class AmpTokenizer<TToken, TKind> : IEnumerable<TToken>, IDisposable
        where TToken : AmpToken<TKind>
        where TKind : struct, Enum
    {
        bool _started;
        protected AmpTokenizer()
        {
        }

        public IEnumerator<TToken> GetEnumerator()
        {
            if (_started)
                throw new InvalidOperationException();
            _started = true;

            return GetTokens();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract IEnumerator<TToken> GetTokens();

        protected abstract TKind? TokenForIdentifier(string value);

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }    
}
