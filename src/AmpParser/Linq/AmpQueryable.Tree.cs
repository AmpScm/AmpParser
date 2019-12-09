using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Amp.Linq
{
    public static partial class AmpQueryable
    {
        public static TreeElement<T> AsTree<T>(this ITree<T> rootNode)
            where T : class
        {
            return new TreeElement<T>((T)rootNode);
        }

        public static IQueryable<TreeElement<T>> TreeChildren<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Children;

        public static IQueryable<TreeElement<T>> TreeDescendants<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Descendants;

        public static IQueryable<TreeElement<T>> TreeDescendantsAndSelf<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().DescendantsAndSelf;

        public static IQueryable<TreeElement<T>> TreeLeafs<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Leafs;

        public static IQueryable<TreeElement<T>> TreeLeafsIncludingSelf<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().LeafsIncludingSelf;

        public static IQueryable<T> Children<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Children.Select(x => x.Value);

        public static IQueryable<T> Descendants<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Descendants.Select(x => x.Value);

        public static IQueryable<T> DescendantsAndSelf<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().DescendantsAndSelf.Select(x => x.Value);

        public static IQueryable<T> Leafs<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().Leafs.Select(x => x.Value);

        public static IQueryable<T> LeafsIncludingSelf<T>(this ITree<T> rootNode)
            where T : class
        => rootNode.AsTree().LeafsIncludingSelf.Select(x => x.Value);


        public static IEnumerable<TSource> EvenItems<TSource>(this IEnumerable<TSource> source)
        {
            int n = 0;
            foreach(var v in source)
            {
                if ((n++ & 1) == 0)
                    yield return v;
            }
        }

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

                for (int i = 0; m.MoveNext(); i = (i + 1) % skip)
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
