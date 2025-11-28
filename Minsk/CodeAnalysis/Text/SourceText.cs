using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Text
{
    public sealed class SourceText
    {
        private readonly string _text;
        private SourceText(string text)
        {
            _text = text;
            Lines = ParseLines(this, text);
        }

        public char this[int index] => _text[index];
        public int Length => _text.Length;
        public ImmutableArray<TextLine> Lines {get;}

        private  ImmutableArray<TextLine>  ParseLines(SourceText sourceText,string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();

            var lineStart = 0;
            var position = 0;

            while (position < text.Length)
            {
                var lineBreakWidth = GetLineBreakWidth(text, position);

                if (lineBreakWidth == 0)
                {
                    position++;
                } else
                {
                    AddLine(result,sourceText,position, lineStart, lineBreakWidth);
                    position += lineBreakWidth;
                    lineStart = position;
                }
            }

            if (position > lineStart)
                AddLine(result, sourceText,position, lineStart, 0);
            return result.ToImmutable();
        }

        public int GetLineIndex(int position)
        {
            var lower = 0;
            var upper = Lines.Length - 1;

            while (lower <= upper)
            {
                var index = lower + (upper-lower) / 2;
                var line = Lines[index];

                if (line.Start <= position && position <= line.End)
                    return index;

                if (position < line.Start)
                {
                    upper = index - 1;
                }     
                if (position > line.End)
                {
                    lower = index + 1;
                }
            }

            return lower - 1;
        }

        private void AddLine(ImmutableArray<TextLine>.Builder result,SourceText sourceText, int position, int lineStart, int lineBreakWidth)
        {
            var length = position - lineStart;
            var lengthIncludingLineBreak = length + lineBreakWidth;
            var line = new TextLine(sourceText, lineStart, length, lengthIncludingLineBreak);
            result.Add(line);
        }
        private static int GetLineBreakWidth(string text, int index)
        {
            var c = text[index];
            var l = index +1 >= text.Length ? '\0' : text[index + 1];

            if (c == '\r' && l == '\n')
            {
                return 2;
            }
            else if (c == '\r' || c == '\n')
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }
        public static SourceText From(string text)
        {
            return new SourceText(text);
        }

      public override string ToString() => _text;

        public string ToString(int start, int length) => _text.Substring(start, length);

        public string ToString(TextSpan span) => ToString(span.Start, span.Length);
    }

  
 
}

