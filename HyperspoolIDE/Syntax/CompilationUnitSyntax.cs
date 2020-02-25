namespace Hyperspool
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(ExpressionSyntax _expression, SyntaxToken _endOfFileToken)
        {
            Expression = _expression;
            EndOfFileToken = _endOfFileToken;
        }

        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

        public ExpressionSyntax Expression { get; }
        public SyntaxToken EndOfFileToken { get; }
    }
}
