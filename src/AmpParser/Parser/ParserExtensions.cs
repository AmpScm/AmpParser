using System;
using System.Collections.Generic;
using System.Text;

namespace Amp.Parser
{
    public static class ParserExtensions
    {
        public static bool In<TEnum>(TEnum value, params TEnum[] values)
            where TEnum : Enum
        {
            foreach(var v in values)
            {
                if (EqualityComparer<TEnum>.Default.Equals(value, v))
                    return true;
            }
            return false;
        }
    }
}
