namespace Hyperspool
{
    public sealed class Diagnostic
    {
        public Diagnostic(TextSpan _span, string _msg)
        {
            Span = _span;
            Msg = _msg;
        }

        public TextSpan Span { get; }
        public string Msg { get; }

        public override string ToString() => Msg;
    }
}
