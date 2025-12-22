using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Symbol;
using Minsk.CodeAnalysis.Syntax;
using Minsk.IO;

namespace Minsk
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Usage: Mi <source-paths>");
                return;
            }

            if (args.Length > 1)
            {
                Console.Error.WriteLine("Error: Only one source file is supported");
                return;
            }

            var path = args.Single();

            var text = File.ReadAllText(path);
            var syntaxTree = SyntaxTree.Parse(text);

            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            if (!result.Diagnostics.Any())
            {
                if (result.Value != null)
                    Console.WriteLine(result.Value);
    
            }
            else
            {
                Console.Error.WriteDiagnostics(result.Diagnostics, syntaxTree);
            
            }
        }
    }

}   