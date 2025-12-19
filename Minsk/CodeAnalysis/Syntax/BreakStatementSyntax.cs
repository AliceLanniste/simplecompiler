using System.Collections.Generic;


namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class BreakStatementSyntax: StatementSyntax
    {
        public BreakStatementSyntax(SyntaxToken breakKeyword)
        {
            BreakKeyword = breakKeyword;
        }

        public override SyntaxKind Kind => SyntaxKind.BreakStatement;

        public SyntaxToken BreakKeyword {get;}
    }
}