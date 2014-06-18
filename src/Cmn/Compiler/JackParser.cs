using System.Linq;
using System.Collections.Generic;
using System.Threading;

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
                return itoken < rgtoken.Count ? rgtoken[itoken] : Token.Eof; 
            }
        }

        private void NextToken()
        {
            itoken++;
        }

        private bool FCurrent(Ktoken ktoken)
        {
            return tokenCur.Ktoken == ktoken;
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
            try
            {
                var lexer = new JackLexer();
                rgtoken = lexer.Entoken(st).ToList();
                itoken = 0;
                return Fill(null, ParseClass());
            }
            catch (Erparse er)
            {
                er.StSrc = st;
                throw;
            }
        }

        private AstNode Fill(AstNode nodeParent, AstNode astNode)
        {
            astNode.NodeParent = nodeParent;
            foreach (var astNodeChild in astNode.EnastNodeChildren())
                Fill(astNode, astNodeChild);
            return astNode;
        }

        private AstClass ParseClass()
        {
            var _class = new AstClass();
            Expect(Ktoken.Class);

            _class.StName = Expect(Ktoken.Id).St;

            Expect(Ktoken.Lbrace);

            _class.RgclassDecl = ParseRgClassDecl().ToArray();
            _class.Rgsubroutine = ParseRgSubroutine().ToArray();
            
            Expect(Ktoken.Rbrace);
            Expect(Ktoken.Eof);
            return _class;
        }

        private IEnumerable<AstSubroutine> ParseRgSubroutine()
        {
            while (FCurrent(Ktoken.Constructor) || FCurrent(Ktoken.Function) || FCurrent(Ktoken.Method))
            {
                var subroutine = new AstSubroutine();

                subroutine.Ksubroutine = 
                    FCurrent(Ktoken.Constructor) ? Ksubroutine.Constructor :
                    FCurrent(Ktoken.Function) ? Ksubroutine.StaticFunction : 
                                                Ksubroutine.MemberFunction;
                
                NextToken();

                subroutine.Type = Accept(Ktoken.Void) ? new AstType {Ktype = Ktype.Void, stType = "void"} : ParseType();

                subroutine.StName = Expect(Ktoken.Id).St;
                subroutine.RgParam = ParseParamDefList().ToArray();

                Expect(Ktoken.Lbrace);
                subroutine.RgVarDeclDecl = ParseVarDeclList().ToArray();
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
                else
                    Error("Expected statement, found " + tokenCur.Ktoken);
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
            
            var astResult = ParseTerm();

            while (true)
            {
                if (Accept(Ktoken.Plus)) astResult = new AstBinOp { KBinop = KBinop.Plus, exprLeft = astResult, exprRight = ParseTerm() };
                else if (Accept(Ktoken.Minus)) astResult = new AstBinOp { KBinop = KBinop.Minus, exprLeft = astResult, exprRight = ParseTerm() };
                else if (Accept(Ktoken.Asterix)) astResult = new AstBinOp { KBinop = KBinop.Mul, exprLeft = astResult, exprRight = ParseTerm() };
                else if (Accept(Ktoken.Slash)) astResult = new AstBinOp { KBinop = KBinop.Div, exprLeft = astResult, exprRight = ParseTerm() };
                else if (Accept(Ktoken.Or)) astResult = new AstBinOp { KBinop = KBinop.Or, exprLeft = astResult, exprRight = ParseTerm() };
                else if (Accept(Ktoken.And)) astResult = new AstBinOp { KBinop = KBinop.And, exprLeft = astResult, exprRight = ParseTerm() };
                else if (Accept(Ktoken.Eq)) astResult = new AstBinOp { KBinop = KBinop.Eq, exprLeft = astResult, exprRight = ParseTerm() };
                else if (Accept(Ktoken.Gt)) astResult = new AstBinOp { KBinop = KBinop.Gt, exprLeft = astResult, exprRight = ParseTerm() };
                else if (Accept(Ktoken.Lt)) astResult = new AstBinOp { KBinop = KBinop.Lt, exprLeft = astResult, exprRight = ParseTerm() };
                else
                    break;
            }
            return astResult;
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
            
            if (Accept(Ktoken.Tilde)) return new AstUnOp {Kunop = KUnop.Not, expr = ParseTerm()};

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

        private IEnumerable<AstVarDecl> ParseParamDefList()
        {
            Expect(Ktoken.Lparen);

            if (Accept(Ktoken.Rparen))
                yield break;
            
            while (true)
            {
                var type = ParseType();
                var stName = Expect(Ktoken.Id).St;
                yield return new AstVarDecl
                {
                    Type = type,
                    StName = stName,
                    FLocal = false
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

        
        private IEnumerable<AstVarDecl> ParseVarDeclList()
        {
            while (Accept(Ktoken.Var))
            {
                var type = ParseType();
                do
                {
                    var stName = Expect(Ktoken.Id).St;
                    yield return new AstVarDecl { Type = type, StName = stName, FLocal = true};
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
}