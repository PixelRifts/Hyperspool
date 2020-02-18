using System;

namespace Hyperspool
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator _op, BoundExpression _operand)
        {
            Op = _op;
            Operand = _operand;
        }

        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }

        public override Type Type => Op.Type;
        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    }
}
