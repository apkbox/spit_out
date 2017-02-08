// --------------------------------------------------------------------------------
// <copyright file="ExpressionEvaluator.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the ExpressionEvaluator type.
// </summary>
// --------------------------------------------------------------------------------
namespace SpitOut.Models
{
    using System;
    using System.IO;
    using System.Text;

    public class ExpressionEvaluator
    {
        #region Fields

        private readonly StringReader reader;

        private Token lookahead = Token.Empty;

        #endregion

        #region Constructors and Destructors

        private ExpressionEvaluator(string expr)
        {
            this.reader = new StringReader(expr);
        }

        #endregion

        #region Public Methods and Operators

        public static bool Evaluate(string expr)
        {
            return new ExpressionEvaluator(expr).Evaluate();
        }

        #endregion

        #region Methods

        private bool Evaluate()
        {
            return this.R1EvalExpr();
        }

        private Token PeekNextToken()
        {
            if (this.lookahead == Token.Empty)
            {
                this.lookahead = this.ReadNextToken();
            }

            return this.lookahead;
        }

        private bool R1EvalExpr()
        {
            return this.R2EvalOrExpr();
        }

        private bool R2EvalOrExpr()
        {
            var left = this.R3EvalAndExpr();
            var t = this.PeekNextToken();
            if (t == Token.Or)
            {
                this.ReadNextToken();
                var right = this.R2EvalOrExpr();
                return left || right;
            }

            return left;
        }

        private bool R3EvalAndExpr()
        {
            var left = this.R4EvalUnaryExpr();
            var t = this.PeekNextToken();
            if (t == Token.And)
            {
                this.ReadNextToken();
                var right = this.R3EvalAndExpr();
                return left && right;
            }

            return left;
        }

        private bool R4EvalUnaryExpr()
        {
            var t = this.PeekNextToken();
            if (t == Token.Not)
            {
                this.ReadNextToken();
                var result = this.R5EvalPrimaryExpr();
                return !result;
            }

            return this.R5EvalPrimaryExpr();
        }

        private bool R5EvalPrimaryExpr()
        {
            var t = this.ReadNextToken();
            if (t == Token.True)
            {
                return true;
            }

            if (t == Token.False)
            {
                return false;
            }

            if (t == Token.LeftParent)
            {
                var result = this.R1EvalExpr();
                t = this.ReadNextToken();
                if (t == Token.RightParent)
                {
                    return result;
                }
            }

            throw new Exception(string.Format("syntax error: Expected 'true', 'false' or '(', got '{0}'", t));
        }

        private Token ReadKeyword()
        {
            var buffer = new StringBuilder();
            while (true)
            {
                int c = this.reader.Peek();
                if (c == -1)
                {
                    break;
                }

                if (char.IsLetter((char)c))
                {
                    buffer.Append((char)this.reader.Read());
                }
                else
                {
                    break;
                }
            }

            switch (buffer.ToString().ToLowerInvariant())
            {
                case "or":
                    return Token.Or;
                case "and":
                    return Token.And;
                case "not":
                    return Token.Not;
                case "true":
                    return Token.True;
                case "false":
                    return Token.False;
            }

            return Token.Error;
        }

        private Token ReadNextToken()
        {
            if (this.lookahead != Token.Empty)
            {
                var tok = this.lookahead;
                this.lookahead = Token.Empty;
                return tok;
            }

            this.SkipWhitespaces();

            int ch = this.reader.Peek();
            if (ch == -1)
            {
                return Token.End;
            }

            if (char.IsLetter((char)ch))
            {
                return this.ReadKeyword();
            }

            ch = this.reader.Read();
            switch (ch)
            {
                case '(':
                    return Token.LeftParent;
                case ')':
                    return Token.RightParent;
            }

            return Token.Error;
        }

        private void SkipWhitespaces()
        {
            while (char.IsWhiteSpace((char)this.reader.Peek()))
            {
                this.reader.Read();
            }
        }

        #endregion
    }
}
