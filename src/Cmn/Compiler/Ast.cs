using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cmn.Compiler
{
    public abstract class AstNode
    {
        public AstNode NodeParent;

        public IEnumerable<AstNode> EnnodeAncestor()
        {
            var node = NodeParent;
            while (node != null)
            {
                yield return node;
                node = node.NodeParent;
            } 
        }

        public T NodeAncestor<T>() where T: AstNode
        {
            return EnnodeAncestor().OfType<T>().Single();
        }

        private bool FListOf<T>(Type type)
        {
            if (type.IsArray)
                return typeof(T).IsAssignableFrom(type.GetElementType());

            return typeof(IList).IsAssignableFrom(type)
                && type.IsGenericType
                && typeof(T).IsAssignableFrom(type.GetGenericArguments().Single());
        }

        public IEnumerable<AstNode> EnastNodeChildren()
        {
            return EnastNodeChildrenI().Where(node => node != null);

        }

        private IEnumerable<AstNode> EnastNodeChildrenI()
        {
            foreach (var fieldInfo in GetType().GetFields())
            {
                if (fieldInfo == GetType().GetField("NodeParent"))
                    continue;
                
                if (typeof(AstNode).IsAssignableFrom(fieldInfo.FieldType))
                {
                    yield return (AstNode)fieldInfo.GetValue(this);
                }
                else if (FListOf<AstNode>(fieldInfo.FieldType) || 
                         FListOf<IList>(fieldInfo.FieldType) && FListOf<AstNode>(fieldInfo.FieldType.GetGenericArguments().Single()))
                {
                    var rgnode = (IList)fieldInfo.GetValue(this);
                    if (rgnode != null)
                        foreach (AstNode node in rgnode)
                         yield return node;
                }
            }

            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (typeof(AstNode).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    yield return (AstNode)propertyInfo.GetValue(this);
                }
                else if (FListOf<AstNode>(propertyInfo.PropertyType) ||
                         FListOf<IList>(propertyInfo.PropertyType) && FListOf<AstNode>(propertyInfo.PropertyType.GetGenericArguments().Single()))
                {
                    var rgnode = (IList) propertyInfo.GetValue(this);
                    if(rgnode != null)
                        foreach (AstNode node in  rgnode)
                            yield return node;
                }
            }
        }
    }

    public class AstClass : AstNode
    {
        public string StName;
        public AstClassDecl[] RgclassDecl;
        public AstSubroutine[] Rgsubroutine;
    }

    public enum Ksubroutine
    {
        Constructor, MemberFunction, StaticFunction
    }
  
    public class AstSubroutine : AstNode
    {
        public Ksubroutine Ksubroutine;
        public string StName;
        public AstType Type { get; set; }
        public AstVarDecl[] RgParam { get; set; }
        public AstStm[] Body { get; set; }
        public AstVarDecl[] RgVarDeclDecl { get; set; }

        public string FQName()
        {
            return NodeAncestor<AstClass>().StName + "." + StName;
        }
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

    public class AstVarDecl : AstNode
    {
        public bool FLocal;
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
        Minus, Not
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
