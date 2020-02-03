using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amp.Parser;

namespace Amp.Syntax
{
    public abstract class AmpSyntaxList<TKind, TElement> : AmpSyntax<TKind>
        where TKind : struct, Enum
        where TElement : AmpSyntax<TKind>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual int Count => Items.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TElement this[int index] => Items[index];

        /// <summary>
        /// 
        /// </summary>
        public abstract IReadOnlyList<TElement> Items { get; }
    }
}
