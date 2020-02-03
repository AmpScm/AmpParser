using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Amp.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class AmpQueryable
    {
        /// <summary>
        /// Gets <paramref name="rootNode"/> as a tree, to allow walking using tree functions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static TreeElement<T> AsTree<T>(this ITree<T> rootNode)
            where T : class
        {
            return new TreeElement<T>((T)rootNode);
        }

        /// <summary>
        /// Gets the direct children of <paramref name="rootNode"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static IEnumerable<TreeElement<T>> TreeChildren<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Children;

        /// <summary>
        /// Gets all the descendants of <paramref name="rootNode"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static IEnumerable<TreeElement<T>> TreeDescendants<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Descendants;

        /// <summary>
        /// Gets <paramref name="rootNode"/>, followed by all its descendants in tree oder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static IEnumerable<TreeElement<T>> TreeDescendantsAndSelf<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().DescendantsAndSelf;

        /// <summary>
        /// Gets all leaf nodes below <paramref name="rootNode"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static IEnumerable<TreeElement<T>> TreeLeafs<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Leafs;

        /// <summary>
        /// Gets all leaf nodes below <paramref name="rootNode"/>, or just <paramref name="rootNode"/> when <paramref name="rootNode"/> does not have children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static IEnumerable<TreeElement<T>> TreeLeafsIncludingSelf<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().LeafsIncludingSelf;

        /// <summary>
        /// Gets the direct children of <paramref name="rootNode"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static IEnumerable<T> Children<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Children.Select(x => x.Value);

        /// <summary>
        /// Gets all descendants of <paramref name="rootNode"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static IEnumerable<T> Descendants<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Descendants.Select(x => x.Value);

        /// <summary>
        /// Gets <paramref name="rootNode"/>, followed by all its descendants in tree oder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static IEnumerable<T> DescendantsAndSelf<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().DescendantsAndSelf.Select(x => x.Value);

        /// <summary>
        /// Gets all leaf nodes below <paramref name="rootNode"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static IEnumerable<T> Leafs<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Leafs.Select(x => x.Value);

        /// <summary>
        /// Gets all leaf nodes below <paramref name="rootNode"/>, or just <paramref name="rootNode"/> when <paramref name="rootNode"/> does not have children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public static IEnumerable<T> LeafsIncludingSelf<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().LeafsIncludingSelf.Select(x => x.Value);

        /// <summary>
        /// Gets every other item in <paramref name="source"/>, starting by the first item
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> EvenItems<TSource>(this IEnumerable<TSource> source)
        {
            int n = 0;
            foreach (var v in source)
            {
                if ((n++ & 1) == 0)
                    yield return v;
            }
        }

        /// <summary>
        /// Gets every other item in <paramref name="source"/>, starting by the second item
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> OddItems<TSource>(this IEnumerable<TSource> source)
        {
            int n = 0;
            foreach (var v in source)
            {
                if ((n++ & 1) == 1)
                    yield return v;
            }
        }


#if prenetstandard21
        /// <summary>
        /// Gets <paramref name="source"/> prepended by <paramref name="element"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            return new TSource[] { element }.Concat(source);
        }

        /// <summary>
        /// Gets <paramref name="source"/> followed by <paramref name="element"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            return source.Concat(new TSource[] { element });
        }

        /// <summary>
        /// Gets all items from <paramref name="source"/>, except for the last <paramref name="skip"/> items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
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

                for (int i = 0; m.MoveNext(); i = (i + 1) % skip)
                {
                    yield return buf[i];
                    buf[i] = m.Current;
                }
            }
        }
#endif

        /// <summary>
        /// Gets a queryable containing just <paramref name="item"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> SingleQueryable<T>(T item)
        {
            return EmptyQueryable<T>().Prepend(item);
        }

        /// <summary>
        /// Gets a queryable containing no elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> EmptyQueryable<T>()
        {
            return Enumerable.Empty<T>();
        }
    }
}
