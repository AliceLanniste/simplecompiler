using System;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Syntax;
using Xunit;

namespace Minsk.Tests.CodeAnalysis.Syntax
{
    public class SyntaxFctTest
    {
        [Theory]
        [MemberData(nameof(GetSyntaxKindData))]
        public void SyntaxFact_GetText_RoundTrips(SyntaxKind kind)
        {
            var text = SyntaxFct.GetText(kind);
        }

        public static IEnumerable<object[]> GetSyntaxKindData()
        {
            var kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
                yield return new object[]{ kind };
        }
    }
}