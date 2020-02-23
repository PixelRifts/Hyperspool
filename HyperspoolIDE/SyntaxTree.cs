using System.Collections.Generic;
using System.Collections.Immutable;

namespace Hyperspool
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(SourceText _text, ImmutableArray<Diagnostic> _diagnostics, ExpressionSyntax _root, SyntaxToken _endOfFileToken)
        {
            Text = _text;
            Diagnostics = _diagnostics;
            Root = _root;
            EndOfFileToken = _endOfFileToken;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string _text)
        {
            var _sourceText = SourceText.From(_text);
            return Parse(_sourceText);
        }

        public static SyntaxTree Parse(SourceText _text)
        {
            Parser _p = new Parser(_text);
            return _p.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string _text)
        {
            var _sourceText = SourceText.From(_text);
            return ParseTokens(_sourceText);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(SourceText _text)
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
