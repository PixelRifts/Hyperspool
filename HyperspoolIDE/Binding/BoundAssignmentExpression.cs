using System;

namespace Hyperspool
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(VariableSymbol _variable, BoundExpression _expression)
        {
            Variable = _variable;
            Expression = _expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
        public override Type Type => Expression.Type;
    }
}
