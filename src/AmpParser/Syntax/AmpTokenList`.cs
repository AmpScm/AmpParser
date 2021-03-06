﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Parser;

namespace Amp.Syntax
{
    public class AmpElementList<TKind> : AmpSyntax<TKind>
        where TKind : struct, Enum
    {
        AmpElement<TKind>[] _tokens;

        public AmpElementList(IEnumerable<AmpElement<TKind>> tokens)
        {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            _tokens = tokens.ToArray();
        }

        protected IEnumerable<AmpElement<TKind>> GetElements()
        {
            return _tokens;
        }

        public override IEnumerator<AmpElement<TKind>> GetEnumerator()
        {
            return GetElements().GetEnumerator();
        }
    }
}
