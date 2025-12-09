using System;
using  Minsk.CodeAnalysis.Symbol;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Operand = operand;
            Op = op;
        }

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public override TypeSymbol Type => Op.OperandType;
        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }
    }
}