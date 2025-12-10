
namespace Minsk.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        //statement
        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,
        CallExpression,
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