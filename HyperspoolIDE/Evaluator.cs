using System;
using System.Collections.Generic;

namespace Hyperspool
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression root;
        private readonly Dictionary<VariableSymbol, object> variables;


        public Evaluator(BoundExpression _root, Dictionary<VariableSymbol, object> _variables)
        {
            root = _root;
            variables = _variables;
        }




        public object Evaluate()
        {
            return EvaluateExpression(root);
        }

        private object EvaluateExpression(BoundExpression _node)
        {
            switch (_node.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    return EvaluateLiteralExpression((BoundLiteralExpression)_node);
                case BoundNodeKind.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)_node);
                case BoundNodeKind.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)_node);
                case BoundNodeKind.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)_node);
                case BoundNodeKind.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)_node);
                default:
                    throw new Exception($"Unexpected Node {_node.Kind}");
            }
        }
        private static object EvaluateLiteralExpression(BoundLiteralExpression _n)
        {
            return _n.Value;
        }

        private object EvaluateVariableExpression(BoundVariableExpression _v)
        {
            return variables[_v.Variable];
        }

        private object EvaluateAssignmentExpression(BoundAssignmentExpression _a)
        {
            var _value = EvaluateExpression(_a.Expression);
            variables[_a.Variable] = _value;
            return _value;
        }

        private object EvaluateUnaryExpression(BoundUnaryExpression _u)
        {
            var _operand = EvaluateExpression(_u.Operand);


            switch (_u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (int)_operand;
                case BoundUnaryOperatorKind.Negation:
                    return -(int)_operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)_operand;
                default:
                    throw new Exception($"Invalid Unary Operator {_u.Op}");
            }
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression _b)
        {
            var _left = EvaluateExpression(_b.Left);
            var _right = EvaluateExpression(_b.Right);

            switch (_b.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    return (int)_left + (int)_right;
                case BoundBinaryOperatorKind.Subtraction:
                    return (int)_left - (int)_right;
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)_left * (int)_right;
                case BoundBinaryOperatorKind.Division:
                    return (int)_left / (int)_right;
                case BoundBinaryOperatorKind.LogicalAnd:
                    return (bool)_left && (bool)_right;
                case BoundBinaryOperatorKind.LogicalOr:
                    return (bool)_left || (bool)_right;
                case BoundBinaryOperatorKind.Equals:
                    return Equals(_left, _right);
                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(_left, _right);
                default:
                    throw new Exception($"Invalid Binary Operator {_b.Op}");
            }
        }
    }
}
