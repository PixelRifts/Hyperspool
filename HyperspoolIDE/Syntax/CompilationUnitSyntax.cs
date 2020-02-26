namespace Hyperspool
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(StatementSyntax _statement, SyntaxToken _endOfFileToken)
        {
            Statement = _statement;
            EndOfFileToken = _endOfFileToken;
        }

        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

        public StatementSyntax Statement { get; }
        public SyntaxToken EndOfFileToken { get; }
    }
}
