using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Amp.Linq
{
    [DebuggerDisplay("{Value,nq}")]
    public class TreeElement<T> : IEquatable<TreeElement<T>>
        where T : class, ITree<T>
    {
        public TreeElement(T item)
        {
            Value = item;
            Ancestors = TreeQueryable.EmptyQueryable<TreeElement<T>>();
        }

        public TreeElement(T item, IQueryable<TreeElement<T>> ancestors)
        {
            Value = item;
            Ancestors = ancestors;
        }

        public T Value { get; }

        /// <summary>
        /// Enumerates the ancestors of the node, starting at the direct parent
        /// </summary>
        public IQueryable<TreeElement<T>> Ancestors { get; }

        /// <summary>
        /// Enumerates the node itself and its ancestors, starting at the node itself
        /// </summary>
        public IQueryable<TreeElement<T>> AncestorsAndSelf
        {
            get => TreeQueryable.SingleQueryable(this).Concat(Ancestors);
        }

        public IQueryable<TreeElement<T>> Children
        {
            get => (Value is IEnumerable<T> e) ? e.AsQueryable().Select(x => new TreeElement<T>(x, AncestorsAndSelf)) : TreeQueryable.EmptyQueryable<TreeElement<T>>();
        }

        /// <summary>
        /// Enumerates the descendants of the node in a depth first order. (So first child, children of that, etc.)
        /// </summary>
        public IQueryable<TreeElement<T>> Descendants
        {
            get => Children.SelectMany(x => TreeQueryable.SingleQueryable(x).Concat(x.Descendants));
        }

        /// <summary>
        /// Enumerates the node and its descendants
        /// </summary>
        public IQueryable<TreeElement<T>> DescendantsAndSelf
        {
            get => TreeQueryable.SingleQueryable(this).Concat(Descendants);
        }


        /// <summary>
        /// Enumerates everything that follows the node in tree order.
        /// </summary>
        public IQueryable<TreeElement<T>> Following
        {
            get => AncestorsAndSelf.SelectMany(x => x.FollowingSiblings.SelectMany(y => y.DescendantsAndSelf));
        }

        /// <summary>
        /// 
        /// </summary>
        public IQueryable<TreeElement<T>> FollowingSiblings
        {
            get => FollowingSiblingsAndSelf.Skip(1);
        }

        public IQueryable<TreeElement<T>> FollowingSiblingsAndSelf
        {
            get => Ancestors.Take(1).SelectMany(x => x.Children.SkipWhile(y => !ReferenceEquals(y.Value, this.Value)));
        }



        public IQueryable<TreeElement<T>> Preceding
        {
            get => PrecedingSiblings.SelectMany(x=>x.DescendantsAndSelf.Reverse())
                       .Concat(Ancestors.SelectMany(x => TreeQueryable.SingleQueryable(x).Concat(x.PrecedingSiblings.SelectMany(y => y.DescendantsAndSelf.Reverse()))));
        }

        public IQueryable<TreeElement<T>> PrecedingSiblings
        {
            get => Ancestors.Take(1).SelectMany(x => x.Children.TakeWhile(y => !ReferenceEquals(y.Value, this.Value))).Reverse();
        }

        public IQueryable<TreeElement<T>> PrecedingSiblingsAndSelf
        {
            get => TreeQueryable.SingleQueryable(this).Concat(PrecedingSiblings);
        }           

        public TreeElement<T> Root
        {
            get => AncestorsAndSelf.Last();
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }

        public override bool Equals(object obj)
        {
            if (obj is TreeElement<T> other)
            {
                return Equals(other);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        public bool Equals(TreeElement<T> other)
        {
            if (other == null)
                return false;

            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }
}
