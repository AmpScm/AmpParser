using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amp.Linq
{
    public static class PeekQueryable
    {

        public static IEnumerable<PeekElement<T>> AsPeekable<T>(this IEnumerable<T> source)
        {
            return new PeekWalkable<T>(source);
        }

        public static IEnumerable<PeekElement<T>> AsPeekable<T>(this Func<T?> source)
            where T :struct
        {
            return AsPeekable(WalkSource(source));
        }

        public static IEnumerable<PeekElement<T>> AsPeekable<T>(this Func<T> source)
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

        sealed class PeekWalkable<T> : IEnumerable<PeekElement<T>>
        {
            readonly IEnumerable<T> _source;

            public PeekWalkable(IEnumerable<T> source)
            {
                _source = source ?? throw new ArgumentNullException(nameof(source));
            }

            public IEnumerator<PeekElement<T>> GetEnumerator()
            {
                return new PeekWalker<T>(_source.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
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
                Current = Current?.Peek.FirstOrDefault();
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
