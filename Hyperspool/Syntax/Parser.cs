using System;
using System.Collections.Generic;
using System.Text;

namespace Hyperspool
{

    internal sealed class Parser
    {






        private readonly SyntaxToken[] tokens;
        private int position;
        private DiagnosticBag diagnostics = new DiagnosticBag();

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

            tokens = _tokens.ToArray();
            diagnostics.AddRange(_lexer.Diagnostics);
        }





        public DiagnosticBag Diagnostics => diagnostics;
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
            diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, _kind);
            return new SyntaxToken(_kind, Current.Position, null, null);
        }







        public SyntaxTree Parse()
        {
            ExpressionSyntax expression = ParseExpression();
            SyntaxToken endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(diagnostics, expression, endOfFileToken);
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
                    var _left = NextToken();
                    var _expression = ParseExpression();
                    var _right = MatchToken(SyntaxKind.CloseParenthesisToken);
                    return new ParenthesizedExpressionSyntax(_left, _expression, _right);
                

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    var _keywordToken = NextToken();
                    bool _value = _keywordToken.Kind == SyntaxKind.TrueKeyword;
                    return new LiteralExpressionSyntax(_keywordToken, _value);

                case SyntaxKind.IdentifierToken:
                    var _identifierToken = NextToken();
                    return new NameExpressionSyntax(_identifierToken);
            }
            var _numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(_numberToken);
        }






    }
}
