using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Amp.Linq;
using Amp.Parser;
using Amp.Tokenizer;

namespace Amp.Syntax
{
    [DebuggerDisplay("{Kind}: {ToString(),nq}")]
    public abstract class AmpSyntax<TKind> : AmpElement<TKind>, IEnumerable<AmpElement<TKind>>, ITree<AmpElement<TKind>>
        where TKind : struct, Enum
    {
        public abstract IEnumerator<AmpElement<TKind>> GetEnumerator();

        public override void WriteTo(TextWriter tw, IFormatProvider formatProvider, string format, bool leading, bool following)
        {
            AmpElement<TKind> last = null;

            // Apply leading and following to just the first and last item
            foreach (AmpElement<TKind> el in this.Leafs())
            {
                if (last != null)
                {
                    last.WriteTo(tw, formatProvider, format, leading, true);
                    leading = true;
                }
                last = el;
            }
            if (last != null)
                last.WriteTo(tw, formatProvider, format, leading, following);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override AmpRange GetRange()
        {
            return new AmpRange(
                this.Leafs().First().GetRange().Start,
                this.Leafs().Last().GetRange().End);
        }
    }
}
