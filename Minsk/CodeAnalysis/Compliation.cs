using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Lowering; 
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis
{
    public sealed class Compilation
    {
        private BoundGlobalScope _globalScope;
        public Compilation(params SyntaxTree[] syntaxTrees)
            : this(null, syntaxTrees)
        {
        }


        private Compilation(Compilation previous,  params SyntaxTree[] syntaxTrees)
        {
            Previous =previous;
            SyntaxTrees = syntaxTrees.ToImmutableArray();
        }

        internal BoundGlobalScope GlobalScope
        {
          get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTrees);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }
                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(this,syntaxTree);
        }
        public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
        public Compilation Previous { get; }

        public ImmutableArray<FunctionSymbol> Functions => GlobalScope.Functions;
        public ImmutableArray<VariableSymbol> Variables => GlobalScope.Variables;

        public IEnumerable<Symbol> GetSymbols()
        {
            var submission = this;
            var seenSymbolNames = new HashSet<string>();

            while (submission != null)
            {
                foreach (var function in submission.Functions)
                {
                    if (seenSymbolNames.Add(function.Name))
                        yield return function;

                }

                foreach (var variable in submission.Variables)
                {
                    if (seenSymbolNames.Add(variable.Name))
                        yield return variable;
                }

                submission = submission.Previous;
            }
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {

            var parseDiagnostics = SyntaxTrees.SelectMany(st => st.Diagnostics);
            var diagnostics = parseDiagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();
            if (diagnostics.Any())
                return new EvaluationResult(diagnostics, null);

            var program = Binder.BindProgram(GlobalScope);

            var appPath = Environment.GetCommandLineArgs()[0];
            var appDir = Path.GetDirectoryName(appPath);
            var cfgPath = Path.Combine(appDir, "cfg.dot");
            var cfgStatement = !program.Statement.Statements.Any() && program.FunctionBodies.Any()
                ? program.FunctionBodies.First().Value
                : program.Statement;

            var cfg = ControlFlowGraph.Create(cfgStatement);
            using(var streamWriter = new StreamWriter(cfgPath))
                cfg.WriteTo(streamWriter);

            if (program.Diagnostics.Any())
                return new EvaluationResult(program.Diagnostics.ToImmutableArray(), null);

            var evaluator = new Evaluator(program, variables);
            var value = evaluator.Evaluate();
            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
        }


        public void EmitTree(TextWriter writer)
        {
          var program = Binder.BindProgram(GlobalScope);

          if (program.Statement.Statements.Any())
            {
                program.Statement.WriteTo(writer);
            }
            else
            {
                foreach (var functionBody in program.FunctionBodies)
                {
                    if (!GlobalScope.Functions.Contains(functionBody.Key))
                        continue;

                    functionBody.Key.WriteTo(writer);
                    functionBody.Value.WriteTo(writer);
                }
            }
        }

        public void EmitTree(FunctionSymbol symbol, TextWriter writer)
        {
            var program = Binder.BindProgram(GlobalScope);
           if (!program.FunctionBodies.TryGetValue(symbol, out var functionBody))   
                 return;

            symbol.WriteTo(writer);
            writer.WriteLine();
            functionBody.WriteTo(writer);
        }
       
    }
}

