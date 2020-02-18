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
            if (_node is BoundLiteralExpression _n)
            {
                return _n.Value;
            }

            if (_node is BoundVariableExpression _v)
            {
                return variables[_v.Variable];
            }

            if (_node is BoundAssignmentExpression _a)
            {
                var _value = EvaluateExpression(_a.Expression);
                variables[_a.Variable] = _value;
                return _value;
            }

            if (_node is BoundUnaryExpression _u)
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

            if (_node is BoundBinaryExpression _b)
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


            else throw new Exception($"Unexpected Node {_node.Kind}");
        }
    }
}
