using System.Collections.Generic;
using System.Collections.Immutable;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{


    internal sealed class Parser
    {
          private readonly ImmutableArray<SyntaxToken> _tokens;
        private SourceText _text;
        private DiagnosticBag _diagnostics = new DiagnosticBag();
        private int _position;

        public Parser(SourceText text)
        {
            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();

                if (token.Kind != SyntaxKind.WhitespaceToken &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);   
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);
            _text = text;
            _tokens = tokens.ToImmutableArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();
            _diagnostics.ReportUnexpectedToken(Current.Span,Current.Kind, kind);
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statement = ParseStatement();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new CompilationUnitSyntax(statement, endOfFileToken);
        }
    
        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.LetKeyword:
                case SyntaxKind.VarKeyword:
                    return ParseDeclarationStatement();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();
                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement();
                case SyntaxKind.ForKeyword:
                    return ParseForStatement();
                default:
                    return ParseExpressionStatement();
            }
        }
       

        private StatementSyntax ParseWhileStatement()
        {
            var whileKeyword = MatchToken(SyntaxKind.WhileKeyword);
            var condition = ParseExpression();
            var body = ParseStatement();
            return new WhileStatementSyntax(whileKeyword, condition, body);
        }
 
        private StatementSyntax ParseForStatement()
        {
            var forKeyword = MatchToken(SyntaxKind.ForKeyword);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var equalsToken = MatchToken(SyntaxKind.EqualsToken);
            var lowerBound = ParseExpression();
            var toKeyword = MatchToken(SyntaxKind.ToKeyword);
            var upperBound = ParseExpression();
            var body = ParseStatement();
            return new ForStatementSyntax(forKeyword, identifier, equalsToken, lowerBound, toKeyword, upperBound, body);
        }

        private StatementSyntax ParseDeclarationStatement()
        {
            var keyword = MatchToken(Current.Kind == SyntaxKind.LetKeyword ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword);
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            var equalsToken = MatchToken(SyntaxKind.EqualsToken);
            var expression = ParseExpression();
            return new VariableDeclarationSyntax(keyword, identifierToken, equalsToken, expression);
        }
        private BlockStatementSyntax ParseBlockStatement()
        {
            var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);
              var statements = ImmutableArray.CreateBuilder<StatementSyntax>();
            while( Current.Kind != SyntaxKind.CloseBraceToken && Current.Kind != SyntaxKind.EndOfFileToken)
            {
               var startToken = Current;
                var statement = ParseStatement();
                statements.Add(statement);

                if (Current == startToken)
                {
                    NextToken();
                }
            }
            var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);
            return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }
        private StatementSyntax ParseIfStatement()
        {
            var ifKeyword = MatchToken(SyntaxKind.IfKeyword);
            var condition = ParseExpression();
            var thenStatement = ParseStatement();
            var elseClause = ParseElseClause();
            return new IfStatementSyntax(ifKeyword, condition, thenStatement, elseClause);
        }

        private ElseClauseSyntax ParseElseClause()
        {
            if (Current.Kind != SyntaxKind.ElseKeyword)
                return null;

            var elseKeyword = NextToken();
            var statement = ParseStatement();
            return new ElseClauseSyntax(elseKeyword, statement);
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            return new ExpressionStatementSyntax(expression);
        }

        private ExpressionSyntax ParseExpression() => ParseAssignmentExpression();
        // 在计算的基础上增加定于语句，eg, a=10,a=b=10
        //     
        //    /   \
        //    a    10
        // 
        //    =
        //   /  \
        //  a     =
        //       /  \
        //       b   5
        // 和计算的语法树相似，
        private ExpressionSyntax ParseAssignmentExpression()
        {
            if(Peek(0).Kind == SyntaxKind.IdentifierToken && Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var left = NextToken();
                var operatorToken = NextToken();
                var expression = ParseAssignmentExpression();

                return new AssignmentExpressionSyntax(left, operatorToken,expression);
            }

            return ParseBinaryExpression();
        }
         private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var operatorToken = NextToken();
                var right =ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    return ParseParenthesizedExpression();
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.TrueKeyword:
                    return ParseBooleanLiteral();
                case SyntaxKind.NumberToken:
                    return ParseNumberLiteral();
                case SyntaxKind.StringToken:
                    return ParseStringLiteral();
                case SyntaxKind.IdentifierToken:
                default:
                    return ParseNameOrCallExpression();
            }
        }

         private ExpressionSyntax ParseParenthesizedExpression()
        {
            var left  = MatchToken(SyntaxKind.OpenParenthesisToken);
            var expression = ParseExpression();
            var right = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        
        private ExpressionSyntax ParseBooleanLiteral()
        {
            var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            var keywordToken = MatchToken(isTrue ? SyntaxKind.TrueKeyword : SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(keywordToken, isTrue);
        }

        private ExpressionSyntax ParseNameOrCallExpression()
        {
            if(Peek(0).Kind == SyntaxKind.IdentifierToken 
            && Peek(1).Kind == SyntaxKind.OpenParenthesisToken)
                return ParseCallExpression();

            return ParseNameExpression();
        }

        private  ExpressionSyntax ParseCallExpression()
        {
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            var openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);
            var arguments = ParseArguments();
            var closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new CallExpressionSyntax(identifierToken, openParenthesisToken, arguments, closeParenthesisToken);
        }

        private SeparatedSyntaxList<ExpressionSyntax> ParseArguments()
        {
            var nodeAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();
            while(Current.Kind != SyntaxKind.CloseParenthesisToken 
                 && Current.Kind != SyntaxKind.EndOfFileToken)
            {
                var expression = ParseExpression();
                nodeAndSeparators.Add(expression);

                if (Current.Kind != SyntaxKind.CloseParenthesisToken)
                {
                    var comma = MatchToken(SyntaxKind.CommaToken);
                    nodeAndSeparators.Add(comma);
                }
            }

            return new SeparatedSyntaxList<ExpressionSyntax>(nodeAndSeparators.ToImmutable());
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var IdentifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(IdentifierToken);
        }

        private ExpressionSyntax ParseNumberLiteral()
        {
             var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }
      
        private ExpressionSyntax ParseStringLiteral()
        {
            var stringLiteral = MatchToken(SyntaxKind.StringToken);
            return new LiteralExpressionSyntax(stringLiteral);
        }
    }
}


