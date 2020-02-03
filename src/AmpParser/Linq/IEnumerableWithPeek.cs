using System;
using System.Collections.Generic;
using System.Text;

namespace Amp.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEnumerableWithPeek<T> : IEnumerable<PeekElement<T>>
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<T> Peek { get; }
    }
}
