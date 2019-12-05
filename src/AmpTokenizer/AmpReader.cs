using System;
using System.Collections.Generic;
using System.Text;

namespace AmpTokenizer
{
    public abstract class AmpReader : IDisposable
    {
        public AmpSource Source { get; }
        protected AmpReader(AmpSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public abstract AmpPosition GetPosition();

        public abstract int Read();

        public abstract int Peek();

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
