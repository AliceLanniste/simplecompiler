using System.Collections.Generic;


namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class CallExpressionSyntax: ExpressionSyntax
    {
        public CallExpressionSyntax(
            SyntaxTree syntaxTree,
            SyntaxToken identifierToken, SyntaxToken openParenthesisToken, SeparatedSyntaxList<ExpressionSyntax> arguments, SyntaxToken closeParenthesisToken)
        : base(syntaxTree)
        {
            IdentifierToken = identifierToken;
            OpenParenthesisToken = openParenthesisToken;
            Arguments = arguments;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public override SyntaxKind Kind => SyntaxKind.CallExpression;
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public SeparatedSyntaxList<ExpressionSyntax> Arguments { get; }
        public SyntaxToken CloseParenthesisToken { get; }
    }
    
}