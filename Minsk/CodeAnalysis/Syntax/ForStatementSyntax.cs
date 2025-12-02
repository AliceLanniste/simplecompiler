namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class ForStatementSyntax: StatementSyntax
    {
        public ForStatementSyntax(SyntaxToken keyword,SyntaxToken identifier, 
            SyntaxToken equalsToken, ExpressionSyntax lowerBound, SyntaxToken toKeyword,
             ExpressionSyntax upperBound,
            StatementSyntax body)
        {
            keyword =keyword;
            Identifier = identifier;
            EqualsToken =equalsToken;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            ToKeyword = toKeyword;
            Body = body;
        }
        public override SyntaxKind Kind => SyntaxKind.ForStatement;
        public SyntaxToken Keyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax LowerBound { get; }
        public SyntaxToken ToKeyword { get; }
        public ExpressionSyntax UpperBound { get; }
        public StatementSyntax Body { get; }
    }
}