using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX;
using System.Linq.Expressions;
using System.Xml.Serialization;
using System.IO;
using AbstraX.ServerInterfaces;
using CodePlex.XPathParser;
using AbstraX.XPathBuilder;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using Utils;
using System.Reflection;
using AbstraX.AssemblyInterfaces;
using AbstraX.BindingsTreeEntities;
using AbstraX.Bindings;

namespace AbstraX
{
    [Flags]
    public enum DebugInfoShowOptions
    {
        ShowID = 1,
        ShowCondensedID = 2,
        ShowName = 4,
        ShowDatatype = 8,
        ShowDescription = 16,
        ShowModifiers = 32,
        ShowInCommentMode = 64
    }

    public static class AbstraXExtensions
    {
        public static DebugInfoShowOptions DebugInfoShowOptions { get; set; }
        public static string DebugInfoLineTerminator { get; set; }
        public static string DebugCommentInitiator { get; set; }
        public static int DebugIndent { get; set; }

        static AbstraXExtensions()
        {
            AbstraXExtensions.DebugInfoShowOptions = DebugInfoShowOptions.ShowID | DebugInfoShowOptions.ShowName | DebugInfoShowOptions.ShowDatatype | DebugInfoShowOptions.ShowDescription | AbstraX.DebugInfoShowOptions.ShowModifiers;
            AbstraXExtensions.DebugInfoLineTerminator = "\r\n";
            AbstraXExtensions.DebugCommentInitiator = "/// ";

            AbstraXExtensions.ShowCondensedID = false;
        }

        private static string Prefix
        {
            get
            {
                var prefix = new string('\t', AbstraXExtensions.DebugIndent);

                if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode))
                {
                    prefix += AbstraXExtensions.DebugCommentInitiator;
                }

