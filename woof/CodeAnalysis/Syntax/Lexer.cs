using System.Collections.Generic;

namespace woof.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private int _start;
        private object _value;
        private SyntaxKind _kind;
        public Lexer(string text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;
        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
                return '\0';
            return _text[index];
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken Lex()
        {
            // numbers
            // + - / * ( )
            //<whitespaces>
            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;

            switch (Current)
                {
                    case '\0':
                        _kind = SyntaxKind.EndOfFileToken;
                        break;
                    case '+':
                        _position++;
                        _kind = SyntaxKind.PlusToken;
                        break;
                    case '-':
                        _position++;
                        _kind = SyntaxKind.MinusToken;
                        break;
                    case '*':
                        _position++;
                        _kind = SyntaxKind.StarToken;
                        break;
                    case '/':
                        _position++;
                        _kind = SyntaxKind.SlashToken;
                        break;
                    case '(':
                        _position++;
                        _kind = SyntaxKind.OpenParenthesisToken;
                        break;
                    case ')':
                        _position++;
                        _kind = SyntaxKind.CloseParanthesisToken;
                        break;
                    case '&':
                        if(LookAhead == '&')
                        {
                            _position += 2;
                            _kind = SyntaxKind.AmpersandAmpersandToken;
                            break;
                        }
                        break;
                    case '|':
                        if(LookAhead == '|')
                        {
                            _position += 2;
                            _kind = SyntaxKind.PipePipeToken;
                            break;
                        }
                        break;
                    case '=':
                        if(LookAhead == '=')
                        {
                            _position += 2;
                            _kind = SyntaxKind.EqualsEqualsToken;
                        }
                        else
                        {
                            _position++;
                            _kind = SyntaxKind.EqualsToken;
                        }
                        break;
                    case '!':
                        if(LookAhead == '=')
                        {
                            _position += 2;
                            _kind = SyntaxKind.BangEqualsToken;
                        }
                        else
                        {
                            _position++;
                            _kind = SyntaxKind.BangToken;
                        }
                        break;
                    default:
                        if(char.IsDigit(Current))
                        {
                            ReadNumberToken();
                        }

                        else if (char.IsWhiteSpace(Current))
                        {
                            ReadWhiteSpace();
                        }

                        else if(char.IsLetter(Current))
                        {
                            ReadIdentifierOrKeyword();
                        }
                        else
                        {    _diagnostics.ReportBadCharacter(_position, Current);
                            _position++;
                        }
                        break;
                }

            var length = _position - _start;
            var text = SyntaxFacts.GetText(_kind);
            if(text == null)
                text = _text.Substring(_start, length);
            return new SyntaxToken(_kind, _start, text, _value);
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current))
                Next();
            var length = _position - _start;
            var text = _text.Substring(_start, length);
            _kind = SyntaxFacts.GetKeywordKind(text);
        }

        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(Current))
                Next();
            _kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                Next();
            var length = _position - _start;
            var text = _text.Substring(_start, length);
            if (!int.TryParse(text, out var value))
            {
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), _text, typeof(int));
            }

            _value = value;
            _kind = SyntaxKind.NumberToken;
        }
    }
}