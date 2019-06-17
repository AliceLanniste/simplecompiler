using System.Collections.Generic;
using System.Collections.Immutable;
using Minsk.CodeAnalysis.Text;


namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(string text)
        {
            var parse = new Parser(text);
            var root = parse.ParseComplicationUnit();
            var diagnostics = parse.Diagnostics.ToImmutableArray();

            Text = text;
            Diagnostics = diagnostics;
            Root = root;
           
        }

        public string Text { get; }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompliactionUnitSyntax Root { get; }
      
        // public static SyntaxTree Parse(string text)
        // {
        //     var parser = new Parser(text);
        //     return parser.Parse();
        // }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var lexer = new Lexer(text);

            while(true)
            {
                var token = lexer.Lex();
                if(token.Kind == SyntaxKind.EndOfFileToken)
                    break;
                
                yield return token;
            }
        }
    }
}
