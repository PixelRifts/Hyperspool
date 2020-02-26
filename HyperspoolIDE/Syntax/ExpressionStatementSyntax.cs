namespace Hyperspool
{
    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionStatementSyntax(ExpressionSyntax _expression)
        {
            Expression = _expression;
        }

        public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;

        public ExpressionSyntax Expression { get; }
    }
}
