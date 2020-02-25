using System.Collections.Generic;
using System.Collections.Immutable;

namespace Hyperspool
{
    public sealed class SyntaxTree
    {
        private SyntaxTree(SourceText _text)
        {
            Parser _p = new Parser(_text);
            var _root = _p.ParseCompilationUnit();
            var _diagnostics = _p.Diagnostics.ToImmutableArray();

            Text = _text;
            Diagnostics = _diagnostics;
            Root = _root;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        public static SyntaxTree Parse(string _text)
        {
            var _sourceText = SourceText.From(_text);
            return Parse(_sourceText);
        }

        public static SyntaxTree Parse(SourceText _text)
        {
            return new SyntaxTree(_text);
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
