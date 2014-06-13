using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cmn.Compiler2;

namespace Cmn.Compiler
{
    class JackCompiler
    {
        public enum Ksyte
        {
            Field,
            Static,
            Arg,
            Var,
            Class,
            Func,
        }

        public class Syte
        {
            public readonly Ksyte ksyte;
            private int isyte;
            public string stName;
        }

        public class Syt
        {
            private Syt sytBase;
            private readonly Dictionary<string, Syte> mpsyteBystName = new Dictionary<string, Syte>();

            public void Add(Syte syte)
            {
                mpsyteBystName.Add(syte.stName, syte);
            }

            public Syte Lookup(string stName)
            {
                if (mpsyteBystName.ContainsKey(stName))
                    return mpsyteBystName[stName];
                return sytBase.Lookup(stName);
            }
        }

        public string Compile(string st)
        {
            var sb = new StringBuilder();
        //    var astNode = new JackParser().Parse(st);

            var syt = new Syt();

         //   CompileRecursive(astNode, syt);
            throw new NotImplementedException();

        }

        private void CompileRecursive(AstNode2 node, Syt syt)
        {
            throw new NotImplementedException();
        }
    }
}
