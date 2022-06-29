using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using CodeInterfaces;
using System.IO;
using Microsoft.CodeAnalysis.Text;

namespace AbstraX.Parser
{
    public static class ParserExtensions
    {
        public static bool HasThisExpression(this MemberAccessExpressionSyntax memberAccessExpression)
        {
            var simpleName = memberAccessExpression.Name;

            while (simpleName != null)
            {
                if (memberAccessExpression.Expression is MemberAccessExpressionSyntax)
                {
                    memberAccessExpression = (MemberAccessExpressionSyntax)memberAccessExpression.Expression;
                    simpleName = memberAccessExpression.Name;
                }
                else if (memberAccessExpression.Expression is SimpleNameSyntax)
                {
                    simpleName = (SimpleNameSyntax)memberAccessExpression.Expression;

                    break;
                }
                else if (memberAccessExpression.Expression is ThisExpressionSyntax)
                {
                    return true;
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            return false;
        }

        public static IEnumerable<SimpleNameSyntax> GetMemberIdentifiers(this MemberAccessExpressionSyntax memberAccessExpression, bool skipThis = false)
        {
            var list = new List<SimpleNameSyntax>();
            var simpleName = memberAccessExpression.Name;

            list.Add(simpleName);

            while (simpleName != null)
            {
                if (memberAccessExpression.Expression is MemberAccessExpressionSyntax)
                {
                    memberAccessExpression = (MemberAccessExpressionSyntax) memberAccessExpression.Expression;
                    simpleName = memberAccessExpression.Name;
                }
                else if (memberAccessExpression.Expression is SimpleNameSyntax)
                {
                    simpleName = (SimpleNameSyntax) memberAccessExpression.Expression;
                    list.Add(simpleName);

                    break;
                }
                else if (memberAccessExpression.Expression is ThisExpressionSyntax)
                {
                    if (skipThis)
                    {
                        break;
                    }
                    else
                    {
                        DebugUtils.Break();
                    }
                }
                else
                {
                    DebugUtils.Break();
                }

                list.Add(simpleName);
            }

            return list.Reverse<SimpleNameSyntax>();
        }

        public static TypeSyntax GetReturnTypeOfBlockChild(this SyntaxNode node)
        {
            var hasReturn = node.DescendantNodesAndSelf().OfType<ReturnStatementSyntax>().Any();
            TypeSyntax returnType = null;

            if (hasReturn)
            {
                var baseMethod = node.Ancestors().OfType<BaseMethodDeclarationSyntax>().FirstOrDefault();

                if (baseMethod == null)
                {
                    var baseProperty = node.Ancestors().OfType<BasePropertyDeclarationSyntax>().FirstOrDefault();

                    switch (baseProperty)
                    {
                        case PropertyDeclarationSyntax property:
                            returnType = property.Type;
                            break;
                        case IndexerDeclarationSyntax indexer:
                            returnType = indexer.Type;
                            break;
                        default:
                            DebugUtils.Break();
                            break;
                    }
                }
                else
                {
                    switch (baseMethod)
                    {
                        case MethodDeclarationSyntax method:
                            returnType = method.ReturnType;
                            break;
                        default:
                            DebugUtils.Break();
                            break;
                    }
                }
            }

            return returnType;
        }

        public static Dictionary<VariableDeclaratorSyntax, TypeSyntax> GetVariableTypes(this FixedStatementSyntax fixedStatement)
        {
            var variableTypes = new Dictionary<VariableDeclaratorSyntax, TypeSyntax>();
            var variableDeclaration = fixedStatement.Declaration;
            var type = variableDeclaration.Type;
            var variables = variableDeclaration.Variables.Cast<VariableDeclaratorSyntax>();
            var statement = fixedStatement.Statement;

            foreach (var variable in variables)
            {
                variableTypes.Add(variable, type);
            }

            if (statement is FixedStatementSyntax fixedStatementInner)
            {
                var innerVariableTypes = fixedStatementInner.GetVariableTypes();

                foreach (var variableType in innerVariableTypes)
                {
                    variableTypes.Add(variableType.Key, variableType.Value);
                }
            }

            return variableTypes;
        }

        public static T AddPrecedingComment<T>(this T node, int tabCount, string comment) where T : SyntaxNode
        {
            var list = new List<SyntaxTrivia>();

            list.Add(CarriageReturnLineFeed);

            for (var x = 0; x < tabCount; x++)
            {
                list.Add(Tab);
            }

            list.Add(Comment("// " + comment));
            list.Add(CarriageReturnLineFeed);
            list.Add(CarriageReturnLineFeed);

            node = node.WithLeadingTrivia(list.ToArray());

            return node;
        }

        public static bool IsDebugNode(this SyntaxNode node)
        {
            // used for debugging to find a particular node.  Code below expected to change per each case.

            if (((SyntaxNode)node) is IdentifierNameSyntax)
            {
                if (node.ToFullString() == "DefaultCharUsed")
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsTypeParameterType(this SyntaxNode node, TypeSyntax type)
        {
            var method = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var typeDeclaration = node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
            var hasTypeOrMethod = false;

            if (method != null)
            {
                if (method.TypeParameterList != null && method.TypeParameterList.Parameters.Any(p => p.ToFullString() == type.ToString()))
                {
                    return true;
                }

                hasTypeOrMethod = true;
            }

            if (typeDeclaration != null)
            {
                if (typeDeclaration.TypeParameterList != null && typeDeclaration.TypeParameterList.Parameters.Any(p => p.ToFullString() == type.ToString()))
                {
                    return true;
                }
                else
                {
                    foreach (var ancestorType in node.Ancestors().OfType<TypeDeclarationSyntax>().Skip(1))
                    {
                        if (ancestorType.TypeParameterList != null && ancestorType.TypeParameterList.Parameters.Any(p => p.ToFullString() == type.ToString()))
                        {
                            return true;
                        }
                    }
                }

                hasTypeOrMethod = true;
            }

            if (!hasTypeOrMethod)
            {
                DebugUtils.Break();
            }

            return false;
        }

        public static bool IsTypeClass(this TypeDeclarationSyntax classDeclaration, string fileName)
        {
            return classDeclaration.Identifier.Text == "Type" && Path.GetFileNameWithoutExtension(fileName) == "Type";
        }

        public static bool IsObjectClass(this TypeDeclarationSyntax classDeclaration, string fileName)
        {
            return classDeclaration.Identifier.Text == "Object" && Path.GetFileNameWithoutExtension(fileName) == "Object";
        }

        public static bool IsValueTypeClass(this TypeDeclarationSyntax typeDeclaration, string fileName)
        {
            return typeDeclaration.Identifier.Text == "Struct" && Path.GetFileNameWithoutExtension(fileName) == "ValueType";
        }

        public static bool IsTypeInfoNode(this SyntaxNode syntaxNode)
        {
            switch (syntaxNode)
            {
                case ExpressionSyntax expression:
                    return true;
                case ConstructorInitializerSyntax initializer:
                    return true;
                case AttributeSyntax attribute:
                    return true;
                case SelectOrGroupClauseSyntax selectOrGroupClause:
                    return true;
                default:
                    return false;
            }
        }

        public static IEnumerable<SyntaxNode> GetSiblings(this SyntaxNode node)
        {
            var parent = node.Parent;

            if (parent != null)
            {
                var descendants = parent.DescendantNodes().Where(n => n != node);

                return descendants;
            }

            return new List<SyntaxNode>();
        }

        public static int GetParentIndex(this SyntaxNode node)
        {
            var parent = node.Parent;
            var index = 0;

            if (parent != null)
            {
                index = parent.DescendantNodes().IndexOf(node);
            }

            return index;
        }

        public static IEnumerable<SyntaxNode> GetLowerSiblings(this SyntaxNode node)
        {
            var parent = node.Parent;
            var index = node.GetParentIndex();
            var lowerSiblings = node.GetSiblings().Where(s => node.GetParentIndex() > index);

            return lowerSiblings;
        }

        public static IEnumerable<SyntaxNodeOrToken> GetSubsequentNodesAndTokens(this SyntaxNode node)
        {
            var list = node.DescendantNodesAndTokens().ToList();

            foreach (var ancestorAndSelf in node.AncestorsAndSelf())
            {
                foreach (var lowerSibling in ancestorAndSelf.GetLowerSiblings())
                {
                    list.Add(lowerSibling);
                }
            }

            return list;
        }

        public static IEnumerable<SyntaxNodeOrToken> GetSubsequentNodesAndTokens(this SyntaxToken token)
        {
            var parent = token.Parent;
            var list = new List<SyntaxNodeOrToken>();

            foreach (var ancestorAndSelf in parent.AncestorsAndSelf())
            {
                foreach (var lowerSibling in ancestorAndSelf.GetLowerSiblings())
                {
                    list.Add(lowerSibling);
                }
            }

            return list;
        }

        public static bool IsValidConstraint(this TypeParameterConstraintSyntax typeParameterConstraint)
        {
            if (typeParameterConstraint is TypeConstraintSyntax typeConstraint)
            {
                var isValid = typeConstraint.Type.GetRighthandName().IsOneOf("IObject", "ValueType");

                return isValid;
            }
            else if (typeParameterConstraint is ClassOrStructConstraintSyntax)
            {
                return false;
            }
            else
            {
                DebugUtils.Break();
            }

            return false;
        }

        public static string ToVariableName(this string identifier)
        {
            if (identifier == "IObject")
            {
                identifier = "object";
            }

            if (identifier.Contains("<"))
            {
                identifier = identifier.ToNonGenericName();
            }

            if (identifier.Contains("."))
            {
                identifier = identifier.RightAtLastIndexOf('.');
            }

            identifier = identifier.ToCamelCase();

            if (identifier.IsPrimitiveTypeName())
            {
                identifier = "_" + identifier;
            }

            if (identifier.Contains("."))
            {
                identifier = identifier.RightAtLastIndexOf('.');
            }

            if (identifier == "enum")
            {
                identifier = "_enum";
            }

            return identifier;
        }

        public static string ToNonGenericName(this string identifier)
        {
            if (identifier.Contains("<"))
            {
                identifier = identifier.LeftUpToIndexOf('<');
            }

            return identifier;
        }

        public static int GetOrderRank(this IVSProjectItem item)
        {
            var fileName = Path.GetFileName(item.FileName);

            switch (fileName)
            {
                case "Global.cs":
                    return 2;
                default:
                    return 1;
            }
        }

        public static string GetSignature(this MemberDeclarationSyntax member, bool includeModifiers = false)
        {
            if (member is MethodDeclarationSyntax)
            {
                return GenerateSignature((MethodDeclarationSyntax) member, includeModifiers);
            }
            else
            {
                DebugUtils.Break();

                return member.ToString();
            }
        }

        public static string GetSignature(this MethodDeclarationSyntax method, bool includeModifiers = false)
        {
            return GenerateSignature(method, includeModifiers);
        }

        public static string GenerateSignature(this MethodDeclarationSyntax method, bool includeModifiers = false)
        {
            var type = method.ReturnType.ToFullString();
            var name = method.Identifier.Text;
            var modifierString = string.Empty;

            if (includeModifiers)
            {
                modifierString = method.Modifiers.ToFullString();
            }

            return modifierString + type + " " + name + method.ParameterList.ToFullString().TrimEnd();
        }

        public static IEnumerable<SyntaxTrivia> GetAllTrivia(this SyntaxNodeOrToken nodeOrToken)
        {
            return nodeOrToken.GetLeadingTrivia().ToSyntaxTriviaList().Concat(nodeOrToken.GetTrailingTrivia().ToSyntaxTriviaList());
        }

        public static string GetNameForOperator(this SyntaxToken operatorToken)
        {
            var words = operatorToken.Kind().ToString().RemoveEndIfMatches("Token").SplitTitleCaseIdentifierToWords().Distinct().ToList();
            var list = new List<string>();

            foreach (var word in words)
            {
                switch (word)
                {
                    case "Equals":

                        if (list.Count == 0)
                        {
                            list.Add("DoesEqual");
                        }
                        else
                        {
                            list.Add(word);
                        }

                        break;

                    case "Exclamation":
                        list.Add("Not");
                        break;
                    default:
                        list.Add(word);
                        break;
                }
            }

            return string.Join(string.Empty, list);
        }

        public static List<string> GetAllMemberNames(this TypeDeclarationSyntax node)
        {
            var methods = node.Members.OfType<MethodDeclarationSyntax>().Select(m => m.Identifier.Text);
            var fields = node.Members.OfType<FieldDeclarationSyntax>().SelectMany(f => f.Declaration.Variables).Select(v => v.Identifier.Text);
            var properties = node.Members.OfType<PropertyDeclarationSyntax>().Select(m => m.Identifier.Text);
            var events = node.Members.OfType<EventDeclarationSyntax>().Select(m => m.Identifier.Text);
            var operators = node.Members.OfType<ConversionOperatorDeclarationSyntax>().Select(m => m.Type.GetName());
            var types = node.Members.OfType<TypeDeclarationSyntax>().Select(m => m.Identifier.Text);
            var allNames = methods.Concat(properties).Concat(events).Concat(operators).Concat(types).ToList();

            return allNames;
        }

        public static bool HasGetter(this BasePropertyDeclarationSyntax property)
        {
            return property.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
        }

        public static bool HasSetter(this BasePropertyDeclarationSyntax property)
        {
            return property.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration);
        }

        public static SyntaxList<AttributeListSyntax> WithoutTrivia(this SyntaxList<AttributeListSyntax> attributeList)
        {
            var list = new SyntaxList<AttributeListSyntax>();

            foreach (var listItem in attributeList)
            {
                var attributes = new SeparatedSyntaxList<AttributeSyntax>();

                foreach (var attribute in listItem.Attributes)
                {
                    attributes = attributes.Add(attribute.WithoutTrivia());
                }

                list = list.Add(AttributeList(attributes));
            }

            return list;
        }

        public static IEnumerable<SyntaxToken> WithoutExtraTrivia(this IEnumerable<SyntaxToken> tokens)
        {
            return tokens.Select(t => t.WithoutTrivia().WithTrailingTrivia(Space));
        }

        public static SyntaxList<MemberDeclarationSyntax> ConvertToIdexerWithBodies(this IndexerDeclarationSyntax indexer)
        {
            var list = new SyntaxList<AccessorDeclarationSyntax>();
            var tabs = ParserExtensions.GetTabs(1);

            if (indexer.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration))
            {
                var newStatements = new List<StatementSyntax>();
                var breakStatement = ExpressionStatement(ParseExpression("DebugUtils.Break()").WithLeadingTrivia(tabs)).WithTrailingTrivia(CarriageReturnLineFeed);
                AccessorDeclarationSyntax accessor;

                newStatements.Add(breakStatement);
                newStatements.Add(ReturnStatement(DefaultExpression(indexer.Type).WithLeadingTrivia(Space)).WithLeadingTrivia(tabs).WithTrailingTrivia(CarriageReturnLineFeed));

                accessor = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(Space)), Block(Token(SyntaxKind.OpenBraceToken).WithLeadingTrivia(CarriageReturnLineFeed).WithTrailingTrivia(CarriageReturnLineFeed), new SyntaxList<StatementSyntax>(newStatements), Token(SyntaxKind.CloseBraceToken).WithLeadingTrivia(CarriageReturnLineFeed).WithTrailingTrivia(CarriageReturnLineFeed)), null);

                list = list.Add(accessor);
            }

            if (indexer.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration))
            {
                var newStatements = new List<StatementSyntax>();
                var breakStatement = ExpressionStatement(ParseExpression("DebugUtils.Break()").WithLeadingTrivia(tabs)).WithTrailingTrivia(CarriageReturnLineFeed);
                AccessorDeclarationSyntax accessor;

                newStatements.Add(breakStatement);
                newStatements.Add(ReturnStatement(DefaultExpression(indexer.Type).WithLeadingTrivia(Space)).WithLeadingTrivia(tabs).WithTrailingTrivia(CarriageReturnLineFeed));

                accessor = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(Space)), Block(Token(SyntaxKind.OpenBraceToken).WithLeadingTrivia(CarriageReturnLineFeed).WithTrailingTrivia(CarriageReturnLineFeed), new SyntaxList<StatementSyntax>(newStatements), Token(SyntaxKind.CloseBraceToken).WithLeadingTrivia(CarriageReturnLineFeed).WithTrailingTrivia(CarriageReturnLineFeed)), null);

