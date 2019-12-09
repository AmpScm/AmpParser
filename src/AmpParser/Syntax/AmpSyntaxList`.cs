using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Parser;

namespace Amp.Syntax
{
    public abstract class AmpSyntaxList<TKind, TElement> : AmpSyntax<TKind>, IReadOnlyCollection<TElement>
        where TKind : struct, Enum
        where TElement : AmpSyntax<TKind>
    {
        public virtual int Count => GetElements().Count();

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        {
            return GetElements().GetEnumerator();
        }

        public override IEnumerator<AmpElement<TKind>> GetEnumerator()
        {
            return GetElements().GetEnumerator();
        }

        protected abstract IEnumerable<TElement> GetElements();
    }
}
