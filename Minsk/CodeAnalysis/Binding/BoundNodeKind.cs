
namespace Minsk.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        //statement
        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
        IfStatement,
    }
}