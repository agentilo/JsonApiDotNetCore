using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace JsonApiDotNetCore.Queries.Expressions
{

    [PublicAPI]
    public class NoDataFilterExpression : FilterExpression
    {
        public override TResult Accept<TArgument, TResult>(QueryExpressionVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.Visit(this, argument);
        }

        public override string ToFullString()
        {
            return "";
        }
    }
}
