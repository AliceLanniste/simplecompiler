using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Symbol
{
    public sealed class ParameterSymbol:VariableSymbol
    {
        public ParameterSymbol(string name, TypeSymbol type)
        : base(name,isReadOnly:true, type)
        {
        }

        public override SymbolKind Kind => SymbolKind.Parameter;


    }
}