using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Hyperspool.Tests
{
    internal sealed class AssertingEnumerator : IDisposable
    {
        private readonly IEnumerator<SyntaxNode> enumerator;
        private bool hasErrors;

        public AssertingEnumerator(SyntaxNode _node)
        {
            enumerator = Flatten(_node).GetEnumerator();
        }

        private static IEnumerable<SyntaxNode> Flatten(SyntaxNode _node)
        {
            var _stack = new Stack<SyntaxNode>();
            _stack.Push(_node);

            while (_stack.Count > 0)
            {
                var _n = _stack.Pop();
                yield return _n;

                foreach (var _child in _n.GetChildren().Reverse())
                    _stack.Push(_child);
            }
        }

        public void AssertToken(SyntaxKind _kind, string _text)
        {
            try
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(_kind, enumerator.Current.Kind);
                var _token = Assert.IsType<SyntaxToken>(enumerator.Current);
                Assert.Equal(_text, _token.Text);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void AssertNode(SyntaxKind _kind)
        {
            try
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(_kind, enumerator.Current.Kind);
                Assert.IsNotType<SyntaxToken>(enumerator.Current);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        private bool MarkFailed()
        {
            hasErrors = true;
            return false;
        }

        public void Dispose()
        {
            if (!hasErrors)
                Assert.False(enumerator.MoveNext());
            enumerator.Dispose();
        }
    }
}
