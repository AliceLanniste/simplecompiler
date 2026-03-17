using System;
using System.Reflection.Metadata;
using  Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Operand = operand;
            Op = op;
            ConstantValue = ConstantFolding.ComputeConstant(op, operand);
        }

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public override TypeSymbol Type => Op.OperandType;
        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }

        public override BoundConstant? ConstantValue { get; }
        
    }
}