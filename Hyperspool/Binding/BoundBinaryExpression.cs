using System;

namespace Hyperspool
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression _left, BoundBinaryOperator _op, BoundExpression _right)
        {
            Left = _left;
            Op = _op;
            Right = _right;
        }

        public BoundExpression Left { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression Right { get; }

        public override Type Type => Op.Type;
        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
    }
}
