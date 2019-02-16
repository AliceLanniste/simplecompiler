using System;
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

        public EvaluateResult evaluate(){
            var binder = new Binder();
            var boundExpression = binder.BoundExpression(SyntaxTree.root);
            var diagnostics = SyntaxTree.diagnostics.Concar(binder.Diagnostics).ToArray();
            
            if (diagnostics.Any())
            {
                return new EvaluateResult(diagnostics,null);
            }

            var value = new Evaluator(boundExpression);
            return new EvaluateResult();
        }
    }
}