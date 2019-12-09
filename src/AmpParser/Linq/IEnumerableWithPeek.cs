using System;
using System.Collections.Generic;
using System.Text;

namespace Amp.Linq
{
    public interface IEnumerableWithPeek<T> : IEnumerable<PeekElement<T>>
    {
        IEnumerable<T> Peek { get; }
    }
}