                return prefix;
            }
        }

        private static string DoubleSuffix
        {
            get
            {
                var suffix = AbstraXExtensions.DebugInfoLineTerminator;

                if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode))
                {
                    suffix += new string('\t', AbstraXExtensions.DebugIndent) + AbstraXExtensions.DebugCommentInitiator;
                }

                suffix += AbstraXExtensions.DebugInfoLineTerminator;

                return suffix;
            }
        }

        private static string NewLine
        {
            get
            {
                var suffix = string.Empty;

                if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode))
                {
                    suffix += new string('\t', AbstraXExtensions.DebugIndent) + AbstraXExtensions.DebugCommentInitiator;
                }

                suffix += AbstraXExtensions.DebugInfoLineTerminator;

                return suffix;
            }
        }

        private static string Suffix
        {
            get
            {
                var suffix = AbstraXExtensions.DebugInfoLineTerminator;

                return suffix;
            }
        }

        public static void ClearIndent()
        {
            AbstraXExtensions.DebugIndent = 0;
        }

        public static bool ShowInCommentsMode
        {
            get
            {
                return AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode);
            }

            set
            {
                if (value)
                {
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.SetFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowInCommentMode);
                }
                else
                {
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.RemoveFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowInCommentMode);
                }
            }
        }
        public static bool ShowCondensedID
        {
            get
            {
                return AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowCondensedID);
            }

            set
            {
                if (value)
                {
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.SetFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowCondensedID);
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.RemoveFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowID);
                }
                else
                {
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.SetFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowID);
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.RemoveFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowCondensedID);
                }
            }
        }

        public static bool IsLocal(this IBase baseObject)
        {
            if (baseObject is IElement)
            {
                var element = (IElement)baseObject;

                return element.Modifiers.HasFlag(Modifiers.IsLocal);
            }
            else if (baseObject is IAttribute)
            {
                var attribute = (IAttribute)baseObject;

                return attribute.Modifiers.HasFlag(Modifiers.IsLocal);
            }
            else if (baseObject is IOperation)
            {
                var operation = (IOperation)baseObject;

                return operation.Modifiers.HasFlag(Modifiers.IsLocal);
            }
            else
            {
                throw new Exception("IsLocal only applies to elements, attributes, and operations");
            }
        }

        public static string GetQueryMethodForID(this IAbstraXProviderService service, string id)
        {
            var baseObject = service.GenerateByID(id);
            var method = service.GetType().GetMethods().Single(m =>
            {
                var returnType = m.ReturnType;
                var args = returnType.GetGenericArguments();

                if (returnType.Name == "IQueryable`1" && args.Length == 1 && args.First().Name == baseObject.GetType().Name && m.GetParameters().Length == 0)
                {
                    return true;
                }
                else if (returnType.Name == baseObject.GetType().Name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            return method.Name;
        }

        public static string MakeID(this IBase baseObject, string predicate)
        {
            var id = "/" + baseObject.GetType().Name + "[@" + predicate + "]";

            if (baseObject.Parent != null)
            {
                id = baseObject.Parent.ID + id;
            }

            return id;
        }

        public static string MakeID(this IBase baseObject, string typeName, string predicate)
        {
            var id = "/" + typeName + "[@" + predicate + "]";

            if (baseObject.Parent != null)
            {
                id = baseObject.Parent.ID + id;
            }

            return id;
        }

        public static BaseType[] GetGenericArguments(this BaseType dataType)
        {
            var type = dataType.UnderlyingType;
            var list = new List<BaseType>();

            foreach (var argType in type.GetGenericArguments())
            {
                if (dataType.SourceParent != null && dataType.SourceParent.FullyQualifiedName == argType.FullName)
                {
                    list.Add(new BaseType(argType, dataType, true));
                }
                else
                {
                    list.Add(new BaseType(argType, dataType));
                }
            }

            return list.ToArray();
        }

        public static BaseType[] GetBaseInterfaces(this BaseType dataType)
        {
            var interfaces = new List<BaseType>();

            foreach (var _interface in dataType.UnderlyingType.GetInterfaces())
            {
                if (dataType.SourceParent != null && dataType.SourceParent.FullyQualifiedName == _interface.FullName)
                {
                    interfaces.Add(new BaseType(_interface, dataType, true));
                }
                else
                {
                    interfaces.Add(new BaseType(_interface, dataType));
                }
            }

            return interfaces.ToArray();
        }

        public static bool Implements(this BaseType type, BaseType implementsType)
        {
            var baseType = type;

            if (!implementsType.IsInterface || type.IsInterface)
            {
                return false;
            }

            while (baseType != null)
            {
                foreach (var interfaceType in type.Interfaces)
                {
                    if (interfaceType.FullyQualifiedName == implementsType.FullyQualifiedName)
                    {
                        return true;
                    }
                    else if (interfaceType.BaseDataType != null && Implements(baseType, interfaceType.BaseDataType))
                    {
                        return true;
                    }
                }

                baseType = baseType.BaseDataType;
            }

            return false;
        }

        public static bool IsSameNamespace(this BaseType type, BaseType otherType)
        {
            var baseType = type.BaseDataType;

            if (type.Namespace == otherType.Namespace)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool InheritsFrom(this BaseType type, BaseType inheritsFromType)
        {
            var baseType = type.BaseDataType;

            if (type.FullyQualifiedName == inheritsFromType.FullyQualifiedName)
            {
                return false;
            }

            while (baseType != null)
            {
                if (baseType.FullyQualifiedName == inheritsFromType.FullyQualifiedName)
                {
                    return true;
                }

                baseType = baseType.BaseDataType;
            }

            return false;
        }

        public static bool InheritsFrom(this Type type, BaseType inheritsFromType)
        {
            var baseType = type.BaseType;

            while (baseType != null)
            {
                if (baseType.FullName == inheritsFromType.FullyQualifiedName)
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        public static BaseType GetBaseDataType(this Type dataType)
        {
            var type = dataType.BaseType;

            if (type != null)
            {
                return new BaseType(type);
            }

            return null;
        }

        public static string GetProperyName(this IBase baseObject)
        {
            if (baseObject is IGetSetProperty)
            {
                return ((IGetSetProperty)baseObject).PropertyName;
            }
            else
            {
                return baseObject.Name;
            }
        }

        public static string GetDebugInfo(this NavigationItem navigationItem)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0}------------------------------------ NavigationItem ------------------------------------{1}", AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
            builder.AppendFormat("{0}{1}{2}", AbstraXExtensions.Prefix, navigationItem.DebugInfo, AbstraXExtensions.Suffix);
            builder.AppendFormat("{0}CanRead: {1}{2}", AbstraXExtensions.Prefix, navigationItem.CanRead ? "true" : "false", AbstraXExtensions.Suffix);
            builder.AppendFormat("{0}CanWrite: {1}{2}", AbstraXExtensions.Prefix, navigationItem.CanWrite ? "true" : "false", AbstraXExtensions.Suffix);
            builder.AppendFormat("{0}{1}----------------------------------------------------------------------------------{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.Suffix);

            return builder.ToString();
        }

        //public static string GetDebugInfo(this IPropertyBinding binding)
        //{
        //    var builder = new StringBuilder();

        //    //builder.AppendFormat("{0}------------------------------------ PropertyBinding ------------------------------------{1}", AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
        //    //builder.AppendFormat("{0}Name: {1}{2}", AbstraXExtensions.Prefix, binding.PropertyBindingName, AbstraXExtensions.Suffix);
        //    //builder.AppendFormat("{0}BindingMode: {1}{2}", AbstraXExtensions.Prefix, Enum<BindingMode>.GetName(binding.BindingMode), AbstraXExtensions.Suffix);

        //    //if (binding.BindingSource is IAbstraXBindingSource)
        //    //{
        //    //    var bindingSource = (IAbstraXBindingSource)binding.BindingSource;

        //    //    builder.AppendFormat("{0}BindingSourceType: AbstraXBindingSource{1}", AbstraXExtensions.Prefix, AbstraXExtensions.Suffix);
        //    //    builder.AppendFormat("{0}IsSearchable: {1}{2}", AbstraXExtensions.Prefix, bindingSource.IsSearchable ? "true" : "false", AbstraXExtensions.Suffix);

        //    //    if (binding.Property != null)
        //    //    {
        //    //        builder.AppendFormat("{0}{1}****************** BindingProperty ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
        //    //        builder.Append(binding.Property.GetDebugInfo());
        //    //        builder.AppendFormat("{0}{1}*****************************************************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
        //    //    }

        //    //    builder.AppendFormat("{0}{1}****************** BindingAttribute ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
        //    //    builder.Append(bindingSource.BindingAttribute.GetDebugInfo());
        //    //    builder.AppendFormat("{0}{1}*****************************************************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
        //    //}
        //    //else
        //    //{
        //    //    Debugger.Break();
        //    //}

        //    //builder.AppendFormat("{0}{1}----------------------------------------------------------------------------------{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.Suffix);

        //    return builder.ToString();
        //}

        public static string GetMethodDebugInfo(this IMethodOperation methodOperation)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0}{1}------------------------------------ MethodOperation ------------------------------------{1}", AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
            builder.AppendFormat("{0}Name: {1}{2}", AbstraXExtensions.Prefix, methodOperation.Name, AbstraXExtensions.DoubleSuffix);

            builder.AppendFormat("{0}{1}****************** OperationInfo ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
            builder.Append(((IOperation)methodOperation).GetDebugInfo());
            builder.AppendFormat("{0}{1}*****************************************************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);

            builder.AppendFormat("{0}{1}{1}----------------------------------------------------------------------------------{1}", AbstraXExtensions.Prefix, AbstraXExtensions.Suffix);

            return builder.ToString();
        }

        public static string GetDebugInfo(this BaseBindingsTreeNode baseTreeNode, string internalName)
        {
            var builder = new StringBuilder();

            //builder.AppendFormat("{0}Node={1}:{2}{3}", AbstraXExtensions.Prefix, baseTreeNode.GetType().Name, internalName, AbstraXExtensions.Suffix);

            //if (baseTreeNode.InternalObject is IBindingsTreeNode)
            //{
            //    var bindingsTreeNode = (IBindingsTreeNode)baseTreeNode.InternalObject;

            //    builder.AppendFormat("{0}Origin={1}{2}", AbstraXExtensions.Prefix, bindingsTreeNode.Origin, AbstraXExtensions.DoubleSuffix);

            //    builder.AppendFormat("{0}{1}------------------------------------ DataContext ------------------------------------{1}", AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);

            //    foreach (var element in bindingsTreeNode.GetContextObjects())
            //    {
            //        object navigationProperty;
            //        IElement navigationOwner;
            //        BaseType propertyType;

            //        builder.AppendFormat("{0}Element: {1}{2}", AbstraXExtensions.Prefix, element.Name, AbstraXExtensions.Suffix);

            //        if (element is ISurrogateElement)
            //        {
            //            var surrogate = (ISurrogateElement)element;

            //            builder.AppendFormat("{0}IsLocal={1}{2}", AbstraXExtensions.Prefix, surrogate.ReferencedFrom.Parent.IsLocal() ? "true" : "false", AbstraXExtensions.Suffix);

            //            if (surrogate.Parent != null)
            //            {
            //                var parent = surrogate.Parent;
            //                var lastParent = parent;

            //                while (parent != null)
            //                {
            //                    lastParent = parent;

            //                    if (!(parent is IElement) || !((IElement)parent).IsContainer)
            //                    {
            //                        break;
            //                    }

            //                    parent = parent.Parent;
            //                }

            //                builder.AppendFormat("{0}LocalParent={1}{2}", AbstraXExtensions.Prefix, lastParent.Name, AbstraXExtensions.DoubleSuffix);
            //            }
            //            else
            //            {
            //                builder.Append(AbstraXExtensions.DebugInfoLineTerminator);
            //            }
            //        }
            //        else
            //        {
            //            builder.AppendFormat("{0}IsLocal={1}{2}", AbstraXExtensions.Prefix, element.IsLocal() ? "true" : "false", AbstraXExtensions.Suffix);

            //            if (element.Parent != null)
            //            {
            //                var parent = element.Parent;
            //                var lastParent = parent;

            //                while (parent != null)
            //                {
            //                    lastParent = parent;

            //                    if (!(parent is IElement) || !((IElement)parent).IsContainer)
            //                    {
            //                        break;
            //                    }

            //                    parent = parent.Parent;
            //                }

            //                builder.AppendFormat("{0}LocalParent={1}{2}", AbstraXExtensions.Prefix, lastParent.Name, AbstraXExtensions.DoubleSuffix);
            //            }
            //            else
            //            {
            //                builder.Append(AbstraXExtensions.DebugInfoLineTerminator);
            //            }
            //        }

            //        if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowID) || AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowCondensedID))
            //        {
            //            builder.AppendFormat("{0}ID={1}{2}", AbstraXExtensions.Prefix, element.GetID(), AbstraXExtensions.DoubleSuffix);
            //        }

            //        if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowDatatype))
            //        {
            //            builder.Append(element.GetDataTypeInfo());
            //        }

            //        if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowDescription))
            //        {
            //            builder.Append(element.GetDesignComments());
            //        }

            //        if (bindingsTreeNode.ParentNode != null)
            //        {
            //            bindingsTreeNode.GetNavigationPropertyOwner(element, out navigationOwner, out navigationProperty, out propertyType);

            //            builder.AppendFormat("{0}{1}****************** Navigation Information ******************{1}", AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);

            //            if (navigationProperty is string)
            //            {
            //                builder.AppendFormat("{0}NavigationOwner: {1}{2}", AbstraXExtensions.Prefix, navigationOwner.Name, AbstraXExtensions.Suffix);
            //                builder.AppendFormat("{0}NavigationOwnerID: {1}{2}", AbstraXExtensions.Prefix, navigationOwner.GetID(), AbstraXExtensions.Suffix);
            //                builder.AppendFormat("{0}NavigationPropertyString: {1}{2}", AbstraXExtensions.Prefix, (string)navigationProperty, AbstraXExtensions.Suffix);
            //                builder.Append(propertyType.GetDataTypeInfo());
            //            }
            //            else if (navigationProperty is IElement)
            //            {
            //                var propertyElement = (IElement)navigationProperty;

            //                builder.AppendFormat("{0}NavigationOwner: {1}{2}", AbstraXExtensions.Prefix, navigationOwner.Name, AbstraXExtensions.Suffix);
            //                builder.AppendFormat("{0}NavigationOwnerID: {1}{2}", AbstraXExtensions.Prefix, navigationOwner.GetID(), AbstraXExtensions.Suffix);
            //                builder.AppendFormat("{0}NavigationPropertyElement: {1}{2}", AbstraXExtensions.Prefix, propertyElement.Name, AbstraXExtensions.Suffix);
            //                builder.AppendFormat("{0}NavigationPropertyElementID: {1}{2}", AbstraXExtensions.Prefix, propertyElement.GetID(), AbstraXExtensions.Suffix);
            //                builder.Append(propertyType.GetDataTypeInfo());
            //            }
            //            else
            //            {
            //                builder.AppendFormat("{0}NavigationPropertyElement: null{1}", AbstraXExtensions.Prefix, AbstraXExtensions.Suffix);
            //            }
            //        }
            //    }

            //    builder.AppendFormat("{0}{1}{1}----------------------------------------------------------------------------------{1}", AbstraXExtensions.Prefix, AbstraXExtensions.Suffix);
            }

            return builder.ToString();
        }

        public static string GetDebugInfo(this IBase baseObject, StringBuilder additional = null)
        {
            var builder = new StringBuilder();

            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowID) || AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowCondensedID))
            {
                builder.AppendFormat("{0}ID={1}{2}", AbstraXExtensions.Prefix, baseObject.GetID(), AbstraXExtensions.Suffix);
            }

            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowName))
            {
                builder.AppendFormat("{0}Name={1}{2}", AbstraXExtensions.Prefix, baseObject.Name, AbstraXExtensions.Suffix);
            }

            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowDatatype))
            {
                builder.Append(baseObject.GetDataTypeInfo());
            }

            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowDescription))
            {
                builder.Append(baseObject.GetDesignComments());
            }

            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowModifiers))
            {
                builder.Append(baseObject.GetModifiersList());
            }

            builder.AppendFormat("{0}**********************************************{1}", AbstraXExtensions.Prefix, AbstraXExtensions.Suffix);

            if (additional != null)
            {
                builder.Append(additional.ToString());
            }

            return builder.ToString();
        }

        public static string GetID(this IBase baseObject)
        {
            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowCondensedID))
            {
                var parser = new XPathParser<string>();
                var builder = new XPathStringBuilder();
                var id = string.Empty;

                parser.Parse(baseObject.ID, builder);

                id = string.Join("../", builder.AxisElementQueue.Select(e => e.Element));
                id += "[" + builder.AxisElementQueue.Last().Predicates.First().ToString() + "]";

                return id;
            }
            else
            {
                return baseObject.ID;
            }
        }

        public static string GetModifiersList(this IBase baseObject)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0}{1}****************** Modifiers ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);

            foreach (var modifier in Enum<Modifiers>.GetValues())
            {
                if (modifier != Modifiers.Unknown)
                {
                    var name = Enum<Modifiers>.GetName(modifier);

                    builder.AppendFormat("{0}{1}={2}{3}", AbstraXExtensions.Prefix, name, baseObject.Modifiers.HasFlag(modifier) ? "true" : "false", AbstraXExtensions.Suffix);
                }
            }

            builder.Append(AbstraXExtensions.NewLine);

            return builder.ToString();
        }

        public static string GetDesignComments(this IBase baseObject)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(baseObject.DesignComments))
            {
                builder.AppendFormat("{0}{1}****************** Description ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
                builder.AppendFormat("{0}{1}{2}", AbstraXExtensions.Prefix, baseObject.DesignComments, AbstraXExtensions.DoubleSuffix);
            }

            return builder.ToString();
        }

        public static string GetDataTypeInfo(this BaseType baseType)
        {
            var builder = new StringBuilder();

            if (baseType is ScalarType)
            {
                var scalarType = (ScalarType)baseType;

                if (scalarType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", AbstraXExtensions.Prefix, scalarType.Name, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}TypeCode='{1}'{2}", AbstraXExtensions.Prefix, Enum.GetName(typeof(TypeCode), scalarType.TypeCode), AbstraXExtensions.DoubleSuffix);
                }
            }
            else
            {
                if (baseType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", AbstraXExtensions.Prefix, baseType.Name, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}FQN='{1}'{2}", AbstraXExtensions.Prefix, baseType.FullyQualifiedName, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}IsCollectionType='{1}'{2}", AbstraXExtensions.Prefix, baseType.IsCollectionType, AbstraXExtensions.DoubleSuffix);
                }
            }

            return builder.ToString();
        }

        public static string GetDataTypeInfo(this IBase baseObject)
        {
            var builder = new StringBuilder();

            if (baseObject is IAttribute)
            {
                var attribute = (IAttribute)baseObject;

                if (attribute.DataType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", AbstraXExtensions.Prefix, attribute.DataType.Name, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}TypeCode='{1}'{2}", AbstraXExtensions.Prefix, Enum.GetName(typeof(TypeCode), attribute.DataType.TypeCode), AbstraXExtensions.DoubleSuffix);
                }
            }
            else if (baseObject is IElement)
            {
                var element = (IElement)baseObject;

                if (element.DataType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", AbstraXExtensions.Prefix, element.DataType.Name, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}FQN='{1}'{2}", AbstraXExtensions.Prefix, element.DataType.FullyQualifiedName, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}IsCollectionType='{1}'{2}", AbstraXExtensions.Prefix, element.DataType.IsCollectionType, AbstraXExtensions.DoubleSuffix);
                }
            }

            return builder.ToString();
        }

        public static string GetRootID(string id)
        {
            var regex = new Regex(@"/(?<providertype>\w+?)\[\@URL=\'(?<url>.*?)\'\]");

            if (regex.IsMatch(id))
            {
                var match = regex.Match(id);
                var rootID = "/" + match.Groups["providertype"].Value + "[@URL='" + match.Groups["url"].Value + "']";

                return rootID;
            }
            else
            {
                return null;
            }
        }

        public static IBase GenerateByID(this IAbstraXProviderService service, string id)
        {
            var queue = new Queue<string>();
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();

            parser.Parse(id, builder);

            if (builder.AxisElementQueue.Count == 1)
            {
                var axisElement = builder.AxisElementQueue.Single();

                var method = service.GetType().GetMethods().Single(m => m.ReturnType.Name == axisElement.Element && m.GetParameters().Length == 0);

                service.LogGenerateByID(id, method);

                var rootObject = (IBase)method.Invoke(service, null);

                service.PostLogGenerateByID();

                return rootObject;
            }
            else
            {
                var axisElement = builder.AxisElementQueue.Last();

                var method = service.GetType().GetMethods().Single(m => m.ReturnType.Name == "IQueryable`1" && m.GetParameters().Length == 0 && m.ReturnType.GetGenericArguments().Any(a => a.Name == axisElement.Element));

                service.LogGenerateByID(id, method);

                var results = (IQueryable<IBase>)method.Invoke(service, null);

                service.PostLogGenerateByID();

                return results.Where(b => b.ID == id).Single();
            }
        }

        public static IQueryable<T> GenerateByID<T>(this IAbstraXProviderService service, string id)
        {
            var queue = new Queue<string>();
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();

            parser.Parse(id, builder);

            var axisElement = builder.AxisElementQueue.Last();

            var method = service.GetType().GetMethods().Single(m => m.ReturnType.Name == "IQueryable`1" && m.GetParameters().Length == 0 && m.ReturnType.GetGenericArguments().Any(a => a.Name == axisElement.Element));

            service.LogGenerateByID(id, method);

            var results = (IQueryable<IBase>)method.Invoke(service, null);

            service.PostLogGenerateByID();

            return results.Where(b => b.ID == id).Cast<T>();
        }
    }
}
