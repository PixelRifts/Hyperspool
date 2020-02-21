using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Hyperspool.Tests
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorPairsData))]
        public void Parser_BinaryExpression_HonoursPrecedences(SyntaxKind _op1, SyntaxKind _op2)
        {
            var _op1Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(_op1);
            var _op2Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(_op2);
            var _op1Text = SyntaxFacts.GetText(_op1);
            var _op2Text = SyntaxFacts.GetText(_op2);
            var _text = $"a {_op1Text} b {_op2Text} c";
            var _expression = SyntaxTree.Parse(_text).Root;

            if (_op1Precedence >= _op2Precedence)
            {
                using (var _e = new AssertingEnumerator(_expression))
                {
                    _e.AssertNode(SyntaxKind.BinaryExpression);
                    _e.AssertNode(SyntaxKind.BinaryExpression);
                    _e.AssertNode(SyntaxKind.NameExpression);
                    _e.AssertToken(SyntaxKind.IdentifierToken, "a");
                    _e.AssertToken(_op1, _op1Text);
                    _e.AssertNode(SyntaxKind.NameExpression);
                    _e.AssertToken(SyntaxKind.IdentifierToken, "b");
                    _e.AssertToken(_op2, _op2Text);
                    _e.AssertNode(SyntaxKind.NameExpression);
                    _e.AssertToken(SyntaxKind.IdentifierToken, "c");
                }
            }
            else
            {
                using (var _e = new AssertingEnumerator(_expression))
                {
                    _e.AssertNode(SyntaxKind.BinaryExpression);
                    _e.AssertNode(SyntaxKind.NameExpression);
                    _e.AssertToken(SyntaxKind.IdentifierToken, "a");
                    _e.AssertToken(_op1, _op1Text);
                    _e.AssertNode(SyntaxKind.BinaryExpression);
                    _e.AssertNode(SyntaxKind.NameExpression);
                    _e.AssertToken(SyntaxKind.IdentifierToken, "b");
                    _e.AssertToken(_op2, _op2Text);
                    _e.AssertNode(SyntaxKind.NameExpression);
                    _e.AssertToken(SyntaxKind.IdentifierToken, "c");
                }
            }
        }

        public static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            foreach (var _op1 in SyntaxFacts.GetBinaryOperatorKinds())
            {
                foreach (var _op2 in SyntaxFacts.GetBinaryOperatorKinds())
                {
                    yield return new object[] { _op1, _op2 };
                    yield break;
                }
            }
        }
    }
}
