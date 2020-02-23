namespace Hyperspool
{
    internal sealed class Lexer
    {
        private readonly string text;
        private int position;

        private int start;
        private SyntaxKind kind;
        private object value;

        public Lexer(string _text)
        {
            text = _text;
        }

        public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            int index = position + offset;
            if (index >= text.Length) return '\0';
            return text[index];
        }

        public SyntaxToken Lex()
        {
            start = position;
            kind = SyntaxKind.BadToken;
            value = null;

            switch (Current)
            {
                case '\0':
                    kind = SyntaxKind.EndOfFileToken;
                    break;

                case '+':
                    kind = SyntaxKind.PlusToken;
                    position++;
                    break;

                case '-':
                    kind = SyntaxKind.MinusToken;
                    position++;
                    break;

                case '*':
                    kind = SyntaxKind.StarToken;
                    position++;
                    break;

                case '/':
                    kind = SyntaxKind.SlashToken;
                    position++;
                    break;

                case '(':
                    kind = SyntaxKind.OpenParenthesisToken;
                    position++;
                    break;

                case ')':
                    kind = SyntaxKind.CloseParenthesisToken;
                    position++;
                    break;

                case '!':
                    position++;
                    if (Current == '=')
                    {
                        kind = SyntaxKind.BangEqualsToken;
                        position++;
                    }
                    else
                        kind = SyntaxKind.BangToken;

                    break;

                case '&':
                    if (LookAhead == '&')
                    {
                        kind = SyntaxKind.AmpersandAmpersandToken;
                        position += 2;
                    }
                    break;

                case '|':
                    if (LookAhead == '|')
                    {
                        kind = SyntaxKind.PipePipeToken;
                        position += 2;
                    }
                    break;

                case '=':
                    position++;
                    if (Current == '=')
                    {
                        kind = SyntaxKind.EqualsEqualsToken;
                        position++;
                    }
                    else
                        kind = SyntaxKind.EqualsToken;

                    break;

                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                    ReadNumberToken();
                    break;
                    
                default:
                    if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeywordToken();
                    }
                    else if (char.IsWhiteSpace(Current))
                    {
                        ReadWhitespaceToken();
                    }
                    else
                    {
                        Diagnostics.ReportBadCharacter(position, Current);
                        position++;
                    }
                    break;
            }
            

            int _length = position - start;
            string _text = SyntaxFacts.GetText(kind);
            if (_text == null)
                _text = text.Substring(start, _length);
            return new SyntaxToken(kind, start, _text, value);
        }

        private void ReadIdentifierOrKeywordToken()
        {
            while (char.IsLetter(Current))
                position++;
            int _length = position - start;
            string _text = text.Substring(start, _length);
            kind = SyntaxFacts.GetKeywordKind(_text);
        }

        private void ReadWhitespaceToken()
        {
            while (char.IsWhiteSpace(Current))
                position++;
            kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                position++;
            var _length = position - start;
            var _text = text.Substring(start, _length);
            if (!int.TryParse(_text, out int _value))
                Diagnostics.ReportInvalidNumber(new TextSpan(start, _length), text, typeof(int));
            kind = SyntaxKind.NumberToken;
            value = _value;
        }
    }
}
