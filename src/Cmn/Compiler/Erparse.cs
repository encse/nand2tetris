using System;
using System.Linq;

namespace Cmn.Compiler
{
    class Erparse : Exception
    {
        public int iline;
        public int icol;
        public string StSrc;

        public Erparse(int iline, int icol, string st) : base(st)
        {
            this.iline = iline;
            this.icol = icol;
        }

        public override string Message
        {
            get
            {
                var stMsg = base.Message;
                var rgstLines = StSrc.ToLines();

                var clineSkip = iline >= 5 ? iline - 5 : 0;
                var clineBefore = iline >= 5 ? 5 : iline;
                var rgstLineBefore = rgstLines.Skip(clineSkip).Take(clineBefore).ToList();

                var rgstLineAfter = rgstLines.Skip(iline).Take(5).ToList();

                var cchTab = rgstLineBefore.Last().Take(icol).Count(ch => ch == '\t');
                
                var stMarker = new String('\t', cchTab) + new String(' ', icol-cchTab-1)+"^";
                var stExcerpt = rgstLineBefore.Concat(stMarker.EnCons()).Concat(rgstLineAfter).StJoin("\n");
                stExcerpt = stExcerpt.Replace("\t", "  ");


                return "Error at {0}:{1}: {2}\n\n{3}".StFormat(iline, icol, stMsg, stExcerpt);
            }
        }
    }
}