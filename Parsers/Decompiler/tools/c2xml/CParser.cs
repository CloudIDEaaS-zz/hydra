﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Tools.C2Xml
{
    // http://www.ssw.uni-linz.ac.at/Coco/C/C.atg

    public class CParser
    {
        /* ANSI C 89 grammar as specified in http://flash-gordon.me.uk/ansi.c.txt

	    Processing the C grammar requires LL(1) conflict resolvers, some of which
	    need to check whether an identifier is a type name (see IsTypeName() below).
	    So the grammar assumes that there is a symbol table, from where you can
	    look up an identifier and find out whether it is a type name.
	
	    The language in the semantic actions here is C#, but it can easily be
	    translated to any other language.
        */

        private LookAheadLexer lexer;
        private CGrammar grammar;

        public CParser(ParserState parserState, CLexer lexer)
        {
            this.ParserState = parserState;
            this.lexer = new LookAheadLexer(
                new CDirectiveLexer(parserState, lexer));
            this.grammar = new CGrammar();
        }

        public ParserState ParserState { get; private set; }

        //------------------ token sets ------------------------------------

        static BitArray startOfTypeName = NewBitArray(
            CTokenType.Const, CTokenType.Volatile, CTokenType.Void, CTokenType.Wchar_t,
            CTokenType.Char, CTokenType.Short, CTokenType.Int, CTokenType.__Int64, 
            CTokenType.Long, CTokenType.Double,CTokenType.Float, CTokenType.Signed, 
            CTokenType.Unsigned, CTokenType.Struct,
            CTokenType.Union, CTokenType.Enum,
            CTokenType.__Stdcall);
        static BitArray startOfDecl = NewBitArray(
            CTokenType.Typedef, CTokenType.Extern, CTokenType.Static, CTokenType.Auto,
            CTokenType.Register, CTokenType.Const, CTokenType.Volatile, CTokenType.Void,
            CTokenType.Char, CTokenType.Wchar_t, CTokenType.Short, CTokenType.Int, 
            CTokenType.__Int64, CTokenType.Long,CTokenType.Double, CTokenType.Float,
            CTokenType.Signed, CTokenType.Unsigned,CTokenType.Struct, CTokenType.Union,
            CTokenType.Enum);
        static BitArray startOfDeclarator = NewBitArray(
            CTokenType.Star, CTokenType.LParen, CTokenType.LBracket, CTokenType.Semicolon);


        private static BitArray NewBitArray(params CTokenType[] val)
        {
            BitArray s = new BitArray(128);
            foreach (int x in val) s[x] = true;
            return s;
        }

        //---------- LL(1) conflict resolvers ------------------------------

        private CTokenType PeekTokenType()
        {
            return lexer.Peek(0).Type;
        }

        private bool PeekThenDiscard(CTokenType type)
        {
            if (lexer.Peek(0).Type != type)
                return false;
            lexer.Read();
            return true;
        }

        private object ExpectToken(CTokenType token)
        {
            var t = lexer.Read();
            if (t.Type != token)
                throw Unexpected(token, t.Type);
            return t.Value;
        }

        private Exception Unexpected(CTokenType expected, CTokenType actual)
        {
            throw new FormatException(string.Format("Expected token '{0}' but saw '{1}' on line {2}.", expected, actual, lexer.LineNumber));
        }

        public bool IsTypeName(CToken x)
        {
            if (x.Type != CTokenType.Id)
                return false;
            return ParserState.Typedefs.Contains((string)x.Value);
        }

        public bool IsTypeName(string id)
        {
            return ParserState.Typedefs.Contains(id);
        }

        /// <summary>
        /// Return true if the next token is a type name
        /// </summary>
        /// <returns></returns>
        bool IsType0()
        {
            var token = lexer.Peek(0);
            if (startOfTypeName[(int) token.Type])
                return true;
            return IsTypeName(lexer.Peek(0));
        }

        
        /// <summary>
        /// return true if "(" TypeName
        /// </summary>
        /// <returns></returns>
        bool IsType1()
        {
            if (lexer.Peek(0).Type != CTokenType.LParen) return false;
            CToken x = lexer.Peek(1);
            if (startOfTypeName[(int)x.Type]) return true;
            return IsTypeName(x);
        }

        /// <summary>
        /// return true if not "," "}"
        /// </summary>
        /// <returns></returns>
        bool IsContinued()
        {
            if (lexer.Peek(0).Type == CTokenType.Comma)
            {
                CToken x = lexer.Peek(1);
                if (x.Type == CTokenType.RBrace) return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if ",", which is not followed by "..."
        /// </summary>
        /// <returns></returns>
        bool IsContinued1()
        { 
            if (lexer.Peek(0).Type == CTokenType.Comma)
            {
                CToken x = lexer.Peek(1);
                if (x.Type != CTokenType.Ellipsis)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if ident ":" | "case" | "default"
        /// </summary>
        /// <returns></returns>
        bool IsLabel()
        {
            var type = lexer.Peek(0).Type;
            if (type == CTokenType.Id)
            {
                CToken x = lexer.Peek(1);
                if (x.Type == CTokenType.Colon)
                    return true;
            }
            else if (type == CTokenType.Case ||
                     type == CTokenType.Default)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if followed by Decl
        /// </summary>
        /// <returns></returns>
        bool IsDecl()
        {
            if (startOfDecl[(int)lexer.Peek(0).Type])
                return true;
            return IsTypeName(lexer.Peek(0));
        }

        /// <summary>
        /// Return true if there is no non-type-ident after '*', '(', "const", "volatile"
        /// </summary>
        /// <returns></returns>
        bool IsAbstractDecl()
        {
            int i = 0;
            CToken x = lexer.Peek(i);
            while (x.Type == CTokenType.Star || x.Type == CTokenType.LParen || x.Type == CTokenType.Const || 
                   x.Type == CTokenType.Volatile || x.Type == CTokenType.__Ptr64 ||
                   x.Type == CTokenType.__Fastcall || x.Type == CTokenType.__Stdcall || 
                   x.Type == CTokenType.__Cdecl)
                x = lexer.Peek(++i);
            if (x.Type != CTokenType.Id)
                return true;
            if (!IsTypeName(x))
                return false;
            x = lexer.Peek(++i);
            return x.Type != CTokenType.RParen &&
                x.Type != CTokenType.Comma;
        }

        // return true if '*', '(', '[', ';', noTypeIdent
        bool IsDeclarator()
        {
            var token = lexer.Peek(0);
            if (startOfDeclarator[(int)token.Type])
                return true;
            if (token.Type != CTokenType.Id) 
                return false;
            return !IsTypeName(token);
        }

#if not
CHARACTERS
	letter     = 'A'..'Z' + 'a'..'z' + '_'.
	oct        = '0'..'7'.
	digit      = '0'..'9'.
	nzdigit    = '1'..'9'.
	hex        = digit + 'a'..'f' + 'A'..'F'.
	notQuote   = ANY - '"' - "\r\n".
	notApo     = ANY - '\'' - "\r\n".
	
	tab        = '\t'.
	cr         = '\r'.
	lf         = '\n'.
	newLine    = cr + lf.
	notNewLine = ANY - newLine .
	ws         = " " + tab + '\u000b' + '\u000c'.
	
	
TOKENS
	ident    = letter {letter | digit}.
	
	floatcon = ( '.' digit {digit} [('e'|'E')  ['+'|'-'] digit {digit}]
						 | digit {digit} '.' {digit} [('e'|'E')  ['+'|'-'] digit {digit}]
						 | digit {digit} ('e'|'E')  ['+'|'-'] digit {digit}
						 )
						 ['f'|'l'|'F'|'L'].

	intcon   = ( nzdigit {digit}
						 | '0' {oct}
						 | ("0x"|"0X") hex {hex}
						 )
						 {'u'|'U'|'l'|'L'}.
	
	string   = '"' {notQuote} '"'.        // no check for valid escape sequences
	
	charcon  = '\'' notApo {notApo} '\''. // no check for valid escape sequences

	// tokens defined in order to get their names for LL(1) conflict resolvers
	auto      = "auto".
	case      = "case".
	char      = "char".
	const     = "const".
	default   = "default".
	double    = "double".
	enum      = "enum".
	extern    = "extern".
	float     = "float".
	int       = "int".
	long      = "long".
	register  = "register".
	short     = "short".
	signed    = "signed".
	static    = "static".
	struct    = "struct".
	typedef   = "typedef".
	union     = "union".
	unsigned  = "unsigned".
	void      = "void".
	volatile  = "volatile".
	comma     = ','.
	semicolon = ';'.
	colon     = ':'.
	star      = '*'.
	lpar      = '('.
	rpar      = ')'.
	lbrack    = '['.
	rbrace    = '}'.
	ellipsis  = "...".


PRAGMAS
	//---- preprocessor commands (not handled here)
	ppDefine  = '#' {ws} "define" {notNewLine} newLine.
	ppUndef   = '#' {ws} "undef" {notNewLine} newLine.
	ppIf      = '#' {ws} "if" {notNewLine} newLine.
	ppElif    = '#' {ws} "elif" {notNewLine} newLine.
	ppElse    = '#' {ws} "else" {notNewLine} newLine.
	ppEndif   = '#' {ws} "endif" {notNewLine} newLine.
	ppInclude = '#' {ws} "include" {notNewLine} newLine.

COMMENTS FROM "/*" TO "*/"
COMMENTS FROM "//" TO lf

IGNORE tab + cr + lf
#endif

        //---------- Compilation Unit ----------

        //CompilationUnit = 
        //    ExternalDecl {ExternalDecl}.

        public List<Decl> Parse()
        {
            var list = new List<Decl>();
            var decl = Parse_ExternalDecl();
            if (decl == null)
                return list;
            list.Add(decl);
            while (PeekTokenType() != CTokenType.EOF)
            {
                list.Add(Parse_ExternalDecl());
            }
            return list;
        }

        //ExternalDecl = 
        //  DeclSpecifierList 
        //  ( Declarator 
        //    ( {Decl} '{' {IF(IsDecl()) Decl | Stat} '}'   // FunctionDef
        //    | ['=' Initializer] {',' InitDeclarator}  ';' // Decl
        //    )
        //  | ';'                                           // Decl
        //  ).

        public Decl Parse_ExternalDecl()
        {
            var decl_spec_list = Parse_DeclSpecifierList();
            if (decl_spec_list == null)
                return null;
            var inits = new List<InitDeclarator>();
            if (PeekThenDiscard(CTokenType.Semicolon))
                return grammar.Decl(decl_spec_list, inits);
            var declarator = Parse_Declarator();
            var token = PeekTokenType();
            if (token == CTokenType.Assign ||
                token == CTokenType.Comma ||
                token == CTokenType.Semicolon)
            {
                // Declaration.
                Initializer init = null;
                if (PeekThenDiscard(CTokenType.Assign))
                {
                    init = Parse_Initializer();
                }
                inits.Add(grammar.InitDeclarator(declarator, init));
                while (PeekThenDiscard(CTokenType.Comma))
                {
                    inits.Add(Parse_InitDeclarator());
                }
                ExpectToken(CTokenType.Semicolon);
                var decl = grammar.Decl(decl_spec_list, inits);
                UpdateNamespaceWithTypedefs(decl_spec_list, inits);
                return decl;
            }
            else if (token != CTokenType.EOF)
            {
                // Function definition
                while (lexer.Peek(0).Type != CTokenType.LBrace)
                {
                    // Old-style C definition.
                    Parse_Decl();
                }
                ExpectToken(CTokenType.LBrace);
                var statements = new List<Stat>();
                while (!PeekThenDiscard(CTokenType.RBrace))
                {
                    if (IsDecl())
                    {
                        statements.Add(grammar.DeclStat(Parse_Decl()));
                    }
                    else
                    {
                        statements.Add(Parse_Stat());
                    }
                }
                return grammar.FunctionDefinition(decl_spec_list, declarator, statements);
            }
            else
                throw new FormatException("Expected ';'");
        }

        private void UpdateNamespaceWithTypedefs(List<DeclSpec> declspecs, List<InitDeclarator> declarators)
        {
            if (declspecs.Count == 0)
               return;
            var typedefSpec = declspecs[0] as StorageClassSpec;
            if (typedefSpec == null || typedefSpec.Type != CTokenType.Typedef)
                return;

            foreach (var declarator in declarators)
            {
                if (declarator.Init != null)
                    throw new FormatException("typedefs can't be initialized.");
                var name = NameExtractor.GetName(declspecs.Skip(1), declarator.Declarator, ParserState);
                ParserState.Typedefs.Add(name);
            }
        }

        //---------- Declarations ----------

        //Decl = DeclSpecifierList [InitDeclarator {',' InitDeclarator}] ';'.
        public Decl Parse_Decl()
        {
            var declSpecifiers = Parse_DeclSpecifierList();
            if (declSpecifiers == null)
                return null;
            var listDecls = new List<InitDeclarator>();
            if (!PeekThenDiscard(CTokenType.Semicolon))
            {
                listDecls.Add(Parse_InitDeclarator());
                while (PeekThenDiscard(CTokenType.Comma))
                {
                    listDecls.Add(Parse_InitDeclarator());
                }
                ExpectToken(CTokenType.Semicolon);
            }
            //UpdateNamespaceWithTypedefs(decl_spec_list, inits);
            return grammar.Decl(declSpecifiers, listDecls);
        }

        
        /// <summary>
        /// Parses a (possibly initialized) declarator.
        /// </summary>
        /// <remarks>
        /// InitDeclarator = Declarator ['=' Initializer].
        /// </remarks>
        /// <returns></returns>
        private InitDeclarator Parse_InitDeclarator()
        {
            var decl = Parse_Declarator();
            Initializer init = null;
            if (PeekThenDiscard(CTokenType.Assign))
            {
                init = Parse_Initializer();
            }
            return grammar.InitDeclarator(decl, init);
        }

        //DeclSpecifierList = DeclSpecifier {IF(!IsDeclarator()) DeclSpecifier}.

        private List<DeclSpec> Parse_DeclSpecifierList()
        {
            int idsSeen = 0;
            int typeDeclsSeen = 0;
            var list = new List<DeclSpec>();
            var ds = Parse_DeclSpecifier();
            if (ds == null)
                return null;
            list.Add(ds);
            bool inTypeDef = (ds is StorageClassSpec && ((StorageClassSpec) ds).Type == CTokenType.Typedef);
                
            while (!IsComplexType(ds))
            {
                var token = lexer.Peek(0);
                if (inTypeDef)
                {
                    if (startOfDeclarator[(int) token.Type])
                        break;
                    if (token.Type == CTokenType.Id && (!IsTypeName(token) || typeDeclsSeen > 0))
                        break;
                }
                else
                {
                    if (IsDeclarator())
                        break;
                }
                if (token.Type == CTokenType.Id)
                    ++idsSeen;
                ds = Parse_DeclSpecifier();
                if (ds == null)
                    break;
                if (ds is TypeSpec)
                    ++typeDeclsSeen;
                list.Add(ds);
            }
            return list;
        }

        private bool IsComplexType(DeclSpec ds)
        {
            return ds is ComplexTypeSpec;
        }

        private bool IsDataType(DeclSpec ds)
        {
            return ds is TypeSpec;
        }
        //DeclSpecifier =
        //        "typedef" | "extern" | "static" | "auto" | "register" // storage class specifier
        //    | "const" | "volatile"                                  // TypeQualifier
        //    | TypeSpecifier.
        private DeclSpec Parse_DeclSpecifier()
        {
            var token = PeekTokenType();
            switch (token)
            {
            case CTokenType.Typedef:
            case CTokenType.Extern:
            case CTokenType.Static:
            case CTokenType.Auto:
            case CTokenType.Register:
            case CTokenType.__Cdecl:
            case CTokenType.__Inline:
            case CTokenType.__Stdcall:
                return grammar.StorageClass( lexer.Read().Type);
            case CTokenType.Const:
            case CTokenType.Volatile:
            case CTokenType.__Ptr64:
                return grammar.TypeQualifier(lexer.Read().Type);
            case CTokenType.__Declspec:
                lexer.Read();
                ExpectToken(CTokenType.LParen);
                var s = (string)ExpectToken(CTokenType.Id);
                if (s == "align")
                {
                    ExpectToken(CTokenType.LParen);
                    ExpectToken(CTokenType.NumericLiteral);
                    ExpectToken(CTokenType.RParen);
                }
                else if (s == "deprecated")
                {
                    if (PeekThenDiscard(CTokenType.LParen))
                    {
                        ExpectToken(CTokenType.StringLiteral);
                        ExpectToken(CTokenType.RParen);
                    }
                }
                else if (s == "dllimport")
                {
                }
                else if (s == "noreturn")            //$BUG: use for termination analysis
                {
                }
                else if (s == "noalias")
                {
                }
                else if (s == "restrict")
                {
                }
                else
                    throw new FormatException(string.Format("Unknown __declspec '{0}'.", s));
                ExpectToken(CTokenType.RParen);
                return grammar.ExtendedDeclspec(s);
            case CTokenType.__Success:
                lexer.Read();
                ExpectToken(CTokenType.LParen);
                ExpectToken(CTokenType.Return);
                lexer.Read();   //   >= 
                lexer.Read();   // 0
                ExpectToken(CTokenType.RParen);
                return Parse_DeclSpecifier();
            default:
                return Parse_TypeSpecifier();
            }
        }

        // TypeSpecifier =
        //      "void" | "char" | "short" | "int" | "long" | "float" | "double" | "signed" | "unsigned"
        //  | ident // type name
        //| ("struct" | "union")
        //  ( ident ['{' StructDecl {StructDecl} '}']
        //  | '{' StructDecl {StructDecl} '}'
        //  )
        //| "enum"
        //  ( ident ['{' Enumerator {',' Enumerator} '}']
        //  | '{' Enumerator {',' Enumerator} '}'
        //  ).

        // struct __declspec(align(16)) X 
        // 
        private TypeSpec Parse_TypeSpecifier()
        {
            var token = PeekTokenType();
            switch (token)
            {
            case CTokenType.Void:
            case CTokenType.Char:
            case CTokenType.Short:
            case CTokenType.Int:
            case CTokenType.__Int64:
            case CTokenType.Long:
            case CTokenType.Float:
            case CTokenType.Double:
            case CTokenType.Signed:
            case CTokenType.Unsigned:
            case CTokenType.Wchar_t:
            case CTokenType.__W64:
                return grammar.SimpleType(lexer.Read().Type);
            case CTokenType.Id: // type name
                return grammar.TypeName((string) lexer.Read().Value);
            case CTokenType.Struct:
            case CTokenType.Union:
                lexer.Read();
                int alignment = 0;
                string tag = null;
                List<StructDecl> decls = null;
                if (PeekThenDiscard(CTokenType.__Declspec))
                {
                    ExpectToken(CTokenType.LParen);
                    var s= (string)ExpectToken(CTokenType.Id);
                    if (s == "align")
                    {
                        ExpectToken(CTokenType.LParen);
                        alignment = (int) ExpectToken(CTokenType.NumericLiteral);
                        ExpectToken(CTokenType.RParen);
                    }
                    else
                        throw new FormatException("Expected __declspec(align(nn)).");

                    ExpectToken(CTokenType.RParen);
                }
                if (PeekTokenType() == CTokenType.Id)
                {
                    tag = (string) lexer.Read().Value;
                    if (PeekThenDiscard(CTokenType.LBrace))
                    {
                        decls = new List<StructDecl>();
                        while (PeekTokenType() != CTokenType.RBrace) 
                        {
                            decls.Add(Parse_StructDecl());
                        }
                        ExpectToken(CTokenType.RBrace);
                    }
                }
                else
                {
                    ExpectToken(CTokenType.LBrace);
                    decls = new List<StructDecl>();
                    do
                    {
                        decls.Add(Parse_StructDecl());
                    } while (PeekTokenType() != CTokenType.RBrace);
                    ExpectToken(CTokenType.RBrace);
                }
                return grammar.ComplexType(token, alignment, tag, decls);
            case CTokenType.Enum:
                lexer.Read();
                List<Enumerator> enums = null;
                if (PeekTokenType() == CTokenType.Id)
                {
                    tag = (string) lexer.Read().Value;
                    if (PeekThenDiscard(CTokenType.LBrace))
                    {
                        enums = new List<Enumerator>();
                        do
                        {
                            enums.Add(Parse_Enumerator());
                            PeekThenDiscard(CTokenType.Comma);
                        } while (PeekTokenType() != CTokenType.RBrace);
                        ExpectToken(CTokenType.RBrace);
                    }
                }
                else
                {
                    tag = null;
                    ExpectToken(CTokenType.LBrace);
                    enums = new List<Enumerator>();
                    do
                    {
                        enums.Add(Parse_Enumerator());
                        PeekThenDiscard(CTokenType.Comma);
                    } while (lexer.Peek(0).Type != CTokenType.RBrace);
                    ExpectToken(CTokenType.RBrace);
                }
                return grammar.Enum(tag, enums);
            case CTokenType.EOF:
            case CTokenType.RParen:
            case CTokenType.Colon:
            case CTokenType.Volatile:
            case CTokenType.Comma:
            case CTokenType.Const:
            case CTokenType.RBrace:
                return null;
            default:
                throw new NotImplementedException(string.Format("Meuf line {0}: {1}", lexer.LineNumber, token));
            }
        }

        //StructDecl = SpecifierQualifierList StructDeclarator {',' StructDeclarator} ';'.

        private StructDecl Parse_StructDecl()
        {
            var sql = Parse_SpecifierQualifierList();
            List<FieldDeclarator> decls = new List<FieldDeclarator>();
            decls.Add(Parse_StructDeclarator());
            while (PeekThenDiscard(CTokenType.Comma))
            {
                decls.Add(Parse_StructDeclarator());
            }
            ExpectToken(CTokenType.Semicolon);
            return grammar.StructDecl(sql, decls);
        }

        //StructDeclarator = Declarator [':' ConstExpr] | ':'  ConstExpr.

        private FieldDeclarator Parse_StructDeclarator()
        {
            Declarator decl = null;
            CExpression bitField = null;
            if (PeekTokenType() != CTokenType.Colon)
            {
                decl = Parse_Declarator();
            }
            if (PeekThenDiscard(CTokenType.Colon))
            {
                bitField = Parse_ConstExpr();
            }
            return grammar.FieldDeclarator(decl, bitField);
        }

        //Enumerator = ident ['=' ConstExpr].

        private Enumerator Parse_Enumerator()
        {
            var id = (string) ExpectToken(CTokenType.Id);
            CExpression init = null;
            if (PeekThenDiscard(CTokenType.Assign))
            {
                init = Parse_ConstExpr();
            }
            return grammar.Enumerator(id, init);
        }

        //SpecifierQualifierList =
        //  (TypeSpecifier | TypeQualifier)
        //  { IF(!IsDeclarator())
        //    (TypeSpecifier | TypeQualifier)
        //  }.

        private List<DeclSpec> Parse_SpecifierQualifierList()
        {
            var sql = new List<DeclSpec>();
            do
            {
                DeclSpec t = Parse_TypeSpecifier();
                if (t == null)
                    t = Parse_TypeQualifier();
                if (t == null)
                    break;
                sql.Add(t);
            } while (!IsDeclarator());
            return sql;
        }
        //TypeQualifier = "const" | "volatile".

        private TypeQualifier Parse_TypeQualifier()
        {
            var token = PeekTokenType();
            switch (token)
            {
            case CTokenType.Const:
            case CTokenType.Volatile:
            case CTokenType.__Ptr64:
                lexer.Read();
                return grammar.TypeQualifier(token);
            }
            return null;
        }

        //Declarator =
        //    [Pointer]
        //    ( ident
        //    | '(' Declarator ')'
        //    )
        //    { '[' [ConstExpr] ']' 
        //    | '(' [IF(!IsType0()) IdentList | ParamTypeList] ')' 
        //    }.
        public Declarator Parse_Declarator()
        {
            Declarator decl;
            var token = lexer.Peek(0).Type;
            switch (token)
            {
            case CTokenType.Id:
                decl = grammar.IdDeclarator((string) lexer.Read().Value);
                break;
            case CTokenType.LParen:
                ExpectToken(CTokenType.LParen);
                decl = Parse_Declarator();
                ExpectToken(CTokenType.RParen);
                break;
            case CTokenType.Star:
                return Parse_Pointer();
            case CTokenType.__Stdcall:
            case CTokenType.__Cdecl:
                lexer.Read();
                decl = Parse_Declarator();
                return grammar.CallConventionDeclarator(token, decl);
            default:
                return null;
            }
            for (; ; )
            {
                switch (PeekTokenType())
                {
                case CTokenType.LBracket:
                    lexer.Read();
                    CExpression expr = null;
                    if (PeekTokenType() != CTokenType.RBracket)
                    {
                        expr = Parse_ConstExpr();
                    }
                    ExpectToken(CTokenType.RBracket);
                    decl = grammar.ArrayDeclarator(decl, expr);
                    break;
                case CTokenType.LParen:
                    lexer.Read();
                    if (lexer.Peek(0).Type == CTokenType.RParen)
                    {
                        var parameters = new List<ParamDecl>();
                        decl = grammar.FunctionDeclarator(decl, parameters);
                    } 
                    else if (!IsType0())
                    {
                        Parse_IdentList();
                    }
                    else
                    {
                        var parameters = Parse_ParamTypeList();
                        decl = grammar.FunctionDeclarator(decl, parameters); 
                    }
                    ExpectToken(CTokenType.RParen);
                    break;
                default: 
                    return decl;
                }
            }
        }

        // Pointer = '*'  {TypeQualifier} {'*'  {TypeQualifier}}.

        Declarator Parse_Pointer()
        {
            ExpectToken(CTokenType.Star);
            List<TypeQualifier> tqs = null;
            var tq = Parse_TypeQualifier();
            while (tq != null)
            {
                if (tqs == null)
                    tqs = new List<TypeQualifier>();
                tqs.Add(tq);
                tq = Parse_TypeQualifier();
            }
            var declarator = Parse_Declarator();
            return grammar.PointerDeclarator(declarator, tqs);
        }

        Declarator Parse_AbstractPointer()
        {
            ExpectToken(CTokenType.Star);
            List<TypeQualifier> tqs = null;
            var tq = Parse_TypeQualifier();
            while (tq != null)
            {
                if (tqs == null)
                    tqs = new List<TypeQualifier>();
                tqs.Add(tq);
                tq = Parse_TypeQualifier();
            }
            var declarator = Parse_DirectAbstractDeclarator();
            return grammar.PointerDeclarator(declarator, tqs);
        }

        // ParamTypeList = ParamDecl {IF(Continued1()) ',' ParamDecl} [',' "..."].

        List<ParamDecl> Parse_ParamTypeList()
        {
            var pds = new List<ParamDecl>();
            var pd = Parse_ParamDecl();
            if (pd == null)
                return null;
            pds.Add(pd);
            while (IsContinued1())
            {
                ExpectToken(CTokenType.Comma);
                pds.Add(Parse_ParamDecl());
            }
            if (PeekThenDiscard(CTokenType.Comma))
            {
                ExpectToken(CTokenType.Ellipsis);
                pds.Add(grammar.Ellipsis());
            }
            return pds;
        }

        // ParamDecl = DeclSpecifierList [IF(IsAbstractDecl()) AbstractDeclarator | Declarator].

        ParamDecl Parse_ParamDecl()
        {
            Declarator decl = null;
            var dsl = Parse_DeclSpecifierList();
            if (dsl == null)
                return null;
            if (IsAbstractDecl())
            {
                decl = Parse_AbstractDeclarator();
            }
            else
            {
                decl = Parse_Declarator();
            }
            return grammar.ParamDecl(dsl, decl);
        }

        //IdentList = ident {',' ident}.

        List<CIdentifier> Parse_IdentList()
        {
            var list = new List<CIdentifier>();
            list.Add(grammar.Id((string) ExpectToken(CTokenType.Id)));
            while (PeekThenDiscard(CTokenType.Comma))
            {
                list.Add(grammar.Id((string) ExpectToken(CTokenType.Id)));
            }
            return list;
        }

        //    TypeName = // a better name would be Type
        //          SpecifierQualifierList [AbstractDeclarator].

        CType Parse_TypeName()
        {
            var sql = Parse_SpecifierQualifierList();
            Declarator decl = null;
            switch (PeekTokenType())
            {
            case CTokenType.Star:
            case CTokenType.LParen:
            case CTokenType.LBracket:
                decl = Parse_AbstractDeclarator();
                break;
            }
            return new CType
            {
                DeclSpecList = sql,
                Declarator = decl,
            };
        }

        //AbstractDeclarator =
        //    Pointer [DirectAbstractDeclarator]
        //| DirectAbstractDeclarator.
        // | Pointer DirectAbstractDeclarator.

        Declarator Parse_AbstractDeclarator()
        {
            Declarator decl;
            var token = lexer.Peek(0);
            switch (token.Type)
            {
            case CTokenType.__Cdecl:
                lexer.Read();
                decl = Parse_AbstractPointer();
                return grammar.CallConventionDeclarator(token.Type, decl);
            case CTokenType.Star:
                var ptr = Parse_Pointer();
                decl = Parse_DirectAbstractDeclarator();
                if (decl == null)
                    return ptr;
                return ptr;
            default:
                return Parse_DirectAbstractDeclarator();
            }
        }

        //DirectAbstractDeclarator =
        //    ( '(' [AbstractDeclarator | ParamTypeList] ')'
        //    | '[' [ConstExpr] ']'
        //    )
        //    { '[' [ConstExpr] ']' 
        //    | '(' [ParamTypeList] ')'
        //    }
        Declarator Parse_DirectAbstractDeclarator()
        {
            CExpression expr;
            Declarator decl = null;
            var token = lexer.Peek(0);
            switch (token.Type)
            {
            case CTokenType.LParen:
                lexer.Read();
                decl = Parse_AbstractDeclarator();
                ExpectToken(CTokenType.RParen);
                break;
            case CTokenType.LBracket:
                lexer.Read();
                expr = null;
                if (PeekTokenType() != CTokenType.RBracket)
                {
                    expr = Parse_ConstExpr();
                }
                ExpectToken(CTokenType.RBracket);
                decl = grammar.ArrayDeclarator(decl, expr);
                break;
            default:
                return null;
            }

            for (; ; )
            {
                token = lexer.Peek(0);
                switch (token.Type)
                {
                case CTokenType.LParen:
                    lexer.Read();
                    var ptl2 = Parse_ParamTypeList();
                    ExpectToken(CTokenType.RParen);
                    decl = grammar.FunctionDeclarator(decl, ptl2);
                    break;
                case CTokenType.LBracket:
                    lexer.Read();
                    expr = null;
                    if (PeekTokenType() != CTokenType.RBracket)
                    {
                        expr = Parse_ConstExpr();
                    }
                    ExpectToken(CTokenType.RBracket);
                    decl = grammar.ArrayDeclarator(decl, expr);
                    break;
                default:
                    return decl;
                }
            }
        }

        //Initializer = 
        //    AssignExpr 
        //  | '{'  Initializer {IF(Continued()) ',' Initializer} [','] '}'.

        Initializer Parse_Initializer()
        {
            if (PeekThenDiscard(CTokenType.LBrace))
            {
                var list = new List<Initializer>();
                list.Add(Parse_Initializer());
                while (IsContinued())
                {
                    ExpectToken(CTokenType.Comma);
                    list.Add(Parse_Initializer());
                }
                PeekThenDiscard(CTokenType.Comma);  // trailing comma
                ExpectToken(CTokenType.RBrace);
                return grammar.ListInitializer(list);
            }
            else
            {
                var expr = Parse_AssignExpr();
                return grammar.ExpressionInitializer(expr);
            }
        }

        //---------- Expressions ----------

        //Expr       = AssignExpr {','  AssignExpr}.
        public CExpression Parse_Expr()
        {
            var left = Parse_AssignExpr();
            while (PeekThenDiscard(CTokenType.Comma))
            {
                var right = Parse_AssignExpr();
                left = grammar.Bin(CTokenType.Comma, left, right);
            }
            return left;
        }

        //AssignExpr = CondExpr [AssignOp AssignExpr]. // relaxed
        public CExpression Parse_AssignExpr()
        {
            var left = Parse_CondExpr();
            var token = PeekTokenType();
            switch (token)
            {
            case CTokenType.Assign:
            case CTokenType.MulAssign:
            case CTokenType.DivAssign:
            case CTokenType.ModAssign:
            case CTokenType.PlusAssign:
            case CTokenType.MinusAssign:
            case CTokenType.ShlAssign:
            case CTokenType.ShrAssign:
            case CTokenType.AndAssign:
            case CTokenType.OrAssign:
            case CTokenType.XorAssign:
                lexer.Read();
                var right = Parse_AssignExpr();
                return grammar.Bin(token, left, right);
            default:
                return left;
            }
        }

        //CondExpr   = LogOrExpr ['?' Expr ':' CondExpr].
        public CExpression Parse_CondExpr()
        {
            var cond = Parse_LogOrExpr();
            if (PeekThenDiscard(CTokenType.Question))
            {
                var consequent = Parse_Expr();
                ExpectToken(CTokenType.Colon);
                var alternant = Parse_CondExpr();
                return grammar.Conditional(cond, consequent, alternant);
            }
            return cond;
        }

        //LogOrExpr  = LogAndExpr {"||" LogAndExpr}.
        public CExpression Parse_LogOrExpr()
        {
            var left = Parse_LogAndExpr();
            while (PeekThenDiscard(CTokenType.LogicalOr))
            {
                var right = Parse_LogAndExpr();
                left = grammar.Bin(CTokenType.LogicalOr, left, right);
            }
            return left;
        }

        //LogAndExpr = OrExpr {"&&" OrExpr}.
        public CExpression Parse_LogAndExpr()
        {
            var left = Parse_OrExpr();
            while (PeekThenDiscard(CTokenType.LogicalAnd))
            {
                var right = Parse_OrExpr();
                left = grammar.Bin(CTokenType.LogicalAnd, left, right);
            }
            return left;
        }
        //OrExpr     = XorExpr {'|' XorExpr}.
        public CExpression Parse_OrExpr()
        {
            var left = Parse_XorExpr();
            while (PeekThenDiscard(CTokenType.Pipe))
            {
                var right = Parse_XorExpr();
                left = grammar.Bin(CTokenType.Pipe, left, right);
            }
            return left;
        }

        //XorExpr    = AndExpr {'^' AndExpr}.
        public CExpression Parse_XorExpr()
        {
            var left = Parse_AndExpr();
            while (PeekThenDiscard(CTokenType.Xor))
            {
                var right = Parse_AndExpr();
                left = grammar.Bin(CTokenType.Xor, left, right);
            }
            return left;
        }

        //AndExpr    = EqlExpr {'&' EqlExpr}.
        public CExpression Parse_AndExpr()
        {
            var left = Parse_EqlExpr();
            while (PeekThenDiscard(CTokenType.Ampersand))
            {
                var right = Parse_EqlExpr();
                left = grammar.Bin(CTokenType.Ampersand, left, right);
            }
            return left;
        }
        //EqlExpr    = RelExpr {("==" | "!=") RelExpr}.
        public CExpression Parse_EqlExpr()
        {
            var left = Parse_RelExpr();
            var token = PeekTokenType();
            while (token == CTokenType.Eq || token == CTokenType.Ne)
            {
                lexer.Read();
                var right = Parse_RelExpr();
                left = grammar.Bin(token, left, right);
                token = PeekTokenType();
            }
            return left;
        }
        //RelExpr    = ShiftExpr {('<' | '>' | "<=" | ">=") ShiftExpr}.
        public CExpression Parse_RelExpr()
        {
            var left = Parse_ShiftExpr();
            var token = PeekTokenType();
            while (token == CTokenType.Le || token == CTokenType.Lt ||
                   token == CTokenType.Ge || token == CTokenType.Gt)
            {
                lexer.Read();
                var right = Parse_ShiftExpr();
                left = grammar.Bin(token, left, right);
                token = PeekTokenType();
            }
            return left;
        }
        //ShiftExpr  = AddExpr {("<<" | ">>") AddExpr}.
        public CExpression Parse_ShiftExpr()
        {
            var left = Parse_AddExpr();
            var token = PeekTokenType();
            while (token == CTokenType.Shl || token == CTokenType.Shr)
            {
                lexer.Read();
                var right = Parse_AddExpr();
                left = grammar.Bin(token, left, right);
                token = PeekTokenType();
            }
            return left;
        }
        //AddExpr    = MultExpr {('+' | '-') MultExpr}.
        public CExpression Parse_AddExpr()
        {
            var left = Parse_MultExpr();
            var token = PeekTokenType();
            while (token == CTokenType.Plus || token == CTokenType.Minus)
            {
                lexer.Read();
                var right = Parse_MultExpr();
                left = grammar.Bin(token, left, right);
                token = PeekTokenType();
            }
            return left;
        }
        //MultExpr   = CastExpr {('*' | '/' | '%') CastExpr}.
        public CExpression Parse_MultExpr()
        {
            var left = Parse_CastExpr();
            var token = PeekTokenType();
            while (token == CTokenType.Star || token == CTokenType.Slash ||
                   token == CTokenType.Percent)
            {
                lexer.Read();
                var right = Parse_CastExpr();
                left = grammar.Bin(token, left, right);
                token = PeekTokenType();
            }
            return left;
        }
        //CastExpr   = IF(IsType1()) '(' TypeName ')' CastExpr
        //           | UnaryExpr.
        public CExpression Parse_CastExpr()
        {
            if (IsType1())
            {
                ExpectToken(CTokenType.LParen);
                var type = Parse_TypeName();
                ExpectToken(CTokenType.RParen);
                var expr = Parse_CastExpr();
                return grammar.Cast(type, expr);
            }
            else
            {
                return Parse_UnaryExpr();
            }
        }

        //UnaryExpr =
        //  {"++" | "--"}
        //  ( PostfixExpr
        //  | UnaryOp CastExpr
        //  | "sizeof"  (IF(IsType1()) '(' TypeName ')' | UnaryExpr)
        //  ).
        public CExpression Parse_UnaryExpr()
        {
            CExpression expr;
            var token = PeekTokenType();
            switch (token)
            {
            case CTokenType.Increment:
            case CTokenType.Decrement:
                lexer.Read();
                expr = Parse_UnaryExpr();
                return grammar.PreIncrement(token, expr);
            case CTokenType.Ampersand:
            case CTokenType.Star:
            case CTokenType.Plus:
            case CTokenType.Minus:
            case CTokenType.Tilde:
            case CTokenType.Bang:
                lexer.Read();
                return grammar.Unary(token, Parse_CastExpr());
            case CTokenType.Sizeof:
                lexer.Read();
                if (IsType1())
                {
                    ExpectToken(CTokenType.LParen);
                    CType type = Parse_TypeName();
                    ExpectToken(CTokenType.RParen);
                    return grammar.Sizeof(type);
                }
                else
                {
                    expr = Parse_UnaryExpr();
                    return grammar.Sizeof(expr);
                }
            default:
                return Parse_PostfixExpr();
            }
        }

        //PostfixExpr =
        //  Primary
        //  { '[' Expr ']' 
        //  | '.'  ident
        //  | "->" ident
        //  | '(' [ArgExprList] ')' 
        //  | "++" 
        //  | "--"
        //  }.
        public CExpression Parse_PostfixExpr()
        {
            var left = Parse_Primary();
            string id = null;
            for (; ; )
            {
                var token = PeekTokenType();
                switch (token)
                {
                case CTokenType.LBracket:
                    lexer.Read();
                    var expr = Parse_Expr();
                    ExpectToken(CTokenType.RBracket);
                    left = grammar.ArrayAccess(left, expr);
                    break;
                case CTokenType.Dot:
                    lexer.Read();
                    id = (string) ExpectToken(CTokenType.Id);
                    left =  grammar.MemberAccess(left, id);
                    break;
                case CTokenType.Arrow:
                    lexer.Read();
                    id = (string) ExpectToken(CTokenType.Id);
                    left = grammar.PtrMemberAccess(left, id);
                    break;
                case CTokenType.LParen:
                    lexer.Read();
                    List<CExpression> args = null;
                    if (PeekThenDiscard(CTokenType.RParen))
                    {
                        args = new List<CExpression>();
                    }
                    else
                    {
                        args = Parse_ArgExprList();
                        ExpectToken(CTokenType.RParen);
                    }
                    left = grammar.Application(left, args);
                    break;
                case CTokenType.Increment:
                case CTokenType.Decrement:
                    lexer.Read();
                    left = grammar.PostIncrement(left, token);
                    break;
                default:
                    return left;
                }
            }
        }

        //Primary = ident | intcon | floatcon | charcon | string | '(' Expr ')'.

        public CExpression Parse_Primary()
        {
            var token = PeekTokenType();
            switch (token)
            {
            case CTokenType.Id:
                return grammar.Id((string) lexer.Read().Value);
            case CTokenType.NumericLiteral:
                return grammar.Const(lexer.Read().Value);
            case CTokenType.StringLiteral:
                return grammar.Const(lexer.Read().Value);
            case CTokenType.RealLiteral:
                return grammar.Const(lexer.Read().Value);
            case CTokenType.CharLiteral:
                return grammar.Const(lexer.Read().Value);
            default:
                ExpectToken(CTokenType.LParen);
                var expr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                return expr;
            }
        }

        //ConstExpr = CondExpr.
        private CExpression Parse_ConstExpr()
        {
            PeekTokenType();
            var e = Parse_CondExpr();
            return e;
        }

        //ArgExprList = AssignExpr {','  AssignExpr}.
        private List<CExpression> Parse_ArgExprList()
        {
            var list = new List<CExpression>();
            list.Add(Parse_AssignExpr());
            while (PeekThenDiscard(CTokenType.Comma))
            {
                list.Add(Parse_AssignExpr());
            }
            return list;
        }
        //UnaryOp = '&' | '*' | '+' | '-' | '~' | '!'.

        //AssignOp = '=' | "*=" | "/=" | "%=" | "+=" | "-=" | "<<=" | ">>=" | "&=" | "^=" | "|=".


        //---------- Statements ----------

        //Stat =
        //      IF(IsLabel()) (ident | "case" ConstExpr | "default") ':' Stat
        //    | Expr ';'
        //    | '{' {IF(IsDecl()) Decl | Stat} '}'
        //    | "if" '(' Expr ')' Stat ["else" Stat]
        //    | "switch" '(' Expr ')' Stat
        //    | "while" '(' Expr ')' Stat
        //    | "do" Stat "while" '(' Expr ')' ';'
        //    | "for" '(' (IF(IsDecl()) Decl | [Expr] ';') [Expr] ';' [Expr] ')' Stat
        //    | "goto" ident ';'
        //    | "continue" ';'
        //    | "break" ';'
        //    | "return" [Expr] ';'
        //    | ';'
        public Stat Parse_Stat()
        {
            if (IsLabel())
            {
                Label label = null;
                CExpression constExpr = null;
                switch (PeekTokenType())
                {
                case CTokenType.Id:
                    label = grammar.Label((string) lexer.Read().Value);
                    break;
                case CTokenType.Case:
                    ExpectToken(CTokenType.Case);
                    constExpr = Parse_ConstExpr();
                    label = grammar.CaseLabel(constExpr);
                    break;
                case CTokenType.Default:
                    label = grammar.DefaultCaseLabel();
                    break;
                }
                ExpectToken(CTokenType.Colon);
                var stat = Parse_Stat();
                return grammar.LabeledStatement(label, stat);
            }

            CExpression expr;
            switch (PeekTokenType())
            {
            case CTokenType.LBrace:
                ExpectToken(CTokenType.LBrace);
                var stms = new List<Stat>();
                while (!PeekThenDiscard(CTokenType.RBrace))
                {
                    if (IsDecl())
                        stms.Add(grammar.DeclStat(Parse_Decl()));
                    else
                        stms.Add(Parse_Stat());
                }
                return grammar.CompoundStatement(stms);
            case CTokenType.If:
                ExpectToken(CTokenType.If);
                ExpectToken(CTokenType.LParen);
                expr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                Stat consequence = Parse_Stat();
                Stat alternative = null;
                if (PeekThenDiscard(CTokenType.Else))
                {
                    alternative = Parse_Stat();
                }
                return grammar.IfStatement(expr, consequence, alternative);
            case CTokenType.Switch:
                ExpectToken(CTokenType.Switch);
                ExpectToken(CTokenType.LParen);
                expr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                var switchBody = Parse_Stat();
                return grammar.SwitchStatement(expr, switchBody);
            case CTokenType.While:
                ExpectToken(CTokenType.While);
                ExpectToken(CTokenType.LParen);
                expr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                var whileBody = Parse_Stat();
                return grammar.WhileStatement(expr, whileBody);
            case CTokenType.Do:
                ExpectToken(CTokenType.Do);
                var doBody = Parse_Stat();
                ExpectToken(CTokenType.While);
                ExpectToken(CTokenType.LParen);
                expr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                ExpectToken(CTokenType.Semicolon);
                return grammar.DoWhileStatement(doBody, expr);
            case CTokenType.For:
                ExpectToken(CTokenType.For);
                ExpectToken(CTokenType.LParen);
                Stat initStat;
                if (IsDecl())
                {
                    initStat = grammar.DeclStat(Parse_Decl());
                }
                else if (PeekTokenType() != CTokenType.Semicolon)
                {
                    initStat = grammar.ExprStatement(Parse_Expr());
                }
                ExpectToken(CTokenType.Semicolon);
                var test = Parse_Expr();
                ExpectToken(CTokenType.Semicolon);
                var incr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                var forBody = Parse_Stat();
                return grammar.ForStatement(null, test, incr, forBody);
            case CTokenType.Goto:
                ExpectToken(CTokenType.Goto);
                var gotoLabel = (string) ExpectToken(CTokenType.Id);
                ExpectToken(CTokenType.Semicolon);
                return grammar.GotoStatement(gotoLabel);
            case CTokenType.Continue:
                ExpectToken(CTokenType.Continue);
                ExpectToken(CTokenType.Semicolon);
                return grammar.ContinueStatement();
            case CTokenType.Break:
                ExpectToken(CTokenType.Break);
                ExpectToken(CTokenType.Semicolon);
                return grammar.BreakStatement();
            case CTokenType.Return:
                ExpectToken(CTokenType.Return);
                expr = null;
                if (PeekTokenType() != CTokenType.Semicolon)
                    expr = Parse_Expr();
                ExpectToken(CTokenType.Semicolon);
                return grammar.ReturnStatement(expr);
            case CTokenType.Semicolon:
                ExpectToken(CTokenType.Semicolon);
                return grammar.EmptyStatement();
            case CTokenType.__Asm:
                ExpectToken(CTokenType.__Asm);
                ExpectToken(CTokenType.LBrace);
                while (lexer.Peek(0).Type != CTokenType.RBrace)
                    lexer.Read();
                ExpectToken(CTokenType.RBrace);
                return grammar.EmptyStatement();        // don't care?
            default:
                expr = Parse_Expr();
                ExpectToken(CTokenType.Semicolon);
                return grammar.ExprStatement(expr);
            }
        }
    }
}
