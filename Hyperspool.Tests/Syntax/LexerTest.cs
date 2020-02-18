using System;
using System.Collections.Generic;
using Xunit;

namespace Hyperspool.Tests
{
    public class LexerTest
    {
        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Token(SyntaxKind _kind, string _text)
        {
            var _tokens = SyntaxTree.ParseTokens(_text);

            var _token = Assert.Single(_tokens);
            Assert.Equal(_kind, _token.Kind);
            Assert.Equal(_text, _token.Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var _t in GetTokens())
                yield return new object[] { _t.Kind, _t.Text };
        }

        private static IEnumerable<(SyntaxKind Kind, string Text)> GetTokens()
        {
            return new[]
            {
                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "abc"),
            };
        }
    }
}
