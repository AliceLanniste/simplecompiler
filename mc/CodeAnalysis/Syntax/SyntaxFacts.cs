
// namespace Minsk.CodeAnalysis.Syntax
// {

//     internal static class SyntaxFacts
//     {

//         public static int getUnaryPrecedence(this SyntaxKind kind){
//             switch (kind)
//             {
//                 case SyntaxKind.PlusToken:
//                 case SyntaxKind.MinusToken:
//                     return 1;
                
//                 default:
//                     return 0;
//             }
//         }

//         public static int getBinaryPrecedence(this SyntaxKind kind){
//             switch (kind)
//             {   
//                 case SyntaxKind.SlashToken:
//                 case SyntaxKind.StarToken:
//                     return 2;
//                 case SyntaxKind.PlusToken:
//                 case SyntaxKind.MinusToken:
//                     return 1;
                
//                 default:
//                     return 0;
//             }
//         }

//         public static SyntaxKind GetKeywordKind(string text){
//             switch (text)
//             {
//                 case "true":
//                     return SyntaxKind.TrueKeyword;
//                 case "false":
//                     return SyntaxKind.FalseKeyword;
//                 default:
//                     return SyntaxKind.IdentifierToken;
//             }
//         }
    
//     }
// }

using System;

namespace Minsk.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 3;

                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 2;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;

                default:
                    return 0;
            }
        }

        public static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }
    }
}