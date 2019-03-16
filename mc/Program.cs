using System;
using System.Collections.Generic;
using System.Linq;

using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;

namespace Minsk
{
   internal static  class Program
    {        
        static void Main(string[] args)
        {
            bool showTree = false;
            var _variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();

            while (true)
            {   
                if (textBuilder.Length == 0)
                {
                Console.Write("> ");
                }
                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if (isBlank)
                    {
                        break;
                    }
                    else if (line == "#showTree")
                    {
                        showTree = !showTree;
                        Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
                        continue;
                    }
                    else if (line == "#cls")
                    {
                        Console.Clear();
                        continue;
                    }
                }

                textBuilder.Append(input);
                var text = textBuilder.toString();
                var syntaxTree = SyntaxTree.Parse(text);
                
                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                var comp = new Compilation(syntaxTree);
                var result = comp.evaluate(_variables);

                if (showTree)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;                
                    syntaxTree.Root.WriteTo(Console.Out);
                     Console.ResetColor();
                }

                if (!result.Diagnostics.Any())
                {
                    Console.WriteLine(result.Value);
                }
                else
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                        var line = syntaxTree.Text.Lines[lineIndex];
                        var lineNumber = lineIndex + 1;
                        var character = diagnostic.Span.Start - line.Start + 1;


                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        // var prefix = line.Substring(0, diagnostic.Span.Start);
                        // var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
                        // var suffix = line.Substring(diagnostic.Span.End);
                        var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                        var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                        var prefix = syntaxTree.Text.ToString(prefixSpan);
                        var error = syntaxTree.Text.ToString(diagnostic.Span);
                        var suffix = syntaxTree.Text.ToString(suffixSpan);


                        Console.Write("    ");
                        Console.Write(prefix);

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();

                        Console.Write(suffix);

                        Console.WriteLine();
                    }
                    Console.WriteLine();
                    // Console.ForegroundColor=ConsoleColor.DarkRed;

                    // foreach (var diagnostic in result.Diagnostics)
                    //     Console.WriteLine(diagnostic);

                    // Console.ResetColor();
                }
                textBuilder.Clear();
            }
        }

       
     
    }
}

