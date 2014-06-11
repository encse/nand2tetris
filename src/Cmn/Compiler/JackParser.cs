using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmn.Compiler
{
    public class JackParser
    {
        public NonTerminal Parse(string st)
        {
            var p = Class;

            var rgtoken = new JackTokenizer().Entoken(st).ToList();
            var res = p.Parse(rgtoken);
            if (res.FError)
                throw new Exception("expected " + res.HlmtokenExpected.StJoin(", ", x=> x) + " found " + 
                                    res.Rgtoken.Concat(new Token(Ktoken.Eof, "end of file").EnCons()).First().St);
            
            return (NonTerminal)res.Ennode.Single();
        }

        private class Result
        {
            public readonly IEnumerable<AstNode> Ennode;
            public readonly List<Token> Rgtoken;
            public readonly HashSet<string> HlmtokenExpected;
            public bool FError{get { return Ennode == null; }}

            public Result(AstNode ennode, List<Token> rgtoken, HashSet<string> hlmtokenExpected)
                : this(ennode.EnCons(), rgtoken, hlmtokenExpected)
            {
                
            }
            public Result(IEnumerable<AstNode> ennode, List<Token> rgtoken, HashSet<string> hlmtokenExpected)
            {
                Ennode = ennode;
                Rgtoken = rgtoken;
                HlmtokenExpected = hlmtokenExpected;
            }

            public static Result Error(List<Token> rgtoken, string tokenExpected)
            {
                return new Result((IEnumerable<AstNode>)null, rgtoken, new HashSet<string> { tokenExpected });
            }

            public static Result Error(List<Token> rgtoken, HashSet<string> hlmtokenExpected)
            {
                return new Result((IEnumerable<AstNode>)null, rgtoken, hlmtokenExpected);
            }
        }

        private delegate Result Dgparse(List<Token> rgtoken);

        class Parser
        {
            public Dgparse dg;

            public Parser(Dgparse dg)
            {
                this.dg = dg;
            }

            public Result Parse(List<Token> rgtoken)
            {
                return dg(rgtoken);
            }
        }

        private readonly Symbol Class             = new Symbol(KnonTerminal.Class);
        private readonly Symbol ClassVarDecl      = new Symbol(KnonTerminal.ClassVarDecl);
        private readonly Symbol Type              = new Symbol(KnonTerminal.Type);
        private readonly Symbol SubroutineDecl    = new Symbol(KnonTerminal.SubroutineDecl);
        private readonly Symbol SubroutineBody    = new Symbol(KnonTerminal.SubroutineBody);
        private readonly Symbol Statements        = new Symbol(KnonTerminal.Statements);
        private readonly Symbol Statement         = new Symbol(KnonTerminal.Statement);
        private readonly Symbol IfStatement       = new Symbol(KnonTerminal.IfStatement);
        private readonly Symbol WhileStatement    = new Symbol(KnonTerminal.WhileStatement);
        private readonly Symbol DoStatement       = new Symbol(KnonTerminal.DoStatement);
        private readonly Symbol ReturnStatement   = new Symbol(KnonTerminal.ReturnStatement);
        private readonly Symbol SubroutineCall    = new Symbol(KnonTerminal.SubroutineCall);
        private readonly Symbol ExpressionList    = new Symbol(KnonTerminal.ExpressionList);
        private readonly Symbol Expression        = new Symbol(KnonTerminal.Expression);
        private readonly Symbol Term              = new Symbol(KnonTerminal.Term);
        private readonly Symbol Op                = new Symbol(KnonTerminal.Op);
        private readonly Symbol UnaryOp           = new Symbol(KnonTerminal.UnaryOp);
        private readonly Symbol LetStatement      = new Symbol(KnonTerminal.LetStatement);
        private readonly Symbol VarDecl           = new Symbol(KnonTerminal.VarDecl);
        private readonly Symbol ParameterList     = new Symbol(KnonTerminal.ParameterList);

        class Symbol : Parser
        {
            private KnonTerminal k;

            public Parser Parser
            {
                set
                {
                    dg = rgtoken =>
                    {
                        var res = value.dg(rgtoken);
                        if (res.FError)
                            return res;

                        return new Result(new NonTerminal(k, res.Ennode.ToArray()), res.Rgtoken, res.HlmtokenExpected);
                    };
                }
            }

            public Symbol(KnonTerminal k) : base(null)
            {
                this.k =k;
            }
        }

        public JackParser()
        {
            Class.Parser = Seq(
                Keyword("class"),
                Id(),
                Terminal('{'),
                Rep(ClassVarDecl),
                Rep(SubroutineDecl),
                Terminal('}')
                );

            ClassVarDecl.Parser = Seq(
                Alt(Keyword("static"), Keyword("field")),
                Type,
                Id(),
                Rep(Terminal(','), Id()),
                Terminal(';'));

            Type.Parser = Alt(Keyword("int"), Keyword("char"), Keyword("boolean"), Id());

            SubroutineDecl.Parser = Seq(
                Alt(Keyword("constructor"), Keyword("function"), Keyword("method")),
                Alt(Keyword("void"), Type),
                Id(),
                Terminal('('),
                ParameterList,
                Terminal(')'),
                SubroutineBody);

            SubroutineBody.Parser = Seq(
                Terminal('{'),
                Rep(VarDecl),
                Statements,
                Terminal('}'));

            Statements.Parser = Rep(Statement);

            IfStatement.Parser = Seq(
                Keyword("if"),
                Terminal('('),
                Expression,
                Terminal(')'),
                Terminal('{'),
                Statements,
                Terminal('}'),
                Opt(
                    Keyword("else"),
                    Terminal('{'),
                    Statements,
                    Terminal('}')
                    ));

            WhileStatement.Parser = Seq(
                Keyword("while"),
                Terminal('('),
                Expression,
                Terminal(')'),
                Terminal('{'),
                Statements,
                Terminal('}'));

            DoStatement.Parser = Seq(
                Keyword("do"),
                SubroutineCall,
                Terminal(';'));

            ReturnStatement.Parser = Seq(
                Keyword("return"),
                Opt(Expression),
                Terminal(';'));

            LetStatement.Parser = Seq(
                Keyword("let"),
                Id(),
                Opt(
                    Terminal('['),
                    Expression,
                    Terminal(']')
                    ),
                Terminal('='),
                Expression,
                Terminal(';'));

            Statement.Parser =
                Alt(LetStatement, IfStatement, WhileStatement, DoStatement, ReturnStatement);

            SubroutineCall.Parser =
                Alt(
                    Seq(
                        Id(),
                        Terminal('('),
                        ExpressionList,
                        Terminal(')')
                        ),
                    Seq(
                        Id(),
                        Terminal('.'),
                        Id(),
                        Terminal('('),
                        ExpressionList,
                        Terminal(')')
                        )
                    );

            ExpressionList.Parser = 
                Opt(Expression,Rep(Terminal(','), Expression));

            Expression.Parser =
                Seq(
                    Term,
                    Rep(Op, Term));

            Term.Parser = Alt(
                SubroutineCall,
                Int(),
                String(),
                Keyword(),
                Seq(Id(), Terminal('['), Expression, Terminal(']')),
                Seq(Terminal('('), Expression, Terminal(')')),
                Id(),
                Seq(UnaryOp, Term));

            Op.Parser = Alt(Terminal('+'), Terminal('-'), Terminal('*'), Terminal('/'), Terminal('&'), Terminal('|'), Terminal('<'), Terminal('>'), Terminal('='));

            UnaryOp.Parser = Alt(Terminal('~'), Terminal('-'));

            VarDecl.Parser = Seq(
                Keyword("var"),
                Type,
                Id(),
                Rep(Terminal(','), Id()),
                Terminal(';'));

            ParameterList.Parser = Opt(Type, Id(), Rep(Terminal(','), Type, Id()));
        }
        
        private Parser Alt(params Parser[] rgparser)
        {
            return new Parser(rgtoken =>
            {
                var hmlTokenExpected = new HashSet<string>();

                foreach (var parser in rgparser)
                {
                    var res = parser.Parse(rgtoken);
                    if (!(res.FError))
                        return res;

                    hmlTokenExpected.UnionWith(res.HlmtokenExpected);
                }

                return Result.Error(rgtoken, hmlTokenExpected);
            });
        }

        private Parser Seq(params Parser[] rgparser)
        {
            return new Parser(rgtoken =>
            {
                var rgastNodeCur = new List<AstNode>();

                var hlmTokenNext = new HashSet<string>();
                foreach (var parser in rgparser)
                {
                    var res = parser.Parse(rgtoken);
                    if (res.FError)
                    {
                        hlmTokenNext.UnionWith(res.HlmtokenExpected);
                        return Result.Error(res.Rgtoken, hlmTokenNext);
                    }
                    rgastNodeCur.AddRange(res.Ennode);
                    rgtoken = res.Rgtoken;

                    if (res.Ennode.Any())
                        hlmTokenNext = res.HlmtokenExpected;
                    else
                        hlmTokenNext.UnionWith(res.HlmtokenExpected);
                }

                return new Result(rgastNodeCur, rgtoken, hlmTokenNext);
            });
        }

        private Parser Opt(params Parser[] parser)
        {
            return new Parser(rgtoken =>
            {
                var res = Seq(parser).Parse(rgtoken);
                return res.FError ? new Result(Enumerable.Empty<AstNode>(), rgtoken, res.HlmtokenExpected) : res;
            });
        }

        private Parser Rep(params Parser[] rgparser)
        {
            return new Parser(rgtoken =>
            {
                var rgastNode = new List<AstNode>();
                
                while (true)
                {
                    var rgastNodeCur = new List<AstNode>();
                    var rgtokenStart = rgtoken;

                    foreach (var parser in rgparser)
                    {
                        var res = parser.Parse(rgtoken);
                        if (res.FError)
                            return new Result(rgastNode, rgtokenStart, res.HlmtokenExpected);
                        
                        rgastNodeCur.AddRange(res.Ennode);
                        rgtoken = res.Rgtoken;
                    }

                    rgastNode.AddRange(rgastNodeCur);
                }
            });

        }

        private Parser Id()
        {
            return Accept(token => token.Ktoken == Ktoken.Id, "id");
        }

        private Parser Int()
        {
            return Accept(p => p.Ktoken == Ktoken.Int, "int");
        }

        private Parser String()
        {
            return Accept(p => p.Ktoken == Ktoken.String, "string");
        }

        private Parser Terminal(char ch)
        {
            return Accept(p => p.Ktoken == Ktoken.Symbol && p.Ch == ch, "'"+ch+"'");
        }

        private Parser Keyword(string st = null)
        {
            return Accept(p => p.Ktoken == Ktoken.Keyword && (st == null || p.St == st), st);
        }

        private Parser Accept(Predicate<Token> p, string st)
        {
            return new Parser(rgtoken =>rgtoken.Any() && 
                                        p(rgtoken.First()) ? new Result(rgtoken.First(), rgtoken.Skip(1).ToList()/*xxx*/, new HashSet<string>()) : 
                Result.Error(rgtoken, st));
        }
    }
}