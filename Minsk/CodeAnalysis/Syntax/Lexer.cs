using System.Collections.Generic;
using Minsk.CodeAnalysis.Text;


namespace Minsk.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly string _text;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private int _position;

        private int _start;
        private SyntaxKind _kind;
        private object _value;

        public Lexer(string text)
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
                     
                 case '&':
                    if (Lookahead == '&')
                    {
                       _kind = SyntaxKind.AmpersandAmpersandToken;
                        _position += 2;
                       break;
                    }
                    break;
                case '|':
                    if (Lookahead == '|')
                    {
                        _kind = SyntaxKind.PipePipeToken;
                        _position += 2;
                        break;
                    }
                    break;
                case '=':
                    _position++;
                    if(Current != '=')
                    {
                        _kind = SyntaxKind.EqualsToken;
                    }
                    else 
                    {
                        _position++;
                        _kind = SyntaxKind.EqualsEqualsToken;
                    }
                    break;
    
                case '!':
                    _position++;
                    if(Current != '=')
                    {
                        _kind = SyntaxKind.BangToken;
                    }
                    else {
                        _position++;
                        _kind = SyntaxKind.BangEqualsToken;
                    }
                    break;
                case '0': case '1': case '2': case '3': case '4': case '5':
                case '6': case '7': case '8': case '9':
                    ReadNumberToken();
                    break;
                
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    ReadWhiteSpace();
                    break;

                default:
                    if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeyword();
                    }
                    else if(char.IsWhiteSpace(Current))
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

            var length =_position -_start;
            var text = SyntaxFacts.GetText(_kind);
            if(text == null)
                text = _text.Substring(_start, length);

            return new SyntaxToken(_kind, _start, text, _value);    
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                Next();

            var length = _position - _start;
            var text = _text.Substring(_start, length);
            if (!int.TryParse(text, out var value))
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), _text, typeof(int));

            _value = value;
            _kind = SyntaxKind.NumberToken;
        }

        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(Current))
                _position++;

            _kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current))
                    _position++;

            var length = _position - _start;
            var text = _text.Substring(_start, length);
            _kind = SyntaxFacts.GetKeywordKind(text);        

            // var length = _position - _start;
            // var text = _text.Substring(_start, length);
            // var kind = SyntaxFacts.GetKeywordKind(text);
            // _kind = SyntaxFacts.GetKeywordKind(text);
        }
    }
}