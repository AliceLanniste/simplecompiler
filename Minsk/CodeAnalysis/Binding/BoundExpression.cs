using System;
using Minsk.CodeAnalysis.Symbol;

namespace Minsk.CodeAnalysis.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
         public abstract TypeSymbol Type { get; }
    }
}