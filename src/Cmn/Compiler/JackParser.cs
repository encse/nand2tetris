using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Proxies;

namespace Cmn.Compiler
{
    public class JackParser
    {
        private List<Token> rgtoken;
        private int itoken;

        private Token tokenCur
        {
            get
            {
                return itoken < rgtoken.Count ? rgtoken[itoken] : new Token(Ktoken.Eof, null, -1, -1);
            }
        }

        private void NextToken()
        {
            itoken++;
        }

        private bool FCurrent(Ktoken ktoken)
        {
            return rgtoken[itoken].Ktoken == ktoken;
        }

        private bool Accept(Ktoken ktoken)
        {
            if (FCurrent(ktoken))
            {
                NextToken();
                return true;
            }
            return false;
        }

        private void Expected(params Ktoken[] ktokens)
        {
            throw new Erparse(tokenCur.Iline, tokenCur.Icol, "Expecting " + ktokens.StJoin(", ", x => x.ToString()) + " found " + tokenCur);
        }

        private void Error(string s)
        {
            throw new Erparse(tokenCur.Iline, tokenCur.Icol, s);
        }
        private Token Expect(params Ktoken[] rgktoken)
        {
            if (rgktoken.All(ktoken => !FCurrent(ktoken)))
                Expected(rgktoken);

            var token = tokenCur;
            NextToken();
            return token;
        }

        public AstNode Parse(string st)
        {
            var lexer = new JackLexer();
            rgtoken = lexer.Entoken(st).ToList();
            itoken = 0;
            return ParseClass();
        }

        private AstClass ParseClass()
        {
            var _class = new AstClass();
            Expect(Ktoken.Class);

            _class.StName = Expect(Ktoken.Id).St;

            Expect(Ktoken.Lbrace);

            _class.rgclassDecl = ParseRgClassDecl().ToArray();
            _class.rgsubroutine = ParseRgSubroutine().ToArray();
            
            Expect(Ktoken.Rbrace);

            return _class;
        }

        private IEnumerable<AstSubroutine> ParseRgSubroutine()
        {
            while (FCurrent(Ktoken.Constructor) || FCurrent(Ktoken.Function) || FCurrent(Ktoken.Method))
            {
                var subroutine = new AstSubroutine();

                subroutine.Ksubroutine = 
                    FCurrent(Ktoken.Constructor) ? Ksubroutine.Constructor :
                    FCurrent(Ktoken.Function) ? Ksubroutine.Function : 
                                                Ksubroutine.Method;
                
                NextToken();

                subroutine.Type = Accept(Ktoken.Void) ? new AstType {Ktype = Ktype.Void, stType = "void"} : ParseType();

                subroutine.StName = Expect(Ktoken.Id).St;
                subroutine.RgParam = ParseParamDefList().ToArray();

                Expect(Ktoken.Lbrace);
                subroutine.RgVarDecl = ParseVarDeclList().ToArray();
                subroutine.Body = ParseStatementList().ToArray();
                Expect(Ktoken.Rbrace);

                yield return subroutine;
            }
        }


        private IEnumerable<AstStm> ParseStatementList()
        {
            while (!FCurrent(Ktoken.Rbrace))
            {
                if (FCurrent(Ktoken.Let))
                    yield return ParseLet();
                else if (FCurrent(Ktoken.Do))
                    yield return ParseDo();
                else if (FCurrent(Ktoken.While))
                    yield return ParseWhile();
                else if (FCurrent(Ktoken.If))
                    yield return ParseIf();
                else if (FCurrent(Ktoken.Return))
                    yield return ParseReturn();
            }
        }

        private AstStm ParseWhile()
        {
            Expect(Ktoken.While);
            Expect(Ktoken.Lparen);
            var exprCond = ParseExpr();
            Expect(Ktoken.Rparen); 
            Expect(Ktoken.Lbrace);
            var rgstm = ParseStatementList().ToArray();
            Expect(Ktoken.Rbrace);

            return new AstWhile {exprCond = exprCond, rgstm = rgstm};
        }

