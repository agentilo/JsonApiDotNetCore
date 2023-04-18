using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace JsonApiDotNetCore.Queries.Expressions
{
    /// <summary>
    /// This filter will always lead to an emtpy list, because all it does is to check if the literal "no" is equal to "yes".
    /// Quick solution to allow filter names, which are no actual property names.
    /// </summary>
    [PublicAPI]
    public class NoDataFilterExpression : FilterExpression
    {
        public override TResult Accept<TArgument, TResult>(QueryExpressionVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitComparison(new ComparisonExpression(ComparisonOperator.Equals, new LiteralConstantExpression("no"), new LiteralConstantExpression("yes")), argument);
        }

        public override string ToFullString()
        {
            return "";
        }

        public override string ToString()
        {
            return "";
        }
    }
}
