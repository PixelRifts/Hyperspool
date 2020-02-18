using System;

namespace Hyperspool
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(SyntaxKind _syntaxKind, BoundUnaryOperatorKind _kind, Type _operandType)
            : this(_syntaxKind, _kind, _operandType, _operandType)
        {
        }

        private BoundUnaryOperator(SyntaxKind _syntaxKind, BoundUnaryOperatorKind _kind, Type _operandType, Type _resultType)
        {
            SyntaxKind = _syntaxKind;
            Kind = _kind;
            OperandType = _operandType;
            Type = _resultType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind Kind { get; }
        public Type OperandType { get; }
        public Type Type { get; }

        private static BoundUnaryOperator[] operators =
        {
            new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),

            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(int)),
        };

        public static BoundUnaryOperator Bind(SyntaxKind _syntaxKind, Type _operandType)
        {
            foreach (var _op in operators)
                if (_op.SyntaxKind == _syntaxKind && _op.OperandType == _operandType)
                    return _op;

            return null;
        }
    }
}
