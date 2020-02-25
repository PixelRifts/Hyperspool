using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hyperspool
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public virtual TextSpan Span
        {
            get
            {
                var _first = GetChildren().First().Span;
                var _last = GetChildren().Last().Span;
                return TextSpan.FromBounds(_first.Start, _last.End);
            }
        }

        public IEnumerable<SyntaxNode> GetChildren()
        {
            var _properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var _property in _properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(_property.PropertyType))
                {
                    var _child = (SyntaxNode)_property.GetValue(this);
                    yield return _child;
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(_property.PropertyType))
                {
                    var _children = (IEnumerable<SyntaxNode>)_property.GetValue(this);
                    foreach (var _child in _children)
                        yield return _child;
                }
            }
        }

        public void WriteTo(TextWriter _writer) => PrettyPrint(_writer, this);
        
        private static void PrettyPrint(TextWriter _writer, SyntaxNode _node, string _indent = "", bool isLast = true)
        {
            var _isToConsole = _writer == Console.Out;
            var _marker = isLast ? "└──" : "├──";

            _writer.Write(_indent);

            if (_isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;
            _writer.Write(_marker);
            Console.ResetColor();

            if (_isToConsole)
                Console.ForegroundColor = _node is SyntaxToken ? ConsoleColor.Cyan : ConsoleColor.Blue;
            _writer.Write(_node.Kind);

            if (_node is SyntaxToken _t && _t.Value != null)
            {
                _writer.Write(" ");
                _writer.Write(_t.Value);
            }
            _writer.WriteLine();
            if (_isToConsole)
                Console.ResetColor();

            _indent += isLast ? "    " : "│   ";
            var _lastChild = _node.GetChildren().LastOrDefault();

            foreach (var _child in _node.GetChildren())
            {
                PrettyPrint(_writer, _child, _indent, _child == _lastChild);
            }
        }

        public override string ToString()
        {
            using (var _writer = new StringWriter())
            {
                WriteTo(_writer);
                return _writer.ToString();
            }
        }
    }
}
