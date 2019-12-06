using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Amp.Linq;
using Amp.Parser;
using Amp.Tokenizer;

namespace Amp.SqlParser
{
    public class SqlSyntaxElement : AmpElement<SqlKind>, IEnumerable<AmpElement<SqlKind>>, ITree<AmpElement<SqlKind>>
    {
        public virtual IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            return Enumerable.Empty<AmpElement<SqlKind>>().GetEnumerator();
        }

        public override AmpRange GetRange(bool includeTrivia)
        {
            throw new NotImplementedException();
        }

        public override void WriteTo(TextWriter tw, IFormatProvider formatProvider, string format, bool leading, bool following)
        {
            AmpElement<SqlKind> last = null;

            // Apply leading and following to just the first and last item
            foreach (AmpElement<SqlKind> el in this.AsTree().Leafs)
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
    }
}
