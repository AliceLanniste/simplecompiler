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
        BangToken,
        AmpersandAmpersandToken,
        EqualsToken,
        EqualsEqualsToken,
        BangEqualsToken,
        PipePipeToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        IdentifierToken,
        OpenBraceToken,
        CloseBraceToken,
        LessToken,
        LessOrEqualsToken,
        GreaterToken,
        GreaterOrEqualsToken,
        // Keywords
        FalseKeyword,
        TrueKeyword,
        LetKeyword,
        VarKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,
        ForKeyword,
        ToKeyword,

        // Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        NameExpression,
        AssignmentExpression,

        //unit
        CompilationUnit,
        // Statements
        ExpressionStatement,
        BlockStatement,
        VariableDeclaration,
        IfStatement,
        ElseClause,
        WhileStatement,
        ForStatement
    }
}

