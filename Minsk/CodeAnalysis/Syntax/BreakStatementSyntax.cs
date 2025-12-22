using System.Collections.Generic;


namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class BreakStatementSyntax: StatementSyntax
    {
        public BreakStatementSyntax(SyntaxTree syntaxTree, SyntaxToken breakKeyword)
        : base(syntaxTree)
        {
            BreakKeyword = breakKeyword;
        }

        public override SyntaxKind Kind => SyntaxKind.BreakStatement;

        public SyntaxToken BreakKeyword {get;}
    }
}