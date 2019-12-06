using System;
using System.Collections.Generic;
using System.Text;
using Amp.Tokenizer;

namespace Amp.SqlParser
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
