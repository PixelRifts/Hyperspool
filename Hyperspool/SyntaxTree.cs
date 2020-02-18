using System.Collections.Generic;
using System.Linq;

namespace Hyperspool
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<Diagnostic> _diagnostics, ExpressionSyntax _root, SyntaxToken _endOfFileToken)
        {
            Diagnostics = _diagnostics.ToArray();
            Root = _root;
            EndOfFileToken = _endOfFileToken;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string line)
        {
            Parser _p = new Parser(line);
            return _p.Parse();
        }
    }
}