                list = list.Add(accessor);
            }

            return new SyntaxList<MemberDeclarationSyntax>().Add(IndexerDeclaration(new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(Space)), indexer.Type, null, indexer.ParameterList, AccessorList(list)));
        }

        public static ArgumentListSyntax ToSingleItemList(this ArgumentSyntax arg)
        {
            var args = new SeparatedSyntaxList<ArgumentSyntax>();

            args = args.Add(arg);

            return ArgumentList(args);
        }

        public static bool IsVoid(this TypeSyntax type)
        {
            return type == null || (type is PredefinedTypeSyntax && ((PredefinedTypeSyntax)type).Keyword.Text == "void");
        }

        public static ITypeSymbol GetPointerRelatedType(this ITypeSymbol symbol)
        {
            ITypeSymbol pointedAtSymbol = null;

            if (symbol is IPointerTypeSymbol)
            {
                pointedAtSymbol = ((IPointerTypeSymbol)symbol).PointedAtType;
            }
            else if (symbol is INamedTypeSymbol)
            {
                pointedAtSymbol = (INamedTypeSymbol)symbol;
            }

            return pointedAtSymbol;
        }

        public static bool MatchesScope(this SyntaxNode node, SyntaxNode foreignNode)
        {
            if (node is MemberDeclarationSyntax && foreignNode is MemberDeclarationSyntax)
            {
                var member = (MemberDeclarationSyntax) node;
                var foreignMember = (MemberDeclarationSyntax) foreignNode;

                if (member is MethodDeclarationSyntax && foreignMember is MethodDeclarationSyntax)
                {
                    var method = (MethodDeclarationSyntax)member;
                    var foreignMethod = (MethodDeclarationSyntax)foreignMember;
                    var parms = method.ParameterList.Parameters;
                    var foreignParms = foreignMethod.ParameterList.Parameters;

                    if (method.Identifier.Text == foreignMethod.Identifier.Text && parms.ParmSignaturesMatch(foreignParms))
                    {
                        return true;
                    }
                }
                else if (member is ConstructorDeclarationSyntax && foreignMember is ConstructorDeclarationSyntax)
                {
                    var constructor = (ConstructorDeclarationSyntax)member;
                    var foreignConstructor = (ConstructorDeclarationSyntax)foreignMember;
                    var parms = constructor.ParameterList.Parameters;
                    var foreignParms = foreignConstructor.ParameterList.Parameters;

                    if (constructor.Identifier.Text == foreignConstructor.Identifier.Text && parms.ParmSignaturesMatch(foreignParms))
                    {
                        return true;
                    }
                }
                else if (member is PropertyDeclarationSyntax && foreignMember is PropertyDeclarationSyntax)
                {
                    var property = (PropertyDeclarationSyntax)member;
                    var foreignProperty = (PropertyDeclarationSyntax)foreignMember;
                    var accessorKind = node.Ancestors().OfType<AccessorDeclarationSyntax>().First().Kind();
                    var foreignAccessorKind = foreignNode.Ancestors().OfType<AccessorDeclarationSyntax>().First().Kind();

                    if (property.Identifier.Text == foreignProperty.Identifier.Text && accessorKind == foreignAccessorKind)
                    {
                        return true;
                    }
                }
                else if (member is IndexerDeclarationSyntax && foreignMember is IndexerDeclarationSyntax)
                {
                    var indexer = (IndexerDeclarationSyntax)member;
                    var foreignIndexer = (IndexerDeclarationSyntax)foreignMember;
                    var accessorKind = node.Ancestors().OfType<AccessorDeclarationSyntax>().First().Kind();
                    var foreignAccessorKind = foreignNode.Ancestors().OfType<AccessorDeclarationSyntax>().First().Kind();
                    var parms = indexer.ParameterList.Parameters;
                    var foreignParms = foreignIndexer.ParameterList.Parameters;

                    if (parms.ParmSignaturesMatch(foreignParms) && accessorKind == foreignAccessorKind)
                    {
                        return true;
                    }
                }
                else if (member is TypeDeclarationSyntax && foreignMember is TypeDeclarationSyntax)
                {
                    var type = (TypeDeclarationSyntax)member;
                    var foreignType = (TypeDeclarationSyntax)foreignMember;

                    if (type.Identifier == foreignType.Identifier)
                    {
                        if (type.TypeParameterList == null && foreignType.TypeParameterList == null)
                        {
                            return true;
                        }
                        else
                        {
                            var parms = type.TypeParameterList.Parameters;
                            var foreignParms = foreignType.TypeParameterList.Parameters;

                            if (parms.ParmSignaturesMatch(foreignParms))
                            {
                                if (parms.Any(p => p == node || node.Ancestors().Any(a => a == node)))
                                {
                                    DebugUtils.Break();
                                }
                                else
                                {
                                    var blockIndex = type.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(node.Ancestors().OfType<BlockSyntax>().First());
                                    var foreignBlockIndex = foreignType.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(foreignNode.Ancestors().OfType<BlockSyntax>().First());

                                    if (blockIndex == foreignBlockIndex)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (node.Ancestors().OfType<BlockSyntax>().Any() && foreignNode.Ancestors().OfType<BlockSyntax>().Any())
            {
                if (node.Ancestors().OfType<BlockSyntax>().Any() && foreignNode.Ancestors().OfType<BlockSyntax>().Any())
                {
                    var member = node.Ancestors().OfType<MemberDeclarationSyntax>().First();
                    var foreignMember = foreignNode.Ancestors().OfType<MemberDeclarationSyntax>().First();

                    if (member is MethodDeclarationSyntax && foreignMember is MethodDeclarationSyntax)
                    {
                        var method = (MethodDeclarationSyntax)member;
                        var foreignMethod = (MethodDeclarationSyntax)foreignMember;
                        var parms = method.ParameterList.Parameters;
                        var foreignParms = foreignMethod.ParameterList.Parameters;

                        if (method.Identifier.Text == foreignMethod.Identifier.Text && parms.ParmSignaturesMatch(foreignParms))
                        {
                            var blockIndex = method.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(node.Ancestors().OfType<BlockSyntax>().First());
                            var foreignBlockIndex = foreignMethod.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(foreignNode.Ancestors().OfType<BlockSyntax>().First());

                            if (blockIndex == foreignBlockIndex)
                            {
                                return true;
                            }
                        }
                    }
                    else if (member is ConstructorDeclarationSyntax && foreignMember is ConstructorDeclarationSyntax)
                    {
                        var constructor = (ConstructorDeclarationSyntax)member;
                        var foreignConstructor = (ConstructorDeclarationSyntax)foreignMember;
                        var parms = constructor.ParameterList.Parameters;
                        var foreignParms = foreignConstructor.ParameterList.Parameters;

                        if (constructor.Identifier.Text == foreignConstructor.Identifier.Text && parms.ParmSignaturesMatch(foreignParms))
                        {
                            var blockIndex = constructor.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(node.Ancestors().OfType<BlockSyntax>().First());
                            var foreignBlockIndex = foreignConstructor.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(foreignNode.Ancestors().OfType<BlockSyntax>().First());

                            if (blockIndex == foreignBlockIndex)
                            {
                                return true;
                            }
                        }
                    }
                    else if (member is PropertyDeclarationSyntax && foreignMember is PropertyDeclarationSyntax)
                    {
                        var property = (PropertyDeclarationSyntax)member;
                        var foreignProperty = (PropertyDeclarationSyntax)foreignMember;
                        var accessorKind = node.Ancestors().OfType<AccessorDeclarationSyntax>().First().Kind();
                        var foreignAccessorKind = foreignNode.Ancestors().OfType<AccessorDeclarationSyntax>().First().Kind();

                        if (property.Identifier.Text == foreignProperty.Identifier.Text && accessorKind == foreignAccessorKind)
                        {
                            var blockIndex = property.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(node.Ancestors().OfType<BlockSyntax>().First());
                            var foreignBlockIndex = foreignProperty.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(foreignNode.Ancestors().OfType<BlockSyntax>().First());

                            if (blockIndex == foreignBlockIndex)
                            {
                                return true;
                            }
                        }
                    }
                    else if (member is IndexerDeclarationSyntax && foreignMember is IndexerDeclarationSyntax)
                    {
                        var indexer = (IndexerDeclarationSyntax)member;
                        var foreignIndexer = (IndexerDeclarationSyntax)foreignMember;
                        var accessorKind = node.Ancestors().OfType<AccessorDeclarationSyntax>().First().Kind();
                        var foreignAccessorKind = foreignNode.Ancestors().OfType<AccessorDeclarationSyntax>().First().Kind();
                        var parms = indexer.ParameterList.Parameters;
                        var foreignParms = foreignIndexer.ParameterList.Parameters;

                        if (parms.ParmSignaturesMatch(foreignParms) && accessorKind == foreignAccessorKind)
                        {
                            var blockIndex = indexer.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(node.Ancestors().OfType<BlockSyntax>().First());
                            var foreignBlockIndex = foreignIndexer.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(foreignNode.Ancestors().OfType<BlockSyntax>().First());

                            if (blockIndex == foreignBlockIndex)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else if (node.Ancestors().OfType<AttributeSyntax>().Any() && foreignNode.Ancestors().OfType<AttributeSyntax>().Any())
            {
                var attribute = (AttributeSyntax)node.Ancestors().OfType<AttributeSyntax>().Single();
                var foreignAttribute = (AttributeSyntax)foreignNode.Ancestors().OfType<AttributeSyntax>().Single();

                if (attribute.Name == foreignAttribute.Name)
                {
                    var member = node.Ancestors().OfType<MemberDeclarationSyntax>().First();
                    var foreignMember = foreignNode.Ancestors().OfType<MemberDeclarationSyntax>().First();

                    return MatchesScope(member, foreignMember);
                }
            }
            else if (node.Ancestors().OfType<ConstructorDeclarationSyntax>().Any() && foreignNode.Ancestors().OfType<ConstructorDeclarationSyntax>().Any())
            {
                var constructor = (ConstructorDeclarationSyntax)node.Ancestors().OfType<ConstructorDeclarationSyntax>().Single();
                var foreignConstructor = (ConstructorDeclarationSyntax)foreignNode.Ancestors().OfType<ConstructorDeclarationSyntax>().Single();
                var parms = constructor.ParameterList.Parameters;
                var foreignParms = foreignConstructor.ParameterList.Parameters;

                if (constructor.Identifier.Text == foreignConstructor.Identifier.Text && parms.ParmSignaturesMatch(foreignParms))
                {
                    if (parms.Any(p => p == node || node.Ancestors().Any(a => a == p)))
                    {
                        return true;
                    }
                    else
                    {
                        var blockIndex = constructor.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(node.Ancestors().OfType<BlockSyntax>().First());
                        var foreignBlockIndex = foreignConstructor.DescendantNodesAndSelf().OfType<BlockSyntax>().IndexOf(foreignNode.Ancestors().OfType<BlockSyntax>().First());

                        if (blockIndex == foreignBlockIndex)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static string GetName(this TypeSyntax typeSyntax)
        {
            var kind = typeSyntax.Kind();

            switch (kind)
            {
                case SyntaxKind.PointerType:
                    {
                        var type = (PointerTypeSyntax)typeSyntax;

                        return "*" + type.ElementType.GetName();
                    }
                case SyntaxKind.PredefinedType:
                    {
                        var type = (PredefinedTypeSyntax)typeSyntax;

                        return type.Keyword.Text;
                    }
                case SyntaxKind.IdentifierName:
                    {
                        var identifierName = (IdentifierNameSyntax)typeSyntax;

                        return identifierName.Identifier.Text;
                    }
                case SyntaxKind.GenericName:
                    {
                        var genericName = (GenericNameSyntax)typeSyntax;

                        return genericName.Identifier.Text;
                    }
                case SyntaxKind.QualifiedName:
                    {
                        var qualifiedName = (QualifiedNameSyntax)typeSyntax;

                        return qualifiedName.ToFullString();
                    }
                case SyntaxKind.RefType:
                    {
                        var refType = (RefTypeSyntax)typeSyntax;

                        return refType.ToFullString();
                    }
                case SyntaxKind.ArrayType:
                    {
                        var arrayTypeName = (ArrayTypeSyntax)typeSyntax;

                        return arrayTypeName.ToFullString();
                    }
                default:
                    DebugUtils.Break();
                    return null;
            }
        }

        public static bool IsGenericArray(this TypeSyntax type)
        {
            if (type is IdentifierNameSyntax && type.ToFullString().EndsWith("[]"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetRighthandName(this TypeSyntax typeSyntax)
        {
            var kind = typeSyntax.Kind();

            switch (kind)
            {
                case SyntaxKind.PredefinedType:
                    {
                        var type = (PredefinedTypeSyntax)typeSyntax;

                        return type.Keyword.Text;
                    }
                case SyntaxKind.IdentifierName:
                    {
                        var identifierName = (IdentifierNameSyntax)typeSyntax;

                        return identifierName.Identifier.Text;
                    }
                case SyntaxKind.GenericName:
                    {
                        var genericName = (GenericNameSyntax)typeSyntax;

                        return genericName.Identifier.Text;
                    }
                case SyntaxKind.QualifiedName:
                    {
                        var qualifiedName = (QualifiedNameSyntax)typeSyntax;

                        return qualifiedName.Right.GetRighthandName();
                    }
                case SyntaxKind.ArrayType:
                    {
                        var arrayType = (ArrayTypeSyntax)typeSyntax;

                        return arrayType.ElementType.GetRighthandName() + "[]";
                    }
                default:
                    //DebugUtils.Break();
                    return null;
            }
        }

        public static bool ParmSignaturesMatch(this SeparatedSyntaxList<ParameterSyntax> parms, SeparatedSyntaxList<ParameterSyntax> parms2)
        {
            if (parms.Count != parms2.Count)
            {
                return false;
            }
            else
            {
                var x = 0;

                foreach (var parm in parms)
                {
                    var parm2 = parms2.ElementAt(x);

                    if (parm.Type.GetName() != parm2.Type.GetName())
                    {
                        return false;
                    }

                    x++;
                }
            }

            return true;
        }

        //public static bool ParmSignaturesMatch(this IList<IParameterSymbol> parms, SeparatedSyntaxList<ArgumentSyntax> args)
        //{
        //    if (parms.Count != args.Count)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        var x = 0;

        //        foreach (var parm in parms)
        //        {
        //            var arg = args.ElementAt(x);

        //            if (!arg.CanMatch(parm.Type))
        //            {
        //                return false;
        //            }

        //            x++;
        //        }
        //    }

        //    return true;
        //}

        public static bool ParmSignaturesMatch(this SeparatedSyntaxList<ParameterSyntax> parms, SeparatedSyntaxList<ArgumentSyntax> args)
        {
            if (parms.Count != args.Count)
            {
                return false;
            }
            else
            {
                var x = 0;

                foreach (var parm in parms)
                {
                    var arg = args.ElementAt(x);

                    if (!arg.CanMatch(parm.Type))
                    {
                        return false;
                    }

                    x++;
                }
            }

            return true;
        }

        public static bool CanMatch(this ArgumentSyntax arg, TypeSyntax type)
        {
            var expression = arg.Expression;
            var typeName = type.GetName();
            var canMatch = false;

            SwitchExtensions.Switch(expression, () => expression.GetType().Name,

                SwitchExtensions.Case<LiteralExpressionSyntax>("LiteralExpressionSyntax", (l) =>
                {
                    switch (l.Kind())
                    {
                        case SyntaxKind.NumericLiteralExpression:
                            break;
                        case SyntaxKind.StringLiteralExpression:
                            canMatch = typeName == "String";
                            break;
                        case SyntaxKind.CharacterLiteralExpression:
                            break;
                        case SyntaxKind.TrueLiteralExpression:
                            break;
                        case SyntaxKind.FalseLiteralExpression:
                            break;
                        case SyntaxKind.NullLiteralExpression:
                            break;
                        case SyntaxKind.DefaultLiteralExpression:
                            break;
                        default:
                            DebugUtils.Break();
                            break;
                    }

                    DebugUtils.NoOp();
                }),
                SwitchExtensions.CaseElse(() =>
                {
                    var a = expression;
                    var t = expression.GetType().Name;

                    // implementation here or throw error

                    DebugUtils.Break();
                })
            );

            return canMatch;
        }

        public static bool ParmSignaturesMatch(this SeparatedSyntaxList<TypeParameterSyntax> parms, SeparatedSyntaxList<TypeParameterSyntax> parms2)
        {
            if (parms.Count != parms2.Count)
            {
                return false;
            }
            else
            {
                var x = 0;

                foreach (var parm in parms)
                {
                    var parm2 = parms2.ElementAt(x);

                    if (parm.Identifier != parm2.Identifier)
                    {
                        return false;
                    }

                    x++;
                }
            }

            return true;
        }

        public static string GetWrappedIdentifier(this SyntaxNode node)
        {
            while (!(node is IdentifierNameSyntax))
            {
                if (node is PostfixUnaryExpressionSyntax)
                {
                    var expression = (PostfixUnaryExpressionSyntax)node;

                    node = expression.Operand;
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            return ((IdentifierNameSyntax)node).Identifier.Text;
        }

        public static bool UsesPointers(this SyntaxNode node)
        {
            if (node.DescendantNodesAndSelf().Any(n => n.Kind() == SyntaxKind.PointerType))
            {
                return true;
            }
            else if (node.DescendantNodesAndSelf().Any(n => n.Kind() == SyntaxKind.AddressOfExpression))
            {
                return true;
            }
            else if (node.DescendantNodesAndSelf().Any(n => n.Kind() == SyntaxKind.PointerIndirectionExpression))
            {
                return true;
            }
            else if (node.DescendantNodesAndSelf().Any(n => n.Kind() == SyntaxKind.PointerMemberAccessExpression))
            {
                return true;
            }

            return false;
        }

        public static IdentifierNameSyntax GetIdentifierName(this NameSyntax name)
        {
            IdentifierNameSyntax identifierName;

            if (name is IdentifierNameSyntax)
            {
                identifierName = (IdentifierNameSyntax)name;
            }
            else if (name is QualifiedNameSyntax)
            {
                identifierName = (IdentifierNameSyntax)((QualifiedNameSyntax)name).Right;
            }
            else
            {
                DebugUtils.Break();
                identifierName = null;
            }

            return identifierName;
        }

        public static SyntaxTriviaList AddToCommentedOutStatementList(this SyntaxTriviaList list, SyntaxTriviaList trivia, StatementSyntax statement)
        {
            var text = statement.ToFullString();
            var lines = text.GetLines();

            foreach (var line in lines)
            {
                list = list.Add(Comment(string.Format("\r\n{0}//// {1}", trivia.ToFullString(), line)));
            }

            return list;
        }

        public static ITypeSymbol GetExpressionType(this SemanticModel model, ExpressionSyntax expression)
        {
            ITypeSymbol type;

            if (expression is InvocationExpressionSyntax)
            {
                var invocation = (InvocationExpressionSyntax)expression;
            }

            try
            {
                type = model.GetTypeInfo(expression).Type;
            }
            catch (Exception ex)
            {
                switch (expression.ToFullString())
                {
                } 

                type = null;
            }

            return type;
        }

        public static string GetBestGuessPrimitiveTypeShortName(this LiteralExpressionSyntax literalExpression)
        {
            var kind = literalExpression.Kind();

            switch (kind)
            {
                case SyntaxKind.NumericLiteralExpression:
                    return "int";
                case SyntaxKind.StringLiteralExpression:
                    return "string";
                case SyntaxKind.CharacterLiteralExpression:
                    return "char";
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    return "bool";
                default:
                    DebugUtils.Break();
                    return null;
            }
        }

        public static object GetValue(this LiteralExpressionSyntax literalExpression)
        {
            var kind = literalExpression.Kind();

            switch (kind)
            {
                case SyntaxKind.NumericLiteralExpression:
                    return (int)literalExpression.Token.Value;
                case SyntaxKind.StringLiteralExpression:
                    return (string) literalExpression.Token.Value;
                case SyntaxKind.CharacterLiteralExpression:
                    return (char)literalExpression.Token.Value;
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    return (bool)literalExpression.Token.Value;
                default:
                    DebugUtils.Break();
                    return null;
            }
        }

        public static string GetPrimitiveTypeShortName(this ITypeSymbol type)
        {
            switch (type.Name)
            {
                case "Void":
                    return "void";
                case "Boolean":
                    return "bool";
                case "Byte":
                    return "byte";
                case "SByte":
                    return "sbyte";
                case "Char":
                    return "char";
                case "Decimal":
                    return "decimal";
                case "Double":
                    return "double";
                case "Single":
                    return "float";
                case "Int32":
                    return "int";
                case "UInt32":
                    return "uint";
                case "Int64":
                    return "long";
                case "UInt64":
                    return "ulong";
                case "Object":
                    return "object";
                case "Int16":
                    return "short";
                case "UInt16":
                    return "ushort";
                case "String":
                    return "string";
                default:
                    DebugUtils.Break();
                    return type.Name;
            }
        }

        public static string GetPrimitiveTypeShortName(this IdentifierNameSyntax type)
        {
            switch (type.Identifier.Text)
            {
                case "Void":
                    return "void";
                case "Boolean":
                    return "bool";
                case "Byte":
                    return "byte";
                case "SByte":
                    return "sbyte";
                case "Char":
                    return "char";
                case "Decimal":
                    return "decimal";
                case "Double":
                    return "double";
                case "Single":
                    return "float";
                case "Int32":
                    return "int";
                case "UInt32":
                    return "uint";
                case "Int64":
                    return "long";
                case "UInt64":
                    return "ulong";
                case "Object":
                    return "object";
                case "Int16":
                    return "short";
                case "UInt16":
                    return "ushort";
                case "String":
                    return "string";
                default:
                    DebugUtils.Break();
                    return type.Identifier.Text;
            }
        }

        public static string GetPrimitiveTypeShortName(this TypeSyntax type)
        {
            var identifierName = type.GetRighthandName();

            switch (identifierName)
            {
                case "Void":
                    return "void";
                case "Boolean":
                    return "bool";
                case "Byte":
                    return "byte";
                case "SByte":
                    return "sbyte";
                case "Char":
                    return "char";
                case "Decimal":
                    return "decimal";
                case "Double":
                    return "double";
                case "Single":
                    return "float";
                case "Int32":
                    return "int";
                case "UInt32":
                    return "uint";
                case "Int64":
                    return "long";
                case "UInt64":
                    return "ulong";
                case "Object":
                    return "object";
                case "Int16":
                    return "short";
                case "UInt16":
                    return "ushort";
                case "String":
                    return "string";
                default:
                    DebugUtils.Break();
                    return identifierName;
            }
        }

        public static string GetPrimitiveTypeAliasName(this TypeSyntax type)
        {
            var identifierName = type.GetRighthandName();

            switch (identifierName)
            {
                case "string":
                    return "String";
                case "int":
                    return "Int32";
                case "uint":
                    return "UInt32";
                case "object":
                    return "Object";
                case "bool":
                    return "Boolean";
                case "sbyte":
                    return "SByte";
                case "byte":
                    return "Byte";
                case "short":
                    return "Int16";
                case "ushort":
                    return "UInt16";
                case "long":
                    return "Int64";
                case "ulong":
                    return "UInt64";
                case "float":
                    return "Single";
                case "double":
                    return "Double";
                case "decimal":
                    return "Decimal";
                case "char":
                    return "Char";
                case "void":
                    return "Void";
                default:
                    DebugUtils.Break();
                    return null;
            }
        }

        public static bool IsPrimitiveType(this TypeSyntax type)
        {
            var identifierName = type.GetRighthandName();

            switch (identifierName)
            {
                case "string":
                case "int":
                case "uint":
                case "object":
                case "bool":
                case "sbyte":
                case "byte":
                case "short":
                case "ushort":
                case "long":
                case "ulong":
                case "float":
                case "double":
                case "decimal":
                case "char":
                case "void":
                case "Void":
                case "Boolean":
                case "Byte":
                case "SByte":
                case "Char":
                case "Decimal":
                case "Double":
                case "Single":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                case "Object":
                case "Int16":
                case "UInt16":
                case "String":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPrimitiveType(this IdentifierNameSyntax type)
        {
            switch (type.Identifier.Text)
            {
                case "string":
                case "int":
                case "uint":
                case "object":
                case "bool":
                case "sbyte":
                case "byte":
                case "short":
                case "ushort":
                case "long":
                case "ulong":
                case "float":
                case "double":
                case "decimal":
                case "char":
                case "void":
                case "Void":
                case "Boolean":
                case "Byte":
                case "SByte":
                case "Char":
                case "Decimal":
                case "Double":
                case "Single":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                case "Object":
                case "Int16":
                case "UInt16":
                case "String":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPrimitiveType(this ITypeSymbol type)
        {
            switch (type.Name)
            {
                case "Void":
                case "Boolean":
                case "Byte":
                case "SByte":
                case "Char":
                case "Decimal":
                case "Double":
                case "Single":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                case "Object":
                case "Int16":
                case "UInt16":
                case "String":
                    return true;
                default:
                    return false;
            }
        }

        public static string GetPrimitiveTypeFullName(this PredefinedTypeSyntax predefinedType)
        {
            switch (predefinedType.Keyword.Text)
            {
                case "string":
                    return "System.String";
                case "int":
                    return "System.Int32";
                case "uint":
                    return "System.UInt32";
                case "object":
                    return "System.Object";
                case "bool":
                    return "System.Boolean";
                case "sbyte":
                    return "System.SByte";
                case "byte":
                    return "System.Byte";
                case "short":
                    return "System.Int16";
                case "ushort":
                    return "System.UInt16";
                case "long":
                    return "System.Int64";
                case "ulong":
                    return "System.UInt64";
                case "float":
                    return "System.Single";
                case "double":
                    return "System.Double";
                case "decimal":
                    return "System.Decimal";
                case "char":
                    return "System.Char";
                case "void":
                    return "System.Void";
                default:
                    DebugUtils.Break();
                    return null;
            }
        }

        public static bool IsPrimitiveTypeName(this string text)
        {
            switch (text)
            {
                case "string":
                    return true;
                case "int":
                    return true;
                case "uint":
                    return true;
                case "object":
                    return true;
                case "bool":
                    return true;
                case "sbyte":
                    return true;
                case "byte":
                    return true;
                case "short":
                    return true;
                case "ushort":
                    return true;
                case "long":
                    return true;
                case "ulong":
                    return true;
                case "float":
                    return true;
                case "double":
                    return true;
                case "decimal":
                    return true;
                case "char":
                    return true;
                case "void":
                    return true;
                default:
                    return false;
            }
        }

        public static string GetHydraPrimitiveTypeFullName(this PredefinedTypeSyntax predefinedType)
        {
            switch (predefinedType.Keyword.Text)
            {
                case "string":
                    return "HydraSystem.String";
                case "int":
                    return "HydraSystem.Int32";
                case "uint":
                    return "HydraSystem.UInt32";
                case "object":
                    return "HydraSystem.Object";
                case "bool":
                    return "HydraSystem.Boolean";
                case "sbyte":
                    return "HydraSystem.SByte";
                case "byte":
                    return "HydraSystem.Byte";
                case "short":
                    return "HydraSystem.Int16";
                case "ushort":
                    return "HydraSystem.UInt16";
                case "long":
                    return "HydraSystem.Int64";
                case "ulong":
                    return "HydraSystem.UInt64";
                case "float":
                    return "HydraSystem.Single";
                case "double":
                    return "HydraSystem.Double";
                case "decimal":
                    return "HydraSystem.Decimal";
                case "char":
                    return "HydraSystem.Char";
                case "void":
                    return "HydraSystem.Void";
                default:
                    DebugUtils.Break();
                    return null;
            }
        }

        public static bool IsHydraPrimitiveType(this TypeSyntax type)
        {
            switch (type.GetName())
            {
                case "HydraSystem.String":
                case "HydraSystem.Int32":
                case "HydraSystem.UInt32":
                case "HydraSystem.Object":
                case "HydraSystem.Boolean":
                case "HydraSystem.SByte":
                case "HydraSystem.Byte":
                case "HydraSystem.Int16":
                case "HydraSystem.UInt16":
                case "HydraSystem.Int64":
                case "HydraSystem.UInt64":
                case "HydraSystem.Single":
                case "HydraSystem.Double":
                case "HydraSystem.Decimal":
                case "HydraSystem.Char":
                case "HydraSystem.Void":
                    return true;
                default:
                    DebugUtils.Break();
                    return false;
            }
        }

        public static bool IsPrimitiveTypeAlias(this TypeSyntax type)
        {
            switch (type.GetRighthandName())
            {
                case "String":
                case "Int32":
                case "UInt32":
                case "Object":
                case "Boolean":
                case "SByte":
                case "Byte":
                case "Int16":
                case "UInt16":
                case "Int64":
                case "UInt64":
                case "Single":
                case "Double":
                case "Decimal":
                case "Char":
                case "Void":
                    return true;
                default:
                    return false;
            }
        }

        public static string GetNamespace(this TypeDeclarationSyntax type)
        {
            string _namespace = string.Empty;

            if (type.Ancestors().Any())
            {
                var typespaces = type.Ancestors().OfType<TypeDeclarationSyntax>().Reverse().Select(n => n.Identifier.Text.ToString());

                if (typespaces.Count() > 0)
                {
                    _namespace = string.Join(".", type.Ancestors().OfType<NamespaceDeclarationSyntax>().Reverse().Select(n => n.Name.ToString()).Concat(typespaces));
                }
                else
                {
                    _namespace = string.Join(".", type.Ancestors().OfType<NamespaceDeclarationSyntax>().Reverse().Select(n => n.Name.ToString()));
                }
            }
            else if (type.GetAnnotations("namespace").Any())
            {
                _namespace = type.GetAnnotations("namespace").Single().Data;
            }

            return _namespace;
        }

        public static IEnumerable<string> GetScopespaces(this TypeDeclarationSyntax type)
        {
            var list = new List<string>();

            if (type.Ancestors().Any())
            {
                string name = null;

                foreach (var parentType in type.Ancestors().OfType<TypeDeclarationSyntax>().Select(t => t.Identifier.Text))
                {
                    if (name == null)
                    {
                        name = parentType;
                    }
                    else
                    {
                        name = parentType + "." + name;
                    }

                    list.Add(name);

                    if (name != parentType)
                    {
                        list.Add(parentType);
                    }
                }

                foreach (var _namespace in type.Ancestors().OfType<NamespaceDeclarationSyntax>().Select(n => n.Name.ToString()))
                {
                    if (name == null)
                    {
                        name = _namespace;
                    }
                    else
                    {
                        name = _namespace + "." + name;
                    }

                    list.Add(name);

                    if (name != _namespace)
                    {
                        list.Add(_namespace);
                    }
                }
            }

            foreach (var scopespace in list.ToList())
            {
                var parts = scopespace.Split(".");

                // remove from bottom

                for (var x = 1; x < parts.Length; x++)
                {
                    var newScopeSpace = string.Join(".", parts.Take(x));

                    if (!list.Contains(newScopeSpace))
                    {
                        list.Add(newScopeSpace);
                    }
                }

                // remove from top

                for (var x = 1; x < parts.Length; x++)
                {
                    var newScopeSpace = string.Join(".", parts.Skip(x));

                    if (!list.Contains(newScopeSpace))
                    {
                        list.Add(newScopeSpace);
                    }
                }
            }

            return list;
        }

        //public static bool IsOfType(this INamedTypeSymbol type, string ofTypeFullName)
        //{
        //    return type.GetAncestors(true).Any(t => t.GetFullName() == ofTypeFullName);
        //}

        //public static List<INamedTypeSymbol> GetAncestors(this INamedTypeSymbol type, bool includeMe = false, bool includeInterfaces = false)
        //{
        //    var list = new List<INamedTypeSymbol>();
        //    var parent = type.BaseType;

        //    if (includeMe)
        //    {
        //        list.Add(type);
        //    }

        //    while (parent != null)
        //    {
        //        list.Add(parent);
        //        parent = parent.BaseType;
        //    }

        //    if (includeInterfaces)
        //    {
        //        return list.Concat(type.AllInterfaces).ToList();
        //    }
        //    else
        //    {
        //        return list;
        //    }
        //}

        public static string GetFullName(this TypeDeclarationSyntax type)
        {
            var _namespace = type.GetNamespace();

            if (!_namespace.IsNullOrEmpty())
            {
                return type.GetNamespace() + "." + type.Identifier.Text;
            }
            else
            {
                return type.Identifier.Text;
            }
        }

        public static string GetFullName(this INamedTypeSymbol type)
        {
            var _namespace = type.ContainingNamespace;

            if (_namespace.IsGlobalNamespace)
            {
                return type.Name;
            }
            else
            {
                var namespaceName = string.Empty;
                var containingType = type.ContainingType;

                while (containingType != null)
                {
                    namespaceName = containingType.Name + "." + namespaceName;
                    containingType = containingType.ContainingType;
                }

                while (!_namespace.IsGlobalNamespace)
                {
                    namespaceName = _namespace.Name + "." + namespaceName;
                    _namespace = _namespace.ContainingNamespace;
                }

                return namespaceName + type.Name;
            }
        }

        public static PredefinedTypeSyntax GetPredefinedType(this TypeDeclarationSyntax type)
        {
            var fullName = type.GetFullName().Replace("HydraSystem", "System");

            switch (fullName)
            {
                case "System.String":
                    return PredefinedType(Token(SyntaxKind.StringKeyword));
                case "System.Int32":
                    return PredefinedType(Token(SyntaxKind.IntKeyword));
                case "System.UInt32":
                    return PredefinedType(Token(SyntaxKind.UIntKeyword));
                case "System.Object":
                    return PredefinedType(Token(SyntaxKind.ObjectKeyword));
                case "System.Boolean":
                    return PredefinedType(Token(SyntaxKind.BoolKeyword));
                case "System.SByte":
                    return PredefinedType(Token(SyntaxKind.SByteKeyword));
                case "System.Byte":
                    return PredefinedType(Token(SyntaxKind.ByteKeyword));
                case "System.Int16":
                    return PredefinedType(Token(SyntaxKind.ShortKeyword));
                case "System.UInt16":
                    return PredefinedType(Token(SyntaxKind.UShortKeyword));
                case "System.Int64":
                    return PredefinedType(Token(SyntaxKind.LongKeyword));
                case "System.UInt64":
                    return PredefinedType(Token(SyntaxKind.ULongKeyword));
                case "System.Single":
                    return PredefinedType(Token(SyntaxKind.FloatKeyword));
                case "System.Double":
                    return PredefinedType(Token(SyntaxKind.DoubleKeyword));
                case "System.Decimal":
                    return PredefinedType(Token(SyntaxKind.DecimalKeyword));
                case "System.Char":
                    return PredefinedType(Token(SyntaxKind.CharKeyword));
                case "System.Void":
                    return PredefinedType(Token(SyntaxKind.VoidKeyword));
                default:
                    DebugUtils.Break();
                    return null;
            }
        }

        public static PredefinedTypeSyntax GetPredefinedType(this TypeSyntax type)
        {
            var fullName = type.GetName();

            switch (fullName)
            {
                case "String":
                    return PredefinedType(Token(SyntaxKind.StringKeyword));
                case "Int32":
                    return PredefinedType(Token(SyntaxKind.IntKeyword));
                case "UInt32":
                    return PredefinedType(Token(SyntaxKind.UIntKeyword));
                case "Object":
                    return PredefinedType(Token(SyntaxKind.ObjectKeyword));
                case "Boolean":
                    return PredefinedType(Token(SyntaxKind.BoolKeyword));
                case "SByte":
                    return PredefinedType(Token(SyntaxKind.SByteKeyword));
                case "Byte":
                    return PredefinedType(Token(SyntaxKind.ByteKeyword));
                case "Int16":
                    return PredefinedType(Token(SyntaxKind.ShortKeyword));
                case "UInt16":
                    return PredefinedType(Token(SyntaxKind.UShortKeyword));
                case "Int64":
                    return PredefinedType(Token(SyntaxKind.LongKeyword));
                case "UInt64":
                    return PredefinedType(Token(SyntaxKind.ULongKeyword));
                case "Single":
                    return PredefinedType(Token(SyntaxKind.FloatKeyword));
                case "Double":
                    return PredefinedType(Token(SyntaxKind.DoubleKeyword));
                case "Decimal":
                    return PredefinedType(Token(SyntaxKind.DecimalKeyword));
                case "Char":
                    return PredefinedType(Token(SyntaxKind.CharKeyword));
                case "Void":
                    return PredefinedType(Token(SyntaxKind.VoidKeyword));
                default:
                    DebugUtils.Break();
                    return null;
            }
        }

        public static string GetDeferralTodoComment(string todo)
        {
            var pattern = @"(?<whitespace>^\s*)(?<remaining>.*?$)";
            var regex = new Regex(pattern);
            string comment = null;

            comment = string.Format("/* TODO - downgrade to C# 4.0 deferred on {1}, todo: {2} */", DateTime.Now.ToDateTimeText(), todo);

            return comment;
        }

        public static bool Matches(this SyntaxNode node, Diagnostic diagnostic)
        {
            var sourceSpan = node.GetLocation().SourceSpan;
            var nodeStart = sourceSpan.Start;
            var nodeEnd = sourceSpan.End;
            var errorSpan = diagnostic.Location.SourceSpan;
            var errorStart = errorSpan.Start;
            var errorEnd = errorSpan.End;

            return errorStart.IsBetween(nodeStart, nodeEnd) && errorEnd.IsBetween(nodeStart, nodeEnd);
        }

        public static T ReplaceWith<T>(this SyntaxNode node, T replaceNode) where T : SyntaxNode
        {
            return (T)node.ReplaceNode(node, replaceNode);
        }

        public static long GetId(this SyntaxNode node)
        {
            var annotation = node.GetAnnotations("Id").Single();

            return annotation.GetFieldValue<long>("_id");
        }
        public static StatementSyntax WrapInCarriageReturnLineFeed(this StatementSyntax expressionStatement)
        {
            return expressionStatement.WithLeadingTrivia(CarriageReturnLineFeed).WithTrailingTrivia(CarriageReturnLineFeed);
        }

        public static List<KeyValuePair<SyntaxToken, TypeSyntax>> GetVariablesInContainingMethodScope(this SyntaxNode node)
        {
            var list = new List<KeyValuePair<SyntaxToken, TypeSyntax>>();

            foreach (var block in node.AncestorsAndSelf().TakeWhile(n => !(n is MethodDeclarationSyntax)).OfType<BlockSyntax>())
            {
                foreach (var variableDeclaration in block.ChildNodes().OfType<VariableDeclarationSyntax>())
                {
                    foreach (var variable in variableDeclaration.Variables)
                    {
                        list.Add(new KeyValuePair<SyntaxToken, TypeSyntax>(variable.Identifier, variableDeclaration.Type));
                    }
                }
            }

            return list;
        }

        public static bool HasLeadingConditionalDirectiveTrivia(this SyntaxNode node)
        {
            return node.GetLeadingTrivia().Any(t => t.Kind().IsOneOf(SyntaxKind.IfDirectiveTrivia, SyntaxKind.RegionDirectiveTrivia));
        }

        public static IEnumerable<SyntaxTrivia> GetLeadingConditionalDirectiveTrivia(this SyntaxNode node)
        {
            return node.GetLeadingTrivia().Where(t => t.Kind().IsOneOf(SyntaxKind.IfDirectiveTrivia, SyntaxKind.RegionDirectiveTrivia));
        }

        public static IEnumerable<SyntaxTrivia> GetLeadingAndTrailingTrivia(this SyntaxNode node)
        {
            return node.GetLeadingTrivia().Concat(node.GetTrailingTrivia());
        }

        public static IEnumerable<SyntaxTrivia> GetConditionalDirectiveTrivia(this SyntaxNode node)
        {
            return node.GetLeadingAndTrailingTrivia().Where(t => t.Kind().IsOneOf(SyntaxKind.IfDirectiveTrivia, SyntaxKind.ElseDirectiveTrivia, SyntaxKind.ElifDirectiveTrivia, SyntaxKind.EndIfDirectiveTrivia, SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia, SyntaxKind.DisabledTextTrivia));
        }
        
        private static TypeSyntax GetReturnType(this BaseMethodDeclarationSyntax baseMethod)
        {
            switch (baseMethod.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    return PredefinedType(Token(SyntaxKind.VoidKeyword));
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)baseMethod).ReturnType;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)baseMethod).ReturnType;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)baseMethod).Type;
                default:
                    DebugUtils.Break();
                    return null;
            }
        }

        private static TypeSyntax GetReturnType(this AccessorDeclarationSyntax accessor)
        {
            if (accessor.Kind() == SyntaxKind.GetAccessorDeclaration)
            {
                var parent = accessor.Parent.Parent;

                switch (parent.Kind())
                {
                    case SyntaxKind.PropertyDeclaration:
                        return ((PropertyDeclarationSyntax)parent).Type;
                    case SyntaxKind.IndexerDeclaration:
                        return ((IndexerDeclarationSyntax)parent).Type;
                    default:
                        DebugUtils.Break();
                        return null;
                }
            }
            else
            {
                return PredefinedType(Token(SyntaxKind.VoidKeyword));
            }
        }

        public static int GetLineNumber(this SyntaxNode syntaxNode)
        {
            var type = syntaxNode.GetType().Name;
            var location = syntaxNode.GetLocation();
            var startPos = location.GetLineSpan().StartLinePosition;
            var lineNumber = startPos.Line + 1;

            return lineNumber;
        }

        public static int GetLineNumber(this Location location)
        {
            var startPos = location.GetLineSpan().StartLinePosition;
            var lineNumber = startPos.Line + 1;

            return lineNumber;
        }

        public static int GetColumNumber(this Location location)
        {
            var startPos = location.GetLineSpan().StartLinePosition;
            var columnNumber = startPos.Character + 1;

            return columnNumber;
        }

        public static int GetLineNumber(this SyntaxTrivia syntaxTrivia)
        {
            var type = syntaxTrivia.GetType().Name;
            var location = syntaxTrivia.GetLocation();
            var startPos = location.GetLineSpan().StartLinePosition;
            var lineNumber = startPos.Line + 1;

            return lineNumber;
        }

        public static T ReplaceChildNode<T>(this T parentNode, SyntaxNode childToReplace, SyntaxNode childReplacement, Func<ConditionalAccessExpressionSyntax, SyntaxNode> getConditionalExpressionReplacement = null) where T : SyntaxNode
        {
            var kind = parentNode.Kind();

            switch (kind)
            {
                case SyntaxKind.CoalesceExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.AsExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)(object)parentNode;
                        BinaryExpressionSyntax newParent;
                        var left = binaryExpression.Left;
                        var right = binaryExpression.Right;

                        if (childToReplace == left)
                        {
                            if (childReplacement is ExpressionStatementSyntax)
                            {
                                childReplacement = ((ExpressionStatementSyntax)childReplacement).Expression;
                            }

                            left = ((ExpressionSyntax)childReplacement).WrapInParenthesis();
                        }
                        else if (childToReplace == right)
                        {
                            if (childReplacement is ExpressionStatementSyntax)
                            {
                                childReplacement = ((ExpressionStatementSyntax)childReplacement).Expression;
                            }

                            right = ((ExpressionSyntax)childReplacement).WrapInParenthesis();
                        }
                        else
                        {
                            DebugUtils.Break();
                        }

                        newParent = BinaryExpression(kind, left, right);
                        newParent = parentNode.ReplaceWith(newParent);

                        return (T)(object) newParent;
                    }
                case SyntaxKind.ConditionalExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)(object)parentNode;
                        ConditionalExpressionSyntax newParent;
                        var whenTrue = conditionalExpression.WhenTrue;
                        var whenFalse = conditionalExpression.WhenFalse;
                        var condition = conditionalExpression.Condition;

                        if (childToReplace == condition)
                        {
                            condition = ((ExpressionSyntax)childReplacement).WrapInParenthesis();
                        }
                        if (childToReplace == whenTrue)
                        {
                            whenTrue = ((ExpressionSyntax)childReplacement).WrapInParenthesis();
                        }
                        else if (childToReplace == whenFalse)
                        {
                            whenFalse = ((ExpressionSyntax)childReplacement).WrapInParenthesis();
                        }
                        else
                        {
                            DebugUtils.Break();
                        }

                        newParent = ConditionalExpression(condition, whenTrue, whenFalse);
                        newParent = parentNode.ReplaceWith(newParent);

                        return (T)(object)newParent;
                    }
                case SyntaxKind.IfStatement:
                    {
                        var ifStatement = (IfStatementSyntax)(object)parentNode;
                        IfStatementSyntax newParent;
                        var condition = ifStatement.Condition;
                        var statement = ifStatement.Statement;

                        if (childToReplace == condition)
                        {
                            var binaryExpression = (BinaryExpressionSyntax)(object)condition;
                            var left = binaryExpression.Left;
                            var right = binaryExpression.Right;

                            if (left is AwaitExpressionSyntax)
                            {
                                if (childReplacement is ExpressionStatementSyntax)
                                {
                                    childReplacement = ((ExpressionStatementSyntax)childReplacement).Expression;
                                }

                                left = (ExpressionSyntax)childReplacement;
                            }
                            else if (right is AwaitExpressionSyntax)
                            {
                                if (childReplacement is ExpressionStatementSyntax)
                                {
                                    childReplacement = ((ExpressionStatementSyntax)childReplacement).Expression;
                                }

                                right = (ExpressionSyntax)childReplacement;
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            condition = BinaryExpression(SyntaxKind.EqualsExpression, left, right);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }

                        newParent = IfStatement(condition, statement, ifStatement.Else);
                        newParent = parentNode.ReplaceWith(newParent);

                        return (T)(object)newParent;
                    }
                case SyntaxKind.EqualsValueClause:
                    {
                        var equalsValueClause = (EqualsValueClauseSyntax)(object)parentNode;
                        EqualsValueClauseSyntax newParent;

                        newParent = EqualsValueClause(((ExpressionSyntax)childReplacement).WrapInParenthesis());
                        newParent = parentNode.ReplaceWith(newParent);

                        return (T)(object)newParent;
                    }
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)(object)parentNode;
                        ReturnStatementSyntax newParent;

                        newParent = ReturnStatement(((ExpressionSyntax)childReplacement).WrapInParenthesis());
                        newParent = parentNode.ReplaceWith(newParent);

                        return (T)(object)newParent;
                    }
                case SyntaxKind.Block:
                    {
                        var block = (BlockSyntax)(object)parentNode;
                        BlockSyntax newParent;

                        newParent = block.ReplaceBlockStatement((StatementSyntax) childToReplace, (StatementSyntax)childReplacement);
                        newParent = parentNode.ReplaceWith(newParent);

                        return (T)(object)newParent;
                    }
                case SyntaxKind.ConditionalAccessExpression:
                    {
                        var expression = (ConditionalAccessExpressionSyntax)(object)parentNode;
                        SyntaxNode newParent;

                        newParent = getConditionalExpressionReplacement(expression);

                        return (T)(object)newParent;
                    }
                case SyntaxKind.ParenthesizedExpression:
                    {
                        ParenthesizedExpressionSyntax newParent;

                        if (childReplacement is ExpressionStatementSyntax)
                        {
                            childReplacement = ((ExpressionStatementSyntax)childReplacement).Expression;
                        }

                        newParent = ParenthesizedExpression(((ExpressionSyntax)childReplacement));
                        newParent = parentNode.ReplaceWith(newParent);

                        return (T)(object)newParent;
                    }
                case SyntaxKind.ExpressionStatement:
                    {
                        ExpressionStatementSyntax newParent;

                        newParent = (ExpressionStatementSyntax) childReplacement;
                        newParent = parentNode.ReplaceWith(newParent);

                        return (T)(object)newParent;
                    }
                case SyntaxKind.Argument:
                    {
                        var argument = (ArgumentSyntax)(object)parentNode;
                        ArgumentSyntax newParent;

                        newParent = Argument(((ExpressionSyntax)childReplacement).WrapInParenthesis());
                        newParent = parentNode.ReplaceWith(newParent);

                        return (T)(object)newParent;
                    }
                case SyntaxKind.DoStatement:
                    {
                        var doStatement = (DoStatementSyntax)(object)parentNode;
                        var condition = doStatement.Condition;
                        DoStatementSyntax newParent;

                        if (childReplacement is ExpressionStatementSyntax)
                        {
                            childReplacement = ((ExpressionStatementSyntax)childReplacement).Expression;
                        }

                        if (childToReplace == condition)
                        {
                            condition = ((ExpressionSyntax)childReplacement);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }

                        newParent = DoStatement(doStatement.Statement, condition);
                        newParent = parentNode.ReplaceWith(newParent);

                        return (T)(object)newParent;
                    }
                case SyntaxKind.VariableDeclarator:
                    {
                        var variableDeclarator = (VariableDeclaratorSyntax)(object)parentNode;
                        var initializer = variableDeclarator.Initializer;
                        VariableDeclaratorSyntax newParent;

                        if (initializer == childToReplace)
                        {
                            if (childReplacement is ExpressionStatementSyntax)
                            {
                                childReplacement = ((ExpressionStatementSyntax)childReplacement).Expression;
                            }

                            initializer = EqualsValueClause((ExpressionSyntax)childReplacement);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }

                        newParent = VariableDeclarator(variableDeclarator.Identifier, variableDeclarator.ArgumentList, initializer);

                        return (T)(object)newParent;
                    }
                default:
                    DebugUtils.Break();
                    break;
            }

            return null;
        }

        public static BlockSyntax ReplaceBlockStatement(this BlockSyntax block, StatementSyntax statementToReplace, StatementSyntax statementReplacement)
        {
            var statements = block.Statements.ToList();
            var list = new SyntaxList<StatementSyntax>();
            BlockSyntax newBlock;

            foreach (var statement in statements)
            {
                if (statement == statementToReplace)
                {
                    list = list.Add(statementReplacement);
                }
                else
                {
                    list = list.Add(statement);
                }
            }

            newBlock = Block(list);

            return newBlock;
        }

        public static PropertyDeclarationSyntax AddBody(this PropertyDeclarationSyntax property, EqualsValueClauseSyntax equalsValueClause)
        {
            var p = property;
            var body = Block(ReturnStatement(equalsValueClause.Value.WithLeadingTrivia(Space)));
            var list = new SyntaxList<AccessorDeclarationSyntax>();
            var getDeclaration = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, body);
            AccessorListSyntax accessorList;

            list = list.Add(getDeclaration);

            accessorList = AccessorList(list);

            return PropertyDeclaration(p.AttributeLists, p.Modifiers, p.Type, p.ExplicitInterfaceSpecifier, p.Identifier, accessorList, null, null).WithTrailingTrivia(CarriageReturnLineFeed);
        }

        public static T WithMatchingTrivia<T>(this T node, SyntaxNode syntaxNode, int extraTabCount = 0) where T : SyntaxNode
        {
            var triviaList = syntaxNode.GetLeadingTrivia();

            for (var x = 0; x < extraTabCount; x++)
            {
                triviaList = triviaList.Add(Tab);
            }

            return node.WithLeadingTrivia(triviaList).WithTrailingTrivia(node.GetTrailingTrivia()).WithAdditionalAnnotations(syntaxNode.GetAnnotations("Id"));
        }

        public static SyntaxToken WithMatchingTrivia(this SyntaxToken token, SyntaxToken syntaxToken, int extraTabCount = 0)
        {
            var triviaList = syntaxToken.GetAllTrivia();

            return token.WithTriviaFrom(syntaxToken);
        }

        public static T WithoutMatchingTrivia<T>(this T node, List<SyntaxTrivia> triviaList) where T : SyntaxNode
        {
            var remainingLeadingTrivia = node.GetLeadingTrivia().Where(t => !triviaList.Any(t2 => t == t2));
            var remainingTrailingTrivia = node.GetTrailingTrivia().Where(t => !triviaList.Any(t2 => t == t2));

            node = node.WithoutTrivia();

            return node.WithLeadingTrivia(remainingLeadingTrivia).WithTrailingTrivia(remainingTrailingTrivia);
        }

        public static ParenthesizedExpressionSyntax WrapInParenthesis<T>(this T node) where T : ExpressionSyntax
        {
            return ParenthesizedExpression(node);
        }

        public static SyntaxTriviaList GetTabs(int count)
        {
            var triviaList = new SyntaxTriviaList();

            for (var x = 0; x < count; x++)
            {
                triviaList = triviaList.Add(Tab);
            }

            return triviaList;
        }

        public static SyntaxTriviaList GetTabs(this SyntaxNode node)
        {
            var triviaList = new SyntaxTriviaList();
            var count = node.GetLeadingTrivia().Where(t => t.Kind() == SyntaxKind.WhitespaceTrivia).Sum(t => t.Span.End - t.Span.Start) / 4;

            for (var x = 0; x < count; x++)
            {
                triviaList = triviaList.Add(Tab);
            }

            return triviaList;
        }

        public static int GetTabCount(this SyntaxNode node)
        {
            var triviaList = node.GetTabs();

            return triviaList.Count;
        }

        public static T WithMatchingTrivia<T>(this T node, SyntaxNode syntaxNode, params SyntaxTrivia[] extraTrivia) where T : SyntaxNode
        {
            var triviaList = syntaxNode.GetLeadingTrivia();

            foreach (var trivia in extraTrivia)
            {
                triviaList = triviaList.Add(trivia);
            }

            return node.WithLeadingTrivia(triviaList);
        }

        public static PropertyDeclarationSyntax ReplaceTypes<T>(this PropertyDeclarationSyntax property, Func<BlockSyntax, SyntaxNode> visitBlock, Func<T, TypeSyntax> replace) where T : TypeSyntax
        {
            var p = property;
            var type = property.Type;
            var accessors = property.AccessorList;
            var getter = accessors.Accessors.SingleOrDefault(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
            var setter = accessors.Accessors.SingleOrDefault(a => a.Kind() == SyntaxKind.SetAccessorDeclaration);
            var list = new SyntaxList<AccessorDeclarationSyntax>();
            BlockSyntax getterBody;
            BlockSyntax setterBody;

            if (property.Type is T)
            {
                type = replace((T)property.Type);
            }

            if (getter != null)
            {
                if (getter.Body.DescendantNodes().Any(n => n is RefTypeSyntax) || getter.Body.DescendantNodes().Any(n => n is RefExpressionSyntax))
                {
                    getterBody = (BlockSyntax)visitBlock(getter.Body);
                    getter = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, getterBody);

                    list = list.Add(getter);
                }
                else
                {
                    list = list.Add(getter);
                }
            }

            if (setter != null)
            {
                if (setter.Body.DescendantNodes().Any(n => n is RefTypeSyntax) || setter.Body.DescendantNodes().Any(n => n is RefExpressionSyntax))
                {
                    setterBody = (BlockSyntax)visitBlock(setter.Body);
                    setter = AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, setterBody);

                    list = list.Add(setter);
                }
                else
                {
                    list = list.Add(setter);
                }
            }

            return PropertyDeclaration(p.AttributeLists, p.Modifiers, type, p.ExplicitInterfaceSpecifier, p.Identifier, AccessorList(list));
        }

        public static IndexerDeclarationSyntax ReplaceTypes<T>(this IndexerDeclarationSyntax Indexer, Func<BlockSyntax, SyntaxNode> visitBlock, Func<T, TypeSyntax> replace) where T : TypeSyntax
        {
            var i = Indexer;
            var type = Indexer.Type;
            var accessors = Indexer.AccessorList;
            var getter = accessors.Accessors.SingleOrDefault(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
            var setter = accessors.Accessors.SingleOrDefault(a => a.Kind() == SyntaxKind.SetAccessorDeclaration);
            var list = new SyntaxList<AccessorDeclarationSyntax>();
            BlockSyntax getterBody;
            BlockSyntax setterBody;

            if (Indexer.Type is T)
            {
                type = replace((T)Indexer.Type);
            }

            if (getter != null)
            {
                if (getter.Body.DescendantNodes().Any(n => n is RefTypeSyntax) || getter.Body.DescendantNodes().Any(n => n is RefExpressionSyntax))
                {
                    getterBody = (BlockSyntax)visitBlock(getter.Body);
                    getter = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, getterBody);

                    list = list.Add(getter);
                }
                else
                {
                    list = list.Add(getter);
                }
            }

            if (setter != null)
            {
                if (setter.Body.DescendantNodes().Any(n => n is RefTypeSyntax) || setter.Body.DescendantNodes().Any(n => n is RefExpressionSyntax))
                {
                    setterBody = (BlockSyntax)visitBlock(setter.Body);
                    setter = AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, setterBody);

                    list = list.Add(setter);
                }
                else
                {
                    list = list.Add(setter);
                }
            }

            return IndexerDeclaration(i.AttributeLists, i.Modifiers, type, i.ExplicitInterfaceSpecifier, i.ParameterList, AccessorList(list));
        }

        public static MethodDeclarationSyntax ReplaceTypes<T>(this MethodDeclarationSyntax method, Func<BlockSyntax, SyntaxNode> visitBlock, Func<T, TypeSyntax> replace) where T : TypeSyntax
        {
            var m = method;
            var returnType = method.ReturnType;
            var parms = method.ParameterList;
            var body = method.Body;

            if (method.ReturnType is T)
            {
                returnType = replace((T)method.ReturnType);
            }

            if (parms.Parameters.Any(p => p.Type is T))
            {
                parms = parms.ReplaceTypes(replace);
            }

            if (body.DescendantNodes().Any(n => n is RefTypeSyntax) || body.DescendantNodes().Any(n => n is RefExpressionSyntax))
            {
                body = (BlockSyntax) visitBlock(body);
            }

            return MethodDeclaration(m.AttributeLists, m.Modifiers, returnType, m.ExplicitInterfaceSpecifier, m.Identifier, m.TypeParameterList, parms, m.ConstraintClauses, body, m.ExpressionBody);
        }

        public static ConstructorDeclarationSyntax ReplaceTypes<T>(this ConstructorDeclarationSyntax constructor, Func<BlockSyntax, SyntaxNode> visitBlock, Func<T, TypeSyntax> replace) where T : TypeSyntax
        {
            var c = constructor;
            var parms = constructor.ParameterList;
            var body = constructor.Body;

            if (parms.Parameters.Any(p => p.Type is T))
            {
                parms = parms.ReplaceTypes(replace);
            }

            if (body.DescendantNodes().Any(n => n is RefTypeSyntax) || body.DescendantNodes().Any(n => n is RefExpressionSyntax))
            {
                body = (BlockSyntax)visitBlock(body);
            }

            return ConstructorDeclaration(c.AttributeLists, c.Modifiers, c.Identifier, parms, c.Initializer, body, c.ExpressionBody);
        }

        public static ParameterListSyntax ReplaceTypes<T>(this ParameterListSyntax parms, Func<T, TypeSyntax> replace) where T : TypeSyntax
        {
            var list = new SeparatedSyntaxList<ParameterSyntax>();

            foreach (var parm in parms.Parameters)
            {
                var p = parm;

                if (parm.Type is T)
                {
                    var newType = replace((T)parm.Type);
                    var newParm = Parameter(p.AttributeLists, p.Modifiers, newType, p.Identifier, p.Default);

                    list = list.Add(newParm);
                }
                else
                {
                    list = list.Add(parm);
                }
            }

            return ParameterList(list);
        }

        public static OperatorDeclarationSyntax ReplaceTypes<T>(this OperatorDeclarationSyntax _operator, Func<BlockSyntax, SyntaxNode> visitBlock, Func<T, TypeSyntax> replace) where T : TypeSyntax
        {
            var o = _operator;
            var returnType = _operator.ReturnType;
            var parms = _operator.ParameterList;
            var body = _operator.Body;

            if (_operator.ReturnType is T)
            {
                returnType = replace((T)_operator.ReturnType);
            }

            if (parms.Parameters.Any(p => p.Type is T))
            {
                parms = parms.ReplaceTypes(replace);
            }

            if (body.DescendantNodes().Any(n => n is RefTypeSyntax))
            {
                body = (BlockSyntax)visitBlock(body);
            }

            return OperatorDeclaration(o.AttributeLists, o.Modifiers, returnType, o.OperatorKeyword, o.OperatorToken, parms, body, o.ExpressionBody, o.SemicolonToken);
        }


        public static ConversionOperatorDeclarationSyntax ReplaceTypes<T>(this ConversionOperatorDeclarationSyntax _operator, Func<BlockSyntax, SyntaxNode> visitBlock, Func<T, TypeSyntax> replace) where T : TypeSyntax
        {
            var o = _operator;
            var type = _operator.Type;
            var parms = _operator.ParameterList;
            var body = _operator.Body;

            if (_operator.Type is T)
            {
                type = replace((T)_operator.Type);
            }

            if (parms.Parameters.Any(p => p.Type is T))
            {
                parms = parms.ReplaceTypes(replace);
            }

            if (body.DescendantNodes().Any(n => n is RefTypeSyntax))
            {
                body = (BlockSyntax)visitBlock(body);
            }

            return ConversionOperatorDeclaration(o.AttributeLists, o.Modifiers, o.ImplicitOrExplicitKeyword, o.OperatorKeyword, type, parms, body, o.ExpressionBody, o.SemicolonToken);
        }
    }
}
