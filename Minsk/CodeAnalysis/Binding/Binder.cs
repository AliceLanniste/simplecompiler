using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis.Binding
{

    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private readonly Dictionary<VariableSymbol, object> _variables;

        public Binder(Dictionary<VariableSymbol, object> variables)
        {
            _variables = variables;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                 case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression(((ParenthesizedExpressionSyntax)syntax));
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            // var name = syntax.IdentifierToken.Text;
            // var boundExpression = BindExpression(syntax.Expression);
            // var defaultValue =
            //     boundExpression.Type == typeof(int)
            //         ? (object)0
            //         : boundExpression.Type == typeof(bool)
            //             ? (object)false
            //             : null;

            // if (defaultValue == null)
            //     throw new Exception($"Unsupported variable type: {boundExpression.Type}");

            // _variables[name] = defaultValue;
            // return new BoundAssignmentExpression(name, boundExpression);
            // a=1
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            var existingVariable = _variables.Keys.FirstOrDefault(v => v.Name == name);
            if (existingVariable != null)
                _variables.Remove(existingVariable);

            var variable = new VariableSymbol(name, boundExpression.Type);
            _variables[variable] = null;

            return new BoundAssignmentExpression(variable, boundExpression);
            
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            // var name = syntax.IdentifierToken.Text;
            // if (!_variables.TryGetValue(name, out var value))
            // {
            //     _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
            //     return new BoundLiteralExpression(0);
            // }

            // var type = value.GetType();
            // return new BoundVariableExpression(name, type);
            var name = syntax.IdentifierToken.Text;
            var variable = _variables.Keys.FirstOrDefault(v => v.Name == name);

            if (variable == null)
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundLiteralExpression(0);
            }

            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span,syntax.OperatorToken.Text, boundOperand.Type);
                // _diagnostics.Add($"Unary operator '{syntax.OperatorToken.Text}' is not defined for type {boundOperand.Type}.");
                return boundOperand;
            }

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type,boundRight.Type);
                //_diagnostics.Add($"Binary operator '{syntax.OperatorToken.Text}' is not defined for types {boundLeft.Type} and {boundRight.Type}.");
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }

        // private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
        // {
        //     if (operandType != typeof(int))
        //         return  null;

        //     switch (kind)
        //     {
        //         case SyntaxKind.PlusToken:
        //             return BoundUnaryOperatorKind.Identity;
        //         case SyntaxKind.MinusToken:
        //             return BoundUnaryOperatorKind.Negation;
        //         default:
        //             throw new Exception($"Unexpected unary operator {kind}");
        //     }
        // }

        // private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        // {
        //     if (leftType != typeof(int) || rightType != typeof(int))
        //         return  null;

        //     switch (kind)
        //     {
        //         case SyntaxKind.PlusToken:
        //             return BoundBinaryOperatorKind.Addition;
        //         case SyntaxKind.MinusToken:
        //             return BoundBinaryOperatorKind.Subtraction;
        //         case SyntaxKind.StarToken:
        //             return BoundBinaryOperatorKind.Multiplication;
        //         case SyntaxKind.SlashToken:
        //             return BoundBinaryOperatorKind.Division;
        //         default:
        //             throw new Exception($"Unexpected binary operator {kind}");
        //     }
        // }
    }
}