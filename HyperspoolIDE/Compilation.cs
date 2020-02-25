using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Hyperspool
{
    public sealed class Compilation
    {
        private BoundGlobalScope globalScope;

        public Compilation(SyntaxTree _syntaxTree)
            : this(null, _syntaxTree)
        {
        }

        public Compilation(Compilation previous, SyntaxTree _syntaxTree)
        {
            Previous = previous;
            SyntaxTree = _syntaxTree;
        }

        public Compilation Previous { get; }
        public SyntaxTree SyntaxTree { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (globalScope == null)
                {
                    var _globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                    Interlocked.CompareExchange(ref globalScope, _globalScope, null);
                }

                return globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree _syntaxTree)
        {
            return new Compilation(this, _syntaxTree);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> _variables)
        {
            var _diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();

            if (_diagnostics.Any())
                return new EvaluationResult(_diagnostics, null);
            
            Evaluator _evaluator = new Evaluator(GlobalScope.Expression, _variables);
            var value = _evaluator.Evaluate();
            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
        }
    }
}
