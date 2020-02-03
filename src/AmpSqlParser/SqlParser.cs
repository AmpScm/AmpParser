using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
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
            where TElement : AmpElement, IAmpParsable<SqlKind>
        {
            ParsingConstructor<TElement> pc = ParseTypeInfo<TElement>.Instance.Constructor;

            result = pc(state, out var error);

            if (error == null)
            {
#if DEBUG
                //Debug.Assert(result is AmpElement<SqlKind> sq ? sq.Kind != SqlKind.None : true);
#endif
                return true;
            }
            else
            {
                state.Error = error;
                result = null;
                return false;
            }
        }

        delegate TElement ParsingConstructor<TElement>(SqlParserState state, out AmpElement error);

        sealed class ParseTypeInfo<TElement>
            where TElement : AmpElement, IAmpParsable<SqlKind>
        {
            public static ParseTypeInfo<TElement> Instance { get; } = new ParseTypeInfo<TElement>();



            public ParsingConstructor<TElement> Constructor { get; } = typeof(TElement).GetConstructor<ParsingConstructor<TElement>>(BindingFlags.NonPublic);

            private ParseTypeInfo()
            {

            }

            //public bool Parse(SqlParserState state, out )
        }
    }
}
