using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace JsonApiDotNetCore.Queries.Internal.Parsing;

[PublicAPI]
public sealed class NotCastableQueryException : Exception
{
    public NotCastableQueryException(string message)
        : base(message)
    {
    }
}
