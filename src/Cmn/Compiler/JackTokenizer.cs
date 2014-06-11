using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cmn.Compiler
{
    public class JackTokenizer
    {
        public IEnumerable<Token> Entoken(string st)
        {
            return EntokenI(st).Where(jtok => !U.FIn(jtok.Ktoken, Ktoken.Whitespace, Ktoken.Comment));
        }

        IEnumerable<Token> EntokenI(string st)
        {
            var rxWhitespace = new Regex(@"\G\s+");
            var rxBlockComment = new Regex(@"\G/\*(?:(?!\*/)(?:.|[\n]+))*\*/");
            var rxApiComment = new Regex(@"\G/\*\*(?:(?!\*/)(?:.|[\n]+))*\*/");
            var rxLineComment = new Regex(@"\G//(.*?)\r?\n");
            var rxId = new Regex(@"\G[a-zA-Z_][a-zA-Z_0-9]*");
            var rxInt = new Regex(@"\G[0-9]+");
            var rxString = new Regex(@"\G""[^""]*""");

            for(var ich=0;ich<st.Length;)
            {
                string stMatch;
              
                if (FAccept(st, ich, out stMatch, rxWhitespace))
                    yield return new Token(Ktoken.Whitespace, stMatch);
                else if(FAccept(st, ich, out stMatch, rxApiComment))
                    yield return new Token(Ktoken.Comment, stMatch);

                else if (FAccept(st, ich, out stMatch, rxBlockComment))
                    yield return new Token(Ktoken.Comment, stMatch);

                else if(FAccept(st, ich, out stMatch, rxLineComment))
                    yield return new Token(Ktoken.Comment, stMatch);

                else if (FAccept(st, ich, out stMatch, 
                    "class","constructor", "function", "method", "field", "static","var",
                    "int","char","boolean","void","true","false","null",
                    "this","let","do","if","else","while","return"))
                    yield return new Token(Ktoken.Keyword, stMatch);

                else if (FAccept(st, ich, out stMatch, "{", "}", "[", "]", "(", ")", ".", ",", ";", "-", "+", "*", "/", "&", "|", "<", ">", "=" ,"~"))
                    yield return new Token(Ktoken.Symbol, stMatch);

                else if (FAccept(st, ich, out stMatch, rxId))
                    yield return new Token(Ktoken.Id, stMatch);

                else if (FAccept(st, ich, out stMatch, rxInt))
                    yield return new Token(Ktoken.Int, stMatch);

                else if (FAccept(st, ich, out stMatch, rxString))
                    yield return new Token(Ktoken.String, stMatch.Trim('"'));

                ich += stMatch.Length;
            }
        }

        private bool FAccept(string st, int ich, out string stMatch, params string[] rgstNeedle)
        {
            foreach (var stNeedle in rgstNeedle)
            {
                var fOk = true;
                for (int i = 0; i < stNeedle.Length && fOk; i++)
                    fOk = ich + i < st.Length && st[ich + i] == stNeedle[i];
                
                if (fOk)
                {
                    stMatch = stNeedle;
                    return true;
                }
            }

            stMatch = "";
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
            
            stMatch = "";
            return false;
        }
    }
}