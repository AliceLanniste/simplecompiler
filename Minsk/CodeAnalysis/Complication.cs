using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis
{
    public sealed class Complication
    {
        public Complication(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        public SyntaxTree SyntaxTree { get; }

        public EvaluateResult evaluate(Dictionary<VariableSymbol,object> variables){
            var binder = new Binder(variables);
            var boundExpression = binder.BindExpression(SyntaxTree.Root);
            var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();
            
            if (diagnostics.Any())
            {
                return new EvaluateResult(diagnostics,null);
            }

            var value = new Evaluator(boundExpression,variables).Evaluate();
            return new EvaluateResult(Array.Empty<Diagnostic>(),value);
        }
    }
}

