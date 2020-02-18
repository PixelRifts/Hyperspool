using System;

namespace Hyperspool
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object _value)
        {
            Value = _value;
        }

        public object Value { get; }

        public override Type Type => Value.GetType();
        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
    }
}
