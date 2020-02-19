using System;
using System.Collections.Generic;
using System.Linq;
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

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void Lexer_Lexes_TokenPairs(SyntaxKind _t1kind, string _t1text, SyntaxKind _t2kind, string _t2text)
        {
            var _text = _t1text + _t2text;
            var _tokens = SyntaxTree.ParseTokens(_text).ToArray();

            Assert.Equal(2, _tokens.Length);
            Assert.Equal(_tokens[0].Kind, _t1kind);
            Assert.Equal(_tokens[0].Text, _t1text);
            Assert.Equal(_tokens[1].Kind, _t2kind);
            Assert.Equal(_tokens[1].Text, _t2text);
        }




        private static bool RequiresSeparator(SyntaxKind _t1Kind, SyntaxKind _t2Kind)
        {
            var _t1IsKeyword = _t1Kind.ToString().EndsWith("Keyword");
            var _t2IsKeyword = _t2Kind.ToString().EndsWith("Keyword");

            if (_t1Kind == SyntaxKind.IdentifierToken && _t2Kind == SyntaxKind.IdentifierToken) return true;

            else if (_t1IsKeyword && _t2IsKeyword) return true;

            else if (_t1IsKeyword && _t2Kind == SyntaxKind.IdentifierToken) return true;

            else if (_t1Kind == SyntaxKind.IdentifierToken && _t2IsKeyword) return true;

            else if (_t1Kind == SyntaxKind.NumberToken && _t2Kind == SyntaxKind.NumberToken) return true;

            else if (_t1Kind == SyntaxKind.BangToken && _t2Kind == SyntaxKind.EqualsToken) return true;

            else if (_t1Kind == SyntaxKind.EqualsToken && _t2Kind == SyntaxKind.EqualsToken) return true;

            else if (_t1Kind == SyntaxKind.EqualsToken && _t2Kind == SyntaxKind.EqualsEqualsToken) return true;

            else if (_t1Kind == SyntaxKind.BangToken && _t2Kind == SyntaxKind.EqualsEqualsToken) return true;

            return false;
        }





        private static IEnumerable<(SyntaxKind Kind, string Text)> GetTokens()
        {
            return new[]
            {
                (SyntaxKind.EqualsToken, "="),
                (SyntaxKind.PlusToken, "+"),
                (SyntaxKind.MinusToken, "-"),
                (SyntaxKind.StarToken, "*"),
                (SyntaxKind.SlashToken, "/"),
                (SyntaxKind.OpenParenthesisToken, "("),
                (SyntaxKind.CloseParenthesisToken, ")"),
                (SyntaxKind.BangToken, "!"),
                (SyntaxKind.AmpersandAmpersandToken, "&&"),
                (SyntaxKind.PipePipeToken, "||"),
                (SyntaxKind.EqualsEqualsToken, "=="),
                (SyntaxKind.BangEqualsToken, "!="),
                (SyntaxKind.TrueKeyword, "true"),
                (SyntaxKind.FalseKeyword, "false"),
                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "abc"),
                (SyntaxKind.NumberToken, "1"),
                (SyntaxKind.NumberToken, "123"),
            };
        }

        private static IEnumerable<(SyntaxKind Kind, string Text)> GetSeparators()
        {
            return new[]
            {
                (SyntaxKind.WhitespaceToken, " "),
                (SyntaxKind.WhitespaceToken, "  "),
                (SyntaxKind.WhitespaceToken, "\r"),
                (SyntaxKind.WhitespaceToken, "\n"),
                (SyntaxKind.WhitespaceToken, "\r\n"),
            };
        }





        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var _t in GetTokens().Concat(GetSeparators()))
                yield return new object[] { _t.Kind, _t.Text };
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach (var _t in GetTokenPairs())
                yield return new object[] { _t.T1Kind, _t.T1Text, _t.T2Kind, _t.T2Text };
        }
        

        

        private static IEnumerable<(SyntaxKind T1Kind, string T1Text, SyntaxKind T2Kind, string T2Text)> GetTokenPairs()
        {
            foreach (var _t1 in GetTokens())
            {
                foreach (var _t2 in GetTokens())
                {
                    if (!RequiresSeparator(_t1.Kind, _t2.Kind))
                        yield return (_t1.Kind, _t1.Text, _t2.Kind, _t2.Text);
                }
            }
        }
        
    }
}
