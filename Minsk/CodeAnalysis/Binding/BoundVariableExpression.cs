using System;
using Minsk.CodeAnalysis.Symbol;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundVariableExpression: BoundExpression
    {
         public BoundVariableExpression(VariableSymbol variable)
        {
             Variable = variable;
        }

        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
        public override TypeSymbol Type => Variable.Type;
        public VariableSymbol Variable { get; }
    }
}