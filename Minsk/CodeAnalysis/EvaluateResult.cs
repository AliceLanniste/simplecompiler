using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis
{
    public sealed class EvaluateResult
    {
        public EvaluateResult(IEnumerable<string> diagnostics, object value)
        {
            Diagnostics = diagnostics.ToArray();
            Value = value;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public object Value { get; }
    }
}