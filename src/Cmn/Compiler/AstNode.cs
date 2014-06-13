using System;
using System.Xml;
using Cmn.Compiler;

namespace Cmn.Compiler2
{
    public abstract class AstNode2
    {
        public abstract void ToXml(XmlWriter xw);
    }


    public class NonTerminal2 : AstNode2
    {
        public readonly KnonTerminal Kind;
        public readonly AstNode2[] Children;

        public NonTerminal2(KnonTerminal kind, AstNode2[] rgChildren)
        {
            Kind = kind;
            Children = rgChildren;
        }

        public override void ToXml(XmlWriter xw)
        {
            if(FWrapWithElement())
                xw.WriteStartElement(Kind.ToMnemonic());

            foreach (var astNode in Children)
                astNode.ToXml(xw);

            if (FWrapWithElement())
                xw.WriteFullEndElement();
        }

        private bool FWrapWithElement()
        {
            switch (Kind)
            {
                case KnonTerminal.Class:
                case KnonTerminal.ClassVarDecl:
                case KnonTerminal.SubroutineDecl:
                case KnonTerminal.SubroutineBody:
                case KnonTerminal.VarDecl:
                case KnonTerminal.Statements:
                case KnonTerminal.LetStatement:
                case KnonTerminal.IfStatement:
                case KnonTerminal.WhileStatement:
                case KnonTerminal.DoStatement:
                case KnonTerminal.ReturnStatement:
                case KnonTerminal.Expression:
                case KnonTerminal.ExpressionList:
                case KnonTerminal.ParameterList:
                case KnonTerminal.Term:
                    return true;

                case KnonTerminal.SubroutineCall:
                case KnonTerminal.Type:
                case KnonTerminal.Statement:
                case KnonTerminal.Op:
                case KnonTerminal.UnaryOp:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum KnonTerminal
    {
        [Mnemonic("class")]
        Class,
        [Mnemonic("classVarDec")]
        ClassVarDecl,
        [Mnemonic("subroutineDec")]
        SubroutineDecl,
        [Mnemonic("subroutineBody")]
        SubroutineBody,
        [Mnemonic("varDec")]
        VarDecl,
        [Mnemonic("statements")]
        Statements,
        [Mnemonic("letStatement")]
        LetStatement,
        [Mnemonic("ifStatement")]
        IfStatement,
        [Mnemonic("whileStatement")]
        WhileStatement,
        [Mnemonic("doStatement")]
        DoStatement,
        [Mnemonic("returnStatement")]
        ReturnStatement,
        [Mnemonic("expression")]
        Expression,
        [Mnemonic("subrouteCall")]
        SubroutineCall,
        [Mnemonic("expressionList")]
        ExpressionList,
        [Mnemonic("type")]
        Type,
        
        Statement,
        Term,
        Op,
        UnaryOp,

        [Mnemonic("parameterList")]
        ParameterList,
    }

    public enum Ktoken2
    {
        Eof,
        Whitespace,
        Comment,
        [Mnemonic("keyword")]
        Keyword,
        [Mnemonic("symbol")]
        Symbol,
        [Mnemonic("identifier")]
        Id,
        [Mnemonic("integerConstant")]
        Int,
        [Mnemonic("stringConstant")]
        String
    }

    public class Terminal2 : AstNode2
    {
        public readonly Ktoken2 Kind;
        public readonly string St;
        public int I { get { return int.Parse(St); } }
        public char Ch { get { return St[0]; } }

        public Terminal2(Ktoken2 kind, string st)
        {
            Kind = kind;
            St = st;
        }

        public override string ToString()
        {
            return Kind+"("+St+")";
        }

        public override void ToXml(XmlWriter xw)
        {
            xw.WriteStartElement(Kind.ToMnemonic());
            xw.WriteString(" " + St + " ");
            xw.WriteFullEndElement();
        }
    }
}
