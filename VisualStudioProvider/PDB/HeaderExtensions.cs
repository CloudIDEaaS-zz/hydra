using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Diagnostics;
using SDKInterfaceLibrary.Entities;
using VisualStudioProvider.PDB;

namespace VisualStudioProvider.PDB.Headers
{
    public static class HeaderExtensions
    {
        public static void AssertNotNull(this object obj)
        {
            if (obj == null)
            {
                Debugger.Break();
            }

            Debug.Assert(obj != null);
        }

        public static void AssertNotNullAndOfType<T>(this object obj)
        {
            obj.AssertNotNull();

            if (obj.GetType().Name != typeof(T).Name)
            {
                Debugger.Break();
            }

            Debug.Assert(obj.GetType().Name == typeof(T).Name);
        }

        public static bool IsParsedSuccessfully(this tblSDKHeaderFile headerFile)
        {
            return headerFile.ParsedSuccessfully != null && headerFile.ParsedSuccessfully.Value;
        }

        public static Expression GetRealExpressionInternal(this CppSharp.Parser.AST.Expression expression)
        {
            var realExpression = expression.GetRealExpression();

            Debugger.Break();
            return null;
        }

        public static Class GetRealClassInternal(this CppSharp.Parser.AST.Class _class)
        {
            var realClass = _class.GetRealClass();

            switch (_class.Kind)
            {
                case CppSharp.Parser.AST.DeclarationKind.ClassTemplatePartialSpecialization:
                    return new ClassTemplatePartialSpecialization((CppSharp.Parser.AST.ClassTemplatePartialSpecialization) realClass);
                case CppSharp.Parser.AST.DeclarationKind.ClassTemplateSpecialization:
                    return new ClassTemplateSpecialization((CppSharp.Parser.AST.ClassTemplateSpecialization)realClass);
                case CppSharp.Parser.AST.DeclarationKind.Class:
                    return new Class(realClass);
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public static Template GetRealTemplateInternal(this CppSharp.Parser.AST.Template template, DeclarationContext owningDeclarationContext)
        {
            var realTemplate = template.GetRealTemplate();

            switch (template.Kind)
            {
                case CppSharp.Parser.AST.DeclarationKind.TemplateTemplateParm:
                    return new TemplateTemplateParameter(owningDeclarationContext, (CppSharp.Parser.AST.TemplateTemplateParameter)realTemplate);
                case CppSharp.Parser.AST.DeclarationKind.TypeAliasTemplate:
                    return new TypeAliasTemplate(owningDeclarationContext, (CppSharp.Parser.AST.TypeAliasTemplate)realTemplate);
                case CppSharp.Parser.AST.DeclarationKind.ClassTemplate:
                    return new ClassTemplate(owningDeclarationContext, (CppSharp.Parser.AST.ClassTemplate)realTemplate);
                case CppSharp.Parser.AST.DeclarationKind.FunctionTemplate:
                    return new FunctionTemplate(owningDeclarationContext, (CppSharp.Parser.AST.FunctionTemplate)realTemplate);
                case CppSharp.Parser.AST.DeclarationKind.VarTemplate:
                    return new VarTemplate(owningDeclarationContext, (CppSharp.Parser.AST.VarTemplate)realTemplate);
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public static Template GetRealTemplateInternal(this CppSharp.Parser.AST.Template template)
        {
            var realTemplate = template.GetRealTemplate();

            switch (template.Kind)
            {
                case CppSharp.Parser.AST.DeclarationKind.TemplateTemplateParm:
                    return new TemplateTemplateParameter((CppSharp.Parser.AST.TemplateTemplateParameter)realTemplate);
                case CppSharp.Parser.AST.DeclarationKind.TypeAliasTemplate:
                    return new TypeAliasTemplate((CppSharp.Parser.AST.TypeAliasTemplate)realTemplate);
                case CppSharp.Parser.AST.DeclarationKind.ClassTemplate:
                    return new ClassTemplate((CppSharp.Parser.AST.ClassTemplate)realTemplate);
                case CppSharp.Parser.AST.DeclarationKind.FunctionTemplate:
                    return new FunctionTemplate((CppSharp.Parser.AST.FunctionTemplate)realTemplate);
                case CppSharp.Parser.AST.DeclarationKind.VarTemplate:
                    return new VarTemplate((CppSharp.Parser.AST.VarTemplate)realTemplate);
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public static Type GetRealTypeInternal(this CppSharp.Parser.AST.Type astType)
        {
            var realType = astType.GetRealType();

            switch (astType.Kind)
            {
                case CppSharp.Parser.AST.TypeKind.Tag:
                    return new TagType((CppSharp.Parser.AST.TagType) realType);
                case CppSharp.Parser.AST.TypeKind.Array:
                    return new ArrayType((CppSharp.Parser.AST.ArrayType) realType);
                case CppSharp.Parser.AST.TypeKind.Function:
                    return new FunctionType((CppSharp.Parser.AST.FunctionType) realType);
                case CppSharp.Parser.AST.TypeKind.Pointer:
                    return new PointerType((CppSharp.Parser.AST.PointerType) realType);
                case CppSharp.Parser.AST.TypeKind.MemberPointer:
                    return new MemberPointerType((CppSharp.Parser.AST.MemberPointerType) realType);
                case CppSharp.Parser.AST.TypeKind.Typedef:
                    return new TypedefType((CppSharp.Parser.AST.TypedefType) realType);
                case CppSharp.Parser.AST.TypeKind.Attributed:
                    return new AttributedType((CppSharp.Parser.AST.AttributedType) realType);
                case CppSharp.Parser.AST.TypeKind.Decayed:
                    return new DecayedType((CppSharp.Parser.AST.DecayedType) realType);
                case CppSharp.Parser.AST.TypeKind.TemplateSpecialization:
                    return new TemplateSpecializationType((CppSharp.Parser.AST.TemplateSpecializationType) realType);
                case CppSharp.Parser.AST.TypeKind.DependentTemplateSpecialization:
                    return new DependentTemplateSpecializationType((CppSharp.Parser.AST.DependentTemplateSpecializationType) realType);
                case CppSharp.Parser.AST.TypeKind.TemplateParameter:
                    return new TemplateParameterType((CppSharp.Parser.AST.TemplateParameterType) realType);
                case CppSharp.Parser.AST.TypeKind.TemplateParameterSubstitution:
                    return new TemplateParameterSubstitutionType((CppSharp.Parser.AST.TemplateParameterSubstitutionType)realType);
                case CppSharp.Parser.AST.TypeKind.InjectedClassName:
                    return new InjectedClassNameType((CppSharp.Parser.AST.InjectedClassNameType) realType);
                case CppSharp.Parser.AST.TypeKind.DependentName:
                    return new DependentNameType((CppSharp.Parser.AST.DependentNameType) realType);
                case CppSharp.Parser.AST.TypeKind.PackExpansion:
                    return new PackExpansionType((CppSharp.Parser.AST.PackExpansionType) realType);
                case CppSharp.Parser.AST.TypeKind.Builtin:
                    return new BuiltinType((CppSharp.Parser.AST.BuiltinType) realType);
                case CppSharp.Parser.AST.TypeKind.UnaryTransform:
                    return new UnaryTransformType((CppSharp.Parser.AST.UnaryTransformType) realType);
                case CppSharp.Parser.AST.TypeKind.Vector:
                    return new VectorType((CppSharp.Parser.AST.VectorType) realType);
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public static Comment GetRealCommentInternal(this CppSharp.Parser.AST.BlockContentComment comment, Comment owningComment)
        {
            var realComment = comment.GetRealComment();

            switch (comment.Kind)
            {
                case CppSharp.Parser.AST.CommentKind.BlockCommandComment:
                    return new BlockCommandComment(owningComment, (CppSharp.Parser.AST.BlockCommandComment) realComment);
                case CppSharp.Parser.AST.CommentKind.ParagraphComment:
                    return new ParagraphComment(owningComment, (CppSharp.Parser.AST.ParagraphComment)realComment);
                case CppSharp.Parser.AST.CommentKind.FullComment:
                    return new FullComment(owningComment, (CppSharp.Parser.AST.FullComment)realComment);
                case CppSharp.Parser.AST.CommentKind.InlineContentComment:
                    return new InlineContentComment(owningComment, (CppSharp.Parser.AST.InlineContentComment)realComment);
                case CppSharp.Parser.AST.CommentKind.VerbatimLineComment:
                    return new VerbatimLineComment(owningComment, (CppSharp.Parser.AST.VerbatimLineComment)realComment);
                case CppSharp.Parser.AST.CommentKind.VerbatimBlockLineComment:
                    return new VerbatimBlockLineComment(owningComment, (CppSharp.Parser.AST.VerbatimBlockLineComment)realComment);
                case CppSharp.Parser.AST.CommentKind.ParamCommandComment:
                    return new ParamCommandComment(owningComment, (CppSharp.Parser.AST.ParamCommandComment)realComment);
                case CppSharp.Parser.AST.CommentKind.TParamCommandComment:
                    return new TParamCommandComment(owningComment, (CppSharp.Parser.AST.TParamCommandComment)realComment);
                case CppSharp.Parser.AST.CommentKind.BlockContentComment:
                    return new BlockContentComment(owningComment, (CppSharp.Parser.AST.BlockContentComment)realComment);
                case CppSharp.Parser.AST.CommentKind.VerbatimBlockComment:
                    return new VerbatimBlockComment(owningComment, (CppSharp.Parser.AST.VerbatimBlockComment)realComment);
                case CppSharp.Parser.AST.CommentKind.HTMLTagComment:
                    return new HTMLTagComment(owningComment, (CppSharp.Parser.AST.HTMLTagComment)realComment);
                case CppSharp.Parser.AST.CommentKind.HTMLStartTagComment:
                    return new HTMLStartTagComment(owningComment, (CppSharp.Parser.AST.HTMLStartTagComment)realComment);
                case CppSharp.Parser.AST.CommentKind.HTMLEndTagComment:
                    return new HTMLEndTagComment(owningComment, (CppSharp.Parser.AST.HTMLEndTagComment)realComment);
                case CppSharp.Parser.AST.CommentKind.TextComment:
                    return new TextComment(owningComment, (CppSharp.Parser.AST.TextComment)realComment);
                case CppSharp.Parser.AST.CommentKind.InlineCommandComment:
                    return new InlineCommandComment(owningComment, (CppSharp.Parser.AST.InlineCommandComment)realComment);
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public static PreprocessedEntity GetRealPreprocessedEntityInternal(this CppSharp.Parser.AST.PreprocessedEntity entity, Declaration owningDeclaration)
        {
            var realDeclaration = entity.GetRealPreprocessedEntity();

            switch (entity.Kind)
            {
                case CppSharp.Parser.AST.DeclarationKind.MacroDefinition:
                    return new MacroDefinition(owningDeclaration, (CppSharp.Parser.AST.MacroDefinition) realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.MacroExpansion:
                    return new MacroExpansion(owningDeclaration, (CppSharp.Parser.AST.MacroExpansion)realDeclaration);
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public static Declaration GetRealDeclarationInternal(this CppSharp.Parser.AST.Declaration declaration)
        {
            var realDeclaration = declaration.GetRealDeclaration();

            switch (declaration.Kind)
            {
                case CppSharp.Parser.AST.DeclarationKind.DeclarationContext:
                    return new Headers.DeclarationContext((CppSharp.Parser.AST.DeclarationContext)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Typedef:
                    return new Headers.TypeDefDecl((CppSharp.Parser.AST.TypedefDecl)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.TypeAlias:
                    return new Headers.TypeAlias((CppSharp.Parser.AST.TypeAlias)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Parameter:
                    return new Headers.Parameter((CppSharp.Parser.AST.Parameter)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Function:
                    return new Headers.Function((CppSharp.Parser.AST.Function)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Method:
                    return new Headers.Method((CppSharp.Parser.AST.Method)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Enumeration:
                    return new Headers.Enumeration((CppSharp.Parser.AST.Enumeration)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.EnumerationItem:
                    return new Headers.EnumerationItem((CppSharp.Parser.AST.Enumeration.Item)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Variable:
                    return new Headers.Variable((CppSharp.Parser.AST.Variable)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Field:
                    return new Headers.Field((CppSharp.Parser.AST.Field)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.AccessSpecifier:
                    return new Headers.AccessSpecifierDecl((CppSharp.Parser.AST.AccessSpecifierDecl)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Class:
                    return new Headers.Class((CppSharp.Parser.AST.Class)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Template:
                    return new Headers.Template((CppSharp.Parser.AST.Template)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.TypeAliasTemplate:
                    return new Headers.TypeAliasTemplate((CppSharp.Parser.AST.TypeAliasTemplate)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.ClassTemplate:
                    return new Headers.ClassTemplate((CppSharp.Parser.AST.ClassTemplate)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.ClassTemplateSpecialization:
                    return new Headers.ClassTemplateSpecialization((CppSharp.Parser.AST.ClassTemplateSpecialization)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.ClassTemplatePartialSpecialization:
                    return new Headers.ClassTemplatePartialSpecialization((CppSharp.Parser.AST.ClassTemplatePartialSpecialization)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.FunctionTemplate:
                    return new Headers.FunctionTemplate((CppSharp.Parser.AST.FunctionTemplate)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Namespace:
                    return new Headers.Namespace((CppSharp.Parser.AST.Namespace)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.TranslationUnit:
                    return new Headers.TranslationUnit((CppSharp.Parser.AST.TranslationUnit)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.Friend:
                    return new Headers.Friend((CppSharp.Parser.AST.Friend)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.TemplateTemplateParm:
                    return new Headers.TemplateTemplateParameter((CppSharp.Parser.AST.TemplateTemplateParameter)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.NonTypeTemplateParm:
                    return new Headers.NonTypeTemplateParameter((CppSharp.Parser.AST.NonTypeTemplateParameter)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.VarTemplate:
                    return new Headers.VarTemplate((CppSharp.Parser.AST.VarTemplate)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.VarTemplateSpecialization:
                    return new Headers.VarTemplateSpecialization((CppSharp.Parser.AST.VarTemplateSpecialization)realDeclaration);
                case CppSharp.Parser.AST.DeclarationKind.VarTemplatePartialSpecialization:
                    return new Headers.VarTemplatePartialSpecialization((CppSharp.Parser.AST.VarTemplatePartialSpecialization)realDeclaration);
                default:
                    Debugger.Break();
                    return null;
            }
        }
    }
}
