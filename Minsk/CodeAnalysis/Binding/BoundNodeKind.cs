// namespace Minsk.CodeAnalysis.Binding
// {
//     internal enum BoundNodeKind
//     {
//         LiteralExpression,
//         UnaryExpression
//     }
// }
namespace Minsk.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        BoundLiteralExpression,
        BoundVariableExpression,
        BoundAssignmentExpression,
        BoundUnaryExpression,
        BoundBinaryExpression,
    }
}