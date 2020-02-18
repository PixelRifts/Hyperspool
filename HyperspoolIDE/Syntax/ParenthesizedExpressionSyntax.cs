using System.Collections.Generic;

namespace Hyperspool
{
    public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken _openParenthesisToken, ExpressionSyntax _expression, SyntaxToken _closeParenthesisToken)
        {
            OpenParenthesisToken = _openParenthesisToken;
            Expression = _expression;
            CloseParenthesisToken = _closeParenthesisToken;
        }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisToken;
            yield return Expression;
            yield return CloseParenthesisToken;
        }
    }
}
