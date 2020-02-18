using System.Collections.Generic;
using System.Linq;

namespace Hyperspool
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(IEnumerable<Diagnostic> _diagnostics, object _value)
        {
            Diagnostics = _diagnostics.ToArray();
            Value = _value;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public object Value { get; }
    }
}