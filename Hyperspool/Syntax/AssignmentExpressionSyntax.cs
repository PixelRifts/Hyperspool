using System.Collections.Generic;

namespace Hyperspool
{
    public sealed class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public AssignmentExpressionSyntax(SyntaxToken _identifierToken, SyntaxToken _equalsToken, ExpressionSyntax _expression)
        {
            IdentifierToken = _identifierToken;
            EqualsToken = _equalsToken;
            Expression = _expression;
        }

        public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Expression { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return IdentifierToken;
            yield return EqualsToken;
            yield return Expression;
        }
    }
}
