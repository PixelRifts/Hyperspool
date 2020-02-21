using System.Collections.Generic;
using Xunit;

namespace Hyperspool.Tests
{
    public class EvaluatorTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]

        [InlineData("14 + 12", 26)]
        [InlineData("12 - 3", 9)]
        [InlineData("4 * 2", 8)]
        [InlineData("9 / 3", 3)]
        [InlineData("(10)", 10)]
        [InlineData("(a = 10) * a", 10)]

        [InlineData("12 == 12", true)]
        [InlineData("12 == 13", false)]
        [InlineData("12 != 12", false)]
        [InlineData("12 != 13", true)]

        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]

        [InlineData("true == true", true)]
        [InlineData("false == false", true)]
        [InlineData("true == false", false)]

        [InlineData("true != true", false)]
        [InlineData("false != false", false)]
        [InlineData("true != false", true)]

        [InlineData("true || true", true)]
        [InlineData("true || false", true)]
        [InlineData("false || false", false)]

        [InlineData("true && true", true)]
        [InlineData("true && false", false)]
        [InlineData("false && false", false)]
        public void SyntaxFact_GetText_RoundTrips(string _text, object _expectedValue)
        {
            var _syntaxTree = SyntaxTree.Parse(_text);
            var _compilation = new Compilation(_syntaxTree);
            var _variables = new Dictionary<VariableSymbol, object>();
            var _result = _compilation.Evaluate(_variables);
            Assert.Empty(_result.Diagnostics);
            Assert.Equal(_expectedValue, _result.Value);
        }
    }
}
