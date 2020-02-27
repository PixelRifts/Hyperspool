using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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
            var _expression = _binder.BindStatement(_syntax.Statement);
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

        private BoundStatement BindStatement(StatementSyntax _syntax)
        {
            switch (_syntax.Kind)
            {
                case SyntaxKind.BlockStatement: return BindBlockStatement((BlockStatementSyntax)_syntax);
                case SyntaxKind.ExpressionStatement: return BindExpressionStatement((ExpressionStatementSyntax)_syntax);
                case SyntaxKind.VariableDeclarationStatement: return BindVariableDeclaration((VariableDeclarationSyntax)_syntax);
                default: throw new Exception($"Unexpected Syntax: {_syntax.Kind}");
            }
        }

        private BoundExpressionStatement BindExpressionStatement(ExpressionStatementSyntax _syntax)
        {
            var _expression = BindExpression(_syntax.Expression);
            return new BoundExpressionStatement(_expression);
        }

        private BoundVariableDeclaration BindVariableDeclaration(VariableDeclarationSyntax _syntax)
        {
            var _name = _syntax.Identifier.Text;
            var _isReadOnly = _syntax.KeywordToken.Kind == SyntaxKind.LetKeyword;
            var _expression = BindExpression(_syntax.Initializer);
            var _variable = new VariableSymbol(_name, _isReadOnly, _expression.Type);

            if (!scope.TryDeclare(_variable))
                Diagnostics.ReportVariableAlreadyDeclared(_syntax.Identifier.Span, _name);

            return new BoundVariableDeclaration(_variable, _expression);
        }

        private BoundBlockStatement BindBlockStatement(BlockStatementSyntax _syntax)
        {
            var _statements = ImmutableArray.CreateBuilder<BoundStatement>();
            scope = new BoundScope(scope);
            foreach (var _statementSyntax in _syntax.Statements)
            {
                var _statement = BindStatement(_statementSyntax);
                _statements.Add(_statement);
            }
            scope = scope.Parent;
            return new BoundBlockStatement(_statements.ToImmutable());
        }

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

            if (!scope.TryLookup(_name, out var _variable))
            {
                Diagnostics.ReportUndefinedName(_syntax.IdentifierToken.Span, _name);
                return _boundExpression;
            }

            if (_variable.IsReadOnly)
            {
                Diagnostics.ReportCannotAssign(_syntax.EqualsToken.Span, _name);
            }

            if (_boundExpression.Type != _variable.Type)
            {
                Diagnostics.ReportCannotConvert(_syntax.Expression.Span, _boundExpression.Type, _variable.Type);
                return _boundExpression;
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
