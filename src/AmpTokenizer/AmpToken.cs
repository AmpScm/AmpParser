using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AmpTokenizer
{
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public abstract class AmpToken<TKind> : AmpElement<TKind>
        where TKind : Enum
    {
        protected AmpToken(TKind kind)
        {
            Kind = kind;
        }

        public virtual AmpPosition Position
        {
            get => GetRange(false).Start;
        }

        public override void WriteTo(TextWriter tw, IFormatProvider formatProvider, string format, bool leadingTrivia, bool trailingTrivia)
        {
            if (leadingTrivia)
                WriteLeadingTrivia(tw, formatProvider, format);

            WriteToken(tw, formatProvider, format);

            if (trailingTrivia)
                WriteTrailingTrivia(tw, formatProvider, format);
        }

        public abstract void WriteToken(TextWriter tw, IFormatProvider formatProvider, string format);

        protected void WriteLeadingTrivia(TextWriter tw, IFormatProvider formatProvider, string format)
        {
            foreach(var t in LeadingTrivia)
            {
                t.WriteTo(tw, formatProvider, format);
            }
        }

        protected void WriteTrailingTrivia(TextWriter tw, IFormatProvider formatProvider, string format)
        {
            foreach (var t in TrailingTrivia)
            {
                t.WriteTo(tw, formatProvider, format);
            }
        }

        public IEnumerable<AmpTrivia> LeadingTrivia => GetLeadingTrivia();
        public IEnumerable<AmpTrivia> TrailingTrivia => GetTrailingTrivia();

        protected virtual IEnumerable<AmpTrivia> GetLeadingTrivia()
        {
            return Enumerable.Empty<AmpTrivia>();
        }

        protected virtual IEnumerable<AmpTrivia> GetTrailingTrivia()
        {
            return Enumerable.Empty<AmpTrivia>();
        }


        private string DebuggerDisplay()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Kind);
            sb.Append(": ");

            if (LeadingTrivia?.Any() ?? false)
                sb.Append(". ");

            WriteToken(new StringWriter(sb), CultureInfo.InvariantCulture, "");

            if (TrailingTrivia?.Any() ?? false)
                sb.Append(" .");

            return sb.ToString();
        }
    }
}
