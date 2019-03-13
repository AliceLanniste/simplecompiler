using System;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Operand = operand;
            Op = op;
        }

        public override BoundNodeKind Kind => BoundNodeKind.BoundUnaryExpression;
        public override Type Type => Op.OperandType;
        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }
    }
}