using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Amp.Syntax
{
    internal class AmpQueryProvider<T> : IQueryProvider
    {
        public static IQueryProvider Instance { get; } = new AmpQueryProvider<T>();

        public IQueryable CreateQuery(Expression expression)
        {
            return new AmpQuery<T>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AmpQuery<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return Expression.Lambda<Func<TResult>>(expression).Compile().Invoke();
        }
    }

    class AmpQuery<T> : IQueryable<T>
    {
        IEnumerable<T> _enumerable;
        public AmpQuery(Expression expression)
        {
            Expression = expression;
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider => AmpQueryProvider<T>.Instance;

        public IEnumerator<T> GetEnumerator()
        {
            if (_enumerable == null)
            {
                Expression body = this.Expression;
                Expression<Func<IEnumerable<T>>> expression = Expression.Lambda<Func<IEnumerable<T>>>(body, null);
                _enumerable = expression.Compile()();
            }

            return _enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
