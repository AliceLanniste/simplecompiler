using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace Minsk.CodeAnalysis.Symbol
{
    public sealed class ParameterSymbol:LocalVariableSymbol
    {
        public ParameterSymbol(string name, TypeSymbol type)
        : base(name,isReadOnly:true, type)
        {
        }

        public override SymbolKind Kind => SymbolKind.Parameter;


    }
}