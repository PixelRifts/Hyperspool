using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;

namespace Hyperspool.Tests
{
    internal sealed class AnnotatedText
    {
        public AnnotatedText(string _text, ImmutableArray<TextSpan> _spans)
        {
            Text = _text;
            Spans = _spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }

        public static AnnotatedText Parse(string _text)
        {
            _text = Unindent(_text);

            var _textBuilder = new StringBuilder();
            var _spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var _startStack = new Stack<int>();

            var _pos = 0;
            foreach (var c in _text)
            {
                if (c == '[')
                {
                    //_startStack.Push();
                }
            }

            return new AnnotatedText(_textBuilder.ToString(), _spanBuilder.ToImmutable());
        }

        private static string Unindent(string _text)
        {
            var _lines = new List<string>();
            using (var _reader = new StringReader(_text))
            {
                string _line;
                while ((_line = _reader.ReadLine()) != null)
                    _lines.Add(_line);
            }

            var _minIndent = int.MaxValue;
            for (int i = 0; i < _lines.Count; i++)
            {
                string _line = _lines[i];
                if (_line.Trim().Length == 0)
                {
                    _lines[i] = string.Empty;
                    continue;
                }
                var _indent = _line.Length - _line.TrimStart().Length;
                _minIndent = Math.Min(_minIndent, _indent);
            }

            for (int i = 0; i < _lines.Count; i++)
            {
                if (_lines[i].Length == 0)
                    continue;
                _lines[i] = _lines[i].Substring(_minIndent);
            }

            while (_lines.Count > 0 && _lines[0].Length == 0)
                _lines.RemoveAt(0);
            while (_lines.Count > 0 && _lines[_lines.Count - 1].Length == 0)
                _lines.RemoveAt(_lines.Count - 1);

            return string.Join(Environment.NewLine, _lines);
        }
    }
}
