using System.Collections.Immutable;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis.Symbol
{
    public sealed class FunctionSymbol:Symbol
    {
        public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol type, FunctionDeclarationSyntax declaration=null)
        : base(name)
        {
            Parameters = parameters;
            Type = type;
            Declaration = declaration;
        }

        public override SymbolKind Kind => SymbolKind.Function;

        public ImmutableArray<ParameterSymbol> Parameters {get;}
        public TypeSymbol Type {get;}
        public FunctionDeclarationSyntax Declaration {get;}

    }
}