﻿using System;
using System.Collections.Generic;
using System.Text;
using Amp.Parser;

namespace Amp.SqlParser.Syntax
{
    public class SqlWhereClause : SqlSyntaxElement
    {
        public override IEnumerator<AmpElement<SqlKind>> GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
