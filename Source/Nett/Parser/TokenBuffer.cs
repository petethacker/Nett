﻿namespace Nett.Parser
{
    using System;

    internal sealed class TokenBuffer : LookaheadBuffer<Token>
    {
        private bool autoThrowAwayNewlines = false;

        public TokenBuffer(Func<Token?> read, int lookAhead)
            : base(read, lookAhead)
        {
        }

        public override bool End => this.Peek().type == TokenType.Eof || base.End;

        public void ConsumeAllNewlines()
        {
            while (this.Peek().type == TokenType.NewLine) { this.Consume(); }
        }

        public Token Expect(TokenType tt)
        {
            var t = this.Peek();
            if (t.type != tt)
            {
                throw Parser.CreateParseError(t, $"Expected token of type '{tt}' but token with value of '{t.value}' and the type '{t.type}' was found.");
            }

            return t;
        }

        public Token ExpectAndConsume(TokenType tt)
        {
            this.Expect(tt);
            return this.Consume();
        }

        public bool TryExpect(string tokenValue)
        {
            return this.Peek().value == tokenValue;
        }

        public bool TryExpectAndConsume(TokenType tt)
        {
            var r = this.TryExpect(tt);
            if (r)
            {
                this.Consume();
            }

            return r;
        }

        public bool TryExpect(TokenType tt)
        {
            return !this.End && this.Peek().type == tt;
        }

        public bool TryExpectAt(int index, TokenType tt)
        {
            return !this.End && this.PeekAt(index).type == tt;
        }

        public bool TryExpectAt(TokenType tt)
        {
            return this.Peek().type == tt;
        }

        public IDisposable UseIgnoreNewlinesContext()
        {
            this.ConsumeAllNewlines();
            return new AutoThrowAwayNewLinesContext(this);
        }

        public override Token Consume()
        {
            var t = base.Consume();

            if (this.autoThrowAwayNewlines)
            {
                this.ConsumeAllNewlines();
            }

            return t;
        }

        public ImaginaryContext GetImaginaryContext()
        {
            return new ImaginaryContext(this);
        }

        public struct ImaginaryContext
        {
            private readonly TokenBuffer buffer;

            private int position;

            public ImaginaryContext(TokenBuffer buffer)
            {
                this.buffer = buffer;
                this.position = 0;
            }

            public bool TryExpect(TokenType tt)
            {
                return this.buffer.TryExpectAt(this.position, tt);
            }

            public bool TryExpectAt(int la, TokenType tt)
            {
                return this.buffer.TryExpectAt(this.position + la, tt);
            }

            public Token Consume()
            {
                return this.buffer.PeekAt(this.position++);
            }

            public bool TryExpectAndConsume(TokenType tt)
            {
                var r = this.TryExpect(tt);
                if (r)
                {
                    this.position++;
                }

                return r;
            }

            public void ConsumeAllNewlines()
            {
                while (this.TryExpectAndConsume(TokenType.NewLine)) { }
            }

            public void MakeItReal()
            {
                while (this.position-- > 0)
                {
                    this.buffer.Consume();
                }
            }
        }

        private class AutoThrowAwayNewLinesContext : IDisposable
        {
            private readonly TokenBuffer buffer;

            public AutoThrowAwayNewLinesContext(TokenBuffer buffer)
            {
                this.buffer = buffer;
                buffer.autoThrowAwayNewlines = true;
            }

            public void Dispose() => this.buffer.autoThrowAwayNewlines = false;
        }
    }
}
