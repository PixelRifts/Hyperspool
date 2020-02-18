using System.Collections.Generic;

namespace Hyperspool
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken _literalToken)
            : this(_literalToken, _literalToken.Value)
        {
        }

        public LiteralExpressionSyntax(SyntaxToken _literalToken, object _value)
        {
            LiteralToken = _literalToken;
            Value = _value;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
        public SyntaxToken LiteralToken { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralToken;
        }
    }
}
