using System;

namespace Hyperspool
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(SyntaxKind _syntaxKind, BoundBinaryOperatorKind _kind, Type _types)
            : this(_syntaxKind, _kind, _types, _types, _types)
        {
        }

        private BoundBinaryOperator(SyntaxKind _syntaxKind, BoundBinaryOperatorKind _kind, Type _types, Type _resultType)
            : this(_syntaxKind, _kind, _types, _types, _resultType)
        {
        }

        private BoundBinaryOperator(SyntaxKind _syntaxKind, BoundBinaryOperatorKind _kind, Type _leftType, Type _rightType, Type _resultType)
        {
            SyntaxKind = _syntaxKind;
            Kind = _kind;
            LeftType = _leftType;
            RightType = _rightType;
            Type = _resultType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public Type LeftType { get; }
        public Type RightType { get; }
        public Type Type { get; }

        private static BoundBinaryOperator[] operators = 
        {
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, typeof(int)),

            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, typeof(int), typeof(bool)),

            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, typeof(bool)),
        };

        public static BoundBinaryOperator Bind(SyntaxKind _syntaxKind, Type _leftType, Type _rightType)
        {
            foreach (var _op in operators)
                if (_op.SyntaxKind == _syntaxKind && _op.LeftType == _leftType && _op.RightType == _rightType)
                    return _op;

            return null;
        }
    }
}
