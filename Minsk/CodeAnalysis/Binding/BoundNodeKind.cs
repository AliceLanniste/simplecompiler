
namespace Minsk.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        //statement
        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,
        CallExpression,
        ConversionExpression,
        ErrorExpression,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
        IfStatement,
        WhileStatement,
        ForStatement,
        LabelStatement,
        GotoStatement,
        ConditionalGotoStatement,
    
    }
}