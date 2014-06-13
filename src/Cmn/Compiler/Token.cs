using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmn.Compiler
{
    public static class KtokenExtensions
    {
        public static IEnumerable<Ktoken> Enktoken(Mtoken mtoken)
        {
            return U.Enk<Ktoken>().Where(ktoken => ktoken.Mtoken() == mtoken);
        }

        public static Mtoken Mtoken(this Ktoken k)
        {
            return k.GetAttribute<TokenDscAttribute>().Mtoken;
        }

        public static bool FRegex(this Ktoken k)
        {
            return k.GetAttribute<TokenDscAttribute>().FRegex;
        }

        public static string Rx(this Ktoken k)
        {
            return k.GetAttribute<TokenDscAttribute>().Rx;
        }
    }

    class TokenDscAttribute : Attribute
    {
        public readonly Mtoken Mtoken;
        public readonly string Rx;
        public bool FRegex { get { return Mtoken.FIn(Mtoken.Comment, Mtoken.Other, Mtoken.Whitespace); } }

        public TokenDscAttribute(Mtoken mtoken, string rx)
        {
            Rx = rx;
            Mtoken = mtoken;
        }
    }

    public enum Mtoken
    {
        Virtual, Whitespace, Comment, Keyword, Symbol, Other
    }

    public class Token : AstNode
    {
        public readonly Ktoken Ktoken;
        public readonly Mtoken Mtoken;
        public readonly string St;
        public int I { get { return int.Parse(St); } }
        public char Ch { get { return St[0]; } }
        public readonly int Iline;
        public readonly int Icol;

        public Token(Ktoken ktoken, string st, int iline, int icol)
        {
            Ktoken = ktoken;
            Mtoken = ktoken.Mtoken();
            St = st;
            Iline = iline;
            Icol = icol;
        }

        public override string ToString()
        {
            return Mtoken.FIn(Mtoken.Keyword, Mtoken.Symbol) ? St : Ktoken + "(" + St + ")";
        }
    }
}