using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmn.Compiler
{
    public class JackCompiler
    {
        private readonly Dictionary<string, int> mpcidBystPrefix = new Dictionary<string, int>();
        string Id(string stPrefix)
        {
            if (!mpcidBystPrefix.ContainsKey(stPrefix))
                mpcidBystPrefix[stPrefix] = 0;

            mpcidBystPrefix[stPrefix]++;
            return stPrefix + mpcidBystPrefix[stPrefix];
        }

        public enum Ksyte
        {
            Field,
            Static,
            Arg,
            Var,
            Class,
            StaticFunction,
            MemberFunction,
            Constructor,
        }

        public class Syte
        {
            public readonly Ksyte Ksyte;
            public readonly  int Isyte;
            public readonly string StName;
            public readonly string StType;

            public Syte(Ksyte ksyte, string stName, int isyte, string stType)
            {
                Ksyte = ksyte;
                StName = stName;
                Isyte = isyte;
                StType = stType;
            }
        }

        public class Syt
        {
            private Syt sytBase;
            private readonly Dictionary<string, Syte> mpsyteBystName = new Dictionary<string, Syte>();

            public Syt(Syt sytBase)
            {
                this.sytBase = sytBase;
            }

            public void Add(Ksyte ksyte, string stName, string stType)
            {
                mpsyteBystName.Add(stName, new Syte(ksyte, stName, mpsyteBystName.Count(kvp => kvp.Value.Ksyte == ksyte), stType));
            }

            public Syte Lookup(string stName)
            {
                if (mpsyteBystName.ContainsKey(stName))
                    return mpsyteBystName[stName];

                if (sytBase == null)
                    return null;
                    
                return sytBase.Lookup(stName);
            }
        }

        public string Compile(string st)
        {
            var sb = new StringBuilder();
            var astNode = new JackParser().Parse(st);

            var syt = new Syt(null);

            CompileRecursive(astNode, syt, sb);

            return sb.ToString();
        }

        private void CompileRecursive(IEnumerable<AstNode> rgnode, Syt syt, StringBuilder sb)
        {
            foreach (var node in rgnode)
                CompileRecursive(node, syt, sb);
        }

        private void CompileRecursive(AstNode node, Syt syt, StringBuilder sb)
        {
            if (node is AstBinOp) CompileBinOp((AstBinOp)node, syt, sb);
            else if (node is AstBoolLit) CompileBoolLit((AstBoolLit)node, syt, sb);
            else if (node is AstCall) CompileCall((AstCall)node, syt, sb);
            else if (node is AstClass) CompileClass((AstClass)node, syt, sb);
            else if (node is AstClassDecl) CompileClassDecl((AstClassDecl)node, syt, sb);
            else if (node is AstDo) CompileDo((AstDo)node, syt, sb);
            else if (node is AstDot) CompileDot((AstDot)node, syt, sb);
            else if (node is AstIf) CompileIf((AstIf)node, syt, sb);
            else if (node is AstIndex) CompileIndex((AstIndex)node, syt, sb);
            else if (node is AstIntLit) CompileIntLit((AstIntLit)node, syt, sb);
            else if (node is AstLet) CompileLet((AstLet)node, syt, sb);
            else if (node is AstNull) CompileNull((AstNull)node, syt, sb);
            else if (node is AstReturn) CompileReturn((AstReturn)node, syt, sb);
            else if (node is AstStringLit) CompileStringLit((AstStringLit)node, syt, sb);
            else if (node is AstSubroutine) CompileSubroutine((AstSubroutine)node, syt, sb);
            else if (node is AstThis) CompileThis((AstThis)node, syt, sb);
            else if (node is AstUnOp) CompileUnOp((AstUnOp)node, syt, sb);
            else if (node is AstVarDecl) CompileVarDecl((AstVarDecl)node, syt, sb);
            else if (node is AstVarRef) CompileVarRef((AstVarRef)node, syt, sb);
            else if (node is AstWhile) CompileWhile((AstWhile)node, syt, sb);
            else throw new ArgumentException("Unkonwn node " + node.GetType());
        }

        private void CompileVarRef(AstVarRef node, Syt syt, StringBuilder sb)
        {
            var syte = syt.Lookup(node.StName);
            if (syte.Ksyte.FIn(Ksyte.Field, Ksyte.Arg, Ksyte.Var, Ksyte.Static))
                sb.AppendLine("push {0} {1}".StFormat(StSegment(syte.Ksyte), syte.Isyte));
            else
                throw new Erparse(node, "not an rvalue");
        }

        private void CompileVarDecl(AstVarDecl node, Syt syt, StringBuilder sb)
        {
            syt.Add(node.FLocal ? Ksyte.Var : Ksyte.Arg, node.StName, node.Type.stType);
        }

        private void CompileThis(AstThis node, Syt syt, StringBuilder sb)
        {
            sb.AppendLine("push pointer 0");
        }

        private void CompileCall(AstCall node, Syt syt, StringBuilder sb)
        {
            var cparam = node.rgexprParam.Length;

            if (node.exprFunc is AstVarRef)
            {
                var nodeRef = (AstVarRef) node.exprFunc;
                var syte = syt.Lookup(nodeRef.StName);

                switch (syte.Ksyte)
                {
                    case Ksyte.StaticFunction:
                        break;
                    case Ksyte.MemberFunction:
                        sb.AppendLine("push pointer 0");
                        cparam++;
                        break;
                    default: 
                        throw new Erparse(node, "unexpected ksyte " + syte.Ksyte);
                }

                foreach (var nodeParam in node.rgexprParam)
                    CompileRecursive(nodeParam, syt, sb);

                sb.AppendLine("call {0}.{1} {2}".StFormat(node.NodeAncestor<AstClass>().StName, syte.StName, cparam));
            }
            else if (node.exprFunc is AstDot)
            {
                var nodeDot = (AstDot) node.exprFunc;

                var syteLeft = syt.Lookup(nodeDot.varLeft.StName) ?? new Syte(Ksyte.Class, nodeDot.varLeft.StName, 0, nodeDot.varLeft.StName);

                if (!syteLeft.Ksyte.FIn(Ksyte.Arg, Ksyte.Field, Ksyte.Var, Ksyte.Static, Ksyte.Class))
                   throw new Erparse(node, "unexpected ksyte " + syteLeft.Ksyte);

                if (syteLeft.Ksyte.FIn(Ksyte.Arg, Ksyte.Field, Ksyte.Var, Ksyte.Static))
                {
                    cparam++; 
                    CompileRecursive(nodeDot.varLeft, syt, sb);
                }

                Ksyte ksyteRight;
                if (syteLeft.Ksyte == Ksyte.Class && syteLeft.StName != node.NodeAncestor<AstClass>().StName)
                    ksyteRight = Ksyte.StaticFunction /*constuctor???*/;
                else
                {
                    var syte = syt.Lookup(nodeDot.varRight.StName);
                    ksyteRight = syte == null ? Ksyte.MemberFunction : syte.Ksyte;
                }

                switch (ksyteRight)
                {
                    case Ksyte.Constructor:
                        if (syteLeft.Ksyte != Ksyte.Class)
                            throw new Erparse(node, "cannot call constructor on instance");
                        break;
                    case Ksyte.StaticFunction:
                    case Ksyte.MemberFunction:
                        break;
                    default: 
                        throw new Erparse(node, "unexpected ksyte " + ksyteRight);
                }

                foreach (var nodeParam in node.rgexprParam)
                    CompileRecursive(nodeParam, syt, sb);

                sb.AppendLine("call {0}.{1} {2}".StFormat(syteLeft.StType, nodeDot.varRight.StName, cparam));
            }
        }

        private void CompileSubroutine(AstSubroutine node, Syt syt, StringBuilder sb)
        {
            var sytFunc = new Syt(syt);
            
            sb.AppendLine("function {0} {1}".StFormat(node.FQName(), node.RgVarDeclDecl.Length));

            if (node.Ksubroutine == Ksubroutine.Constructor)
            {
                var csize = node.NodeAncestor<AstClass>().RgclassDecl.Length;
                sb.AppendLine("push constant {0}".StFormat(csize));
                sb.AppendLine("call Memory.alloc 1");
                sb.AppendLine("pop pointer 0");

            }
            else if (node.Ksubroutine == Ksubroutine.MemberFunction)
            {
                sb.AppendLine("push argument 0");
                sb.AppendLine("pop pointer 0");
                sytFunc.Add(Ksyte.Arg, "this", node.NodeAncestor<AstClass>().StName);
            }

            CompileRecursive(node.RgParam, sytFunc, sb);
            CompileRecursive(node.RgVarDeclDecl, sytFunc, sb);

            CompileRecursive(node.Body, sytFunc, sb);

            if (node.Type.Ktype == Ktype.Void)
            {
                sb.AppendLine("push constant 0");
                sb.AppendLine("return");
            }
        }

        private void CompileStringLit(AstStringLit node, Syt syt, StringBuilder sb)
        {
            sb.AppendLine("push constant {0}".StFormat(node.st.Length));
            sb.AppendLine("call String.new 1");
            sb.AppendLine("pop pointer 1");
            foreach (var ch in node.st)
            {
                sb.AppendLine("push pointer 1"); 
                sb.AppendLine("push constant {0}".StFormat((int)ch));
                sb.AppendLine("call String.appendChar 2");
            }
        }

        private void CompileReturn(AstReturn node, Syt syt, StringBuilder sb)
        {
            var nodeFnc = node.NodeAncestor<AstSubroutine>();
            if (nodeFnc.Type.Ktype == Ktype.Void)
            {
                if (node.expr != null) 
                    throw new Erparse(node, "void method cannot return value");
                sb.AppendLine("push constant 0");
            }
            else
            {
                if (node.expr == null)
                    throw new Erparse(node, "void method must return value");
                CompileRecursive(node.expr, syt, sb);
            }
            sb.AppendLine("return");
        }

        private void CompileNull(AstNull node, Syt syt, StringBuilder sb)
        {
            sb.AppendLine("push constant 0");
        }

        private void CompileLet(AstLet node, Syt syt, StringBuilder sb)
        {
            CompileRecursive(node.exprRight, syt, sb);

            if (node.exprLeft is AstIndex)
            {
                var astIndex = (AstIndex) node.exprLeft;
                CompileRecursive(astIndex.exprLeft, syt, sb);
                CompileRecursive(astIndex.exprRight, syt, sb);
                sb.AppendLine("add");
                sb.AppendLine("pop pointer 1");
                sb.AppendLine("pop that 0");
                return;
            }
            
            if (node.exprLeft is AstVarRef)
            {
                var nodeVarRef = (AstVarRef) node.exprLeft;
                var syte = syt.Lookup(nodeVarRef.StName);
                if(syte.Ksyte.FIn(Ksyte.Field, Ksyte.Arg, Ksyte.Var, Ksyte.Static))
                    sb.AppendLine("pop {0} {1}".StFormat(StSegment(syte.Ksyte), syte.Isyte));
                return;
            }
         
            throw new Erparse(node, "not an lvalue");
        }


        string StSegment(Ksyte ksyte)
        {
            switch (ksyte)
            {
                case Ksyte.Field:
                    return "this";
                case Ksyte.Static:
                    return "static";
                case Ksyte.Arg:
                    return "argument";
                case Ksyte.Var:
                    return "local";
                default:
                    throw new ArgumentOutOfRangeException("ksyte");
            }
        }
        private void CompileIntLit(AstIntLit node, Syt syt, StringBuilder sb)
        {
            sb.AppendLine("push constant " + Math.Abs(node.i));
            if (node.i < 0)
                sb.AppendLine("neg");
        }

        private void CompileIndex(AstIndex node, Syt syt, StringBuilder sb)
        {
            CompileRecursive(node.exprLeft, syt, sb);
            CompileRecursive(node.exprRight, syt, sb);
            sb.AppendLine("add");
            sb.AppendLine("pop pointer 1");
            sb.AppendLine("push that 0");
        }

        private void CompileIf(AstIf node, Syt syt, StringBuilder sb)
        {
            var lblTrue = Id("IF_TRUE");
            var lblEndif = Id("IF_END");
            
            CompileRecursive(node.exprCond, syt, sb);
            sb.AppendLine("if-goto " + lblTrue);

            if (node.rgstmElse != null)
                CompileRecursive(node.rgstmElse, syt, sb);
            sb.AppendLine("goto " + lblEndif);
                
            sb.AppendLine("label " + lblTrue);
            CompileRecursive(node.rgstm, syt, sb);

            sb.AppendLine("label " + lblEndif);
        }


        private void CompileWhile(AstWhile node, Syt syt, StringBuilder sb)
        {
            var lblWhile = Id("WHILE");
            var lblTrue = Id("WHILE_TRUE");
            var lblEnd = Id("WHILE_END");

            sb.AppendLine("label " + lblWhile);
            CompileRecursive(node.exprCond, syt, sb);
            sb.AppendLine("if-goto " + lblTrue);
            sb.AppendLine("goto " + lblEnd);

            sb.AppendLine("label " + lblTrue);
            CompileRecursive(node.rgstm, syt, sb);
            sb.AppendLine("goto " + lblWhile);

            sb.AppendLine("label " + lblEnd);
        }

        private void CompileDot(AstDot node, Syt syt, StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        private void CompileDo(AstDo node, Syt syt, StringBuilder sb)
        {
            CompileRecursive(node.exprCall, syt, sb);
            sb.AppendLine("pop temp 0");
        }

        private void CompileClassDecl(AstClassDecl node, Syt syt, StringBuilder sb)
        {
            syt.Add(node.KClassDecl == KClassDecl.Field ? Ksyte.Field : Ksyte.Static, node.StName, node.Type.stType);
        }

        private void CompileClass(AstClass node, Syt syt, StringBuilder sb)
        {
            syt.Add(Ksyte.Class, node.StName, node.StName);

            CompileRecursive(node.RgclassDecl, syt, sb);

            foreach (var astSubroutine in node.Rgsubroutine)
            {
                Ksyte ksyte;
                switch (astSubroutine.Ksubroutine)
                {
                    case Ksubroutine.Constructor: ksyte = Ksyte.Constructor; break;
                    case Ksubroutine.MemberFunction: ksyte = Ksyte.MemberFunction; break;
                    case Ksubroutine.StaticFunction: ksyte = Ksyte.StaticFunction; break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                syt.Add(ksyte, astSubroutine.StName, null);
            }
            CompileRecursive(node.Rgsubroutine, syt, sb);
        }

        private void CompileBoolLit(AstBoolLit node, Syt syt, StringBuilder sb)
        {
            if (node.f)
            {
                sb.AppendLine("push constant 1");
                sb.AppendLine("neg");
            }
            else
                sb.AppendLine("push constant 0");
        }

        private void CompileBinOp(AstBinOp node, Syt syt, StringBuilder sb)
        {
            CompileRecursive(node.exprLeft, syt, sb);
            CompileRecursive(node.exprRight, syt, sb);

            switch (node.KBinop)
            {
                case KBinop.Plus: sb.AppendLine("add"); break;
                case KBinop.Minus: sb.AppendLine("sub"); break;
             
                case KBinop.And: sb.AppendLine("and"); break;
                case KBinop.Or: sb.AppendLine("or"); break;
               
                case KBinop.Lt: sb.AppendLine("lt"); break;
                case KBinop.Gt: sb.AppendLine("gt"); break;
                case KBinop.Eq: sb.AppendLine("eq"); break;

                case KBinop.Mul: sb.AppendLine("call Math.multiply 2"); break;
                case KBinop.Div: sb.AppendLine("call Math.divide 2"); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CompileUnOp(AstUnOp node, Syt syt, StringBuilder sb)
        {
            CompileRecursive(node.expr, syt, sb);
            switch (node.Kunop)
            {
                case KUnop.Minus: sb.AppendLine("neg"); break;
                case KUnop.Not: sb.AppendLine("not"); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
