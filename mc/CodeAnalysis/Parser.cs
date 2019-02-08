using System.Collections.Generic;

namespace Minsk.CodeAnalysis
{
    internal sealed   class Parser
    {
        private readonly SyntaxToken[] _tokens;

        private List<string> _diagnostics = new List<string>();
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

        public IEnumerable<string> Diagnostics => _diagnostics;

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

            _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

       

        public SyntaxTree Parse()
        {
            var expresion = ParseExpression();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostics, expresion, endOfFileToken);
        }

        //  private ExpressionSyntax ParseExpression()
        // {
        //     return ParseTerm();
        // }


        // 1+2*3 =7
        // left 1 ，operatorToken + 1，right =ParseExpression(1)
        //right-left 2 operatorToken * 2, right = 3  binaryExpression(2,*,3)
        // binaryExpression(1,+,bb)
        private ExpressionSyntax ParseExpression(int parentPrecedence=0){
            var left =  ParsePrimaryExpression();

            while (true)
            {
                var precedence = getPrecedence(Current.Kind);
                if(precedence == 0 || precedence <= parentPrecedence)
                  break;

                var operatorToken = NextToken();
                var right = ParseExpression(precedence);

                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            return left;
        }

        public static int getPrecedence(SyntaxKind kind){
            switch (kind)
            {   
                case SyntaxKind.SlashToken:
                case SyntaxKind.StarToken:
                    return 2;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;
                
                default:
                    return 0;
            }
        }

        // private ExpressionSyntax ParseTerm()
        // {
        //     var left = ParseFactor();

        //     while (Current.Kind == SyntaxKind.PlusToken ||
        //            Current.Kind == SyntaxKind.MinusToken)
        //     {
        //         var operatorToken = NextToken();
        //         var right = ParseFactor();
        //         left = new BinaryExpressionSyntax(left, operatorToken, right);
        //     }

        //     return left;
        // }

        // private ExpressionSyntax ParseFactor()
        // {
        //     var left = ParsePrimaryExpression();

        //     while (Current.Kind == SyntaxKind.StarToken ||
        //            Current.Kind == SyntaxKind.SlashToken)
        //     {
        //         var operatorToken = NextToken();
        //         var right = ParsePrimaryExpression();
        //         left = new BinaryExpressionSyntax(left, operatorToken, right);
        //     }

        //     return left;
        // }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = MatchToken(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }
    }
}