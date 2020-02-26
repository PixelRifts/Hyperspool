using System.Collections.Immutable;

namespace Hyperspool
{
    public sealed class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(SyntaxToken _openBraceToken, ImmutableArray<StatementSyntax> _statements, SyntaxToken _closeBraceToken)
        {
            OpenBraceToken = _openBraceToken;
            Statements = _statements;
            CloseBraceToken = _closeBraceToken;
        }

        public override SyntaxKind Kind => SyntaxKind.BlockStatement;

        public SyntaxToken OpenBraceToken { get; }
        public ImmutableArray<StatementSyntax> Statements { get; }
        public SyntaxToken CloseBraceToken { get; }
    }
}
