namespace Cmn.Compiler
{
    public class AstNode
    {
    }

    public class AstClass : AstNode
    {
        public string StName;
        public AstClassDecl[] RgclassDecl;
        public AstSubroutine[] Rgsubroutine;
    }

    public enum Ksubroutine
    {
        Constructor, Method, Function
    }
    public class AstSubroutine : AstNode
    {
        public Ksubroutine Ksubroutine;
        public string StName;
        public AstType Type { get; set; }
        public AstVar[] RgParam { get; set; }
        public AstStm[] Body { get; set; }
        public AstVar[] RgVarDecl { get; set; }
    }

    public enum KClassDecl { Field, Static }
    public class AstClassDecl : AstNode
    {
        public KClassDecl KClassDecl;
        public string StName;
        public AstType Type;
    }

    public enum Ktype
    {
        Void, Int, Char, Bool, Class
    }

    public class AstType : AstNode
    {
        public Ktype Ktype;
        public string stType;
    }

    public class AstVar : AstNode
    {
        public AstType Type;
        public string StName;
    }

    public class AstStm : AstNode
    {

    }

    public class AstLet : AstStm
    {
        public AstExpr exprLeft;
        public AstExpr exprRight;
    }

    public class AstReturn : AstStm
    {
        public AstExpr expr;
    }

    public class AstDo : AstStm
    {
        public AstCall exprCall;
    }

    public class AstWhile : AstStm
    {
        public AstExpr exprCond;
        public AstStm[] rgstm;
    }

    public class AstIf : AstStm
    {
        public AstExpr exprCond;
        public AstStm[] rgstm;
        public AstStm[] rgstmElse;
    }


    public class AstExpr : AstNode
    {

    }

    public enum KUnop
    {
        Minus, Negate
    }

    public class AstUnOp : AstExpr
    {
        public KUnop Kunop;
        public AstExpr expr;
    }

    public enum KBinop
    {
        Plus, Div, Mul, And, Or, Lt, Gt, Eq, Minus
    }

    public class AstBinOp : AstExpr
    {
        public KBinop KBinop;
        public AstExpr exprLeft;
        public AstExpr exprRight;
    }


    public class AstIndex : AstExpr
    {
        public AstExpr exprLeft;
        public AstExpr exprRight;
    }

    public class AstVarRef : AstExpr
    {
        public string StName;
    }

    public class AstDot : AstExpr
    {
        public AstVarRef varLeft;
        public AstVarRef varRight;
    }


    public class AstThis : AstExpr { }
    public class AstNull : AstExpr { }

    public class AstBoolLit : AstExpr
    {
        public bool f;
    }
    public class AstStringLit : AstExpr
    {
        public string st;
    }
    public class AstIntLit : AstExpr
    {
        public int i;
    }

    public class AstCall : AstExpr
    {
        public AstExpr exprFunc;
        public AstExpr[] rgexprParam;
    }
}
