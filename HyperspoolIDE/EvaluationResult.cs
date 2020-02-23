using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Hyperspool
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(ImmutableArray<Diagnostic> _diagnostics, object _value)
        {
            Diagnostics = _diagnostics;
            Value = _value;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public object Value { get; }
    }
}