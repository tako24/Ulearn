
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reflection.Differentiation
{
    public static class Algebra
    {
        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> function)
        {
            return Expression.Lambda<Func<double, double>>(DifferentiateBody(function.Body), function.Parameters);
        }

        private static BinaryExpression GetBinaryOperation(BinaryExpression binaryOperation)
        {
            var leftOperand = binaryOperation.Left;
            var rightOperand = binaryOperation.Right;
            switch (binaryOperation.NodeType)
            {
                case ExpressionType.Add:
                    return Expression.Add(DifferentiateBody(leftOperand),
                        DifferentiateBody(rightOperand));
                case ExpressionType.Multiply:
                    return Expression.Add(Expression.Multiply
                        (DifferentiateBody(leftOperand), rightOperand),
                        Expression.Multiply(DifferentiateBody
                        (rightOperand), leftOperand));
                default:
                    throw new ArgumentException(binaryOperation.NodeType.ToString());
            }
        }

        private static BinaryExpression GetTrigonometryOperations(MethodCallExpression methodCallExpression)
        {
            var param = methodCallExpression.Arguments[0];
            ParameterExpression paramExpr = Expression.Parameter(typeof(double), "arg");
            switch (methodCallExpression.Method.Name)
            {
                case "Sin":
                    return Expression.Multiply(Expression.Call
                        (typeof(Math).GetMethod("Cos"), param), DifferentiateBody(param));
                case "Cos":
                    return Expression.Multiply
                        (
                           Expression.Negate
                           (
                               Expression.Call(typeof(Math).GetMethod("Sin"), param)
                           ),
                            DifferentiateBody(param)
                        );
                default:
                    throw new ArgumentException(methodCallExpression.Method.Name);
            }
        }

        private static Expression DifferentiateBody(Expression body)
        {
            if (body is ConstantExpression)
                return Expression.Constant(0.0);

            if (body is ParameterExpression)
                return Expression.Constant(1.0);

            if (body is BinaryExpression binaryOperation)
                return GetBinaryOperation(binaryOperation);

            if (body is MethodCallExpression methodCallExpression)
                return GetTrigonometryOperations(methodCallExpression);

            throw new ArgumentException(body.ToString());
        }
    }
}