namespace Hyperspool
{
    public sealed class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind _kind, int _position, string _text, object _value)
        {
            Kind = _kind;
            Position = _position;
            Text = _text;
            Value = _value;
        }

        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
        public override TextSpan Span => new TextSpan(Position, Text?.Length ?? 0);
    }
}
