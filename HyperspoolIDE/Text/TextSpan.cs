namespace Hyperspool
{
    public struct TextSpan
    {
        public TextSpan(int _start, int _length)
        {
            Start = _start;
            Length = _length;
        }

        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;

        public static TextSpan FromBounds(int _start, int _end)
        {
            var _length = _end - _start;
            return new TextSpan(_start, _length);
        }
    }
}
