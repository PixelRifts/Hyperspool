using System;
using System.Collections;
using System.Collections.Generic;

namespace Hyperspool
{
    public sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public void AddRange(DiagnosticBag _diagnostics)
        {
            diagnostics.AddRange(_diagnostics.diagnostics);
        }

        private void Report(TextSpan _span, string _msg)
        {
            var _diagnostic = new Diagnostic(_span, _msg);
            diagnostics.Add(_diagnostic);
        }

        public void ReportInvalidNumber(TextSpan _textSpan, string _text, Type _type)
        {
            var _msg = $"ERROR: The number {_text} isn't a valid {_type}.";
            Report(_textSpan, _msg);
        }

        public void ReportBadCharacter(int _position, char _character)
        {
            var _msg = $"ERROR: Bad Character In Input: '{_character}'.";
            Report(new TextSpan(_position, 1), _msg);
        }

        public void ReportUnexpectedToken(TextSpan _span, SyntaxKind _actualKind, SyntaxKind _expectedKind)
        {
            var _msg = $"ERROR: Unexpected Token <{_actualKind}>, expected <{_expectedKind}>.";
            Report(_span, _msg);
        }

        public void ReportUndefinedUnaryOperator(TextSpan _span, string _operatorText, Type _operandType)
        {
            var _msg = $"ERROR: Unary Operator '{_operatorText}' is not defined for type {_operandType}.";
            Report(_span, _msg);
        }

        public void ReportUndefinedName(TextSpan _span, string _name)
        {
            var _msg = $"ERROR: Variable {_name} doesn't exist";
            Report(_span, _msg);
        }

        public void ReportUndefinedBinaryOperator(TextSpan _span, string _operatorText, Type _leftType, Type _rightType)
        {
            var _msg = $"ERROR: Binary Operator '{_operatorText}' is not defined for types {_leftType} and {_rightType}.";
            Report(_span, _msg);
        }

        internal void ReportVariableAlreadyDeclared(TextSpan _span, string _name)
        {
            var _msg = $"ERROR: Variable {_name} already declared";
            Report(_span, _msg);
        }
    }
}
