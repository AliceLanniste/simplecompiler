namespace Minsk.CodeAnalysis.Text
{
    public sealed class TextLine
    {
        public TextLine(SourceText text,int start,int length,int lengthIncludingBreak)
        {
            Text = text;
            Start = start;
            Length = length;
            LengthIncludingBreak = lengthIncludingBreak;
        }

        public SourceText Text { get; }
        public int Start { get; }
        public int Length { get; }
        public int LengthIncludingBreak { get; }

        public int End => Start+Length;
        public TextSpan Span => new TextSpan(Start,Length);
        public TextSpan SpanIncludingLineBreak => new TextSpan(Start, LengthIncludingBreak);

        public override string ToString() => Text.ToString(Span);
    }
}