        private AstStm ParseIf()
        {
            Expect(Ktoken.If);
            Expect(Ktoken.Lparen);
            var exprCond = ParseExpr();
            Expect(Ktoken.Rparen);
            Expect(Ktoken.Lbrace);
            var rgstm = ParseStatementList().ToArray();
            Expect(Ktoken.Rbrace);

            AstStm[] rgstmElse = null;

            if (Accept(Ktoken.Else))
            {
                Expect(Ktoken.Lbrace);
                rgstmElse = ParseStatementList().ToArray();
                Expect(Ktoken.Rbrace);
            }

            return new AstIf { exprCond = exprCond, rgstm = rgstm.ToArray(), rgstmElse = rgstmElse};
        }
        private AstLet ParseLet()
        {
            Expect(Ktoken.Let);
            var exprLeft = ParseRef();
            Expect(Ktoken.Eq);
            var exprRight = ParseExpr();
            Expect(Ktoken.Semicolon);

            return new AstLet
            {
                exprLeft = exprLeft,
                exprRight = exprRight,
            };
        }

        private AstDo ParseDo()
        {
            Expect(Ktoken.Do);
            var expCall = ParseCall();
            Expect(Ktoken.Semicolon);

            return new AstDo { exprCall = expCall };
        }

        private AstCall ParseCall()
        {
            var astExpr = ParseExpr();
            if (!(astExpr is AstCall))
                Error("Expected function call found "+astExpr);

            return (AstCall)astExpr;
        }

        

        private AstReturn ParseReturn()
        {
            Expect(Ktoken.Return);
            if (Accept(Ktoken.Semicolon))
                return new AstReturn();

            var expr = ParseExpr();
            Expect(Ktoken.Semicolon);

            return new AstReturn { expr = expr};
        }

        private AstExpr ParseExpr()
        {
            var term = ParseTerm();

            if (Accept(Ktoken.Plus)) return new AstBinOp {KBinop = KBinop.Plus, exprLeft = term, exprRight = ParseExpr()};
            if (Accept(Ktoken.Minus)) return new AstBinOp {KBinop = KBinop.Minus, exprLeft = term, exprRight = ParseExpr()};
            if (Accept(Ktoken.Asterix)) return new AstBinOp {KBinop = KBinop.Mul, exprLeft = term, exprRight = ParseExpr()};
            if (Accept(Ktoken.Slash)) return new AstBinOp {KBinop = KBinop.Div, exprLeft = term, exprRight = ParseExpr()};
            if (Accept(Ktoken.Or)) return new AstBinOp {KBinop = KBinop.Or, exprLeft = term, exprRight = ParseExpr()};
            if (Accept(Ktoken.And)) return new AstBinOp {KBinop = KBinop.And, exprLeft = term, exprRight = ParseExpr()};
            if (Accept(Ktoken.Eq)) return new AstBinOp {KBinop = KBinop.Eq, exprLeft = term, exprRight = ParseExpr()};
            if (Accept(Ktoken.Gt)) return new AstBinOp {KBinop = KBinop.Gt, exprLeft = term, exprRight = ParseExpr()};
            if (Accept(Ktoken.Lt)) return new AstBinOp {KBinop = KBinop.Lt, exprLeft = term, exprRight = ParseExpr()};
            
            return term;
        }

        private AstExpr ParseTerm()
        {
            if (Accept(Ktoken.True)) return new AstBoolLit {f = true};
            
            if (Accept(Ktoken.False)) return new AstBoolLit {f = false};
            
            if (Accept(Ktoken.This)) return new AstThis();
            
            if (Accept(Ktoken.Null)) return new AstNull();
            
            if (FCurrent(Ktoken.IntLit)) return new AstIntLit {i = Expect(Ktoken.IntLit).I};
            
            if (FCurrent(Ktoken.StringLit)) return new AstStringLit {st = Expect(Ktoken.StringLit).St};

            if (Accept(Ktoken.Minus)) return new AstUnOp {Kunop = KUnop.Minus, expr = ParseTerm()};
            
            if (Accept(Ktoken.Tilde)) return new AstUnOp {Kunop = KUnop.Minus, expr = ParseTerm()};

            if (Accept(Ktoken.Lparen))
            {
                var expr = ParseExpr();
                Expect(Ktoken.Rparen);
                return expr;
            }
            return ParseRef();
        }

