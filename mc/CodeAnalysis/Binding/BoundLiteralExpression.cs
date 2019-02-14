// using System;

// namespace Minsk.CodeAnalysis.Binding
// {
//     internal sealed class BoundLiteralExpression : BoundExpression
//     {
//         public BoundLiteralExpression(object value)
//         {
//             Value = value;
//         }

//         public object Value { get; }

//         public override BoundNodeKind Kind =>  BoundNodeKind.LiteralExpression;

//         public override Type Type => Value.GetType();
//     }
// }

using System;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
        }

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
        public override Type Type => Value.GetType();
        public object Value { get; }
    }
}