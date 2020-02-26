using System.Collections.Immutable;

namespace Hyperspool
{
    internal sealed class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(ImmutableArray<BoundStatement> _statements)
        {
            Statements = _statements;
        }

        public override BoundNodeKind Kind => BoundNodeKind.BlockStatement;

        public ImmutableArray<BoundStatement> Statements { get; }
    }
}
