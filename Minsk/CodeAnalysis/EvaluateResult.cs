using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Minsk.CodeAnalysis
{
    public sealed class EvaluateResult
    {
        public EvaluateResult(ImmutableArray<Diagnostic> diagnostics, object value)
        {
            Diagnostics = diagnostics.ToArray();
            Value = value;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public object Value { get; }
    }
}