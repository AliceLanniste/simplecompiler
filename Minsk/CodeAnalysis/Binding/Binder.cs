using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Syntax;
using System.Collections.Immutable;
using Minsk.CodeAnalysis.Symbol;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private BoundScope _scope;

        public Binder( BoundScope parent)
        {
            _scope = new BoundScope(parent);
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax)
        {
            var parent = CreateParentScope(previous);
            var binder = new Binder(parent);
            var statement = binder.BindStatement(syntax.Statement);
            var variables =binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();
            if (previous != null)
            {
                diagnostics = diagnostics.InsertRange(0,previous.Diagnostics);
            }
            return new BoundGlobalScope(previous, diagnostics, variables, statement);
        }

        public DiagnosticBag Diagnostics => _diagnostics;

       private static BoundScope CreateParentScope(BoundGlobalScope previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope parent = null;
            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent); 
                foreach (var v in previous.Variables)
                    scope.TryDeclare(v);
                parent = scope;
            }
            return parent;
        }

        public BoundStatement BindStatement(StatementSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);
                case SyntaxKind.BlockStatement:
                    return BindBlockStatement((BlockStatementSyntax)syntax);
                case SyntaxKind.VariableDeclaration:
                    return BindVariableDeclaration((VariableDeclarationSyntax)syntax);
                case SyntaxKind.IfStatement:
                    return BindIfStatement((IfStatementSyntax)syntax);
                case SyntaxKind.WhileStatement:
                    return BindWhileStatement((WhileStatementSyntax)syntax);
                case SyntaxKind.ForStatement:
                    return BindForStatement((ForStatementSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, bool canBeVoid = false)
        {
            var result = BindExpressionInternal(syntax);
            if (!canBeVoid && result.Type == TypeSymbol.Void)
            {
                _diagnostics.ReportExpressionMustHaveValue(syntax.Span);
                return new BoundErrorExpression();
            }

            return result;
        }
        public BoundExpression BindExpressionInternal(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.CallExpression:
                    return BindCallExpression((CallExpressionSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
          var lowerBound = BindExpression(syntax.LowerBound, TypeSymbol.Int);
          var upperBound = BindExpression(syntax.UpperBound, TypeSymbol.Int);
         
          _scope = new BoundScope(_scope);
          var  variable = BindVariable(syntax.Identifier, true, TypeSymbol.Int);

          var body = BindStatement(syntax.Body);
          _scope = _scope.Parent;
          return new BoundForStatement(variable, lowerBound, upperBound, body);
       
        }
        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var body = BindStatement(syntax.Body);
            return new BoundWhileStatement(condition, body);
        }

        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
           
            var initializer = BindExpression(syntax.Initializer);
            var isReadOnly = syntax.Keyword.Kind == SyntaxKind.LetKeyword;
            var variable = BindVariable(syntax.Identifier, isReadOnly, initializer.Type);
           
            return new BoundVariableDeclaration(variable, initializer);
        }

        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {  
            var boundExpression = BindExpression(syntax.Expression,canBeVoid:true);
            return new BoundExpressionStatement(boundExpression);
        }
        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var boundStatements = new List<BoundStatement>();
            _scope = new BoundScope(_scope);
            foreach (var statement in syntax.Statements)
            {
                var boundStatement = BindStatement(statement);
                boundStatements.Add(boundStatement);
            }
            _scope = _scope.Parent;
            return new BoundBlockStatement(boundStatements.ToImmutableArray());
        }

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition,TypeSymbol.Bool);
            var thenStatement = BindStatement(syntax.ThenStatement);
            var elseStatement = syntax.ElseClause == null ? null : BindStatement(syntax.ElseClause.ElseStatement );
            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, TypeSymbol targetType)
        {
           var result = BindExpression(syntax);
             if (targetType != TypeSymbol.Error &&
                result.Type != TypeSymbol.Error &&
                result.Type != targetType)
            {
                _diagnostics.ReportCannotConvert(syntax.Span, result.Type, targetType);
            }
                
            return result;
        }

        private BoundExpression BindCallExpression(CallExpressionSyntax syntax)
        {
            var boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();
            foreach (var argument in syntax.Arguments)
            {
                var boundArgument = BindExpression(argument);
                boundArguments.Add(boundArgument);
            }

            var functions = BuiltinFunctions.GetAll();
            var function = functions.SingleOrDefault(f => f.Name == syntax.IdentifierToken.Text);

            if (function == null)
            {
                _diagnostics.ReportUndefinedFunction(syntax.IdentifierToken.Span, syntax.IdentifierToken.Text);
                return new BoundErrorExpression();
            }

            if (syntax.Arguments.Count != function.Parameters.Length)
            {
                 _diagnostics.ReportWrongArgumentCount(syntax.Span, function.Name, function.Parameters.Length, syntax.Arguments.Count);
                return new BoundErrorExpression();
            }

            for (var i =0; i< syntax.Arguments.Count; i++)
            {
                var boundArgument = boundArguments[i];
                var parameter = function.Parameters[i];
                if (boundArgument.Type != parameter.Type)
                {
                    _diagnostics.ReportWrongArgumentType(syntax.Span, function.Name, parameter.Type, boundArgument.Type);
                    return new BoundErrorExpression();
                }
            }
            
            return new BoundCallExpression(function, boundArguments.ToImmutable());
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;

             if (syntax.IdentifierToken.IsMissing)
            {
                // This means the token was inserted by the parser. We already
                // reported error so we can just return an error expression.
                return new BoundLiteralExpression(0);
            }
            if(!_scope.TryLookup(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundLiteralExpression(0);
            }

            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if(!_scope.TryLookup(name,out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundErrorExpression();
            }

            if (variable.IsReadOnly)
                _diagnostics.ReportCannotAssign(syntax.EqualsToken.Span, name);

            if (boundExpression.Type != variable.Type)
            {
                _diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);
                return new BoundErrorExpression();
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

            if (boundOperand.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
                return new BoundErrorExpression();
            }

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
            
            if (boundLeft.Type ==TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return new BoundErrorExpression();
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
    
        private VariableSymbol BindVariable(SyntaxToken identifierToken, bool isReadOnly, TypeSymbol type)
        {
            var name = identifierToken.Text ?? "?";
            var declare = !identifierToken.IsMissing;
            var variable = new VariableSymbol(name, isReadOnly, type);
            
            if (declare && !_scope.TryDeclare(variable))
                _diagnostics.ReportVariableAlreadyDeclared(identifierToken.Span, name);

            return variable;
        }
    }
}
