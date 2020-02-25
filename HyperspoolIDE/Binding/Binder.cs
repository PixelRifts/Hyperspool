﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Hyperspool
{
    internal sealed class Binder
    {
        private BoundScope scope;

        public Binder(BoundScope _parent)
        {
            scope = new BoundScope(_parent);
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope _previous, CompilationUnitSyntax _syntax)
        {
            var _parentScope = CreateParentScopes(_previous);
            var _binder = new Binder(_parentScope);
            var _expression = _binder.BindExpression(_syntax.Expression);
            var _variables = _binder.scope.GetDeclaredVariables();
            var _diagnostics = _binder.Diagnostics.ToImmutableArray();

            if (_previous != null)
                _diagnostics = _diagnostics.InsertRange(0, _previous.Diagnostics);

            return new BoundGlobalScope(_previous, _diagnostics, _variables, _expression);
        }

        private static BoundScope CreateParentScopes(BoundGlobalScope _previous)
        {
            var _stack = new Stack<BoundGlobalScope>();
            while (_previous != null)
            {
                _stack.Push(_previous);
                _previous = _previous.Previous;
            }

            BoundScope _parent = null;

            while (_stack.Count > 0)
            {
                _previous = _stack.Pop();
                var _scope = new BoundScope(_parent);
                foreach (var _v in _previous.Variables)
                    _scope.TryDeclare(_v);
                _parent = _scope;
            }

            return _parent;
        }

        public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

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

            if (!scope.TryLookup(_name, out var _variable))
            {
                Diagnostics.ReportUndefinedName(_syntax.IdentifierToken.Span, _name);
                return new BoundLiteralExpression(0);
            }
            var _type = _variable.GetType();
            return new BoundVariableExpression(_variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax _syntax)
        {
            var _boundExpression = BindExpression(_syntax.Expression);
            string _name = _syntax.IdentifierToken.Text;
            var _variable = new VariableSymbol(_name, _boundExpression.Type);

            if (!scope.TryDeclare(_variable))
            {
                Diagnostics.ReportVariableAlreadyDeclared(_syntax.IdentifierToken.Span, _name);
            }

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
                Diagnostics.ReportUndefinedUnaryOperator(_syntax.OperatorToken.Span, _syntax.OperatorToken.Text, _boundOperand.Type);
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
                Diagnostics.ReportUndefinedBinaryOperator(_syntax.OperatorToken.Span, _syntax.OperatorToken.Text, _boundLeft.Type, _boundRight.Type);
                return _boundLeft;
            }
            return new BoundBinaryExpression(_boundLeft, _boundOperator, _boundRight);
        }
    }
}
