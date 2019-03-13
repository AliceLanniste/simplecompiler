using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis
{
    public sealed class Compilation
    {
        public Compilation(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        public SyntaxTree SyntaxTree { get; }

        public EvaluateResult evaluate(Dictionary<VariableSymbol,object> variables){
            // var binder = new Binder(variables);
            // var boundExpression = binder.BindExpression(SyntaxTree.Root);
            // var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();
            
            // if (diagnostics.Any())
            // {
            //     return new EvaluateResult(diagnostics,null);
            // }

            // var value = new Evaluator(boundExpression,variables).Evaluate();
            // return new EvaluateResult(Array.Empty<Diagnostic>(),value);
            var binder = new Binder(variables);
            var boundExpression = binder.BindExpression(SyntaxTree.Root);

            var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToImmutableArray();
            if (diagnostics.Any())
                return new EvaluateResult(diagnostics, null);

            var evaluator = new Evaluator(boundExpression, variables);
            var value = evaluator.Evaluate();
            return new EvaluateResult(ImmutableArray<Diagnostic>.Empty, value);
        }
    }
}

