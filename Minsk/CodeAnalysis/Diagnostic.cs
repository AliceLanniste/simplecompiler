using Minsk.CodeAnalysis.Text;


/** 
 diagnostic目的是当输入的代码与语法规则不合的时候指出错误，
 之前的diagnostic都是大致提示，比如` operator xxx is not defined for xtype and ytype`
 现在需要更明确的提示，明确到是哪行出错,然后替换之前所有的diagnostic
*/ 
namespace Minsk.CodeAnalysis
{
    public sealed class Diagnostic
    {
        public Diagnostic(TextSpan span, string message)
        {
            Span = span;
            Message = message;
        }

        public TextSpan Span { get; }
        public string Message { get; }

        public override string ToString() => Message;
    }
}

