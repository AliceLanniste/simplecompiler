using System.Collections.Immutable;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundProgram
    {
        public BoundProgram(BoundProgram? previous, ImmutableArray<Diagnostic> diagnostics,     FunctionSymbol? mainFunction,
                            FunctionSymbol? scriptFunction,ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functionBodies)
        {
           Previous = previous;
            Diagnostics = diagnostics;
            MainFunction = mainFunction;
            ScriptFunction = scriptFunction;
            FunctionBodies = functionBodies;
        }

         public BoundProgram? Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public FunctionSymbol? MainFunction { get; }
        public FunctionSymbol? ScriptFunction { get; }
        public ImmutableDictionary<FunctionSymbol, BoundBlockStatement> FunctionBodies { get; }
    }
}