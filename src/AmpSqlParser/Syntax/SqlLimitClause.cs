using System;
using System.Collections.Generic;
using System.Text;
using Amp.Parser;

namespace Amp.SqlParser.Syntax
{
    public class SqlLimitClause : SqlSyntaxElement, ISqlParsable
    {
        List<AmpElement<SqlKind>> items = new List<AmpElement<SqlKind>>();

        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            return items.GetEnumerator();
        }


        #region Parser
        protected SqlLimitClause(SqlParserState state, out AmpElement error)
        {
            if (state.IsKind(SqlKind.FetchToken))
            {
                items.Add(state.CurrentToken);
                state.Read();

                if (state.IsKind(SqlKind.FirstToken))
                {
                    items.Add(state.CurrentToken);
                    state.Read();

                    if (state.IsKind(SqlKind.NumericValueToken))
                    {
                        items.Add(state.CurrentToken);
                        state.Read();

                        if (state.IsKind(SqlKind.RowsToken))
                        {
                            items.Add(state.CurrentToken);
                            state.Read();

                            if (state.IsKind(SqlKind.OnlyToken))
                            {
                                items.Add(state.CurrentToken);
                                state.Read();
                            }
                        }
                    }
                }
            }
            error = null;
        }
        #endregion
    }
}
