﻿namespace Hyperspool
{
    internal enum BoundNodeKind
    {
        // Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,

        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
    }
}
