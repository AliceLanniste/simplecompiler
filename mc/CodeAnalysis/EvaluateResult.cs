using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis
{
    public sealed class EvaluateResult
    {
        public EvaluateResult(IEnumerable<string> diagnostics, object value)
        {
            Diagnostics = diagnostics;
            Value = value;
        }

        public IEnumerable<string> Diagnostics { get; }
        public object Value { get; }
    }
}