using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperspool
{

    internal sealed class Binder
    {
        private readonly DiagnosticBag diagnostics = new DiagnosticBag();
        private readonly Dictionary<VariableSymbol, object> variables;

        public Binder(Dictionary<VariableSymbol, object> _variables)
        {
            variables = _variables;
        }

        public DiagnosticBag Diagnostics => diagnostics;





        public BoundExpression BindExpression(ExpressionSyntax _syntax)
        {
            switch (_syntax.Kind)
            {
                case SyntaxKind.LiteralExpression: return BindLiteralExpression((LiteralExpressionSyntax)_syntax);
                case SyntaxKind.UnaryExpression: return BindUnaryExpression((UnaryExpressionSyntax)_syntax);
                case SyntaxKind.BinaryExpression: return BindBinaryExpression((BinaryExpressionSyntax)_syntax);
                case SyntaxKind.NameExpression: return BindNameExpression((NameExpressionSyntax)_syntax);
                case SyntaxKind.AssignmentExpression: return BindAssignmentExpression((AssignmentExpressionSyntax)_syntax);
                case SyntaxKind.ParenthesizedExpression: return BindExpression(((ParenthesizedExpressionSyntax)_syntax).Expression);
                default: throw new Exception($"Unexpected Syntax: {_syntax.Kind}");
            }
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax _syntax)
        {
            var _name = _syntax.IdentifierToken.Text;

            var _variable = variables.Keys.FirstOrDefault(v => v.Name == _name);
            if (_variable == null)
            {
                diagnostics.ReportUndefinedName(_syntax.IdentifierToken.Span, _name);
                return new BoundLiteralExpression(0);
            }
            var _type = _variable.GetType();
            return new BoundVariableExpression(_variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax _syntax)
        {
            var _boundExpression = BindExpression(_syntax.Expression);
            string _name = _syntax.IdentifierToken.Text;

            var _existingVariable = variables.Keys.FirstOrDefault(v => v.Name == _name);
            if (_existingVariable != null) variables.Remove(_existingVariable);
            var _variable = new VariableSymbol(_name, _boundExpression.Type);
            variables[_variable] = null;

            return new BoundAssignmentExpression(_variable, _boundExpression);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax _syntax)
        {
            var _value = _syntax.Value ?? 0;
            return new BoundLiteralExpression(_value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax _syntax)
        {
            var _boundOperand = BindExpression(_syntax.Operand);
            var _boundOperator = BoundUnaryOperator.Bind(_syntax.OperatorToken.Kind, _boundOperand.Type);
            if (_boundOperator == null)
            {
                diagnostics.ReportUndefinedUnaryOperator(_syntax.OperatorToken.Span, _syntax.OperatorToken.Text, _boundOperand.Type);
                return _boundOperand;
            }
            return new BoundUnaryExpression(_boundOperator, _boundOperand);
        }


        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax _syntax)
        {
            var _boundLeft = BindExpression(_syntax.Left);
            var _boundRight = BindExpression(_syntax.Right);
            var _boundOperator = BoundBinaryOperator.Bind(_syntax.OperatorToken.Kind, _boundLeft.Type, _boundRight.Type);
            if (_boundOperator == null)
            {
                diagnostics.ReportUndefinedBinaryOperator(_syntax.OperatorToken.Span, _syntax.OperatorToken.Text, _boundLeft.Type, _boundRight.Type);
                return _boundLeft;
            }
            return new BoundBinaryExpression(_boundLeft, _boundOperator, _boundRight);
        }
    }
}
