﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Amp.Linq
{
    [DebuggerDisplay("{Value,nq}")]
    public sealed class TreeElement<T> : IEquatable<TreeElement<T>>
        where T : class
    {
        public TreeElement(T item)
        {
            Value = item;
            Ancestors = AmpQueryable.EmptyQueryable<TreeElement<T>>();
        }

        public TreeElement(T item, IQueryable<TreeElement<T>> ancestors)
        {
            Value = item;
            Ancestors = ancestors ?? AmpQueryable.EmptyQueryable<TreeElement<T>>();
        }

        private TreeElement()
        { } // Used by .Children to allow introspection via Linq

        public T Value { get; private set; }

        /// <summary>
        /// Enumerates the ancestors of the node, starting at the direct parent
        /// </summary>
        public IQueryable<TreeElement<T>> Ancestors { get; private set; }

        /// <summary>
        /// Enumerates the node itself and its ancestors, starting at the node itself
        /// </summary>
        public IQueryable<TreeElement<T>> AncestorsAndSelf
        {
            get => Ancestors.Prepend(this);
        }

        public IQueryable<TreeElement<T>> Children
        {
            get => (Value is IEnumerable<T> e) ? e.AsQueryable().Select(x => new TreeElement<T> { Value = x, Ancestors = AncestorsAndSelf }) : AmpQueryable.EmptyQueryable<TreeElement<T>>();
        }

        /// <summary>
        /// Enumerates the descendants of the node in a depth first order. (So first child, children of that, etc.)
        /// </summary>
        public IQueryable<TreeElement<T>> Descendants
        {
            get => Children.SelectMany(x => x.Descendants.Prepend(x));
        }

        /// <summary>
        /// Enumerates the node and its descendants
        /// </summary>
        public IQueryable<TreeElement<T>> DescendantsAndSelf
        {
            get => Descendants.Prepend(this);
        }

        /// <summary>
        /// Enumerates the descendants that don't have children
        /// </summary>
        public IQueryable<TreeElement<T>> Leafs
        {
            get => Descendants.Where(x => !x.Children.Any());
        }

        /// <summary>
        /// Enumerates the descendants that don't have children, including the node itself if it doesn't have children
        /// </summary>
        public IQueryable<TreeElement<T>> LeafsIncludingSelf
        {
            get => DescendantsAndSelf.Where(x => !x.Children.Any());
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
                       .Concat(Ancestors.SelectMany(x => x.PrecedingSiblings.SelectMany(y => y.DescendantsAndSelf.Reverse()).Prepend(x)));
        }

        public IQueryable<TreeElement<T>> PrecedingSiblings
        {
            get => Ancestors.Take(1).SelectMany(x => x.Children.TakeWhile(y => !ReferenceEquals(y.Value, this.Value))).Reverse();
        }

        public IQueryable<TreeElement<T>> PrecedingSiblingsAndSelf
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

        public static explicit operator T(TreeElement<T> item)
        {
            return item?.Value;
        }
    }
}
