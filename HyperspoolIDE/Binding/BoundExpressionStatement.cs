namespace Hyperspool
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpression _expression)
        {
            Expression = _expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ExpressionStatement;

        public BoundExpression Expression { get; }
    }
}
