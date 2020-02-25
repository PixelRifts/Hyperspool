using System.Collections.Immutable;

namespace Hyperspool
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(BoundGlobalScope _previous, ImmutableArray<Diagnostic> _diagnostics, ImmutableArray<VariableSymbol> _variables, BoundExpression _expression)
        {
            Previous = _previous;
            Diagnostics = _diagnostics;
            Variables = _variables;
            Expression = _expression;
        }

        public BoundGlobalScope Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public BoundExpression Expression { get; }
    }
}
