using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Syntax;
using System.Collections.Immutable;
using Minsk.CodeAnalysis.Symbol;
using Minsk.CodeAnalysis.Text;
using System.Reflection.Metadata;
using Minsk.CodeAnalysis.Lowering;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly FunctionSymbol _fuction;
        private BoundScope _scope;

        public Binder( BoundScope parent, FunctionSymbol function)
        {
            _scope = new BoundScope(parent);
            _fuction = function;

            if (function != null)
            {
                foreach (var parameter in function.Parameters)
                    _scope.TryDeclareVariable(parameter);
            }
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax)
        {
            var parent = CreateParentScope(previous);
            var binder = new Binder(parent, function:null);
            foreach (var function in syntax.Members.OfType<FunctionDeclarationSyntax>())
                binder.BindFunctionDeclaration(function);

            var statementBuilder = ImmutableArray.CreateBuilder<BoundStatement>();

            foreach(var globalStatement in syntax.Members.OfType<GlobalStatementSyntax>())
            {
                var s = binder.BindStatement(globalStatement.Statement);
                statementBuilder.Add(s);
            }

            var statement  = new BoundBlockStatement(statementBuilder.ToImmutable());
            var functions = binder._scope.GetDeclaredFunctions();
            var variables =binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();
            if (previous != null)
            {
                diagnostics = diagnostics.InsertRange(0,previous.Diagnostics);
            }
            return new BoundGlobalScope(previous, diagnostics,functions, variables, statement);
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

            var parent = CreateRootScope();
            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);

                foreach (var f in previous.Functions)
                    scope.TryDeclareFunction(f);

                foreach (var v in previous.Variables)
                    scope.TryDeclareVariable(v);
                parent = scope;
            }
            return parent;
        }

        private static BoundScope CreateRootScope()
        {
            var scope = new BoundScope(null);
            foreach (var f in BuiltinFunctions.GetAll())
                scope.TryDeclareFunction(f);
            return scope;
        }

        public static BoundProgram BindProgram(BoundGlobalScope globalScope)
        {
            var parentScope = CreateParentScope(globalScope);
            var functionBodies = ImmutableDictionary.CreateBuilder<FunctionSymbol, BoundBlockStatement>();
            var diagnostics = new DiagnosticBag();

            var scope = globalScope;

            while (scope != null)
            {
                foreach( var function in scope.Functions)
                {
                    var binder = new Binder(parentScope, function);
                    var body = binder.BindStatement(function.Declaration.Body);
                    var loweredBody = Lowerer.Lower(body);
                    functionBodies.Add(function, loweredBody);

                    diagnostics.AddRange(binder.Diagnostics);
                }
                scope = scope.Previous;
            }

            return new BoundProgram(globalScope, diagnostics, functionBodies.ToImmutable());

        }

        private void BindFunctionDeclaration(FunctionDeclarationSyntax syntax)
        {
            var parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();
            var seenParameterNames = new HashSet<string>();
            foreach (var parameterSyntax in syntax.Parameters)
            {
                var parameterName = parameterSyntax.Identifier.Text; 
                var parameterType = BindTypeClause(parameterSyntax.Type);
                if (!seenParameterNames.Add(parameterName))
                {
                    _diagnostics.ReportParameterAlreadyDeclared(parameterSyntax.Span, parameterName);
                }
                 else
                {
                    var parameter = new ParameterSymbol(parameterName, parameterType);
                    parameters.Add(parameter);
                }
            }

            var type = BindTypeClause(syntax.Type) ?? TypeSymbol.Void;

            if (type != TypeSymbol.Void)
            {
                _diagnostics.XXX_ReportFunctionsAreUnsupported(syntax.Type.Span);
            }
            var function = new FunctionSymbol(syntax.Identifier.Text, parameters.ToImmutable(), type,syntax);

            if (!_scope.TryDeclareFunction(function))
            {
                _diagnostics.ReportFunctionAlreadyDeclared(syntax.Identifier.Span, function.Name);
            }
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
                case SyntaxKind.DoWhileStatement:
                    return BindDoWhileStatement((DoWhileStatementSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundStatement BindDoWhileStatement(DoWhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var body = BindStatement(syntax.Body);
            return new BoundDoWhileStatement(body, condition);
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
            var type = BindTypeClause(syntax.TypeClause);
            var initializer = BindExpression(syntax.Initializer);
            var isReadOnly = syntax.Keyword.Kind == SyntaxKind.LetKeyword;
            var variableType = type ?? initializer.Type;
            var variable = BindVariable(syntax.Identifier, isReadOnly, variableType);
            var convertedInitializer = BindConversion(syntax.Initializer.Span,initializer, variable.Type);
           
            return new BoundVariableDeclaration(variable, convertedInitializer);
        }

        private TypeSymbol BindTypeClause(TypeClauseSyntax syntax)
        {
            if (syntax == null)
                return null;
            
            var type = LookupType(syntax.Identifier.Text);
            if (type == null)
               _diagnostics.ReportUndefinedName(syntax.Identifier.Span, syntax.Identifier.Text);
            
            return type;
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
            return BindConversion(syntax, targetType);
        }

        private BoundExpression BindCallExpression(CallExpressionSyntax syntax)
        {
           if (syntax.Arguments.Count == 1 && LookupType(syntax.IdentifierToken.Text)  is TypeSymbol type)
                return BindConversion( syntax.Arguments[0], type, allowExplicit: true);

            var boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();
            foreach (var argument in syntax.Arguments)
            {
                var boundArgument = BindExpression(argument);
                boundArguments.Add(boundArgument);
            }

            if (!_scope.TryLookupFunction(syntax.IdentifierToken.Text, out var function))
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
                    _diagnostics.ReportWrongArgumentType(syntax.Arguments[i].Span, parameter.Name, parameter.Type, boundArgument.Type);
                    return new BoundErrorExpression();
                }
            }
            
            return new BoundCallExpression(function, boundArguments.ToImmutable());
        }

        private BoundExpression BindConversion(ExpressionSyntax syntax, TypeSymbol type,bool allowExplicit = false)
        {
            var expression = BindExpression(syntax);
            return BindConversion(syntax.Span,expression, type,allowExplicit);
        }

        private BoundExpression BindConversion(TextSpan diagnosticSpan,BoundExpression expression,TypeSymbol type, bool allowExplicit = false)
        {
            var conversion = Conversion.Classify(expression.Type, type);
            if (!conversion.Exists)
            {
                if (expression.Type != TypeSymbol.Error && type != TypeSymbol.Error)
                    _diagnostics.ReportCannotConvert(diagnosticSpan, expression.Type, type);
                
                return new BoundErrorExpression();
            }

            if (!allowExplicit && conversion.IsExplicit)
                _diagnostics.ReportCannotConvertImplicitly(diagnosticSpan, expression.Type, type);

            if (conversion.IsIdentity)
                return expression;

            return new BoundConversionExpression(type, expression);
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
            if(!_scope.TryLookupVariable(name, out var variable))
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

            if(!_scope.TryLookupVariable(name,out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundErrorExpression();
            }

            if (variable.IsReadOnly)
                _diagnostics.ReportCannotAssign(syntax.EqualsToken.Span, name);

            var conversion = BindConversion(syntax.Expression.Span,boundExpression, variable.Type);

            return new BoundAssignmentExpression(variable, conversion);
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
            var variable = _fuction == null ?
                                      (VariableSymbol) new GlobalVariableSymbol(name,isReadOnly, type) :
                                      new LocalVariableSymbol(name,  isReadOnly, type);
            
            
            if (declare && !_scope.TryDeclareVariable(variable))
                _diagnostics.ReportVariableAlreadyDeclared(identifierToken.Span, name);

            return variable;
        }

        private TypeSymbol LookupType(string name)
        {
            switch (name)
            {
                case "int":
                    return TypeSymbol.Int;
                    
                case "bool":
                    return TypeSymbol.Bool;
                case "string":
                    return TypeSymbol.String;
                
                default:
                    return null;
            }
        }
    }
}
