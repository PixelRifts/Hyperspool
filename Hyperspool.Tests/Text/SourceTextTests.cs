using Xunit;

namespace Hyperspool.Tests
{
    public class SourceTextTests
    {
        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceText_IncludesLastLine(string _text, int _expectedLineCount)
        {
            var _sourceText = SourceText.From(_text);
            Assert.Equal(_expectedLineCount, _sourceText.Lines.Length);
        }
    }
}
