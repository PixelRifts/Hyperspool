using System;
using System.Collections.Immutable;

namespace Hyperspool
{
    public sealed class SourceText
    {
        private readonly string text;

        private SourceText(string _text)
        {
            Lines = ParseLines(this, _text);
            text = _text;
        }

        public ImmutableArray<TextLine> Lines { get; }

        public char this[int index] => text[index];

        public int Length => text.Length;

        public int GetLineIndex(int _position)
        {
            var _lower = 0;
            var _upper = Lines.Length - 1;
            while (_lower <= _upper)
            {
                var _index = _lower + (_upper - _lower) / 2;
                var _start = Lines[_index].Start;

                if (_start == _position)
                    return _index;

                if (_start > _position)
                    _upper = _index - 1;
                else
                    _lower = _index + 1;
            }
            return _lower - 1;
        }

        private static ImmutableArray<TextLine> ParseLines(SourceText _sourceText, string _text)
        {
            var _result = ImmutableArray.CreateBuilder<TextLine>();
            var _position = 0;
            var _lineStart = 0;
            while (_position < _text.Length)
            {
                var _lineBreakWidth = GetLineBreakWidth(_text, _position);
                if (_lineBreakWidth == 0)
                {
                    _position++;
                }
                else
                {
                    AddLine(_result, _sourceText, _position, _lineStart, _lineBreakWidth);
                    _position += _lineBreakWidth;
                    _lineStart = _position;
                }
            }
            if (_position >= _lineStart)
            {
                AddLine(_result, _sourceText, _position, _lineStart, 0);
            }

            return _result.ToImmutable();
        }

        private static void AddLine(ImmutableArray<TextLine>.Builder _result, SourceText _sourceText, int _position, int _lineStart, int _lineBreakWidth)
        {
            var _lineLength = _position - _lineStart;
            var _lineLengthWithLineBreak = _lineLength + _lineBreakWidth;
            var _line = new TextLine(_sourceText, _lineStart, _lineLength, _lineLengthWithLineBreak);
            _result.Add(_line);
        }

        private static int GetLineBreakWidth(string _text, int _position)
        {
            var _c = _text[_position];
            var _l = _position  + 1 >= _text.Length ? '\0' : _text[_position + 1];

            if (_c == '\r' && _l == '\n')
                return 2;
            else if (_c == '\r' || _c == '\n')
                return 1;
            else
                return 0;
        }

        public static SourceText From(string _text)
        {
            return new SourceText(_text);
        }

        public override string ToString() => text;

        public string ToString(int _start, int _length) => text.Substring(_start, _length);

        public string ToString(TextSpan _span) => ToString(_span.Start, _span.Length);
    }
}