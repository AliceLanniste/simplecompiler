using System;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
        }

        public override BoundNodeKind Kind => BoundNodeKind.BoundLiteralExpression;
        public override Type Type => Value.GetType();
        public object Value { get; }
    }
}