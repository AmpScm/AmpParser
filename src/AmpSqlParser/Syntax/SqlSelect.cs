using System;
using System.Collections.Generic;
using System.Text;
using Amp.Parser;
using Amp.Syntax;

namespace Amp.SqlParser.Syntax
{
    public class SqlSelect : SqlCommandSyntax, IAmpParsable<SqlKind>
    {
        //
        public SqlSyntaxElement WithPart { get; } // WITH [RECURSIVE] <common-table-expression...,..>
        public SqlToken SelectToken { get; } // SELECT
        public SqlToken DistinctAllModifier { get; } // [DISTINCT|ALL]
        public SqlSeparatedTokenList<SqlSelectColumn> ResultColumns {get;} // a AS A, b AS B, c C, d
        public SqlFromClause FromClause { get; }

        public SqlSyntaxList<SqlJoinClause> JoinClauses { get; }
        public SqlWhereClause WhereClause { get; }
        public SqlGroupByClause GroupByClause { get; }

        public SqlSyntaxList<SqlCompoundSelect> CompoundSelects { get; }

        public SqlOrderByClause OrderBy { get; }

        public SqlLimitClause LimitClause { get; }

        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            if (WithPart != null)
                yield return WithPart;

            yield return SelectToken;

            if (DistinctAllModifier != null)
                yield return DistinctAllModifier;

            yield return ResultColumns;

            if (FromClause != null)
                yield return FromClause;

            if (JoinClauses != null)
                yield return JoinClauses;

            if (WhereClause != null)
                yield return WhereClause;

            if (GroupByClause != null)
                yield return GroupByClause;

            if (CompoundSelects != null)
                yield return CompoundSelects;

            if (OrderBy != null)
                yield return OrderBy;

            if (LimitClause != null)
                yield return LimitClause;
        }

        #region Parser
        protected SqlSelect(SqlParserState state, out AmpElement error)
        {
            Kind = SqlKind.SelectSyntax;

            if (state.IsKind(SqlKind.WithToken))
            {
                if (!SqlParser.TryParse<SqlWithClause>(state, out var wp))
                {
                    error = state.Error;
                    return;
                }
                WithPart = wp;
            }

            if (state.IsKind(SqlKind.SelectToken))
            {
                SelectToken = (SqlToken) state.Current;
                state.Read();
            }

            if (state.IsKind(SqlKind.DistinctToken))
            {
                DistinctAllModifier = state.CurrentToken;
                state.Read();
            }
            else if (state.IsKind(SqlKind.AllToken))
            {
                DistinctAllModifier = state.CurrentToken;
                state.Read();
            }

            if (SqlParser.TryParse<SqlCommaSeparatedTokenList<SqlSelectColumn>>(state, out var cols))
            {
                ResultColumns = cols;
            }
            else
            {
                error = state.Error;
                return;
            }

            if (state.IsKind(SqlKind.FromToken))
            {
                if (!SqlParser.TryParse<SqlFromClause>(state, out var from))
                {
                    error = state.Error;
                    return;
                }
                FromClause = from;
            }

            List<SqlJoinClause> joins = new List<SqlJoinClause>();

            while (state.IsKind(SqlKind.JoinToken, SqlKind.OuterJoinToken, SqlKind.LeftToken, SqlKind.RightToken, SqlKind.InnerToken, SqlKind.CrossToken))
            {
                if (!SqlParser.TryParse<SqlJoinClause>(state, out var joinClause))
                {
                    error = state.Error;
                    return;
                }
                joins.Add(joinClause);
            }

            if (joins.Count > 0)
                JoinClauses = SqlSyntaxList<SqlJoinClause>.FromItems(joins);

            if (state.IsKind(SqlKind.WhereToken))
            {
                if (!SqlParser.TryParse<SqlWhereClause>(state, out var where))
                {
                    error = state.Error;
                    return;
                }
                WhereClause = where;
            }

            // GROUP BY
            // COMPOUND
            // ORDER BY


            if (state.IsKind(SqlKind.FetchToken))
            {
                if (!SqlParser.TryParse<SqlLimitClause>(state, out var limit))
                {
                    error = state.Error;
                    return;
                }
                LimitClause = limit;
            }


            error = null;
        }
#endregion
    }
}
