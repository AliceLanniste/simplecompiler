// using Minsk.CodeAnalysis.Syntax;
// using System.Collections.Generic;
// using Xunit;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Syntax;
using Xunit;



namespace Minsk.Tests.CodeAnalysis.Syntax
{
    public class  ParserTest
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorData))]
        public void Parser_BinaryExpression_HonorsPrecedences(SyntaxKind op1, SyntaxKind op2) 
        {
            var op1Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(op1);
            var op2Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(op2);
            var op1Text = SyntaxFacts.GetText(op1);
            var op2Text = SyntaxFacts.GetText(op2);


            var text =  $"a {op1Text} b {op2Text} c";
            var expression = SyntaxTree.Parse(text).Root;

            if (op1Precedence >= op2Precedence)
            {
                using(var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken,"a");
                    e.AssertToken(op1,op1Text);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "b");
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken,"c");
                }
            }
            else {

                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken,"a");
                    e.AssertToken(op1,op1Text);
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "b");
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "c");
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void Parser_UnaryExpression_HonorsPrecedences(SyntaxKind unaryKind, SyntaxKind binaryKind)
        {
            var uPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(unaryKind);
            var bPrecedence = SyntaxFacts.GetBinaryOperatorPrecedence(binaryKind);
            var uText = SyntaxFacts.GetText(unaryKind);
            var bText = SyntaxFacts.GetText(binaryKind);
            var text = $"{uText} a {bText} b";
            var expression = SyntaxTree.Parse(text).Root;

            if (uPrecedence >= bPrecedence)
            {
                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.UnaryExpression);
                    e.AssertToken(unaryKind, uText);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken,"a");
                    e.AssertToken(binaryKind, bText);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "b");
                }    
            } 
            else
            {
                 using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.UnaryExpression);
                    e.AssertToken(unaryKind, unaryText);
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "a");
                    e.AssertToken(binaryKind, binaryText);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "b");
                }   
            }
        }

        public static IEnumerable<object[]> GetUnaryOperatorPairsData() {
            foreach (var a in SyntaxFacts.GetunaryOperatorKinds())
            {
                foreach (var b in SyntaxFacts.GetBinaryOperatorKinds())
                {
                    yield return new object[] {a, b};
                }
            }
        }

        public static IEnumerable<object[]> GetBinaryOperatorData() 
        {
            foreach (var op1 in SyntaxFacts.GetBinaryOperatorKinds())
            {
                foreach (var op2 in SyntaxFacts.GetBinaryOperatorKinds())
                {
                    yield return new object[] {op1, op2};
                }
            }
        }
    }
}