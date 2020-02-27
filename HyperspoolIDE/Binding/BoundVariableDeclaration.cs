namespace Hyperspool
{
    internal sealed class BoundVariableDeclaration : BoundStatement
    {
        public BoundVariableDeclaration(VariableSymbol _variable, BoundExpression _initializer)
        {
            Variable = _variable;
            Initializer = _initializer;
        }

        public override BoundNodeKind Kind => BoundNodeKind.VariableDeclaration;

        public VariableSymbol Variable { get; }
        public BoundExpression Initializer { get; }
    }
}
