// namespace Minsk.CodeAnalysis.Syntax
// {
//     public enum SyntaxKind
//     {
//         // Tokens
//         BadToken,
//         EndOfFileToken,
//         WhitespaceToken,
//         NumberToken,
//         PlusToken,
//         MinusToken,
//         StarToken,
//         SlashToken,
//         BangToken,
//         AmpersandAmpersandToken,
//         PipePipeToken,
//         OpenParenthesisToken,
//         CloseParenthesisToken,
//         IdentifierToken,

//         // Keywords
//         FalseKeyword,
//         TrueKeyword,

//         // Expressions
//         LiteralExpression,
//         UnaryExpression,
//         BinaryExpression,
//         ParenthesizedExpression,
//     }
// }

namespace Minsk.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        BadToken,
        EndOfFileToken,
        WhitespaceToken,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        IdentifierToken,

        // Keywords
        FalseKeyword,
        TrueKeyword,

        // Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}