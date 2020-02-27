using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Hyperspool
{

    internal sealed class Parser
    {
        private readonly ImmutableArray<SyntaxToken> tokens;
        private readonly SourceText text;
        private int position;

        public Parser(SourceText _text)
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
            text = _text;
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

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var _statement = ParseStatement();
            var _endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new CompilationUnitSyntax(_statement, _endOfFileToken);
        }

        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();

                case SyntaxKind.LetKeyword:
                case SyntaxKind.VarKeyword:
                    return ParseVariableDeclaration();

                default:
                    return ParseExpressionStatement();
            }
        }

        private BlockStatementSyntax ParseBlockStatement()
        {
            var _statements = ImmutableArray.CreateBuilder<StatementSyntax>();

            var _openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);
            while (Current.Kind != SyntaxKind.EndOfFileToken && Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var _statement = ParseStatement();
                _statements.Add(_statement);
            }
            var _closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);

            return new BlockStatementSyntax(_openBraceToken, _statements.ToImmutable(), _closeBraceToken);
        }

        private VariableDeclarationSyntax ParseVariableDeclaration()
        {
            var _expected = Current.Kind == SyntaxKind.LetKeyword ? SyntaxKind.LetKeyword: SyntaxKind.VarKeyword;
            var _keyword = MatchToken(_expected);
            var _identifier = MatchToken(SyntaxKind.IdentifierToken);
            var _equals = MatchToken(SyntaxKind.EqualsToken);
            var _initializer = ParseExpression();
            return new VariableDeclarationSyntax(_keyword, _identifier, _equals, _initializer);
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            var _expression = ParseExpression();
            return new ExpressionStatementSyntax(_expression);
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
