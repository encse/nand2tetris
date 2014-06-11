using System;
using System.Xml;

namespace Cmn.Compiler
{
    public abstract class AstNode
    {
        public abstract void ToXml(XmlWriter xw);
    }

    public class NonTerminal : AstNode
    {
        public readonly KnonTerminal KnonTerminal;
        public readonly AstNode[] Children;

        public NonTerminal(KnonTerminal knonTerminal, AstNode[] rgChildren)
        {
            KnonTerminal = knonTerminal;
            Children = rgChildren;
        }

        public override void ToXml(XmlWriter xw)
        {
            if(FWrapWithElement())
                xw.WriteStartElement(KnonTerminal.ToMnemonic());

            foreach (var astNode in Children)
                astNode.ToXml(xw);

            if (FWrapWithElement())
                xw.WriteFullEndElement();
        }

        private bool FWrapWithElement()
        {
            switch (KnonTerminal)
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

    public enum Ktoken
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

    public class Token : AstNode
    {
        public readonly Ktoken Ktoken;
        public readonly string St;
        public int I { get { return int.Parse(St); } }
        public char Ch { get { return St[0]; } }

        public Token(Ktoken ktoken, string st)
        {
            Ktoken = ktoken;
            St = st;
        }

        public override string ToString()
        {
            return Ktoken+"("+St+")";
        }

        public override void ToXml(XmlWriter xw)
        {
            xw.WriteStartElement(Ktoken.ToMnemonic());
            xw.WriteString(" " + St + " ");
            xw.WriteFullEndElement();
        }
    }
}
