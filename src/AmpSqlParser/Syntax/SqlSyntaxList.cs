using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Parser;
using Amp.Syntax;

namespace Amp.SqlParser.Syntax
{
    public abstract class SqlSyntaxList<TElement> : AmpSyntaxList<SqlKind, TElement>
        where TElement : AmpSyntax<SqlKind>
    {
        


        public static SqlSyntaxList<TElement> FromItems(IEnumerable<TElement> items)
        {
            return new ItemSyntaxList(items);
        }

        sealed class ItemSyntaxList : SqlSyntaxList<TElement>
        {
            List<TElement> _items;
            public ItemSyntaxList(IEnumerable<TElement> items)
            {
                _items = new List<TElement>(items);
            }

            public override IReadOnlyList<TElement> Items => _items;

            public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
            {
                return Items.GetEnumerator();
            }
        }
    }
}
