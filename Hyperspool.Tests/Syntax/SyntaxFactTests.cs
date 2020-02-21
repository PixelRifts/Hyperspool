using System;
using System.Collections.Generic;
using Xunit;

namespace Hyperspool.Tests
{
    public class SyntaxFactTests
    {
        [Theory]
        [MemberData(nameof(GetSyntaxKindData))]
        public void SyntaxFact_GetText_RoundTrips(SyntaxKind _kind)
        {
            var _text = SyntaxFacts.GetText(_kind);
            if (_text == null) return;
            var _tokens = SyntaxTree.ParseTokens(_text);
            var _token = Assert.Single(_tokens);
            Assert.Equal(_kind, _token.Kind);
            Assert.Equal(_text, _token.Text);
        }

        public static IEnumerable<object[]> GetSyntaxKindData()
        {
            var _kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach (var _kind in _kinds)
                yield return new object[] { _kind };
        }
    }
}
