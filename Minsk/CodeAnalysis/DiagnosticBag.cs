using System;
using System.Collections;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Syntax;


namespace Minsk.CodeAnalysis
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostic = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostic.GetEnumerator();
        

        IEnumerator IEnumerable.GetEnumerator()=> GetEnumerator();

         public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostic.AddRange(diagnostics._diagnostic);
        }

        public void Report(TextSpan span, string message)
        {
            var diagnostic = new Diagnostic(span, message);
            _diagnostic.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextSpan span, string text, Type type)
        {
            var message = $"The number {text} isn't valid {type}";
            Report(span, message);
        }

        public void ReportBadToken(int position, char current)
        {
            var span = new TextSpan(position, 1);
            var message = $"Bad character input: '{current}'";
            Report(span, message);
        }

        public void ReportUnexpectToken(TextSpan span,  SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, Type operandType)
        {
            var message = $"Unary operator '{operatorText}' is not defined for type {operandType}.";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, Type leftType, Type rightType)
        {
            var message = $"Binary operator '{operatorText}' is not defined for types {leftType} and {rightType}.";
            Report(span, message);
        }

         public void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"Variable '{name}' doesn't exist.";
            Report(span, message);
        }
    }
}

