using System;
using System.Collections.Generic;
using System.Linq;

namespace Hyperspool
{
    internal sealed class Compilation
    {
        public Compilation(SyntaxTree _syntax)
        {
            Syntax = _syntax;
        }

        public SyntaxTree Syntax { get; }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> _variables)
        {
            Binder _binder = new Binder(_variables);
            var _boundExpression = _binder.BindExpression(Syntax.Root);
            var _diagnostics = Syntax.Diagnostics.Concat(_binder.Diagnostics).ToArray();

            if (_diagnostics.Any())
                return new EvaluationResult(_diagnostics, null);
            
            Evaluator _evaluator = new Evaluator(_boundExpression, _variables);
            var value = _evaluator.Evaluate();
            return new EvaluationResult(Array.Empty<Diagnostic>(), value);
        }
    }
}
