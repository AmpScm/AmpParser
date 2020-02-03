using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Amp.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{Value,nq}")]
    public sealed class TreeElement<T> : IEquatable<TreeElement<T>>, IEquatable<T>
        where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public TreeElement(T item)
        {
            Value = item;
            Ancestors = AmpQueryable.EmptyQueryable<TreeElement<T>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ancestors"></param>
        public TreeElement(T item, IEnumerable<TreeElement<T>> ancestors)
        {
            Value = item;
            Ancestors = ancestors ?? AmpQueryable.EmptyQueryable<TreeElement<T>>();
        }

        private TreeElement()
        { } // Used by .Children to allow introspection via Linq

        public T Value { get; }

        /// <summary>
        /// Enumerates the ancestors of the node, starting at the direct parent
        /// </summary>
        public IEnumerable<TreeElement<T>> Ancestors { get; private set; }

        /// <summary>
        /// Enumerates the node itself and its ancestors, starting at the node itself
        /// </summary>
        public IEnumerable<TreeElement<T>> AncestorsAndSelf
        {
            get => Ancestors.Prepend(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TreeElement<T>> Children
        {
            get => (Value is IEnumerable<T> e) ? e.Select(x => new TreeElement<T>(x, AncestorsAndSelf)) : AmpQueryable.EmptyQueryable<TreeElement<T>>();
        }

        /// <summary>
        /// Enumerates the descendants of the node in a depth first order. (So first child, children of that, etc.)
        /// </summary>
        public IEnumerable<TreeElement<T>> Descendants
        {
            get => Children.SelectMany(x => x.Descendants.Prepend(x));
        }

        /// <summary>
        /// Enumerates the node and its descendants
        /// </summary>
        public IEnumerable<TreeElement<T>> DescendantsAndSelf
        {
            get => Descendants.Prepend(this);
        }

        /// <summary>
        /// Enumerates the descendants that don't have children
        /// </summary>
        public IEnumerable<TreeElement<T>> Leafs
        {
            get => Descendants.Where(x => !x.Children.Any());
        }

        /// <summary>
        /// Enumerates the descendants that don't have children, including the node itself if it doesn't have children
        /// </summary>
        public IEnumerable<TreeElement<T>> LeafsIncludingSelf
        {
            get => DescendantsAndSelf.Where(x => !x.Children.Any());
        }

        /// <summary>
        /// Enumerates everything that follows the node in tree order.
        /// </summary>
        public IEnumerable<TreeElement<T>> Following
        {
            get => AncestorsAndSelf.SelectMany(x => x.FollowingSiblings.SelectMany(y => y.DescendantsAndSelf));
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TreeElement<T>> FollowingSiblings
        {
            get => FollowingSiblingsAndSelf.Skip(1);
        }

        public IEnumerable<TreeElement<T>> FollowingSiblingsAndSelf
        {
            get => Ancestors.Take(1).SelectMany(x => x.Children.SkipWhile(y => !ReferenceEquals(y.Value, this.Value)));
        }



        public IEnumerable<TreeElement<T>> Preceding
        {
            get => PrecedingSiblings.SelectMany(x => x.DescendantsAndSelf.Reverse())
                       .Concat(Ancestors.SelectMany(x => x.PrecedingSiblings.SelectMany(y => y.DescendantsAndSelf.Reverse()).Prepend(x)));
        }

        public IEnumerable<TreeElement<T>> PrecedingSiblings
        {
            get => Ancestors.Take(1).SelectMany(x => x.Children.TakeWhile(y => !ReferenceEquals(y.Value, this.Value))).Reverse();
        }

        public IEnumerable<TreeElement<T>> PrecedingSiblingsAndSelf
        {
            get => PrecedingSiblings.Prepend(this);
        }

        public TreeElement<T> Root
        {
            get => AncestorsAndSelf.Last();
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }

        #region Equatable support
        public override bool Equals(object obj)
        {
            if (obj is TreeElement<T> other)
            {
                return Equals(other);
            }
            else if (obj is T otherValue)
            {
                return Equals(otherValue);
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

        public bool Equals(T other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other);
        }

        public static implicit operator T(TreeElement<T> item)
        {
            return item?.Value;
        }
        #endregion
    }
}
