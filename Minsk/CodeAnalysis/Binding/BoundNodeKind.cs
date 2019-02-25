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
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
    }
}