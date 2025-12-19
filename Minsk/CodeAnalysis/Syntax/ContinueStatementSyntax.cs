using System.Collections.Generic;


namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class ContinueStatementSyntax: StatementSyntax
    {
        public ContinueStatementSyntax(SyntaxToken continueKeyword)
        {
            ContinueKeyword = continueKeyword;
        }

        public override SyntaxKind Kind => SyntaxKind.ContinueStatement;

        public SyntaxToken ContinueKeyword {get;}
    }
}