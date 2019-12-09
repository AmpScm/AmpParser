using System;
using System.Collections.Generic;
using System.Text;
using Amp.Linq;
using Amp.Parser;

namespace Amp.SqlParser
{
    public class SqlParser
    {
        static SqlParser()
        {

        }

        public static bool TryParse<TElement>(SqlParserState state, out TElement result)
            where TElement : AmpElement, IParsable<SqlKind>
        {
            result = (TElement)Activator.CreateInstance(typeof(TElement), System.Reflection.BindingFlags.NonPublic, null, new object[] { state }, null);

            return !result.IsError;
        }
    }
}
