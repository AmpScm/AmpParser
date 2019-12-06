using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amp.Parser
{
    public abstract class AmpElement<TKind> : AmpElement
        where TKind : Enum
    {
        protected AmpElement()
        {

        }

        /// <summary>
        /// Gets the syntax/token kind
        /// </summary>
        public TKind Kind { get; protected set; }

        protected int KindValue
        {
            get => AmpKindConverter<TKind>.ToInt(Kind);
        }
    }

    sealed class AmpKindConverter<TKind> where TKind : Enum
    {
        static Func<TKind, int> _convert;
        internal static int ToInt(TKind kind)
        {
            if (_convert == null)
            {
                ParameterExpression pKind = Expression.Parameter(typeof(TKind), "kind");

                Expression op = Expression.Convert(pKind, Enum.GetUnderlyingType(typeof(TKind)));

                if (op.Type != typeof(int))
                    op = Expression.Convert(op, typeof(int));

                _convert = Expression.Lambda<Func<TKind, int>>(op, pKind).Compile();
            }

            return _convert(kind);
        }
    }
}
