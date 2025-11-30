using System.Collections.Generic;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly SourceText _text;
        private int _position;
        private DiagnosticBag _diagnostics = new DiagnosticBag();
        private int _start;

        private object _value;

        private SyntaxKind _kind;
        public Lexer(SourceText text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private char Lookahead => peek(1);
        private char Current => peek(0);
        private char peek(int offset)
        {   
            var index = _position + offset;
            if(index >= _text.Length)
                return '\0';
            
            return _text[index];

        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken Lex()
        {

            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null; 

            switch (Current)
            {
                case '\0':
                    _kind = SyntaxKind.EndOfFileToken;
                    break;

                case '+':
                    _kind = SyntaxKind.PlusToken;
                    _position++;
                    break;
                case '-':
                    _kind = SyntaxKind.MinusToken;
                    _position++;
                    break;
                case '*':
                    _kind = SyntaxKind.StarToken;
                    _position++;
                    break;

                case '/':
                    _kind = SyntaxKind.SlashToken;
                    _position++;
                    break;
                case '(':
                    _kind = SyntaxKind.OpenParenthesisToken;
                    _position++;
                    break;
                case ')':
                    _kind = SyntaxKind.CloseParenthesisToken;
                    _position++;
                    break;
                case '{':
                    _kind = SyntaxKind.OpenBraceToken;
                    _position++;
                    break;
                case '}':
                    _kind = SyntaxKind.CloseBraceToken;
                    _position++;
                    break;
                 case '&':
                 
                    if (Lookahead == '&')
                    {
                        _position += 2;
                        _kind = SyntaxKind.AmpersandAmpersandToken;
                    }
                    break;
                case '|':
                    if (Lookahead == '|')
                    {
                        _position += 2;
                        _kind = SyntaxKind.PipePipeToken;
                    }
                    break;
                case '=':
                    _position++;
                    if (Current != '=')
                    {
                        
                        _kind = SyntaxKind.EqualsToken;
                    } else
                    {
                        _position++;
                        _kind = SyntaxKind.EqualsEqualsToken;
                    }
                    break;
                case '!':
                    _position++;
                    if (Current != '='){
                        _kind = SyntaxKind.BangToken;
                        
                    }
                    else
                    {
                         _position ++;
                        _kind = SyntaxKind.BangEqualsToken;
                    }
                    break;
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                    ReadNumberToken();
                    break;

                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    ReadWhiteSpace();
                    break;
                default:
                    if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeyword();
                    }
                    else if (char.IsWhiteSpace(Current))
                    {
                        ReadWhiteSpace();
                    }
                    else
                    {
                        _diagnostics.ReportBadCharacter(_position, Current);
                        _position++;
                    }
                    break;
            }
             
            var length = _position - _start;
           var text = SyntaxFacts.GetText(_kind);
            if (text == null)
                text = _text.ToString(_start, length);
         

            return new SyntaxToken(_kind, _start, text, _value);
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                _position++;

            var length = _position - _start;
            var text = _text.ToString(_start, length);
            if (!int.TryParse(text, out var value))
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(int));
            _kind = SyntaxKind.NumberToken;
            _value = value;
         
        }

        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(Current))
                _position++;

            var length = _position - _start;
            _kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadIdentifierOrKeyword()
        {
            
            while (char.IsLetter(Current))
                _position++;

            var length = _position - _start;
             var text = _text.ToString(_start, length);
            _kind = SyntaxFacts.GetKeywordKind(text);
        }
    }
}
