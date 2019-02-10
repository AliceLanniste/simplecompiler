using System;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Syntax;


namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly List<string> _diagnostics = new List<string>();

        public IEnumerable<string> Diagnostics => _diagnostics;
        public BoundExpression BindExpression(ExpressionSyntax syntax) {
            switch (syntax.Kind)
            {   
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);

                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.LiteralToken.Value as int? ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {   
            var BoundLeft = BindExpression(syntax.Left);
            var BoundRight = BindExpression(syntax.Right);
            var BoundOperatorKind= BindBinaryOperatorKind(syntax.OperatorToken.Kind,BoundLeft.Type,BoundRight.Type);
            if (BoundOperatorKind == null)
            {
                _diagnostics.Add($"Binary operator '{syntax.OperatorToken.Text}' is not defined for type {BoundLeft.Type} and {BoundRight.Type}.");
                return BoundLeft;
            }

            return new BoundBinaryExpression(BoundLeft,BoundOperatorKind.Value,BoundRight);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var Boundoperand = BindExpression(syntax.Operand);
            var BoundOperatorKind= BindUnaryOperatorKind(syntax.OperatorToken.Kind, Boundoperand.Type);
            
            if (BoundOperatorKind == null)
            {
                _diagnostics.Add($"Unary operator '{syntax.OperatorToken.Text}' is not defined for type {Boundoperand.Type}.");
                return Boundoperand;
            }
            
            return new BoundUnaryExpression(BoundOperatorKind.Value,Boundoperand);
        }

        private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        {
            if (leftType != typeof(int) || rightType != typeof(int))
            {
                return null;
            }

            switch (kind)
            {   case SyntaxKind.PlusToken:
                    return BoundBinaryOperatorKind.Addition;
                case SyntaxKind.MinusToken:
                    return BoundBinaryOperatorKind.Subtraction;
                case SyntaxKind.StarToken:
                    return BoundBinaryOperatorKind.Multiplication;
                case SyntaxKind.SlashToken:
                    return BoundBinaryOperatorKind.Division;
                
                default:
                    throw new Exception($"Unexpected binary operator {kind}");
                    
            }
        }


        private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
        {
            if (operandType != typeof(int))
            {
                return null;
            }

           switch (kind)
            {   case SyntaxKind.PlusToken:
                    return BoundUnaryOperatorKind.Identity;
                case SyntaxKind.MinusToken:
                    return BoundUnaryOperatorKind.Negation;
                    
                default:
                    throw new Exception($"Unexpected unary operator {kind}");
            }
        }
    }
}