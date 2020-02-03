using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amp.Linq
{
    public static partial class AmpQueryable
    {
        /// <summary>
        /// Converts the enumerable <paramref name="source"/> to a peekable enumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerableWithPeek<T> AsPeekable<T>(this IEnumerable<T> source)
        {
            return new PeekWalkable<T>(source);
        }

        /// <summary>
        /// Converts the fetch function <paramref name="source"/> to a peekable enumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerableWithPeek<T> AsPeekable<T>(this Func<T?> source)
            where T :struct
        {
            return AsPeekable(WalkSource(source));
        }

        /// <summary>
        /// Converts the fetch function <paramref name="source"/> to a peekable enumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerableWithPeek<T> AsPeekable<T>(this Func<T> source)
            where T : class
        {
            return AsPeekable(WalkSource2(source));
        }

        private static IEnumerable<T> WalkSource<T>(Func<T?> source)
            where T : struct
        {
            T? value;
            while((value = source()).HasValue)
            {
                yield return value.Value;
            }
        }

        private static IEnumerable<T> WalkSource2<T>(Func<T> source)
            where T : class
        {
            T value;
            while ((value = source()) != null)
            {
                yield return value;
            }
        }

        sealed class PeekWalkable<T> : IEnumerable<PeekElement<T>>, IEnumerableWithPeek<T>
        {
            readonly IEnumerable<T> _source;
            PeekWalker<T> _walker;

            public PeekWalkable(IEnumerable<T> source)
            {
                _source = source ?? throw new ArgumentNullException(nameof(source));
                _walker = new PeekWalker<T>(_source.GetEnumerator());
            }

            public IEnumerator<PeekElement<T>> GetEnumerator()
            {
                if (_walker != null)
                {
                    var v = _walker;
                    _walker = null;
                    return v;
                }
                else
                    return new PeekWalker<T>(_source.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerable<T> Peek
            {
                get
                {
                    if (_walker != null)
                        return _walker.Current.Peek;
                    else
                        return Enumerable.Empty<T>();
                }
            }
        }

        sealed class PeekWalker<T> : IEnumerator<PeekElement<T>>
        {
            readonly IEnumerator<T> _enumerator;


            public PeekWalker(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;

                Current = new PeekElement<T>(default(T), new Lazy<PeekElement<T>>(GetNextPeek));
            }

            private PeekElement<T> GetNextPeek()
            {
                if (_enumerator.MoveNext())
                {
                    return new PeekElement<T>(_enumerator.Current, new Lazy<PeekElement<T>>(GetNextPeek));
                }
                else
                    return null;
            }

            public PeekElement<T> Current { get; private set; }

            object IEnumerator.Current => Current;

            void IDisposable.Dispose()
            { }

            public bool MoveNext()
            {
                Current = Current.GetPeek().FirstOrDefault();
                return (Current != null);
            }

            public void Reset()
            {
                Current = null;
                _enumerator.Reset();
            }
        }
    }
}
