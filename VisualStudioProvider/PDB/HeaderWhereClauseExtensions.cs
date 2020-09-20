using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDKInterfaceLibrary.Entities;
using Utils;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public static class HeaderWhereClauseExtensions
    {
        public static SdkInterfaceLibraryEntities Entities { private get; set; }
        public static string CurrentWhere { get; private set; }

 
        public static Func<tblSDKHeaderIntegerValue, bool> GetWhere(this IntegerValue integerValue, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderIntegerValue, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Value = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), integerValue.Value.ToString());
            where = (i) => i.HeaderFileId == tblSdkHeaderFile.HeaderFileId && i.Value == integerValue.Value;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderType, bool> GetWhere(this Type type, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderVTableLayout, bool> GetWhere(this VTableLayout vTableLayout, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderVTableLayout, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderBlockCommandCommentArgument, bool> GetWhere(this BlockCommandCommentArgument blockCommandCommentArgument, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderBlockCommandCommentArgument, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Text = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), blockCommandCommentArgument.Text.ToString());
            where = (b) => b.HeaderFileId == tblSdkHeaderFile.HeaderFileId && b.Text == blockCommandCommentArgument.Text;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderBlockCommandCommentArgument, bool> GetWhere(this BlockCommandCommentArgument blockCommandCommentArgument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderBlockCommandComment tblOwningSDKHeaderBlockCommandComment)
        {
            Func<tblSDKHeaderBlockCommandCommentArgument, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningBlockCommandCommentId = '{1}' AND Text = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderBlockCommandComment.BlockCommandCommentId.ToString(), blockCommandCommentArgument.Text.ToString());
            where = (b) => b.HeaderFileId == tblSdkHeaderFile.HeaderFileId && b.OwningBlockCommandCommentId == tblOwningSDKHeaderBlockCommandComment.BlockCommandCommentId && b.Text == blockCommandCommentArgument.Text;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderBuiltinType, bool> GetWhere(this BuiltinType builtinType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderBuiltinType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (b) => b.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderBuiltinTypeExpression, bool> GetWhere(this BuiltinTypeExpression builtinTypeExpression, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderBuiltinTypeExpression, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (b) => b.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderCallExpr, bool> GetWhere(this CallExpr callExpr, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderCallExpr, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (c) => c.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderClassLayout, bool> GetWhere(this ClassLayout classLayout, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderClassLayout, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (c) => c.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderClassTemplate, bool> GetWhere(this ClassTemplate classTemplate, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderClassTemplate, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), classTemplate.LocationIdentifier.ToString());
            where = (c) => c.HeaderFileId == tblSdkHeaderFile.HeaderFileId && c.LocationIdentifier == classTemplate.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderClassTemplatePartialSpecialization, bool> GetWhere(this ClassTemplatePartialSpecialization classTemplatePartialSpecialization, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderClassTemplatePartialSpecialization, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), classTemplatePartialSpecialization.LocationIdentifier.ToString());
            where = (c) => c.HeaderFileId == tblSdkHeaderFile.HeaderFileId && c.LocationIdentifier == classTemplatePartialSpecialization.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderComment, bool> GetWhere(this Comment comment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (c) => c.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderComment, bool> GetWhere(this Comment comment, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclaration tblOwningSDKHeaderDeclaration)
        {
            Func<tblSDKHeaderComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningDeclarationId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderDeclaration.DeclarationId.ToString());
            where = (c) => c.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderCXXConstructExpr, bool> GetWhere(this CXXConstructExpr cXXConstructExpr, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderCXXConstructExpr, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (c) => c.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderExpression, bool> GetWhere(this Expression expression, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderExpression, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (e) => e.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderFunctionTemplate, bool> GetWhere(this FunctionTemplate functionTemplate, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderFunctionTemplate, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), functionTemplate.LocationIdentifier.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId && f.LocationIdentifier == functionTemplate.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderHeader, bool> GetWhere(this Header header, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderHeader, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Name = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), header.Name.ToString(), header.LocationIdentifier.ToString());
            where = (h) => h.HeaderFileId == tblSdkHeaderFile.HeaderFileId && h.Name == header.Name && h.LocationIdentifier == header.LocationIdentifier;

            return where;
        }
 
        public static Func<tblSDKHeaderHTMLEndTagComment, bool> GetWhere(this HTMLEndTagComment hTMLEndTagComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderHTMLEndTagComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND TagName = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), hTMLEndTagComment.TagName.ToString());
            where = (h) => h.HeaderFileId == tblSdkHeaderFile.HeaderFileId && h.TagName == hTMLEndTagComment.TagName;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderHTMLStartTagComment, bool> GetWhere(this HTMLStartTagComment hTMLStartTagComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderHTMLStartTagComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND TagName = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), hTMLStartTagComment.TagName.ToString());
            where = (h) => h.HeaderFileId == tblSdkHeaderFile.HeaderFileId && h.TagName == hTMLStartTagComment.TagName;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderHTMLStartTagCommentAttribute, bool> GetWhere(this HTMLStartTagCommentAttribute hTMLStartTagCommentAttribute, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderHTMLStartTagCommentAttribute, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Name = '{1}' AND Value = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), hTMLStartTagCommentAttribute.Name.ToString(), hTMLStartTagCommentAttribute.Value.ToString());
            where = (h) => h.HeaderFileId == tblSdkHeaderFile.HeaderFileId && h.Name == hTMLStartTagCommentAttribute.Name && h.Value == hTMLStartTagCommentAttribute.Value;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderHTMLStartTagCommentAttribute, bool> GetWhere(this HTMLStartTagCommentAttribute hTMLStartTagCommentAttribute, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderHTMLStartTagComment tblOwningSDKHeaderHTMLStartTagComment)
        {
            Func<tblSDKHeaderHTMLStartTagCommentAttribute, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningHTMLStartTagCommentId = '{1}' AND Name = '{2}' AND Value = '{3}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderHTMLStartTagComment.HTMLStartTagCommentId.ToString(), hTMLStartTagCommentAttribute.Name.ToString(), hTMLStartTagCommentAttribute.Value.ToString());
            where = (h) => h.HeaderFileId == tblSdkHeaderFile.HeaderFileId && h.OwningHTMLStartTagCommentId == tblOwningSDKHeaderHTMLStartTagComment.HTMLStartTagCommentId && h.Name == hTMLStartTagCommentAttribute.Name && h.Value == hTMLStartTagCommentAttribute.Value;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderHTMLTagComment, bool> GetWhere(this HTMLTagComment hTMLTagComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderHTMLTagComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (h) => h.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderInlineCommandCommentArgument, bool> GetWhere(this InlineCommandCommentArgument inlineCommandCommentArgument, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderInlineCommandCommentArgument, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Text = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), inlineCommandCommentArgument.Text.ToString());
            where = (i) => i.HeaderFileId == tblSdkHeaderFile.HeaderFileId && i.Text == inlineCommandCommentArgument.Text;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderInlineCommandCommentArgument, bool> GetWhere(this InlineCommandCommentArgument inlineCommandCommentArgument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderInlineCommandComment tblOwningSDKHeaderInlineCommandComment)
        {
            Func<tblSDKHeaderInlineCommandCommentArgument, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningInlineCommandCommentId = '{1}' AND Text = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderInlineCommandComment.InlineCommandCommentId.ToString(), inlineCommandCommentArgument.Text.ToString());
            where = (i) => i.HeaderFileId == tblSdkHeaderFile.HeaderFileId && i.OwningInlineCommandCommentId == tblOwningSDKHeaderInlineCommandComment.InlineCommandCommentId && i.Text == inlineCommandCommentArgument.Text;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderMacroDefinition, bool> GetWhere(this MacroDefinition macroDefinition, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderMacroDefinition, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Name = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), macroDefinition.Name.ToString());
            where = (m) => m.HeaderFileId == tblSdkHeaderFile.HeaderFileId && m.Name == macroDefinition.Name;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderNamespace, bool> GetWhere(this Namespace _namespace, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderNamespace, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Name = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), _namespace.Name.ToString(), _namespace.LocationIdentifier.ToString());
            where = (n) => n.HeaderFileId == tblSdkHeaderFile.HeaderFileId && n.Name == _namespace.Name && n.LocationIdentifier == _namespace.LocationIdentifier;

            return where;
        }
 
        public static Func<tblSDKHeaderPackExpansionType, bool> GetWhere(this PackExpansionType packExpansionType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderPackExpansionType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (p) => p.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderParagraphComment, bool> GetWhere(this ParagraphComment paragraphComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderParagraphComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (p) => p.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderPreprocessedEntity, bool> GetWhere(this PreprocessedEntity preprocessedEntity, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderPreprocessedEntity, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (p) => p.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderPreprocessedEntity, bool> GetWhere(this PreprocessedEntity preprocessedEntity, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclaration tblOwningSDKHeaderDeclaration)
        {
            Func<tblSDKHeaderPreprocessedEntity, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningDeclarationId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderDeclaration.DeclarationId.ToString());
            where = (p) => p.HeaderFileId == tblSdkHeaderFile.HeaderFileId && p.OwningDeclarationId == tblOwningSDKHeaderDeclaration.DeclarationId;

            return where;
        }
 
        public static Func<tblSDKHeaderQualifiedType, bool> GetWhere(this QualifiedType qualifiedType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderQualifiedType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (q) => q.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderRawComment, bool> GetWhere(this RawComment rawComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderRawComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND BriefText = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), rawComment.BriefText.ToString());
            where = (r) => r.HeaderFileId == tblSdkHeaderFile.HeaderFileId && r.BriefText == rawComment.BriefText;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderStatement, bool> GetWhere(this Statement statement, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderStatement, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (s) => s.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTemplateTemplateParameter, bool> GetWhere(this TemplateTemplateParameter templateTemplateParameter, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTemplateTemplateParameter, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), templateTemplateParameter.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.LocationIdentifier == templateTemplateParameter.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTextComment, bool> GetWhere(this TextComment textComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTextComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Text = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), textComment.Text.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.Text == textComment.Text;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTParamCommandComment, bool> GetWhere(this TParamCommandComment tParamCommandComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTParamCommandComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTranslationUnit, bool> GetWhere(this TranslationUnit translationUnit, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTranslationUnit, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND FileName = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), translationUnit.FileName.ToString(), translationUnit.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.FileName == translationUnit.FileName && t.LocationIdentifier == translationUnit.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTypeAliasTemplate, bool> GetWhere(this TypeAliasTemplate typeAliasTemplate, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTypeAliasTemplate, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), typeAliasTemplate.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.LocationIdentifier == typeAliasTemplate.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTypeDefDecl, bool> GetWhere(this TypeDefDecl typeDefDecl, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTypeDefDecl, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), typeDefDecl.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.LocationIdentifier == typeDefDecl.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderVarTemplate, bool> GetWhere(this VarTemplate varTemplate, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderVarTemplate, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), varTemplate.LocationIdentifier.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId && v.LocationIdentifier == varTemplate.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderVerbatimBlockComment, bool> GetWhere(this VerbatimBlockComment verbatimBlockComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderVerbatimBlockComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderVerbatimLineComment, bool> GetWhere(this VerbatimLineComment verbatimLineComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderVerbatimLineComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Text = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), verbatimLineComment.Text.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId && v.Text == verbatimLineComment.Text;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderAccessSpecifierDecl, bool> GetWhere(this AccessSpecifierDecl accessSpecifierDecl, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderAccessSpecifierDecl, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), accessSpecifierDecl.LocationIdentifier.ToString());
            where = (a) => a.HeaderFileId == tblSdkHeaderFile.HeaderFileId && a.LocationIdentifier == accessSpecifierDecl.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderAccessSpecifierDecl, bool> GetWhere(this AccessSpecifierDecl accessSpecifierDecl, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblOwningSDKHeaderClass)
        {
            Func<tblSDKHeaderAccessSpecifierDecl, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningClassId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderClass.ClassId.ToString(), accessSpecifierDecl.LocationIdentifier.ToString());
            where = (a) => a.HeaderFileId == tblSdkHeaderFile.HeaderFileId && a.OwningClassId == tblOwningSDKHeaderClass.ClassId && a.LocationIdentifier == accessSpecifierDecl.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderArrayType, bool> GetWhere(this ArrayType arrayType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderArrayType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (a) => a.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderBaseClassSpecifier, bool> GetWhere(this BaseClassSpecifier baseClassSpecifier, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderBaseClassSpecifier, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (b) => b.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderBaseClassSpecifier, bool> GetWhere(this BaseClassSpecifier baseClassSpecifier, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblOwningSDKHeaderClass)
        {
            Func<tblSDKHeaderBaseClassSpecifier, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningClassId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderClass.ClassId.ToString());
            where = (b) => b.HeaderFileId == tblSdkHeaderFile.HeaderFileId && b.OwningClassId == tblOwningSDKHeaderClass.ClassId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderBlockCommandComment, bool> GetWhere(this BlockCommandComment blockCommandComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderBlockCommandComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (b) => b.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderBlockContentComment, bool> GetWhere(this BlockContentComment blockContentComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderBlockContentComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (b) => b.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderBlockContentComment, bool> GetWhere(this BlockContentComment blockContentComment, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderComment tblOwningSDKHeaderComment)
        {
            Func<tblSDKHeaderBlockContentComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningCommentId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderComment.CommentId.ToString());
            where = (b) => b.HeaderFileId == tblSdkHeaderFile.HeaderFileId && b.OwningCommentId == tblOwningSDKHeaderComment.CommentId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderClass, bool> GetWhere(this Class _class, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderClass, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND ClassName = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), _class.ClassName.ToString(), _class.LocationIdentifier.ToString());
            where = (c) => c.HeaderFileId == tblSdkHeaderFile.HeaderFileId && c.ClassName == _class.ClassName && c.LocationIdentifier == _class.LocationIdentifier;

            return where;
        }
 
        public static Func<tblSDKHeaderDeclaration, bool> GetWhere(this Declaration declaration, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderDeclaration, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Name = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), declaration.Name.ToString(), declaration.LocationIdentifier.ToString());
            where = (d) => d.HeaderFileId == tblSdkHeaderFile.HeaderFileId && d.Name == declaration.Name && d.LocationIdentifier == declaration.LocationIdentifier;

            return where;
        }
        public static Func<tblSDKHeaderDeclaration, bool> GetWhere(this Declaration declaration, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclaration tblOwningSDKHeaderDeclaration)
        {
            Func<tblSDKHeaderDeclaration, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningDeclarationId = '{1}' AND Name = '{2}' AND LocationIdentifier = '{3}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderDeclaration.DeclarationId.ToString(), declaration.Name.ToString(), declaration.LocationIdentifier.ToString());
            where = (d) => d.HeaderFileId == tblSdkHeaderFile.HeaderFileId && d.OwningDeclarationId == tblOwningSDKHeaderDeclaration.DeclarationId && d.Name == declaration.Name && d.LocationIdentifier == declaration.LocationIdentifier;

            return where;
        }
 
        public static Func<tblSDKHeaderDeclarationContext, bool> GetWhere(this DeclarationContext declarationContext, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderDeclarationContext, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Name = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), declarationContext.Name.ToString(), declarationContext.LocationIdentifier.ToString());
            where = (d) => d.HeaderFileId == tblSdkHeaderFile.HeaderFileId && d.Name == declarationContext.Name && d.LocationIdentifier == declarationContext.LocationIdentifier;

            return where;
        }
 
        public static Func<tblSDKHeaderDependentNameType, bool> GetWhere(this DependentNameType dependentNameType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderDependentNameType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (d) => d.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderDependentTemplateSpecializationType, bool> GetWhere(this DependentTemplateSpecializationType dependentTemplateSpecializationType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderDependentTemplateSpecializationType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (d) => d.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderEnumeration, bool> GetWhere(this Enumeration enumeration, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderEnumeration, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), enumeration.LocationIdentifier.ToString());
            where = (e) => e.HeaderFileId == tblSdkHeaderFile.HeaderFileId && e.LocationIdentifier == enumeration.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderFriend, bool> GetWhere(this Friend friend, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderFriend, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), friend.LocationIdentifier.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId && f.LocationIdentifier == friend.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderFunctionTemplateSpecialization, bool> GetWhere(this FunctionTemplateSpecialization functionTemplateSpecialization, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderFunctionTemplateSpecialization, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderFunctionTemplateSpecialization, bool> GetWhere(this FunctionTemplateSpecialization functionTemplateSpecialization, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionTemplate tblOwningSDKHeaderFunctionTemplate)
        {
            Func<tblSDKHeaderFunctionTemplateSpecialization, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningFunctionTemplateId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderFunctionTemplate.FunctionTemplateId.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId && f.OwningFunctionTemplateId == tblOwningSDKHeaderFunctionTemplate.FunctionTemplateId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderFunctionType, bool> GetWhere(this FunctionType functionType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderFunctionType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderInlineCommandComment, bool> GetWhere(this InlineCommandComment inlineCommandComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderInlineCommandComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (i) => i.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderInlineContentComment, bool> GetWhere(this InlineContentComment inlineContentComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderInlineContentComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (i) => i.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderInlineContentComment, bool> GetWhere(this InlineContentComment inlineContentComment, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderComment tblOwningSDKHeaderComment)
        {
            Func<tblSDKHeaderInlineContentComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningCommentId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderComment.CommentId.ToString());
            where = (i) => i.HeaderFileId == tblSdkHeaderFile.HeaderFileId && i.OwningCommentId == tblOwningSDKHeaderComment.CommentId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderMacroExpansion, bool> GetWhere(this MacroExpansion macroExpansion, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderMacroExpansion, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Name = '{1}' AND Text = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), macroExpansion.Name.ToString(), macroExpansion.Text.ToString());
            where = (m) => m.HeaderFileId == tblSdkHeaderFile.HeaderFileId && m.Name == macroExpansion.Name && m.Text == macroExpansion.Text;

            return where;
        }
 
        public static Func<tblSDKHeaderMemberPointerType, bool> GetWhere(this MemberPointerType memberPointerType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderMemberPointerType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (m) => m.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderParamCommandComment, bool> GetWhere(this ParamCommandComment paramCommandComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderParamCommandComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (p) => p.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderPointerType, bool> GetWhere(this PointerType pointerType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderPointerType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (p) => p.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTagType, bool> GetWhere(this TagType tagType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTagType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTemplate, bool> GetWhere(this Template template, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTemplate, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), template.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.LocationIdentifier == template.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTemplateParameterSubstitutionType, bool> GetWhere(this TemplateParameterSubstitutionType templateParameterSubstitutionType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTemplateParameterSubstitutionType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTypeDefNameDecl, bool> GetWhere(this TypeDefNameDecl typeDefNameDecl, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTypeDefNameDecl, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), typeDefNameDecl.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.LocationIdentifier == typeDefNameDecl.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTypedefType, bool> GetWhere(this TypedefType typedefType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTypedefType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTypeTemplateParameter, bool> GetWhere(this TypeTemplateParameter typeTemplateParameter, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTypeTemplateParameter, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), typeTemplateParameter.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.LocationIdentifier == typeTemplateParameter.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderVarTemplatePartialSpecialization, bool> GetWhere(this VarTemplatePartialSpecialization varTemplatePartialSpecialization, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderVarTemplatePartialSpecialization, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), varTemplatePartialSpecialization.LocationIdentifier.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId && v.LocationIdentifier == varTemplatePartialSpecialization.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderVarTemplatePartialSpecialization, bool> GetWhere(this VarTemplatePartialSpecialization varTemplatePartialSpecialization, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVarTemplate tblOwningSDKHeaderVarTemplate)
        {
            Func<tblSDKHeaderVarTemplatePartialSpecialization, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningVarTemplateId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderVarTemplate.VarTemplateId.ToString(), varTemplatePartialSpecialization.LocationIdentifier.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId && v.OwningVarTemplateId == tblOwningSDKHeaderVarTemplate.VarTemplateId && v.LocationIdentifier == varTemplatePartialSpecialization.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderVarTemplateSpecialization, bool> GetWhere(this VarTemplateSpecialization varTemplateSpecialization, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderVarTemplateSpecialization, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), varTemplateSpecialization.LocationIdentifier.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId && v.LocationIdentifier == varTemplateSpecialization.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderVerbatimBlockLineComment, bool> GetWhere(this VerbatimBlockLineComment verbatimBlockLineComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderVerbatimBlockLineComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND Text = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), verbatimBlockLineComment.Text.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId && v.Text == verbatimBlockLineComment.Text;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderVerbatimBlockLineComment, bool> GetWhere(this VerbatimBlockLineComment verbatimBlockLineComment, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderComment tblOwningSDKHeaderComment)
        {
            Func<tblSDKHeaderVerbatimBlockLineComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningCommentId = '{1}' AND Text = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderComment.CommentId.ToString(), verbatimBlockLineComment.Text.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId && v.OwningCommentId == tblOwningSDKHeaderComment.CommentId && v.Text == verbatimBlockLineComment.Text;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderAttributedType, bool> GetWhere(this AttributedType attributedType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderAttributedType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (a) => a.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderBinaryOperator, bool> GetWhere(this BinaryOperator binaryOperator, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderBinaryOperator, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (b) => b.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderClassTemplateSpecialization, bool> GetWhere(this ClassTemplateSpecialization classTemplateSpecialization, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderClassTemplateSpecialization, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), classTemplateSpecialization.LocationIdentifier.ToString());
            where = (c) => c.HeaderFileId == tblSdkHeaderFile.HeaderFileId && c.LocationIdentifier == classTemplateSpecialization.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderClassTemplateSpecialization, bool> GetWhere(this ClassTemplateSpecialization classTemplateSpecialization, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClassTemplate tblOwningSDKHeaderClassTemplate)
        {
            Func<tblSDKHeaderClassTemplateSpecialization, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningClassTemplateId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderClassTemplate.ClassTemplateId.ToString(), classTemplateSpecialization.LocationIdentifier.ToString());
            where = (c) => c.HeaderFileId == tblSdkHeaderFile.HeaderFileId && c.OwningClassTemplateId == tblOwningSDKHeaderClassTemplate.ClassTemplateId && c.LocationIdentifier == classTemplateSpecialization.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderEnumerationItem, bool> GetWhere(this EnumerationItem enumerationItem, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderEnumerationItem, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), enumerationItem.LocationIdentifier.ToString());
            where = (e) => e.HeaderFileId == tblSdkHeaderFile.HeaderFileId && e.LocationIdentifier == enumerationItem.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderEnumerationItem, bool> GetWhere(this EnumerationItem enumerationItem, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderEnumeration tblOwningSDKHeaderEnumeration)
        {
            Func<tblSDKHeaderEnumerationItem, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningEnumerationId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderEnumeration.EnumerationId.ToString(), enumerationItem.LocationIdentifier.ToString());
            where = (e) => e.HeaderFileId == tblSdkHeaderFile.HeaderFileId && e.OwningEnumerationId == tblOwningSDKHeaderEnumeration.EnumerationId && e.LocationIdentifier == enumerationItem.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderFullComment, bool> GetWhere(this FullComment fullComment, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderFullComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderFullComment, bool> GetWhere(this FullComment fullComment, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderComment tblOwningSDKHeaderComment)
        {
            Func<tblSDKHeaderFullComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningCommentId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderComment.CommentId.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId && f.OwningCommentId == tblOwningSDKHeaderComment.CommentId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderFullComment, bool> GetWhere(this FullComment fullComment, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderRawComment tblOwningSDKHeaderRawComment)
        {
            Func<tblSDKHeaderFullComment, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningRawCommentId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderRawComment.RawCommentId.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId && f.OwningRawCommentId == tblOwningSDKHeaderRawComment.RawCommentId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderInjectedClassNameType, bool> GetWhere(this InjectedClassNameType injectedClassNameType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderInjectedClassNameType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (i) => i.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderMethod, bool> GetWhere(this Method method, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderMethod, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), method.LocationIdentifier.ToString());
            where = (m) => m.HeaderFileId == tblSdkHeaderFile.HeaderFileId && m.LocationIdentifier == method.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderMethod, bool> GetWhere(this Method method, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblOwningSDKHeaderClass)
        {
            Func<tblSDKHeaderMethod, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningClassId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderClass.ClassId.ToString(), method.LocationIdentifier.ToString());
            where = (m) => m.HeaderFileId == tblSdkHeaderFile.HeaderFileId && m.OwningClassId == tblOwningSDKHeaderClass.ClassId && m.LocationIdentifier == method.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderNonTypeTemplateParameter, bool> GetWhere(this NonTypeTemplateParameter nonTypeTemplateParameter, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderNonTypeTemplateParameter, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), nonTypeTemplateParameter.LocationIdentifier.ToString());
            where = (n) => n.HeaderFileId == tblSdkHeaderFile.HeaderFileId && n.LocationIdentifier == nonTypeTemplateParameter.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTemplateParameter, bool> GetWhere(this TemplateParameter templateParameter, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTemplateParameter, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), templateParameter.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.LocationIdentifier == templateParameter.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTemplateSpecializationType, bool> GetWhere(this TemplateSpecializationType templateSpecializationType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTemplateSpecializationType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTypeDef, bool> GetWhere(this TypeDef typeDef, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTypeDef, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), typeDef.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.LocationIdentifier == typeDef.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderTypeDef, bool> GetWhere(this TypeDef typeDef, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblOwningSDKHeaderDeclarationContext)
        {
            Func<tblSDKHeaderTypeDef, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningDeclarationContextId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderDeclarationContext.DeclarationContextId.ToString(), typeDef.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.OwningDeclarationContextId == tblOwningSDKHeaderDeclarationContext.DeclarationContextId && t.LocationIdentifier == typeDef.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderUnaryTransformType, bool> GetWhere(this UnaryTransformType unaryTransformType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderUnaryTransformType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (u) => u.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderVariable, bool> GetWhere(this Variable variable, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderVariable, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), variable.LocationIdentifier.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId && v.LocationIdentifier == variable.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderVariable, bool> GetWhere(this Variable variable, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblOwningSDKHeaderDeclarationContext)
        {
            Func<tblSDKHeaderVariable, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningDeclarationContextId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderDeclarationContext.DeclarationContextId.ToString(), variable.LocationIdentifier.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId && v.OwningDeclarationContextId == tblOwningSDKHeaderDeclarationContext.DeclarationContextId && v.LocationIdentifier == variable.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderVectorType, bool> GetWhere(this VectorType vectorType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderVectorType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderVTableComponent, bool> GetWhere(this VTableComponent vTableComponent, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderVTableComponent, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderVTableComponent, bool> GetWhere(this VTableComponent vTableComponent, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVTableLayout tblOwningSDKHeaderVTableLayout)
        {
            Func<tblSDKHeaderVTableComponent, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningVTableLayoutId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderVTableLayout.VTableLayoutId.ToString());
            where = (v) => v.HeaderFileId == tblSdkHeaderFile.HeaderFileId && v.OwningVTableLayoutId == tblOwningSDKHeaderVTableLayout.VTableLayoutId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderDecayedType, bool> GetWhere(this DecayedType decayedType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderDecayedType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (d) => d.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderField, bool> GetWhere(this Field field, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderField, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), field.LocationIdentifier.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId && f.LocationIdentifier == field.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderField, bool> GetWhere(this Field field, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClass tblOwningSDKHeaderClass)
        {
            Func<tblSDKHeaderField, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningClassId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderClass.ClassId.ToString(), field.LocationIdentifier.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId && f.OwningClassId == tblOwningSDKHeaderClass.ClassId && f.LocationIdentifier == field.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderFunction, bool> GetWhere(this Function function, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderFunction, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND FunctionName = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), function.FunctionName.ToString());
            where = (f) => f.HeaderFileId == tblSdkHeaderFile.HeaderFileId && f.FunctionName == function.FunctionName;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTemplateParameterType, bool> GetWhere(this TemplateParameterType templateParameterType, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTemplateParameterType, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTypeAlia, bool> GetWhere(this TypeAlias typeAlias, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTypeAlia, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), typeAlias.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.LocationIdentifier == typeAlias.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderTypeAlia, bool> GetWhere(this TypeAlias typeAlias, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDeclarationContext tblOwningSDKHeaderDeclarationContext)
        {
            Func<tblSDKHeaderTypeAlia, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningDeclarationContextId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderDeclarationContext.DeclarationContextId.ToString(), typeAlias.LocationIdentifier.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.OwningDeclarationContextId == tblOwningSDKHeaderDeclarationContext.DeclarationContextId && t.LocationIdentifier == typeAlias.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderParameter, bool> GetWhere(this Parameter parameter, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderParameter, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND LocationIdentifier = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), parameter.LocationIdentifier.ToString());
            where = (p) => p.HeaderFileId == tblSdkHeaderFile.HeaderFileId && p.LocationIdentifier == parameter.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderParameter, bool> GetWhere(this Parameter parameter, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionType tblOwningSDKHeaderFunctionType)
        {
            Func<tblSDKHeaderParameter, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningFunctionTypeId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderFunctionType.FunctionTypeId.ToString(), parameter.LocationIdentifier.ToString());
            where = (p) => p.HeaderFileId == tblSdkHeaderFile.HeaderFileId && p.OwningFunctionTypeId == tblOwningSDKHeaderFunctionType.FunctionTypeId && p.LocationIdentifier == parameter.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderParameter, bool> GetWhere(this Parameter parameter, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunction tblOwningSDKHeaderFunction)
        {
            Func<tblSDKHeaderParameter, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningFunctionId = '{1}' AND LocationIdentifier = '{2}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderFunction.FunctionId.ToString(), parameter.LocationIdentifier.ToString());
            where = (p) => p.HeaderFileId == tblSdkHeaderFile.HeaderFileId && p.OwningFunctionId == tblOwningSDKHeaderFunction.FunctionId && p.LocationIdentifier == parameter.LocationIdentifier;

            throw new NotImplementedException();

            return where;
        }
 
        public static Func<tblSDKHeaderTemplateArgument, bool> GetWhere(this TemplateArgument templateArgument, tblSDKHeaderFile tblSdkHeaderFile)
        {
            Func<tblSDKHeaderTemplateArgument, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}'", tblSdkHeaderFile.HeaderFileId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderTemplateArgument, bool> GetWhere(this TemplateArgument templateArgument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderClassTemplateSpecialization tblOwningSDKHeaderClassTemplateSpecialization)
        {
            Func<tblSDKHeaderTemplateArgument, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningClassTemplateSpecializationId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderClassTemplateSpecialization.ClassTemplateSpecializationId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.OwningClassTemplateSpecializationId == tblOwningSDKHeaderClassTemplateSpecialization.ClassTemplateSpecializationId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderTemplateArgument, bool> GetWhere(this TemplateArgument templateArgument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderDependentTemplateSpecializationType tblOwningSDKHeaderDependentTemplateSpecializationType)
        {
            Func<tblSDKHeaderTemplateArgument, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningDependentTemplateSpecializationTypeId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderDependentTemplateSpecializationType.DependentTemplateSpecializationTypeId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.OwningDependentTemplateSpecializationTypeId == tblOwningSDKHeaderDependentTemplateSpecializationType.DependentTemplateSpecializationTypeId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderTemplateArgument, bool> GetWhere(this TemplateArgument templateArgument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderVarTemplateSpecialization tblOwningSDKHeaderVarTemplateSpecialization)
        {
            Func<tblSDKHeaderTemplateArgument, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningVarTemplateSpecializationId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderVarTemplateSpecialization.VarTemplateSpecializationId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.OwningVarTemplateSpecializationId == tblOwningSDKHeaderVarTemplateSpecialization.VarTemplateSpecializationId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderTemplateArgument, bool> GetWhere(this TemplateArgument templateArgument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderTemplateSpecializationType tblOwningSDKHeaderTemplateSpecializationType)
        {
            Func<tblSDKHeaderTemplateArgument, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningTemplateSpecializationTypeId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderTemplateSpecializationType.TemplateSpecializationTypeId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.OwningTemplateSpecializationTypeId == tblOwningSDKHeaderTemplateSpecializationType.TemplateSpecializationTypeId;

            throw new NotImplementedException();

            return where;
        }
        public static Func<tblSDKHeaderTemplateArgument, bool> GetWhere(this TemplateArgument templateArgument, tblSDKHeaderFile tblSdkHeaderFile, tblSDKHeaderFunctionTemplateSpecialization tblOwningSDKHeaderFunctionTemplateSpecialization)
        {
            Func<tblSDKHeaderTemplateArgument, bool> where;

            CurrentWhere = string.Format("HeaderFileId = '{0}' AND OwningFunctionTemplateSpecializationId = '{1}'", tblSdkHeaderFile.HeaderFileId.ToString(), tblOwningSDKHeaderFunctionTemplateSpecialization.FunctionTemplateSpecializationId.ToString());
            where = (t) => t.HeaderFileId == tblSdkHeaderFile.HeaderFileId && t.OwningFunctionTemplateSpecializationId == tblOwningSDKHeaderFunctionTemplateSpecialization.FunctionTemplateSpecializationId;

            throw new NotImplementedException();

            return where;
        }
    }
}
