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
        IReadOnlyList<TElement> _publicItems;

        protected AmpSeparatedSyntaxList(IEnumerable<AmpElement<TKind>> items)
        {
            if (typeof(TElement) == typeof(AmpElement<TKind>))
                throw new InvalidOperationException("Enumeration not supported this way");
            SetItems(items);
        }

        protected void SetItems(IEnumerable<AmpElement<TKind>> items)
        {
            if (items == null)
                throw new ArgumentException(nameof(items));

            _items = items.ToArray();
        }

        public override IReadOnlyList<TElement> Items => _publicItems ?? (_publicItems = _items.EvenItems().Cast<TElement>().ToList());

        public override int Count
        {
            get => (_items.Length + 1) / 2;
        }

        public override IEnumerator<AmpElement<TKind>> GetEnumerator()
        {
            return _items.AsEnumerable().GetEnumerator();
        }
    }
}
