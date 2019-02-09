
namespace Minsk.CodeAnalysis.Syntax
{

    internal static class SyntaxFact
    {

        public static int getUnaryPrecedence(this SyntaxKind kind){
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;
                
                default:
                    return 0;
            }
        }

        public static int getBinaryPrecedence(this SyntaxKind kind){
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
    
    }
}