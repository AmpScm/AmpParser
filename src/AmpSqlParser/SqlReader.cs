using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amp.Parser;
using Amp.Tokenizer;

namespace Amp.SqlParser
{
    public class SqlReader : IDisposable
    {
        int _lastC;
        int _line, _column;
        readonly Queue<int> _peek = new Queue<int>();

        public TextReader Reader { get; }

        public SqlSource Source { get; }

        public SqlReader(TextReader reader, SqlSource source)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            _lastC = -1;
        }

        public SqlPosition GetPosition()
        {
            return new SqlPosition(Source, _line, _column);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Reader.Dispose();
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

        public int Peek()
        {
            return Peek(0);
        }

        public int Read()
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

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
