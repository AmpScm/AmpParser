using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmpTokenizer;

namespace AmpSqlParser
{
    public class SqlReader : AmpReader
    {
        int _lastC;
        int _line, _column;
        readonly Queue<int> _peek = new Queue<int>();

        public TextReader Reader { get; }

        public SqlReader(TextReader reader, AmpSource source)
            : base(source)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _lastC = -1;
        }

        public override AmpPosition GetPosition()
        {
            return new AmpPosition(Source, _line, _column);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    Reader.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public int Peek(int depth)
        {
            if (depth < 0)
                throw new ArgumentOutOfRangeException(nameof(depth));

            while(depth >= _peek.Count)
            {
                _peek.Enqueue(DoRead());
            }
            if (depth == 0)
                return _peek.Peek();
            else
                return _peek.ElementAt(depth);
        }

        public override int Peek()
        {
            return Peek(0);
        }

        public override int Read()
        {
            int c;

            if (_peek.Count > 0)
            {
                c = _peek.Dequeue();
            }
            else
                c = DoRead();

            if (c < 0)
                return c;

            if (_lastC > 0)
            {
                if (_lastC == '\n')
                {
                    _line++;
                    _column = 0;
                }
                else
                {
                    _column++;
                }
            }
            _lastC = c;
            return c;
        }

        private int DoRead()
        {
            int c= Reader.Read();

            if (c == '\r' && Reader.Peek() == '\n')
                return Reader.Read(); // Return just the '\n' of "\r\n"

            return c;
        }
    }
}
