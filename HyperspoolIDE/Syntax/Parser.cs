using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Hyperspool
{

    internal sealed class Parser
    {
        private readonly ImmutableArray<SyntaxToken> tokens;
        private int position;

        public Parser(string _text)
        {
            List<SyntaxToken> _tokens = new List<SyntaxToken>();

            Lexer _lexer = new Lexer(_text);
            SyntaxToken _token;
            do
            {
                _token = _lexer.Lex();
                if (_token.Kind != SyntaxKind.WhitespaceToken && _token.Kind != SyntaxKind.BadToken)
                {
                    _tokens.Add(_token);
                }
            } while (_token.Kind != SyntaxKind.EndOfFileToken);

            tokens = _tokens.ToImmutableArray();
            Diagnostics.AddRange(_lexer.Diagnostics);
        }

        public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();
        private SyntaxToken Current => Peek(0);
        private SyntaxToken LookAhead => Peek(1);

        private SyntaxToken Peek(int _offset)
        {
            int _index = position + _offset;
            if (_index >= tokens.Length) return tokens[tokens.Length - 1];
            return tokens[_index];
        }

        private SyntaxToken NextToken()
        {
            var _current = Current;
            position++;
            return _current;
        }

        private SyntaxToken MatchToken(SyntaxKind _kind)
        {
            if (Current.Kind == _kind) return NextToken();
            Diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, _kind);
            return new SyntaxToken(_kind, Current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            ExpressionSyntax expression = ParseExpression();
            SyntaxToken endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(Diagnostics.ToImmutableArray(), expression, endOfFileToken);
        }

        private ExpressionSyntax ParseExpression() => ParseAssignmentExpression();

        private ExpressionSyntax ParseAssignmentExpression()
        {
            if (Current.Kind == SyntaxKind.IdentifierToken && LookAhead.Kind == SyntaxKind.EqualsToken)
            {
                var _identifierToken = NextToken();
                var _operatorToken = NextToken();
                var _right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(_identifierToken, _operatorToken, _right);
            }
            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int _parentPrecedence = 0)
        {
            ExpressionSyntax _left;
            var _unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (_unaryOperatorPrecedence != 0 && _unaryOperatorPrecedence >= _parentPrecedence)
            {
                var _operatorToken = NextToken();
                var _operand = ParseBinaryExpression(_unaryOperatorPrecedence);
                _left = new UnaryExpressionSyntax(_operatorToken, _operand);
            }
            else
                _left = ParsePrimary();
            
            while (true)
            {
                int _precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (_precedence == 0 || _precedence <= _parentPrecedence) break;
                else
                {
                    var _operatorToken = NextToken();
                    var _right = ParseBinaryExpression(_precedence);
                    _left = new BinaryExpressionSyntax(_left, _operatorToken, _right);
                }
            }
            return _left;
        }
        
        private ExpressionSyntax ParsePrimary()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    return ParseParenthesizedExpression();

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return ParseBooleanLiteral();

                case SyntaxKind.NumberToken:
                    return ParseNumberLiteral();

                case SyntaxKind.IdentifierToken:
                default:
                    return ParseNameExpression();
            }
        }

        private ExpressionSyntax ParseNumberLiteral()
        {
            var _numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(_numberToken);
        }

        private ExpressionSyntax ParseParenthesizedExpression()
        {
            var _left = MatchToken(SyntaxKind.OpenParenthesisToken);
            var _expression = ParseExpression();
            var _right = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new ParenthesizedExpressionSyntax(_left, _expression, _right);
        }

        private ExpressionSyntax ParseBooleanLiteral()
        {
            var _isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            var _keywordToken = _isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(_keywordToken, _isTrue);
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var _identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(_identifierToken);
        }
    }
}
