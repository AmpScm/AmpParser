using System;
using System.Collections.Generic;
using System.Text;
using Amp.Parser;
using Amp.Syntax;

namespace Amp.SqlParser.Syntax
{
    public class SqlSelect : SqlCommandSyntax
    {
        //
        public SqlSyntaxElement WithPart { get; } // WITH [RECURSIVE] <common-table-expression...,..>
        public SqlToken SelectToken { get; } // SELECT
        public SqlToken DistinctAllModifier { get; } // [DISTINCT|ALL]
        public SqlSeparatedTokenList<SqlSelectColumn> ResultColumns {get;} // a AS A, b AS B, c C, d
        public SqlFromClause FromClause { get; }
        public SqlWhereClause WhereClause { get; }
        public SqlGroupByClause GroupByClause { get; }

        public SqlSyntaxList<SqlCompoundSelect> CompoundSelects { get; }

        public SqlOrderByClause OrderBy { get; }

        public SqlSyntaxList<AmpSyntax<SqlKind>> Limits { get; }

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

            if (WhereClause != null)
                yield return WhereClause;

            if (GroupByClause != null)
                yield return GroupByClause;

            if (CompoundSelects != null)
                foreach (var v in CompoundSelects)
                    yield return v;

            if (OrderBy != null)
                yield return OrderBy;

            if (Limits != null)
                yield return Limits;
        }
    }
}
