using System;
using System.Collections.Generic;
using System.Text;

namespace Amp.Linq
{
    /// <summary>
    /// Marker class to mark support for <see cref="TreeElement{T}"/> and <see cref="AmpQueryable"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree<out T>
        where T : class
    {
    }
}
