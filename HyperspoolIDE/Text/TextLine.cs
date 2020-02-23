namespace Hyperspool
{
    public sealed class TextLine
    {
        public TextLine(SourceText _text, int _start, int _length, int _lengthIncludingLineBreak)
        {
            Text = _text;
            Start = _start;
            Length = _length;
            LengthIncludingLineBreak = _lengthIncludingLineBreak;
        }

        public SourceText Text { get; }
        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;
        public int LengthIncludingLineBreak { get; }
        public TextSpan Span => new TextSpan(Start, Length);
        public TextSpan SpanIncludingLineBreak => new TextSpan(Start, LengthIncludingLineBreak);
        public override string ToString() => Text.ToString(Span);
    }
}