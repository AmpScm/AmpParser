using System;
using System.Collections.Generic;
using System.Text;

namespace Amp.Parser
{
    public interface IAmpParsable<TKind>
        where TKind: struct, Enum
    {
    }
}
