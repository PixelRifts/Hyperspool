
using System.Collections.Generic;

namespace Hyperspool
{
    internal sealed class Lexer
    {



        private readonly string text;
        private int position;
        private DiagnosticBag diagnostics = new DiagnosticBag();




        public Lexer(string _text)
        {
            text = _text;
        }




        public DiagnosticBag Diagnostics => diagnostics;

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            int index = position + offset;
            if (index >= text.Length) return '\0';
            return text[index];
        }

        private void Next() => position++;




        public SyntaxToken Lex()
        {
            if (position >= text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, position, "\0", null);
            }

            var _start = position;

            if (char.IsDigit(Current))
            {
                while (char.IsDigit(Current))
                    Next();
                var _length = position - _start;
                var _text = text.Substring(_start, _length);
                if (!int.TryParse(_text, out int _value))
                    diagnostics.ReportInvalidNumber(new TextSpan(_start, _length), text, typeof(int));

                return new SyntaxToken(SyntaxKind.NumberToken, _start, _text, _value);
            }
            else if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current))
                    Next();
                int _length = position - _start;
                return new SyntaxToken(SyntaxKind.WhitespaceToken, _start, text.Substring(_start, _length), null);
            }
            else if (char.IsLetter(Current))
            {
                while (char.IsLetter(Current))
                    Next();
                int _length = position - _start;
                string _text = text.Substring(_start, _length);
                SyntaxKind _kind = SyntaxFacts.GetKeywordKind((string)_text);
                return new SyntaxToken(_kind, _start, _text, null);
            }

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, position++, ")", null); 
                case '!':
                    if (LookAhead == '=')
                    {
                        position += 2;
                        return new SyntaxToken(SyntaxKind.BangEqualsToken, _start, "!=", null);
                    }
                    else
                        return new SyntaxToken(SyntaxKind.BangToken, position++, "!", null);
                case '&':
                    if (LookAhead == '&')
                    {
                        position += 2;
                        return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, _start, "&&", null);
                    }
                    break;
                case '|':
                    if (LookAhead == '|')
                    {
                        position += 2;
                        return new SyntaxToken(SyntaxKind.PipePipeToken, _start, "||", null);
                    }
                    break;
                case '=':
                    if (LookAhead != '=')
                    {
                        return new SyntaxToken(SyntaxKind.EqualsToken, position++, "=", null);
                    }
                    else
                    {
                        position += 2;
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, _start, "==", null);
                    }
            }

            diagnostics.ReportBadCharacter(position, Current);

            return new SyntaxToken(SyntaxKind.BadToken, position++, text.Substring(position - 1), null);
        }
    }
}
