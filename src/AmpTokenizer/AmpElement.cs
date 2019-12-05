using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace AmpTokenizer
{
    public abstract class AmpElement<TKind> : IFormattable
        where TKind : Enum
    {
        protected AmpElement()
        {

        }

        public TKind Kind { get; protected set; }

        protected int KindValue
        {
            get => AmpKindConverter<TKind>.ToInt(Kind);
        }

        public sealed override string ToString()
        {
            return ToString("", CultureInfo.InvariantCulture, false, false);
        }

        public string ToFullString()
        {
            return ToString("", CultureInfo.InvariantCulture, true, true);
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(format, formatProvider, false, false);
        }

        protected virtual string ToString(string format, IFormatProvider formatProvider, bool leading, bool following)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                WriteTo(sw, formatProvider, format, leading, following);
            }

            return sb.ToString();
        }

        public abstract void WriteTo(TextWriter tw, IFormatProvider formatProvider, string format, bool leading, bool following);

        public abstract AmpRange GetRange(bool includeTrivia);
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
