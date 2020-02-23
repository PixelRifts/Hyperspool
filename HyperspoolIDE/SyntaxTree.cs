using System.Collections.Generic;
using System.Collections.Immutable;

namespace Hyperspool
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(ImmutableArray<Diagnostic> _diagnostics, ExpressionSyntax _root, SyntaxToken _endOfFileToken)
        {
            Diagnostics = _diagnostics;
            Root = _root;
            EndOfFileToken = _endOfFileToken;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string _line)
        {
            Parser _p = new Parser(_line);
            return _p.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string _text)
        {
            Lexer _lexer = new Lexer(_text);
            while (true)
            {
                var _token = _lexer.Lex();
                if (_token.Kind == SyntaxKind.EndOfFileToken)
                    break;
                yield return _token;
            }
        }
    }
}
