using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{


    internal sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;

        private DiagnosticBag _diagnostics = new DiagnosticBag();
        private int _position;

        public Parser(string text)
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

            _tokens = tokens.ToArray();
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

            _diagnostics.ReportUnexpectToken(Current.Span,Current.Kind, kind);
            // _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

       
        public CompliactionUnitSyntax ParseComplicationUnit()
        {
            var expresion = ParseExpression();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new  CompliactionUnitSyntax(expresion, endOfFileToken);
        }

        private ExpressionSyntax ParseExpression() => ParseAssignmentExpression();
        // 在计算的基础上增加定于语句，eg, a=10,a=b=10
        //     =
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
                {
                    var left = NextToken();
                    var expression = ParseExpression();
                    var right = MatchToken(SyntaxKind.CloseParenthesisToken);
                    return new ParenthesizedExpressionSyntax(left, expression, right);
                }

                case SyntaxKind.FalseKeyword:
                case SyntaxKind.TrueKeyword:
                {
                    var keywordToken = NextToken();
                    var value = keywordToken.Kind == SyntaxKind.TrueKeyword;
                    return new LiteralExpressionSyntax(keywordToken, value);
                }
                case SyntaxKind.IdentifierToken:
                {    
                    var variable = NextToken();
                    return new NameExpressionSyntax(variable);
                }

                default:
                {
                    var numberToken = MatchToken(SyntaxKind.NumberToken);
                    return new LiteralExpressionSyntax(numberToken);
                }
            }
        }
    }
}


