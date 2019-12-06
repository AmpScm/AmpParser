using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Amp.Tokenizer
{
    [DebuggerDisplay("{ToString()}")]
    public abstract class AmpTrivia : IFormattable
    {
        public sealed override string ToString()
        {
            return ToString("", CultureInfo.InvariantCulture);
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(format, formatProvider);
        }

        protected virtual string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                WriteTo(sw, formatProvider, format);
            }

            return sb.ToString();
        }

        public abstract void WriteTo(TextWriter tw, IFormatProvider formatProvider, string format);
    }
}
