using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Amp.Linq
{
    public static class TreeQueryable
    {
        public static TreeElement<T> AsTree<T>(this T rootNode)
            where T : class, ITree<T>
        {
            return new TreeElement<T>(rootNode);
        }

        public static IQueryable<TreeElement<T>> TreeChildren<T>(this T rootNode)
            where T : class, ITree<T>
        => rootNode.AsTree().Children;

        public static IQueryable<TreeElement<T>> TreeDescendants<T>(this T rootNode)
            where T : class, ITree<T>
        => rootNode.AsTree().Descendants;

        public static IQueryable<TreeElement<T>> TreeDescendantsAndSelf<T>(this T rootNode)
            where T : class, ITree<T>
        => rootNode.AsTree().DescendantsAndSelf;

        public static IQueryable<TreeElement<T>> TreeFollowing<T>(this T item)
            where T : class, ITree<T>
        => item.AsTree().Following;

        public static IQueryable<TreeElement<T>> TreeFollowingSiblings<T>(this T item)
            where T : class, ITree<T>
        => item.AsTree().FollowingSiblings;

        public static IQueryable<TreeElement<T>> TreeFollowingSiblingsAndSelf<T>(this T item)
            where T : class, ITree<T>
        => item.AsTree().FollowingSiblingsAndSelf;

        public static IQueryable<TreeElement<T>> TreePreceding<T>(this T item)
            where T : class, ITree<T>
        => item.AsTree().Preceding;

        public static IQueryable<TreeElement<T>> TreePrecedingSiblings<T>(this T item)
            where T : class, ITree<T>
        => item.AsTree().PrecedingSiblings;

        public static IQueryable<TreeElement<T>> TreePrecedingSiblingsAndSelf<T>(this T item)
            where T : class, ITree<T>
        => item.AsTree().PrecedingSiblingsAndSelf;

#if prenetstandard21
        internal static IQueryable<TSource> Prepend<TSource>(this IQueryable<TSource> source, TSource element)
        {
            return SingleQueryable(element).Concat(source);
        }

        internal static IQueryable<TSource> Append<TSource>(this IQueryable<TSource> source, TSource element)
        {
            return source.Concat(SingleQueryable(element));
        }
#endif

        public static IQueryable<T> SingleQueryable<T>(T item)
        {
            return new T[] { item }.AsQueryable();
        }

        public static IQueryable<T> EmptyQueryable<T>()
        {
            return Enumerable.Empty<T>().AsQueryable();
        }
    }
}
