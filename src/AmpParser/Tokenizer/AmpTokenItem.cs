using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Amp.Parser;

namespace Amp.Tokenizer
{
    [DebuggerDisplay("Token: {Token}")]
    public class AmpTokenItem<TToken, TKind>
        where TToken : AmpToken<TKind>
        where TKind : struct, Enum
    {
        readonly TToken _current;
        AmpTokenItem<TToken, TKind> _next;

        public AmpTokenItem(TToken token, AmpTokenizer<TToken, TKind> tokenizer)
        {
            Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            _current = token ?? throw new ArgumentNullException(nameof(token));
        }

        public AmpTokenStream<TToken, TKind> Peek()
        {
            return new AmpTokenStream<TToken, TKind>(GetNext(), Tokenizer);
        }

        internal AmpTokenItem<TToken, TKind> GetNext()
        {
            if (_next == null)
            {
                TToken nextToken;
                if (Tokenizer.ReadNext(out nextToken))
                {
                    _next = new AmpTokenItem<TToken, TKind>(nextToken, Tokenizer);
                }
            }

            return _next;            
        }

        public TToken Token => _current;
        public AmpPosition Position => Token?.Position;
        public AmpTokenizer<TToken, TKind> Tokenizer { get; }

        public TKind Kind => (Token != null) ? Token.Kind : default(TKind);


        public static implicit operator TToken(AmpTokenItem<TToken, TKind> item) => item._current;
    }
}
