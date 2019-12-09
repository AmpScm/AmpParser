using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Linq;
using Amp.Parser;

namespace Amp.Syntax
{
    public class AmpSeparatedSyntaxList<TKind, TElement> : AmpSyntaxList<TKind, TElement>
        where TKind: struct, Enum
        where TElement : AmpSyntax<TKind>
    {
        AmpElement<TKind>[] _items;

        protected AmpSeparatedSyntaxList(IEnumerable<AmpElement<TKind>> items)
        {
            if (items == null)
                throw new ArgumentException(nameof(items));

            _items = items.ToArray();
        }

        public override int Count
        {
            get => (_items.Length + 1) / 2;
        }

        public override IEnumerator<AmpElement<TKind>> GetEnumerator()
        {
            return _items.AsEnumerable().GetEnumerator();
        }

        protected override IEnumerable<TElement> GetElements()
        {
            return _items.EvenItems().Cast<TElement>();
        }
    }
}
