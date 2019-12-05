using System;
using System.Collections.Generic;
using System.Text;

namespace AmpTokenizer
{
    public class AmpRange
    {
        public AmpRange(AmpPosition start, AmpPosition end)
        {
            Start = start;
            End = end;
        }

        public virtual AmpPosition Start { get;}
        public virtual AmpPosition End { get; }

        public AmpSource Source 
        { 
            get => Start?.Source ?? End?.Source;
        }
    }
}
