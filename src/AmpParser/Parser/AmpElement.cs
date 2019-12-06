using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Amp.Tokenizer;

namespace Amp.Parser
{
    public abstract class AmpElement : IFormattable
    {
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

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsError { get; set; }

    }
}
