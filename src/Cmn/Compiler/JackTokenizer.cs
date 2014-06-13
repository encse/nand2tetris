using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cmn.Compiler
{

    class Erparse : Exception
    {
        public Erparse(int iline, int icol, string st) : base("Error at {0}:{1}: {2}".StFormat(iline, icol, st))
        {

        }
    }
    public enum Ktoken
    {
        [TokenDsc(Mtoken.Virtual, null)]Eof, 

        [TokenDsc(Mtoken.Whitespace, @"\s+")]Whitespace,

        [TokenDsc(Mtoken.Comment, @"/\*(?:(?!\*/)(?:.|[\n]+))*\*/")] BlockComment, 
        [TokenDsc(Mtoken.Comment, @"/\*\*(?:(?!\*/)(?:.|[\n]+))*\*/")]ApiComment, 
        [TokenDsc(Mtoken.Comment, @"//(.*?)\r?\n")]LineComment,

        [TokenDsc(Mtoken.Other, @"[a-zA-Z_][a-zA-Z_0-9]*")] Id,
        [TokenDsc(Mtoken.Other, @"[0-9]+")] IntLit,
        [TokenDsc(Mtoken.Other, @"""[^""]*""")] StringLit,

        [TokenDsc(Mtoken.Keyword, "class")] Class,
        [TokenDsc(Mtoken.Keyword, "constructor")] Constructor,
        [TokenDsc(Mtoken.Keyword, "function")] Function,
        [TokenDsc(Mtoken.Keyword, "method")] Method,
        [TokenDsc(Mtoken.Keyword, "field")] Field,
        [TokenDsc(Mtoken.Keyword, "static")] Static,
        [TokenDsc(Mtoken.Keyword, "var")] Var,
        [TokenDsc(Mtoken.Keyword, "char")] Char,
        [TokenDsc(Mtoken.Keyword, "int")] Int,
        [TokenDsc(Mtoken.Keyword, "boolean")] Bool,
        [TokenDsc(Mtoken.Keyword, "void")] Void,
        [TokenDsc(Mtoken.Keyword, "true")] True,
        [TokenDsc(Mtoken.Keyword, "false")] False,
        [TokenDsc(Mtoken.Keyword, "null")] Null,
        [TokenDsc(Mtoken.Keyword, "this")] This,
        [TokenDsc(Mtoken.Keyword, "let")] Let,
        [TokenDsc(Mtoken.Keyword, "do")] Do,
        [TokenDsc(Mtoken.Keyword, "if")] If,
        [TokenDsc(Mtoken.Keyword, "else")] Else,
        [TokenDsc(Mtoken.Keyword, "while")] While,
        [TokenDsc(Mtoken.Keyword, "return")] Return,
        [TokenDsc(Mtoken.Symbol, "{")] Lbrace,
        [TokenDsc(Mtoken.Symbol, "}")] Rbrace,
        [TokenDsc(Mtoken.Symbol, "[")] Lbracket,
        [TokenDsc(Mtoken.Symbol, "]")] Rbracket,
        [TokenDsc(Mtoken.Symbol, "(")] Lparen,
        [TokenDsc(Mtoken.Symbol, ")")] Rparen,
        [TokenDsc(Mtoken.Symbol, ".")] Dot,
        [TokenDsc(Mtoken.Symbol, ",")] Comma,
        [TokenDsc(Mtoken.Symbol, ";")] Semicolon,
        [TokenDsc(Mtoken.Symbol, "-")] Minus,
        [TokenDsc(Mtoken.Symbol, "+")] Plus,
        [TokenDsc(Mtoken.Symbol, "*")] Asterix,
        [TokenDsc(Mtoken.Symbol, "/")] Slash,
        [TokenDsc(Mtoken.Symbol, "&")] And,
        [TokenDsc(Mtoken.Symbol, "|")] Or,
        [TokenDsc(Mtoken.Symbol, "<")] Lt,
        [TokenDsc(Mtoken.Symbol, ">")] Gt,
        [TokenDsc(Mtoken.Symbol, "=")] Eq,
        [TokenDsc(Mtoken.Symbol, "~")] Tilde
    }

    public class JackLexer
    {
        public IEnumerable<Token> Entoken(string st)
        {
            return EntokenI(st).Where(jtok => !jtok.Mtoken.FIn(Mtoken.Comment, Mtoken.Whitespace));
        }

        IEnumerable<Token> EntokenI(string st)
        {
            var mpRegexByKtoken = new Dictionary<Ktoken, Regex>();
            foreach (var ktoken in U.Enk<Ktoken>().Where(ktoken => ktoken.FRegex()))
                mpRegexByKtoken[ktoken] = new Regex(@"\G" + ktoken.Rx());


            var mpstByKtoken = new Dictionary<Ktoken, string>();
            foreach (var ktoken in U.Enk<Ktoken>().Where(ktoken => !ktoken.FRegex()))
                mpstByKtoken[ktoken] = ktoken.Rx();

            var rgktoken = new[] {Mtoken.Whitespace, Mtoken.Comment, Mtoken.Keyword, Mtoken.Symbol, Mtoken.Other}.SelectMany(KtokenExtensions.Enktoken).ToArray();

            int iline=1;
            int icol=1;
            for(var ich=0;ich<st.Length;)
            {
                string stMatch = null;

                foreach (var ktoken in rgktoken )
                {

                    if (mpRegexByKtoken.ContainsKey(ktoken))
                    {
                        if (FAccept(st, ich, out stMatch, mpRegexByKtoken[ktoken]))
                        {
                            if(ktoken == Ktoken.StringLit)
                                yield return new Token(ktoken, stMatch.Trim('"'), iline, icol);    
                            else
                                yield return new Token(ktoken, stMatch, iline, icol);    
                        }
                    }
                    else
                    {
                        if (FAccept(st, ich, out stMatch, mpstByKtoken[ktoken]))
                            yield return new Token(ktoken, stMatch, iline, icol);
                    }

                    if (stMatch != null)
                    {
                        ich += stMatch.Length;
                        break;
                    }
                }

                
                if (stMatch == null)
                {
                    var stAt = st.Substring(ich, 10);

                    throw new Erparse(iline, icol, "Lexer error around " +stAt );
                }

                var rgstLines = stMatch.Replace("\r", "").Split('\n');

                iline += rgstLines.Length - 1;
                if (rgstLines.Length > 1)
                    icol = rgstLines.Last().Length + 1;
                else
                    icol += rgstLines.Last().Length;
            }
        }

        private bool FAccept(string st, int ich, out string stMatch, string stNeedle)
        {
            var fOk = true;
            for (var i = 0; i < stNeedle.Length && fOk; i++)
                fOk = ich + i < st.Length && st[ich + i] == stNeedle[i];

            if (fOk)
            {
                stMatch = stNeedle;
                return true;
            }
            
            stMatch = null;
            return false;
        }

        private bool FAccept(string st, int ich, out string stMatch, Regex rx)
        {
            var match = rx.Match(st, ich);
            if (match.Success)
            {
                stMatch = match.Value;
                return true;
            }
            
            stMatch = null;
            return false;
        }
    }
}