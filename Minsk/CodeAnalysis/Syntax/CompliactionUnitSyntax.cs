
    // _diagnostics是为了在出现不符合语法的时候，编译器跳出提醒

namespace Minsk.CodeAnalysis.Syntax 
{
    public sealed class CompliactionUnitSyntax : SyntaxNode
    {
        
        public CompliactionUnitSyntax(ExpressionSyntax root, SyntaxToken endOfFileToken){
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public override SyntaxKind Kind  => SyntaxKind.CompliactionUnitSyntax;

    }
}    