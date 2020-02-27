namespace Hyperspool
{
    public sealed class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax(SyntaxToken _keywordToken, SyntaxToken _identifier, SyntaxToken _equalsToken, ExpressionSyntax _initializer)
        {
            KeywordToken = _keywordToken;
            Identifier = _identifier;
            EqualsToken = _equalsToken;
            Initializer = _initializer;
        }

        public override SyntaxKind Kind => SyntaxKind.VariableDeclarationStatement;

        public SyntaxToken KeywordToken { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Initializer { get; }
    }
}
