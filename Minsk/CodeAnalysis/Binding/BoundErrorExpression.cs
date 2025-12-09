using System;
using Minsk.CodeAnalysis.Symbol;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundErrorExpression : BoundExpression
   {
      public BoundErrorExpression() {}

      public override TypeSymbol Type => TypeSymbol.Error;
    public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;
   }
}   