using System;
using System.Collections.Generic;
using System.Text;
using AmpTokenizer;

namespace AmpSqlParser
{
    public class SqlSource : AmpSource
    {
        private string file;

        public SqlSource(string file)
        {
            this.file = file;
        }
    }
}
