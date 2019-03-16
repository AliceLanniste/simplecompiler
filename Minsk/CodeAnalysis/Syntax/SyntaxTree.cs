using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            // var parser = new Parser(text);
            // return parser.Parse();
            var sourcetext =  SourceText.From(text);
            return Parse(sourcetext);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText);
        }
        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
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

// _diagnostics是为了在出现不符合语法的时候，编译器跳出提醒