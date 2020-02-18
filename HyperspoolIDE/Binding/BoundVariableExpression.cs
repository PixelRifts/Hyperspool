using System;

namespace Hyperspool
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol _variable)
        {
            Variable = _variable;
        }

        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
        public VariableSymbol Variable { get; }

        public override Type Type => Variable.Type;
    }
}
