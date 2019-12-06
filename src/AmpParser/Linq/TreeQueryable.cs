using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Amp.Linq
{
    public static class TreeQueryable
    {
        public static TreeElement<T> AsTree<T>(this ITree<T> rootNode)
            where T : class
        {
            return new TreeElement<T>((T)rootNode);
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

        public static IQueryable<TreeElement<T>> TreeLeafs<T>(this T rootNode)
            where T : class, ITree<T>
        => rootNode.AsTree().Leafs;

        public static IQueryable<TreeElement<T>> TreeLeafsIncludingSelf<T>(this T rootNode)
            where T : class, ITree<T>
        => rootNode.AsTree().LeafsIncludingSelf;

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
        public static IQueryable<TSource> Prepend<TSource>(this IQueryable<TSource> source, TSource element)
        {
            return SingleQueryable(element).Concat(source);
        }

        public static IQueryable<TSource> Append<TSource>(this IQueryable<TSource> source, TSource element)
        {
            return source.Concat(SingleQueryable(element));
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int skip)
        {
            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip));

            using (IEnumerator<T> m = source.GetEnumerator())
            {
                T[] buf = new T[skip];

                for (int i = 0; i < skip; i++)
                {
                    if (!m.MoveNext())
                        yield break;

                    buf[i] = m.Current;
                }

                for (int i = 0; m.MoveNext(); i = (i+1) % skip)
                {
                    yield return buf[i];
                    buf[i] = m.Current;
                }
            }
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
