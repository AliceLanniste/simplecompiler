using System.Collections.Immutable;
using Minsk.CodeAnalysis.Symbol;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundProgram
    {
        public BoundProgram(ImmutableArray<Diagnostic> diagnostics, ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functionBodies, BoundBlockStatement statement)
        {
            Diagnostics = diagnostics;
            FunctionBodies = functionBodies;
            Statement = statement;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableDictionary<FunctionSymbol, BoundBlockStatement> FunctionBodies { get; }
        public BoundBlockStatement Statement { get; }
    }
}