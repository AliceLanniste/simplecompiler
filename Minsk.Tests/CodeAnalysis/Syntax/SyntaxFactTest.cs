using Xunit;
using System;
using Minsk.CodeAnalysis.Syntax;
using System.Collections.Generic;


namespace Minsk.Tests.CodeAnalysis.Syntax
{
    public class SyntaxFactTest
    {
        [Theory]
        [MemberData(nameof(GetSyntaxKindData))]
        public void SyntaxFact_GetText(SyntaxKind kind)
        {
            var text = SyntaxFact.GetText(kind);
            var tokens = SyntaxTree.ParseTokens(text);
            var token =  Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Enum(text, token.Text);
        }

        public static IEnumerable<object[]> GetSyntaxKindData() {
            var kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));

            foreach (var kind in kinds)
                yield return new object[] {kind};
        } 
    }
}