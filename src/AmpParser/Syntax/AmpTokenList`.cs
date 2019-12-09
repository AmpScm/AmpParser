using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Parser;

namespace Amp.Syntax
{
    public class AmpTokenList<TKind> : AmpSyntax<TKind>
        where TKind : struct, Enum
    {
        AmpToken<TKind>[] _tokens;

        public AmpTokenList(IEnumerable<AmpToken<TKind>> tokens)
        {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            _tokens = tokens.ToArray();
        }

        protected IEnumerable<AmpToken<TKind>> GetTokens()
        {
            return _tokens;
        }

        public override IEnumerator<AmpElement<TKind>> GetEnumerator()
        {
            return GetTokens().GetEnumerator();
        }
    }
}