        private AstExpr ParseRef()
        {
            AstExpr varRef = new AstVarRef {StName = Expect(Ktoken.Id).St};

            if (Accept(Ktoken.Lbracket))
            {
                var astIndex = new AstIndex { exprLeft = varRef, exprRight = ParseExpr() };
                Expect(Ktoken.Rbracket);
                return astIndex;
            }

            if (Accept(Ktoken.Dot))
            {
                var varRefRight = new AstVarRef { StName = Expect(Ktoken.Id).St };
                varRef = new AstDot { varLeft = (AstVarRef)varRef, varRight = varRefRight };
            }
          
            if (FCurrent(Ktoken.Lparen))
                return new AstCall { exprFunc = varRef, rgexprParam = ParseArgumentList().ToArray() };

            return varRef;
        }

        private IEnumerable<AstExpr> ParseArgumentList()
        {
            Expect(Ktoken.Lparen);

            if (Accept(Ktoken.Rparen))
                yield break;

            while (true)
            {
                yield return ParseExpr();

                if (Accept(Ktoken.Comma))
                    continue;

                Expect(Ktoken.Rparen);
                break;
            }
        }

        private IEnumerable<AstVar> ParseParamDefList()
        {
            Expect(Ktoken.Lparen);

            if (Accept(Ktoken.Rparen))
                yield break;
            
            while (true)
            {
                var type = ParseType();
                var stName = Expect(Ktoken.Id).St;
                yield return new AstVar
                {
                    Type = type,
                    StName = stName
                };

                if (Accept(Ktoken.Comma))
                    continue;

                Expect(Ktoken.Rparen);
                break;
            }
        }

        private IEnumerable<AstClassDecl> ParseRgClassDecl()
        {
            while (FCurrent(Ktoken.Field) || FCurrent(Ktoken.Static))
            {
                var kclassDecl = FCurrent(Ktoken.Field) ? KClassDecl.Field : KClassDecl.Static;
                NextToken();
                var type = ParseType();
                do
                {
                    var stName = Expect(Ktoken.Id).St;
                    yield return new AstClassDecl {KClassDecl = kclassDecl, Type = type, StName = stName};
                } while (Accept(Ktoken.Comma));

                Expect(Ktoken.Semicolon);
            }
        }

        
        private IEnumerable<AstVar> ParseVarDeclList()
        {
            while (Accept(Ktoken.Var))
            {
                var type = ParseType();
                do
                {
                    var stName = Expect(Ktoken.Id).St;
                    yield return new AstVar { Type = type, StName = stName };
                } while (Accept(Ktoken.Comma));

                Expect(Ktoken.Semicolon);
            }
        }

        private AstType ParseType()
        {
            AstType type = null;
            switch (tokenCur.Ktoken)
            {
                case Ktoken.Int: type = new AstType {Ktype = Ktype.Int, stType = tokenCur.St}; break;
                case Ktoken.Bool: type = new AstType { Ktype = Ktype.Bool, stType = tokenCur.St }; break;
                case Ktoken.Char: type = new AstType { Ktype = Ktype.Char, stType = tokenCur.St }; break;
                case Ktoken.Id: type = new AstType { Ktype = Ktype.Class, stType = tokenCur.St }; break;
                default: Expected(Ktoken.Int, Ktoken.Bool, Ktoken.Char, Ktoken.Id); break;
            }
            NextToken();
            return type;
        }

    }

    

    public class AstNode
    {
        public AstNode[] Children;
    }

    public class AstClass : AstNode
    {
        public string StName;
        public AstClassDecl[] rgclassDecl;
        public AstSubroutine[] rgsubroutine;
        
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

    public enum KClassDecl { Field, Static}
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
        Plus, Div, Mul,And,Or,Lt,Gt, Eq,
        Minus
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
    public class AstNull: AstExpr { }

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